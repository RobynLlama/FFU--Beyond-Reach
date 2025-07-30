using FFU_Beyond_Reach;
using MonoMod;
using Ostranauts.Tools;
using System.Collections.Generic;
using UnityEngine;

public partial class patch_Loot : Loot {
    [MonoModReplace] private List<List<LootUnit>> ParseLootDef(string[] aIn) {
        List<List<LootUnit>> aLootList = new List<List<LootUnit>>();

        // Parse Main Array
        foreach (string strIn in aIn) {
            string[] aSubIn = strIn.Split('|');
            List<LootUnit> aLootSubList = new List<LootUnit>();

            // Parse Sub Array
            foreach (string strSubIn in aSubIn) {
                LootUnit vLootUnit = new LootUnit();
                vLootUnit.bPositive = true;
                string strTrueValue = strSubIn;

                // Negative Entry Test
                if (strIn[0] == '-') {
                    vLootUnit.bPositive = false;
                    strTrueValue = strSubIn.Substring(1);
                }

                // Data Value Safeguards
                string[] aValueData = strTrueValue.Split('=');
                vLootUnit.strName = aValueData[0];
                if (aValueData.Length < 2) {
                    if (FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.ModChanges)
                        Debug.Log($"#Info# Loot entry '{strName}' is for patching only and not saved as permanent data.");
                    return new List<List<LootUnit>>();
                }

                // Value Chance Safeguards
                aValueData = aValueData[1].Split('x');
                float.TryParse(aValueData[0], out vLootUnit.fChance);
                if (vLootUnit.fChance < 0f) {
                    JsonLogger.ReportProblem($"[{strName}] {strSubIn} (loot definition chance can't be negative)", ReportTypes.FailingString);
                    continue;
                }
                if (aValueData.Length < 2) {
                    JsonLogger.ReportProblem($"[{strName}] {strSubIn} (loot definition is shorter than expected)", ReportTypes.FailingString);
                    continue;
                }

                // Value Base Range Parsing
                float fRange = 0f;
                if (aValueData[1].StartsWith("-")) {
                    JsonLogger.ReportProblem($"[{strName}] {strSubIn} (loot definition base value can't be negative)", ReportTypes.FailingString);
                    continue;
                }
                aValueData = aValueData[1].Split('-');
                if (float.TryParse(aValueData[0], out fRange)) {
                    vLootUnit.fMin = fRange;
                }

                // Value Max Range Parsing
                if (aValueData.Length > 1) {
                    fRange = 0f;
                    if (aValueData.Length > 2) {
                        JsonLogger.ReportProblem($"[{strName}] {strSubIn} (loot definition value is longer than expected)", ReportTypes.FailingString);
                        continue;
                    }
                    if (float.TryParse(aValueData[1], out fRange)) {
                        vLootUnit.fMax = fRange;
                    }
                }

                // Value Range Validation
                if (vLootUnit.fMax < vLootUnit.fMin) {
                    vLootUnit.fMax = vLootUnit.fMin;
                }

                // Add Parsed Loot Entry
                aLootSubList.Add(vLootUnit);
            }
            aLootList.Add(aLootSubList);
        }
        return aLootList;
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* Loot.ParseLootDef
private List<List<LootUnit>> ParseLootDef(string[] aIn)
{
	List<List<LootUnit>> list = new List<List<LootUnit>>();
	foreach (string text in aIn)
	{
		string[] array = text.Split('|');
		List<LootUnit> list2 = new List<LootUnit>();
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			LootUnit lootUnit = new LootUnit();
			lootUnit.bPositive = true;
			string text3 = text2;
			if (text[0] == '-')
			{
				lootUnit.bPositive = !lootUnit.bPositive;
				text3 = text2.Substring(1);
			}
			string[] array3 = text3.Split('=');
			lootUnit.strName = array3[0];
			if (array3.Length < 2)
			{
				JsonLogger.ReportProblem(text + " (loot definition shorter than expected)", ReportTypes.FailingString);
				Debug.Log("Missing Loot Chance Data: " + strName);
				continue;
			}
			array3 = array3[1].Split('x');
			float.TryParse(array3[0], out lootUnit.fChance);
			if (lootUnit.fChance == 0f)
			{
				continue;
			}
			if (array3.Length < 2)
			{
				JsonLogger.ReportProblem(text + " (loot definition shorter than expected)", ReportTypes.FailingString);
			}
			array3 = array3[1].Split('-');
			float result = 0f;
			if (float.TryParse(array3[0], out result))
			{
				lootUnit.fMin = result;
			}
			if (array3.Length > 1)
			{
				result = 0f;
				if (float.TryParse(array3[1], out result))
				{
					lootUnit.fMax = result;
				}
			}
			if (lootUnit.fMax < lootUnit.fMin)
			{
				lootUnit.fMax = lootUnit.fMin;
			}
			list2.Add(lootUnit);
		}
		list.Add(list2);
	}
	return list;
}
*/