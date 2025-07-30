using MonoMod;

public partial class patch_Slot : Slot {
    [MonoModReplace] public bool CanFit(CondOwner coFit, bool bAuto = true) {
        if (aCOs == null) return false;
        foreach (CondOwner condOwner in aCOs) {
            if (bHoldSlot && condOwner == null &&
                coFit.mapSlotEffects.ContainsKey(strName) && 
                (!bAuto || CanAutoSlot(coFit))) {
                return true;
            }
            CondOwner coParent = condOwner;
            while (coParent != null) {
                if (coFit == coParent) return false;
                coParent = coParent.objCOParent;
            }
            if (condOwner != null && condOwner.objContainer != null &&
                (condOwner.objContainer.ctAllowed == null ||
                condOwner.objContainer.ctAllowed.Triggered(coFit)) &&
                condOwner.objContainer.GetSpaceAvailable() > 0) {
                return true;
            }
        }
        return false;
    }
}

public partial class patch_Container : Container {
    [MonoModReplace] public bool AllowedCO(CondOwner coIn) {
        if (coIn == null || coIn == CO)
            return false;
        CondOwner coParent = CO;
        while (coParent != null) {
            if (coIn == coParent) return false;
            coParent = coParent.objCOParent;
        }
        if (ctAllowed != null)
            return ctAllowed.Triggered(coIn);
        return true;
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* Slot.CanFit
public bool CanFit(CondOwner coFit, bool bAuto = true)
{
	if (aCOs == null)
	{
		return false;
	}
	CondOwner[] array = aCOs;
	foreach (CondOwner condOwner in array)
	{
		if (bHoldSlot && condOwner == null && coFit.mapSlotEffects.ContainsKey(strName) && (!bAuto || CanAutoSlot(coFit)))
		{
			return true;
		}
		if (condOwner != null && condOwner.objContainer != null && (condOwner.objContainer.ctAllowed == null || condOwner.objContainer.ctAllowed.Triggered(coFit)) && condOwner.objContainer.GetSpaceAvailable() > 0)
		{
			return true;
		}
	}
	return false;
}
*/

/* Container.AllowedCO
public bool AllowedCO(CondOwner coIn)
{
	if (coIn == null || coIn == CO)
	{
		return false;
	}
	CondOwner objCOParent = CO.objCOParent;
	while (objCOParent != null)
	{
		if (coIn == objCOParent)
		{
			return false;
		}
		objCOParent = objCOParent.objCOParent;
	}
	if (ctAllowed != null)
	{
		return ctAllowed.Triggered(coIn);
	}
	return true;
}
*/