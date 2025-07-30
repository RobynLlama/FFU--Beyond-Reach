using MonoMod;
using System;
using System.Text;

public partial class patch_CondTrigger : CondTrigger {
    public int nMaxDepth { get; set; }
    public string strMathCond { get; set; }
    public int nMathOp { get; set; }
    public double fMathVal { get; set; }
    public string strMultCond { get; set; }
    public string[] aMultOps {
        get {
            return _aMultOps;
        }
        set {
            _valuesWereChanged = true;
            _aMultOps = value;
        }
    }
    private string[] _aMultOps;

    private extern void orig_Init();
    private void Init() {
        orig_Init();
        aMultOps = _aDefault;
        _valuesWereChanged = false;
    }

    public extern CondTrigger orig_Clone();
    public CondTrigger Clone() {
        patch_CondTrigger condTrigger = orig_Clone() as patch_CondTrigger;
        condTrigger.nMaxDepth = nMaxDepth;
        condTrigger.strMathCond = strMathCond;
        condTrigger.nMathOp = nMathOp;
        condTrigger.fMathVal = fMathVal;
        condTrigger.strMultCond = strMultCond;
        condTrigger.aMultOps = aMultOps;
        return condTrigger;
    }

    public extern CondTrigger orig_CloneDeep(string strFind, string strReplace);
    public CondTrigger CloneDeep(string strFind, string strReplace) {
        patch_CondTrigger condTrigger = orig_CloneDeep(strFind, strReplace) as patch_CondTrigger;
        if (aMultOps != null) {
            condTrigger.aMultOps = new string[aMultOps.Length];
            for (int i = 0; i < aMultOps.Length; i++) {
                condTrigger.aMultOps[i] = CloneDeep(aMultOps[i], strReplace, strFind);
                condTrigger._isBlank = false;
            }
        }
        return condTrigger;
    }

    [MonoModReplace] public bool IsBlank() {
        if ((_isBlank || _valuesWereChanged) && 
            ((double)fChance < 1.0 || 
            aReqs.Length != 0 || 
            aForbids.Length != 0 || 
            aTriggers.Length != 0 || 
            aTriggersForbid.Length != 0 || 
            aLowerConds.Length != 0 ||
            aMultOps.Length != 0)) {
            _isBlank = false;
        }
        return _isBlank;
    }

