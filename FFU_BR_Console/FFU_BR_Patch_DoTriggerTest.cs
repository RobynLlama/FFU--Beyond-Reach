using Ostranauts.UI.MegaToolTip;
using MonoMod;

[MonoModIgnore] public class patch_DataHandler {
    public static bool TryGetCOValue(string strName, out JsonCondOwner refCO) {
        refCO = null;
        return false;
    }
}

public partial class patch_ConsoleResolver : ConsoleResolver {
    private static bool KeywordCondTrigTest(ref string strInput) {
        string[] strTriggerVals = strInput.Split(' ');
        if (strTriggerVals.Length < 3) {
            strInput += "\nMissing command arguments.";
            return false;
        }
        string strTriggerKey = strTriggerVals[1];
        if (!DataHandler.dictCTs.ContainsKey(strTriggerKey)) {
            strInput += "\nCondition trigger not found.";
            return false;
        }
        if (CrewSim.objInstance == null) {
            strInput += "\nCrewSim instance not found.";
            return false;
        }
        CondTrigger refCT = DataHandler.dictCTs[strTriggerKey].Clone();
        string strTriggerTarget = strTriggerVals[2];
        if (strTriggerTarget == "[them]") {
            if (GUIMegaToolTip.Selected == null) {
                strInput += "\nNo target selected or highlighted.";
                return false;
            }
            CondOwner refCO = GUIMegaToolTip.Selected;
            strInput += $"\nTriggering '{strTriggerKey}' against '{refCO.strName}:{refCO.strID}' object.";
            if (!refCT.Triggered(refCO, null, true)) strInput += $"\nOutcome => {refCT.strFailReasonLast}";
            else strInput += $"\nOutcome => Success!";
            refCT.Destroy();
        } else if (patch_DataHandler.TryGetCOValue(strTriggerTarget, out JsonCondOwner refCOjson)) {
            CondOwner refCO = DataHandler.GetCondOwner(strTriggerTarget);
            strInput += $"\nTriggering '{strTriggerKey}' against '{strTriggerTarget}' template.";
            if (!refCT.Triggered(refCO, null, true)) strInput += $"\nOutcome => {refCT.strFailReasonLast}";
            else strInput += $"\nOutcome => Success!";
            refCO.Destroy();
            refCT.Destroy();
        } else {
            strInput += "\nCondition owner template not found.";
            return false;
        }
        return true;
    }
}