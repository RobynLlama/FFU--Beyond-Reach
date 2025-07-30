using MonoMod;
using Ostranauts.UI.MegaToolTip;
using System.Linq;
using UnityEngine;

public partial class patch_ConsoleResolver : ConsoleResolver {
    public static bool bInvokedInventory = false;
    private static bool KeywordOpenInventory(ref string strInput) {
        if (CrewSim.objInstance == null) {
            strInput += "\nCrewSim instance not found.";
            return false;
        }
        if (GUIMegaToolTip.Selected == null) {
            strInput += "\nNo target selected or highlighted.";
            return false;
        }
        var coTarget = GUIMegaToolTip.Selected;
        if (Container.GetSpace(coTarget) < 1 &&
            !coTarget.compSlots.aSlots.Any()) {
            strInput += "\nTarget is not valid or has no inventory.";
            return false;
        }
        strInput += $"\nAccessed inventory: {coTarget.FriendlyName} ({coTarget.strName}) {coTarget.strID}";
        if (coTarget.HasCond("IsHuman") || coTarget.HasCond("IsRobot"))
            CommandInventory.ToggleInventory(coTarget);
        else {
            CrewSim.inventoryGUI.SpawnInventoryWindow(coTarget, InventoryWindowType.Container, true);
            CrewSim.inventoryGUI.SpawnInventoryWindow(coTarget, InventoryWindowType.Ground, true);
        }
        bInvokedInventory = true;
        return true;
    }
}

public partial class patch_GUIInventoryWindow : GUIInventoryWindow {
    [MonoModReplace] public Vector3 WorldPosFromPair(PairXY where) {
        if (type == InventoryWindowType.Ground) {
            Vector3 position = patch_ConsoleResolver.bInvokedInventory && 
                GUIMegaToolTip.Selected != null ? GUIMegaToolTip.Selected.tf.position :
                CrewSim.GetSelectedCrew().tf.position;
            position.x = MathUtils.RoundToInt(position.x) + where.x - 2;
            position.y = MathUtils.RoundToInt(position.y) + 2 - where.y;
            return position;
        }
        return CO.tf.position;
    }
}

public partial class patch_GUIInventory : GUIInventory {
    public extern void orig_Reset(GUIInventoryWindow window = null);
    public void Reset(GUIInventoryWindow window = null) {
        patch_ConsoleResolver.bInvokedInventory = false;
        orig_Reset(window);
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* GUIInventoryWindow.WorldPosFromPair
public Vector3 WorldPosFromPair(PairXY where)
{
	if (type == InventoryWindowType.Ground)
	{
		Vector3 position = CrewSim.GetSelectedCrew().tf.position;
		position.x = MathUtils.RoundToInt(position.x) + where.x - 2;
		position.y = MathUtils.RoundToInt(position.y) + 2 - where.y;
		return position;
	}
	return CO.tf.position;
}
*/

/* GUIInventory.Reset
public void Reset(GUIInventoryWindow window = null)
{
	if (Selected != null && window == null)
	{
		Selected.CleanUpCursorItem();
		Selected = null;
		UnsetDoll();
	}
	GUIInventoryItem[] array = ((!(window == null)) ? window.GetComponentsInChildren<GUIInventoryItem>(includeInactive: true) : GetComponentsInChildren<GUIInventoryItem>(includeInactive: true));
	if (array != null)
	{
		for (int num = array.Length - 1; num >= 0; num--)
		{
			Object.Destroy(array[num].gameObject);
		}
	}
}
*/