    [MonoModReplace] public bool Triggered(CondOwner objOwner, string strIAStatsName = null, bool logOutcome = true) {
        if (logReason) logReason = logOutcome;
        strFailReasonLast = string.Empty;
        if (objOwner == null) return false;
        if (nMaxDepth > 0 && GetDepth(objOwner) > nMaxDepth) return false;
        if (IsBlank()) return true;
        objOwner.ValidateParent();
        SocialStats refSocStats = null;
        if (strIAStatsName != null 
            && DataHandler.dictSocialStats.TryGetValue(strIAStatsName, out refSocStats))
            refSocStats.nChecked++;
        if (!bChanceSkip && fChance < 1f) {
            float rChance = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat);
            if (rChance > fChance) {
                if (refSocStats != null) refSocStats.nChecked++;
                if (logReason) strFailReasonLast = $"Chance: {rChance} / {fChance}";
                return false;
            }
        }
        Condition refCond;
        if (bAND) {
            if (strMathCond != null) {
                refCond = null;
                double vMath = 0.0;
                objOwner.mapConds.TryGetValue(strMathCond, out refCond);
                if (refCond != null) vMath = refCond.fCount;
                if (!MathTrigger(nMathOp, vMath, fMathVal)) {
                    if (logReason) strFailReasonLast = $"Math Lacking: {strMathCond} " +
                        $"({vMath}) is {MathToString(nMathOp)} {fMathVal}";
                    return false;
                }
            }
            if (strMultCond != null) {
                refCond = null;
                double vMath = 0.0;
                objOwner.mapConds.TryGetValue(strMultCond, out refCond);
                if (refCond != null) vMath = refCond.fCount;
                if (aMultOps.Length % 3 == 0) {
                    string[] refMultOps = aMultOps;
                    for (int i = 0; i < refMultOps.Length; i += 3) {
                        if (refMultOps[i] == null) {
                            int.TryParse(refMultOps[i + 1], out int refMultOp);
                            double.TryParse(refMultOps[i + 2], out double vMult);
                            if (!MathTrigger(refMultOp, vMath, vMult)) {
                                if (logReason) strFailReasonLast = $"Mult Lacking: " +
                                    $"{strMultCond} ({vMath}) is {MathToString(refMultOp)} {vMult}";
                                return false;
                            }
                        } else if (objOwner.mapConds.TryGetValue(refMultOps[i], out refCond)) {
                            int.TryParse(refMultOps[i + 1], out int refMultOp);
                            double.TryParse(refMultOps[i + 2], out double refMultVal);
                            double vMult = refCond.fCount * refMultVal;
                            if (!MathTrigger(refMultOp, vMath, vMult)) {
                                if (logReason) strFailReasonLast = $"Mult Lacking: " +
                                    $"{strMultCond} ({vMath}) is {MathToString(refMultOp)} " +
                                    $"{FormatTwoDecimals(refMultVal * 100.0)}% of {refMultOps[i]} ({vMult})";
                                return false;
                            }
                        }
                    }
                }
            }
            if (strHigherCond != null) {
                refCond = null;
                double vHigh = 0.0;
                double vLow = 0.0;
                objOwner.mapConds.TryGetValue(strHigherCond, out refCond);
                if (refCond != null) vHigh = refCond.fCount;
                string[] refLowerConds = aLowerConds;
                foreach (string refLowerCond in refLowerConds) {
                    refCond = null;
                    vLow = objOwner.mapConds.TryGetValue(refLowerCond, out refCond) ? refCond.fCount : 0.0;
                    if (vLow > vHigh) {
                        if (logReason) strFailReasonLast = $"Higher Lacking: " +
                            $"{strHigherCond} ({vHigh}) is higher than {refLowerCond} ({vLow})";
                        return false;
                    }
                }
            }
            string[] refReqsAnd = aReqs;
            foreach (string refReq in refReqsAnd) {
                if (!objOwner.mapConds.TryGetValue(refReq, out refCond)) {
                    StatsTrackReqs(strIAStatsName, refReq, 1f);
                    if (logReason) strFailReasonLast = "Lacking: " + refReq;
                    return false;
                }
                if (refCond == null || refCond.fCount <= 0.0) {
                    StatsTrackReqs(strIAStatsName, refReq, 1f);
                    if (logReason) strFailReasonLast = "Lacking: " + refReq;
                    return false;
                }
            }
            refCond = null;
            string[] refForbidsAnd = aForbids;
            foreach (string refForbid in refForbidsAnd) {
                if (objOwner.mapConds.TryGetValue(refForbid, out refCond) && refCond.fCount > 0.0) {
                    StatsTrackForbids(strIAStatsName, refForbid, 1f);
                    if (logReason) strFailReasonLast = "Forbidden: " + refForbid;
                    return false;
                }
            }
            string[] refTriggersAnd = aTriggers;
            foreach (string strTrigger in refTriggersAnd) {
                CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers);
                if (!refTrigger.Triggered(objOwner, strIAStatsName, logReason)) {
                    if (logReason) strFailReasonLast = refTrigger.strFailReasonLast;
                    return false;
                }
            }
            return true;
        }
        string[] refForbids = aForbids;
        foreach (string refForbid in refForbids) {
            if (objOwner.mapConds.TryGetValue(refForbid, out refCond) && refCond.fCount > 0.0) {
                StatsTrackForbids(strIAStatsName, refForbid, 1f);
                if (logReason) strFailReasonLast = "Forbidden: " + refForbid;
                return false;
            }
        }
        string[] refTriggersForbids = aTriggersForbid;
        foreach (string refTriggersForbid in refTriggersForbids) {
            CondTrigger refTrigger = GetTrigger(refTriggersForbid, CTDict.Forbids);
            if (!refTrigger.Triggered(objOwner, strIAStatsName, logReason)) {
                if (logReason) strFailReasonLast = refTrigger.strFailReasonLast;
                return false;
            }
        }
        bool noMath = false;
        if (strMathCond != null) {
            refCond = null;
            double vMath = 0.0;
            objOwner.mapConds.TryGetValue(strMathCond, out refCond);
            if (refCond != null) vMath = refCond.fCount;
            if (MathTrigger(nMathOp, vMath, fMathVal)) return true;
            if (logReason) strFailReasonLast = $"Math Lacking: " +
                $"{strMathCond} ({vMath}) is {MathToString(nMathOp)} {fMathVal}";
        } else noMath = true;
        string strResult = "Mult Lacking: (";
        bool bFirstAdded = false;
        bool noMult = false;
        if (strMultCond != null) {
            refCond = null;
            int nMultOp = 0;
            double fMultVal = 0.0;
            double vMath = 0.0;
            objOwner.mapConds.TryGetValue(strMultCond, out refCond);
            if (refCond != null) vMath = refCond.fCount;
            if (aMultOps.Length % 3 == 0) {
                string[] refMultOps = aMultOps;
                for (int i = 0; i < refMultOps.Length; i += 3) {
                    if (refMultOps[i] == null) {
                        int.TryParse(refMultOps[i + 1], out int refMultOp);
                        double.TryParse(refMultOps[i + 2], out double vMult);
                        if (MathTrigger(refMultOp, vMath, vMult)) return true;
                    } else if (objOwner.mapConds.TryGetValue(refMultOps[i], out refCond)) {
                        int.TryParse(refMultOps[i + 1], out nMultOp);
                        double.TryParse(refMultOps[i + 2], out fMultVal);
                        double vMult = refCond.fCount * fMultVal;
                        if (MathTrigger(nMultOp, vMath, vMult)) return true;
                    }
                    if (logReason) {
                        if (refMultOps[i] == null) {
                            if (bFirstAdded) strResult = $"{strResult}, " +
                            $"{strMultCond} is {MathToString(nMultOp)} {refMultOps[i + 2]}";
                            else strResult = $"{strResult}" +
                            $"{strMultCond} is {MathToString(nMultOp)} {refMultOps[i + 2]}";
                        } else {
                            if (bFirstAdded) strResult = $"{strResult}, " +
                            $"{strMultCond} is {MathToString(nMultOp)} " +
                            $"{FormatTwoDecimals(fMultVal * 100.0)}% of {refMultOps[i]}";
                            else strResult = $"{strResult}" +
                            $"{strMultCond} is {MathToString(nMultOp)} " +
                            $"{FormatTwoDecimals(fMultVal * 100.0)}% of {refMultOps[i]}";
                        }
                    }
                    bFirstAdded = true;
                }
            }
        } else noMult = true;
        if (bFirstAdded && logReason) strFailReasonLast = strFailReasonLast + 
                (strFailReasonLast.Length > 0 ? " " : "") + strResult + ")";
        if (logReason) strResult = "Higher Lacking: (";
        bFirstAdded = false;
        bool noHigher = false;
        if (strHigherCond != null) {
            refCond = null;
            double vHigh = 0.0;
            objOwner.mapConds.TryGetValue(strHigherCond, out refCond);
            if (refCond != null) vHigh = refCond.fCount;
            string[] refLowerConds = aLowerConds;
            foreach (string refLowerCond in refLowerConds) {
                refCond = null;
                if (objOwner.mapConds.TryGetValue(refLowerCond, out refCond) 
                    && refCond.fCount <= vHigh) {
                    return true;
                }
                if (logReason) {
                    if (bFirstAdded) strResult = $"{strResult}, " +
                    $"{strHigherCond} is higher than {refLowerCond}";
                    else strResult = $"{strResult}" +
                    $"{strHigherCond} is higher than {refLowerCond}";
                }
                bFirstAdded = true;
            }
        } else noHigher = true;
        if (bFirstAdded && logReason) strFailReasonLast = strFailReasonLast + 
                (strFailReasonLast.Length > 0 ? " " : "") + strResult + ")";
        if (logReason) strResult = "Reqs Lacking: (";
        bFirstAdded = false;
        string[] refReqs = aReqs;
        foreach (string refReq in refReqs) {
            if (objOwner.mapConds.TryGetValue(refReq, out refCond) && 
                refCond != null && refCond.fCount > 0.0) return true;
            if (logReason) {
                if (bFirstAdded) strResult = $"{strResult}, {refReq}";
                else strResult = $"{strResult}{refReq}";
            }
            bFirstAdded = true;
        }
        if (bFirstAdded && logReason) strFailReasonLast = strFailReasonLast + 
                (strFailReasonLast.Length > 0 ? " " : "") + strResult + ")";
        if (logReason) strResult = "Triggers Lacking: (";
        bFirstAdded = false;
        string[] refTriggers = aTriggers;
        foreach (string strTrigger in refTriggers) {
            CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers);
            if (refTrigger.Triggered(objOwner, strIAStatsName, logReason)) return true;
            if (logReason) {
                if (bFirstAdded) strResult = $"{strResult}, " +
                $"{strTrigger} ({refTrigger.strFailReasonLast})";
                else strResult = $"{strResult}{strTrigger} " +
                $"({refTrigger.strFailReasonLast})";
            }
            bFirstAdded = true;
        }
        if (bFirstAdded && logReason) strFailReasonLast = strFailReasonLast + 
                (strFailReasonLast.Length > 0 ? " " : "") + strResult + ")";
        if (aReqs.Length + aTriggers.Length == 0 
            && noMath && noMult && noHigher) return true;
        string[] refCondReqs = aReqs;
        foreach (string strCond in refCondReqs) 
            StatsTrackReqs(strIAStatsName, strCond, 1f / aReqs.Length);
        return false;
    }

    public string RulesInfo {
        [MonoModReplace] get {
            if (strFailReason != null) return strFailReason;
            Condition refCond = null;
            StringBuilder strBuilder = new StringBuilder();
            if (bAND) {
                for (int i = 0; i < aReqs.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is ");
                    } else if (i == aReqs.Length - 1) strBuilder.Append(", and ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aReqs[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aReqs.Length - 1) strBuilder.Append(".");
                }
                for (int i = 0; i < aForbids.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is NOT ");
                    } else if (i == aForbids.Length - 1) strBuilder.Append(", and ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aForbids[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aForbids.Length - 1) strBuilder.Append(".");
                }
                if (strMathCond != null && nMathOp > 0) {
                    refCond = DataHandler.GetCond(strMathCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strNameFriendly + " is ");
                    strBuilder.Append(MathToString(nMathOp) + " " + fMathVal);
                    strBuilder.Append(".");
                }
                if (strMultCond != null && aMultOps.Length > 0 && aMultOps.Length % 3 == 0) {
                    refCond = DataHandler.GetCond(strMultCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strNameFriendly + " is");
                    for (int i = 0; i < aMultOps.Length; i += 3) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aMultOps.Length - 3) strBuilder.Append(", and ");
                        else if (i > 0) strBuilder.Append(", ");
                        if (aMultOps[i] == null) {
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double vMult);
                            strBuilder.Append(vMult);
                        } else {
                            refCond = DataHandler.GetCond(aMultOps[i]);
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double nMultVal);
                            if (nMultVal != 1.0) strBuilder.Append(FormatTwoDecimals(nMultVal * 100.0) + "% of ");
                            strBuilder.Append(refCond.strNameFriendly);
                        }
                        if (i == aMultOps.Length - 3) strBuilder.Append(".");
                    }
                }
                if (strHigherCond != null && aLowerConds.Length > 0) {
                    refCond = DataHandler.GetCond(strHigherCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strNameFriendly + " is higher than");
                    for (int i = 0; i < aLowerConds.Length; i++) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aLowerConds.Length - 1) strBuilder.Append(", and ");
                        else if (i > 0) strBuilder.Append(", ");
                        refCond = DataHandler.GetCond(aLowerConds[i]);
                        strBuilder.Append(refCond.strNameFriendly);
                        if (i == aLowerConds.Length - 1) strBuilder.Append(".");
                    }
                }
                string[] refTriggersAnd = aTriggers;
                foreach (string strTrigger in refTriggersAnd) {
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers);
                    strBuilder.Append("{" + refTrigger.RulesInfo + "}");
                }
            } else {
                for (int i = 0; i < aForbids.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is NOT ");
                    } else if (i == aForbids.Length - 1) strBuilder.Append(", or ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aForbids[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aForbids.Length - 1) strBuilder.Append(".");
                }
                for (int i = 0; i < aReqs.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is ");
                    } else if (i == aReqs.Length - 1) strBuilder.Append(", or ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aReqs[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aReqs.Length - 1) strBuilder.Append(".");
                }
                if (strMathCond != null && nMathOp > 0) {
                    refCond = DataHandler.GetCond(strMathCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strNameFriendly + " is ");
                    strBuilder.Append(MathToString(nMathOp) + " " + fMathVal);
                    strBuilder.Append(".");
                }
                if (strMultCond != null && aMultOps.Length > 0 && aMultOps.Length % 3 == 0) {
                    refCond = DataHandler.GetCond(strMultCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strNameFriendly + " is");
                    for (int i = 0; i < aMultOps.Length; i += 3) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aMultOps.Length - 3) strBuilder.Append(", or ");
                        else if (i > 0) strBuilder.Append(", ");
                        if (aMultOps[i] == null) {
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double vMult);
                            strBuilder.Append(vMult);
                        } else {
                            refCond = DataHandler.GetCond(aMultOps[i]);
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double nMultVal);
                            if (nMultVal != 1.0) strBuilder.Append(FormatTwoDecimals(nMultVal * 100.0) + "% of ");
                            strBuilder.Append(refCond.strNameFriendly);
                        }
                        if (i == aMultOps.Length - 3) strBuilder.Append(".");
                    }
                }
                if (strHigherCond != null && aLowerConds.Length > 0) {
                    refCond = DataHandler.GetCond(strHigherCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strNameFriendly + " is higher than");
                    for (int i = 0; i < aLowerConds.Length; i++) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aLowerConds.Length - 1) strBuilder.Append(", or ");
                        else if (i > 0) strBuilder.Append(", ");
                        refCond = DataHandler.GetCond(aLowerConds[i]);
                        strBuilder.Append(refCond.strNameFriendly);
                        if (i == aLowerConds.Length - 1) strBuilder.Append(".");
                    }
                }
                string[] refTriggers = aTriggers;
                foreach (string strTrigger in refTriggers) {
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers);
                    strBuilder.Append("{" + refTrigger.RulesInfo + "}");
                }
                string[] refTriggersForbids = aTriggersForbid;
                foreach (string refTriggersForbid in refTriggersForbids) {
                    if (strBuilder.Length > 0) strBuilder.Append(" Not ");
                    else strBuilder.Append("Not ");
                    CondTrigger refTrigger = GetTrigger(refTriggersForbid, CTDict.Forbids);
                    strBuilder.Append("{" + refTrigger.RulesInfo + "}");
                }
            }
            strFailReason = strBuilder.ToString();
            return strFailReason;
        }
    }

    public string RulesInfo_Dev {
        get {
            Condition refCond = null;
            StringBuilder strBuilder = new StringBuilder();
            if (bAND) {
                for (int i = 0; i < aReqs.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is ");
                    } else if (i == aReqs.Length - 1) strBuilder.Append(", and ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aReqs[i]);
                    strBuilder.Append(refCond.strName);
                    if (i == aReqs.Length - 1) strBuilder.Append(".");
                }
                for (int i = 0; i < aForbids.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is NOT ");
                    } else if (i == aForbids.Length - 1) strBuilder.Append(", and ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aForbids[i]);
                    strBuilder.Append(refCond.strName);
                    if (i == aForbids.Length - 1) strBuilder.Append(".");
                }
                if (strMathCond != null && nMathOp > 0) {
                    refCond = DataHandler.GetCond(strMathCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strName + " is ");
                    strBuilder.Append(MathToString(nMathOp) + " " + fMathVal);
                    strBuilder.Append(".");
                }
                if (strMultCond != null && aMultOps.Length > 0 && aMultOps.Length % 3 == 0) {
                    refCond = DataHandler.GetCond(strMultCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strName + " is");
                    for (int i = 0; i < aMultOps.Length; i += 3) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aMultOps.Length - 3) strBuilder.Append(", and ");
                        else if (i > 0) strBuilder.Append(", ");
                        if (aMultOps[i] == null) {
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double vMult);
                            strBuilder.Append(vMult);
                        } else {
                            refCond = DataHandler.GetCond(aMultOps[i]);
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double nMultVal);
                            if (nMultVal != 1.0) strBuilder.Append(FormatTwoDecimals(nMultVal * 100.0) + "% of ");
                            strBuilder.Append(refCond.strName);
                        }
                        if (i == aMultOps.Length - 3) strBuilder.Append(".");
                    }
                }
                if (strHigherCond != null && aLowerConds.Length > 0) {
                    refCond = DataHandler.GetCond(strHigherCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strName + " is higher than");
                    for (int i = 0; i < aLowerConds.Length; i++) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aLowerConds.Length - 1) strBuilder.Append(", and ");
                        else if (i > 0) strBuilder.Append(", ");
                        refCond = DataHandler.GetCond(aLowerConds[i]);
                        strBuilder.Append(refCond.strName);
                        if (i == aLowerConds.Length - 1) strBuilder.Append(".");
                    }
                }
                string[] refTriggersAnd = aTriggers;
                foreach (string strTrigger in refTriggersAnd) {
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    patch_CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers) as patch_CondTrigger;
                    strBuilder.Append("{" + refTrigger.RulesInfo_Dev + "}");
                }
            } else {
                for (int i = 0; i < aForbids.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is NOT ");
                    } else if (i == aForbids.Length - 1) strBuilder.Append(", or ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aForbids[i]);
                    strBuilder.Append(refCond.strName);
                    if (i == aForbids.Length - 1) strBuilder.Append(".");
                }
                for (int i = 0; i < aReqs.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is ");
                    } else if (i == aReqs.Length - 1) strBuilder.Append(", or ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aReqs[i]);
                    strBuilder.Append(refCond.strName);
                    if (i == aReqs.Length - 1) strBuilder.Append(".");
                }
                if (strMathCond != null && nMathOp > 0) {
                    refCond = DataHandler.GetCond(strMathCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strName + " is ");
                    strBuilder.Append(MathToString(nMathOp) + " " + fMathVal);
                    strBuilder.Append(".");
                }
                if (strMultCond != null && aMultOps.Length > 0 && aMultOps.Length % 3 == 0) {
                    refCond = DataHandler.GetCond(strMultCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strName + " is");
                    for (int i = 0; i < aMultOps.Length; i += 3) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aMultOps.Length - 3) strBuilder.Append(", or ");
                        else if (i > 0) strBuilder.Append(", ");
                        if (aMultOps[i] == null) {
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double vMult);
                            strBuilder.Append(vMult);
                        } else {
                            refCond = DataHandler.GetCond(aMultOps[i]);
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double nMultVal);
                            if (nMultVal != 1.0) strBuilder.Append(FormatTwoDecimals(nMultVal * 100.0) + "% of ");
                            strBuilder.Append(refCond.strName);
                        }
                        if (i == aMultOps.Length - 3) strBuilder.Append(".");
                    }
                }
                if (strHigherCond != null && aLowerConds.Length > 0) {
                    refCond = DataHandler.GetCond(strHigherCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strName + " is higher than");
                    for (int i = 0; i < aLowerConds.Length; i++) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aLowerConds.Length - 1) strBuilder.Append(", or ");
                        else if (i > 0) strBuilder.Append(", ");
                        refCond = DataHandler.GetCond(aLowerConds[i]);
                        strBuilder.Append(refCond.strName);
                        if (i == aLowerConds.Length - 1) strBuilder.Append(".");
                    }
                }
                string[] refTriggers = aTriggers;
                foreach (string strTrigger in refTriggers) {
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    patch_CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers) as patch_CondTrigger;
                    strBuilder.Append("{" + refTrigger.RulesInfo_Dev + "}");
                }
                string[] refTriggersForbids = aTriggersForbid;
                foreach (string refTriggersForbid in refTriggersForbids) {
                    if (strBuilder.Length > 0) strBuilder.Append(" Not ");
                    else strBuilder.Append("Not ");
                    patch_CondTrigger refTrigger = GetTrigger(refTriggersForbid, CTDict.Forbids) as patch_CondTrigger;
                    strBuilder.Append("{" + refTrigger.RulesInfo_Dev + "}");
                }
            }
            return strBuilder.ToString();
        }
    }

    public string RulesInfo_Txt {
        get {
            Condition refCond = null;
            StringBuilder strBuilder = new StringBuilder();
            if (bAND) {
                for (int i = 0; i < aReqs.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is ");
                    } else if (i == aReqs.Length - 1) strBuilder.Append(", and ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aReqs[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aReqs.Length - 1) strBuilder.Append(".");
                }
                for (int i = 0; i < aForbids.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is NOT ");
                    } else if (i == aForbids.Length - 1) strBuilder.Append(", and ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aForbids[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aForbids.Length - 1) strBuilder.Append(".");
                }
                if (strMathCond != null && nMathOp > 0) {
                    refCond = DataHandler.GetCond(strMathCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strNameFriendly + " is ");
                    strBuilder.Append(MathToString(nMathOp) + " " + fMathVal);
                    strBuilder.Append(".");
                }
                if (strMultCond != null && aMultOps.Length > 0 && aMultOps.Length % 3 == 0) {
                    refCond = DataHandler.GetCond(strMultCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strNameFriendly + " is");
                    for (int i = 0; i < aMultOps.Length; i += 3) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aMultOps.Length - 3) strBuilder.Append(", and ");
                        else if (i > 0) strBuilder.Append(", ");
                        if (aMultOps[i] == null) {
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double vMult);
                            strBuilder.Append(vMult);
                        } else {
                            refCond = DataHandler.GetCond(aMultOps[i]);
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double nMultVal);
                            if (nMultVal != 1.0) strBuilder.Append(FormatTwoDecimals(nMultVal * 100.0) + "% of ");
                            strBuilder.Append(refCond.strNameFriendly);
                        }
                        if (i == aMultOps.Length - 3) strBuilder.Append(".");
                    }
                }
                if (strHigherCond != null && aLowerConds.Length > 0) {
                    refCond = DataHandler.GetCond(strHigherCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    strBuilder.Append(refCond.strNameFriendly + " is higher than");
                    for (int i = 0; i < aLowerConds.Length; i++) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aLowerConds.Length - 1) strBuilder.Append(", and ");
                        else if (i > 0) strBuilder.Append(", ");
                        refCond = DataHandler.GetCond(aLowerConds[i]);
                        strBuilder.Append(refCond.strNameFriendly);
                        if (i == aLowerConds.Length - 1) strBuilder.Append(".");
                    }
                }
                string[] refTriggersAnd = aTriggers;
                foreach (string strTrigger in refTriggersAnd) {
                    if (strBuilder.Length > 0) strBuilder.Append(" And ");
                    patch_CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers) as patch_CondTrigger;
                    strBuilder.Append("{" + refTrigger.RulesInfo_Txt + "}");
                }
            } else {
                for (int i = 0; i < aForbids.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is NOT ");
                    } else if (i == aForbids.Length - 1) strBuilder.Append(", or ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aForbids[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aForbids.Length - 1) strBuilder.Append(".");
                }
                for (int i = 0; i < aReqs.Length; i++) {
                    if (i == 0) {
                        if (strBuilder.Length > 0) strBuilder.Append(" ");
                        strBuilder.Append("Is ");
                    } else if (i == aReqs.Length - 1) strBuilder.Append(", or ");
                    else if (i > 0) strBuilder.Append(", ");
                    refCond = DataHandler.GetCond(aReqs[i]);
                    strBuilder.Append(refCond.strNameFriendly);
                    if (i == aReqs.Length - 1) strBuilder.Append(".");
                }
                if (strMathCond != null && nMathOp > 0) {
                    refCond = DataHandler.GetCond(strMathCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strNameFriendly + " is ");
                    strBuilder.Append(MathToString(nMathOp) + " " + fMathVal);
                    strBuilder.Append(".");
                }
                if (strMultCond != null && aMultOps.Length > 0 && aMultOps.Length % 3 == 0) {
                    refCond = DataHandler.GetCond(strMultCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strNameFriendly + " is");
                    for (int i = 0; i < aMultOps.Length; i += 3) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aMultOps.Length - 3) strBuilder.Append(", or ");
                        else if (i > 0) strBuilder.Append(", ");
                        if (aMultOps[i] == null) {
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double vMult);
                            strBuilder.Append(vMult);
                        } else {
                            refCond = DataHandler.GetCond(aMultOps[i]);
                            int.TryParse(aMultOps[i + 1], out int nMultOp);
                            strBuilder.Append(MathToString(nMultOp) + " ");
                            double.TryParse(aMultOps[i + 2], out double nMultVal);
                            if (nMultVal != 1.0) strBuilder.Append(FormatTwoDecimals(nMultVal * 100.0) + "% of ");
                            strBuilder.Append(refCond.strNameFriendly);
                        }
                        if (i == aMultOps.Length - 3) strBuilder.Append(".");
                    }
                }
                if (strHigherCond != null && aLowerConds.Length > 0) {
                    refCond = DataHandler.GetCond(strHigherCond);
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    strBuilder.Append(refCond.strNameFriendly + " is higher than");
                    for (int i = 0; i < aLowerConds.Length; i++) {
                        if (i == 0) strBuilder.Append(" ");
                        else if (i == aLowerConds.Length - 1) strBuilder.Append(", or ");
                        else if (i > 0) strBuilder.Append(", ");
                        refCond = DataHandler.GetCond(aLowerConds[i]);
                        strBuilder.Append(refCond.strNameFriendly);
                        if (i == aLowerConds.Length - 1) strBuilder.Append(".");
                    }
                }
                string[] refTriggers = aTriggers;
                foreach (string strTrigger in refTriggers) {
                    if (strBuilder.Length > 0) strBuilder.Append(" Or ");
                    patch_CondTrigger refTrigger = GetTrigger(strTrigger, CTDict.Triggers) as patch_CondTrigger;
                    strBuilder.Append("{" + refTrigger.RulesInfo_Txt + "}");
                }
                string[] refTriggersForbids = aTriggersForbid;
                foreach (string refTriggersForbid in refTriggersForbids) {
                    if (strBuilder.Length > 0) strBuilder.Append(" Not ");
                    else strBuilder.Append("Not ");
                    patch_CondTrigger refTrigger = GetTrigger(refTriggersForbid, CTDict.Forbids) as patch_CondTrigger;
                    strBuilder.Append("{" + refTrigger.RulesInfo_Txt + "}");
                }
            }
            return strBuilder.ToString();
        }
    }

    public static int GetDepth(CondOwner objCO) {
        int currDepth = 1;
        CondOwner objParent = objCO.objCOParent;
        while (objParent != null) {
            objParent = objParent.objCOParent;
            currDepth++;
        }
        return currDepth;
    }

    public static bool MathTrigger(int mOperation, double mTarget, double mValue) {
        switch (mOperation) {
            case 1: return mTarget != mValue;
            case 2: return mTarget == mValue;
            case 3: return mTarget > mValue;
            case 4: return mTarget >= mValue;
            case 5: return mTarget < mValue;
            case 6: return mTarget <= mValue;
        }
        return true;
    }

    public static string MathToString(int mOperation) {
        switch (mOperation) {
            case 1: return "not equal to";
            case 2: return "equal to";
            case 3: return "greater than";
            case 4: return "greater or equal to";
            case 5: return "less than";
            case 6: return "less or equal to";
        }
        return "invalid for";
    }

    public static string FormatTwoDecimals(double dVal) {
        double absVal = Math.Abs(dVal);

        // Default >1 Approach
        if (absVal >= 1.0) return dVal.ToString("0.##");
        if (dVal == 0.0) return "0";

        // Implemented <0 Approach
        int nonZeroCount = 0;
        int decimalPlaces = 0;
        double temp = absVal;
        bool hasHitLimit = false;

        // Find Fraction (+ Limit)
        while (nonZeroCount < 1 && temp > 0 && !hasHitLimit) {
            temp *= 10.0;
            decimalPlaces++;
            int digit = (int)temp % 10;
            if (decimalPlaces == 20) 
                hasHitLimit = true;
            if (digit != 0) {
                nonZeroCount++;
                decimalPlaces++;
            }
        }

        // Return Processed Value
        if (hasHitLimit) return "0";
        string format = "0." + new string('#', decimalPlaces);
        return dVal.ToString(format);
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* CondTrigger.Init
private void Init()
{
	_fChance = 1f;
	aReqs = _aDefault;
	aForbids = _aDefault;
	aTriggers = _aDefault;
	aTriggersForbid = _aDefault;
	aLowerConds = _aDefault;
	_valuesWereChanged = false;
	if (relStranger == null)
	{
		relStranger = new Relationship();
		relStranger.aRelationships = new List<string> { "RELStranger" };
	}
}
*/

/* CondTrigger.Clone
public CondTrigger Clone()
{
	CondTrigger condTrigger = new CondTrigger();
	condTrigger.strName = strName;
	condTrigger.strCondName = strCondName;
	condTrigger._fChance = _fChance;
	condTrigger._fCount = _fCount;
	condTrigger.bAND = bAND;
	condTrigger.aReqs = aReqs;
	condTrigger.aForbids = aForbids;
	condTrigger.aTriggers = aTriggers;
	condTrigger.aTriggersConds = aTriggersConds;
	condTrigger.aTriggersForbid = aTriggersForbid;
	condTrigger.aTriggersForbidConds = aTriggersForbidConds;
	condTrigger._requiresHumans = _requiresHumans;
	condTrigger.strFailReason = strFailReason;
	condTrigger.strFailReasonLast = strFailReasonLast;
	condTrigger.strHigherCond = strHigherCond;
	condTrigger.aLowerConds = aLowerConds;
	condTrigger.nFilterMultiple = nFilterMultiple;
	condTrigger._isBlank = _isBlank;
	condTrigger._valuesWereChanged = false;
	return condTrigger;
}
*/

/* CondTrigger.CloneDeep
public CondTrigger CloneDeep(string strFind, string strReplace)
{
	if (string.IsNullOrEmpty(strReplace) || string.IsNullOrEmpty(strFind) || strReplace == strFind)
	{
		return Clone();
	}
	CondTrigger condTrigger = Clone();
	condTrigger.strName = strName.Replace(strFind, strReplace);
	condTrigger.strCondName = JsonCond.CloneDeep(strCondName, strReplace, strFind);
	if (aForbids != null)
	{
		condTrigger.aForbids = new string[aForbids.Length];
		for (int i = 0; i < aForbids.Length; i++)
		{
			condTrigger.aForbids[i] = JsonCond.CloneDeep(aForbids[i], strReplace, strFind);
			condTrigger._isBlank = false;
		}
	}
	if (aReqs != null)
	{
		condTrigger.aReqs = new string[aReqs.Length];
		for (int j = 0; j < aReqs.Length; j++)
		{
			condTrigger.aReqs[j] = JsonCond.CloneDeep(aReqs[j], strReplace, strFind);
			condTrigger._isBlank = false;
		}
	}
	if (aTriggers != null)
	{
		condTrigger.aTriggers = new string[aTriggers.Length];
		for (int k = 0; k < aTriggers.Length; k++)
		{
			condTrigger.aTriggers[k] = CloneDeep(aTriggers[k], strReplace, strFind);
			condTrigger._isBlank = false;
		}
	}
	if (aTriggersForbid != null)
	{
		condTrigger.aTriggersForbid = new string[aTriggersForbid.Length];
		for (int l = 0; l < aTriggersForbid.Length; l++)
		{
			condTrigger.aTriggersForbid[l] = CloneDeep(aTriggersForbid[l], strReplace, strFind);
			condTrigger._isBlank = false;
		}
	}
	if (aLowerConds != null)
	{
		condTrigger.aLowerConds = new string[aLowerConds.Length];
		for (int m = 0; m < aLowerConds.Length; m++)
		{
			condTrigger.aLowerConds[m] = CloneDeep(aLowerConds[m], strReplace, strFind);
			condTrigger._isBlank = false;
		}
	}
	condTrigger.PostInit();
	DataHandler.dictCTs[condTrigger.strName] = condTrigger;
	return condTrigger;
}
*/

/* CondTrigger.Triggered
public bool Triggered(CondOwner objOwner, string strIAStatsName = null, bool logOutcome = true)
{
	if (logReason)
	{
		logReason = logOutcome;
	}
	strFailReasonLast = string.Empty;
	if (objOwner == null)
	{
		return false;
	}
	if (IsBlank())
	{
		return true;
	}
	objOwner.ValidateParent();
	SocialStats value = null;
	if (strIAStatsName != null && DataHandler.dictSocialStats.TryGetValue(strIAStatsName, out value))
	{
		value.nChecked++;
	}
	if (!bChanceSkip && fChance < 1f)
	{
		float num = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat);
		if (num > fChance)
		{
			if (value != null)
			{
				value.nChecked++;
			}
			if (logReason)
			{
				strFailReasonLast = "Chance: " + num + " / " + fChance;
			}
			return false;
		}
	}
	Condition value2;
	if (bAND)
	{
		if (strHigherCond != null)
		{
			value2 = null;
			double num2 = 0.0;
			double num3 = 0.0;
			objOwner.mapConds.TryGetValue(strHigherCond, out value2);
			if (value2 != null)
			{
				num2 = value2.fCount;
			}
			string[] array = aLowerConds;
			foreach (string key in array)
			{
				value2 = null;
				num3 = (objOwner.mapConds.TryGetValue(key, out value2) ? value2.fCount : 0.0);
				if (num3 > num2)
				{
					return false;
				}
			}
		}
		string[] array2 = aReqs;
		foreach (string text in array2)
		{
			if (!objOwner.mapConds.TryGetValue(text, out value2))
			{
				StatsTrackReqs(strIAStatsName, text, 1f);
				if (logReason)
				{
					strFailReasonLast = "Lacking: " + text;
				}
				return false;
			}
			if (value2 == null || value2.fCount <= 0.0)
			{
				StatsTrackReqs(strIAStatsName, text, 1f);
				if (logReason)
				{
					strFailReasonLast = "Lacking: " + text;
				}
				return false;
			}
		}
		value2 = null;
		string[] array3 = aForbids;
		foreach (string text2 in array3)
		{
			if (objOwner.mapConds.TryGetValue(text2, out value2) && value2.fCount > 0.0)
			{
				StatsTrackForbids(strIAStatsName, text2, 1f);
				if (logReason)
				{
					strFailReasonLast = "Forbidden: " + text2;
				}
				return false;
			}
		}
		string[] array4 = aTriggers;
		foreach (string strTrig in array4)
		{
			CondTrigger trigger = GetTrigger(strTrig, CTDict.Triggers);
			if (!trigger.Triggered(objOwner, strIAStatsName, logReason))
			{
				if (logReason)
				{
					strFailReasonLast = trigger.strFailReasonLast;
				}
				return false;
			}
		}
		return true;
	}
	string[] array5 = aForbids;
	foreach (string text3 in array5)
	{
		if (objOwner.mapConds.TryGetValue(text3, out value2) && value2.fCount > 0.0)
		{
			StatsTrackForbids(strIAStatsName, text3, 1f);
			if (logReason)
			{
				strFailReasonLast = "Forbidden: " + text3;
			}
			return false;
		}
	}
	string[] array6 = aTriggersForbid;
	foreach (string strTrig2 in array6)
	{
		CondTrigger trigger2 = GetTrigger(strTrig2, CTDict.Forbids);
		if (!trigger2.Triggered(objOwner, strIAStatsName, logReason))
		{
			if (logReason)
			{
				strFailReasonLast = trigger2.strFailReasonLast;
			}
			return false;
		}
	}
	if (strHigherCond != null)
	{
		value2 = null;
		double num4 = 0.0;
		objOwner.mapConds.TryGetValue(strHigherCond, out value2);
		if (value2 != null)
		{
			num4 = value2.fCount;
		}
		string[] array7 = aLowerConds;
		foreach (string key2 in array7)
		{
			value2 = null;
			if (objOwner.mapConds.TryGetValue(key2, out value2) && value2.fCount <= num4)
			{
				return true;
			}
		}
	}
	string text4 = "Lacking: (";
	bool flag = false;
	string[] array8 = aReqs;
	foreach (string text5 in array8)
	{
		if (logReason)
		{
			text4 = text4 + text5 + " ";
		}
		flag = true;
		if (objOwner.mapConds.TryGetValue(text5, out value2) && value2 != null && value2.fCount > 0.0)
		{
			return true;
		}
	}
	if (flag && logReason)
	{
		strFailReasonLast = strFailReasonLast + text4 + ")";
	}
	if (logReason)
	{
		text4 = "Triggers Lacking: (";
	}
	flag = false;
	string[] array9 = aTriggers;
	foreach (string strTrig3 in array9)
	{
		CondTrigger trigger3 = GetTrigger(strTrig3, CTDict.Triggers);
		if (trigger3.Triggered(objOwner, strIAStatsName, logReason))
		{
			return true;
		}
		if (logReason)
		{
			text4 = text4 + trigger3.strFailReasonLast + " ";
		}
		flag = true;
	}
	if (flag && logReason)
	{
		strFailReasonLast = strFailReasonLast + text4 + ")";
	}
	if (aReqs.Length + aTriggers.Length == 0)
	{
		return true;
	}
	string[] array10 = aReqs;
	foreach (string strCond in array10)
	{
		StatsTrackReqs(strIAStatsName, strCond, 1f / (float)aReqs.Length);
	}
	return false;
}
*/

/* CondTrigger.get_RulesInfo
public string RulesInfo
{
	get
	{
		if (strFailReason != null)
		{
			return strFailReason;
		}
		Condition condition = null;
		StringBuilder stringBuilder = new StringBuilder();
		if (bAND)
		{
			for (int i = 0; i < aReqs.Length; i++)
			{
				if (i == 0)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append("Is ");
				}
				else if (i == aReqs.Length - 1)
				{
					stringBuilder.Append(", and ");
				}
				else if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				condition = DataHandler.GetCond(aReqs[i]);
				stringBuilder.Append(condition.strNameFriendly);
				if (i == aReqs.Length - 1)
				{
					stringBuilder.Append(".");
				}
			}
			for (int j = 0; j < aForbids.Length; j++)
			{
				if (j == 0)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append("Is NOT ");
				}
				else if (j == aForbids.Length - 1)
				{
					stringBuilder.Append(", and ");
				}
				else if (j > 0)
				{
					stringBuilder.Append(", ");
				}
				condition = DataHandler.GetCond(aForbids[j]);
				stringBuilder.Append(condition.strNameFriendly);
				if (j == aForbids.Length - 1)
				{
					stringBuilder.Append(".");
				}
			}
			string[] array = aTriggers;
			foreach (string strTrig in array)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				CondTrigger trigger = GetTrigger(strTrig, CTDict.Triggers);
				stringBuilder.Append(trigger.RulesInfo);
			}
		}
		else
		{
			for (int l = 0; l < aForbids.Length; l++)
			{
				if (l == 0)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append("Is NOT ");
				}
				else if (l == aForbids.Length - 1)
				{
					stringBuilder.Append(", or ");
				}
				else if (l > 0)
				{
					stringBuilder.Append(", ");
				}
				condition = DataHandler.GetCond(aForbids[l]);
				stringBuilder.Append(condition.strNameFriendly);
				if (l == aForbids.Length - 1)
				{
					stringBuilder.Append(".");
				}
			}
			for (int m = 0; m < aReqs.Length; m++)
			{
				if (m == 0)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append("Is ");
				}
				else if (m == aReqs.Length - 1)
				{
					stringBuilder.Append(", or ");
				}
				else if (m > 0)
				{
					stringBuilder.Append(", ");
				}
				condition = DataHandler.GetCond(aReqs[m]);
				stringBuilder.Append(condition.strNameFriendly);
				if (m == aReqs.Length - 1)
				{
					stringBuilder.Append(".");
				}
			}
			string[] array2 = aTriggers;
			foreach (string strTrig2 in array2)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				CondTrigger trigger2 = GetTrigger(strTrig2, CTDict.Triggers);
				stringBuilder.Append(trigger2.RulesInfo);
			}
			string[] array3 = aTriggersForbid;
			foreach (string strTrig3 in array3)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				CondTrigger trigger3 = GetTrigger(strTrig3, CTDict.Forbids);
				stringBuilder.Append(trigger3.RulesInfo);
			}
		}
		strFailReason = stringBuilder.ToString();
		return strFailReason;
	}
}
*/