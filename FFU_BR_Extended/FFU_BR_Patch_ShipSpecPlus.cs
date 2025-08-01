﻿public partial class patch_JsonShipSpec : JsonShipSpec {
    public int nIsSameShipCO { get; set; }
    public extern bool orig_Matches(Ship ship, CondOwner coUs = null);
    public bool Matches(Ship ship, CondOwner coUs = null) {
        bool rShipMatch = orig_Matches(ship, coUs);
        if (rShipMatch) {
            if (nIsSameShipCO != 0) {
                if (coUs == null) return false;
                if (!IntMatchesBool(nIsSameShipCO, coUs.ship == ship)) {
                    return false;
                }
            }
        }
        return rShipMatch;
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* JsonShipSpec.Matches
public bool Matches(Ship ship, CondOwner coUs = null)
{
	if (ship == null || ship.bDestroyed)
	{
		return false;
	}
	if (aDMGStatus != null && aDMGStatus.Length > 0 && Array.IndexOf(aDMGStatus, (int)ship.DMGStatus) < 0)
	{
		return false;
	}
	if (aFactions != null && aFactions.Length > 0)
	{
		bool flag = false;
		List<JsonFaction> shipFactions = ship.GetShipFactions();
		string[] array = aFactions;
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				flag = true;
				break;
			}
			if (text == "[us]")
			{
				if (!(coUs == null) && coUs.SharesFactionsWith(shipFactions))
				{
					flag = true;
					break;
				}
			}
			else if (!ship.HasFaction(text))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
	}
	if (aOwners != null && aOwners.Length > 0)
	{
		bool flag2 = false;
		string shipOwner = CrewSim.system.GetShipOwner(ship.strRegID);
		string[] array2 = aOwners;
		foreach (string text2 in array2)
		{
			if (string.IsNullOrEmpty(text2))
			{
				flag2 = true;
				break;
			}
			if (text2 == "[us]")
			{
				if (!(coUs == null) && !(coUs.strID != shipOwner))
				{
					flag2 = true;
					break;
				}
			}
			else if (!(text2 != shipOwner))
			{
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			return false;
		}
	}
	if (nIsAIShip != 0 && !IntMatchesBool(nIsAIShip, ship.IsLocalAuthority))
	{
		return false;
	}
	if (nIsDocked != 0 && !IntMatchesBool(nIsDocked, ship.IsDockedFull()))
	{
		return false;
	}
	if (nIsFlyingDark != 0 && !IntMatchesBool(nIsFlyingDark, ship.IsFlyingDark()))
	{
		return false;
	}
	if (nIsHidden != 0 && !IntMatchesBool(nIsHidden, ship.HideFromSystem))
	{
		return false;
	}
	if (nIsLocalAuthority != 0 && !IntMatchesBool(nIsLocalAuthority, ship.IsLocalAuthority))
	{
		return false;
	}
	if (nIsPlayerShip != 0 && !IntMatchesBool(nIsPlayerShip, ship.IsPlayerShip()))
	{
		return false;
	}
	if (nIsStation != 0 && !IntMatchesBool(nIsStation, ship.IsStation()))
	{
		return false;
	}
	if (nIsStationOrHidden != 0 && !IntMatchesBool(nIsStationOrHidden, ship.IsStation() || ship.IsStationHidden()))
	{
		return false;
	}
	if (nIsStationRegional != 0 && !IntMatchesBool(nIsStationRegional, ship.objSS != null && ship.objSS.bIsRegion))
	{
		return false;
	}
	if (nIsStationHidden != 0 && !IntMatchesBool(nIsStationHidden, ship.IsStationHidden()))
	{
		return false;
	}
	if (nLoadState != 0)
	{
		if (nLoadState > 0 && ship.LoadState < Ship.Loaded.Edit)
		{
			return false;
		}
		if (nLoadState < 0 && ship.LoadState >= Ship.Loaded.Edit)
		{
			return false;
		}
	}
	if (strDockedWith != null)
	{
		JsonShipSpec shipSpec = DataHandler.GetShipSpec(strDockedWith);
		if (shipSpec != null && !shipSpec.Matches(ship, coUs))
		{
			return false;
		}
	}
	if (!string.IsNullOrEmpty(strLootRegIDs))
	{
		List<string> lootNames = DataHandler.GetLoot(strLootRegIDs).GetLootNames();
		if (lootNames.Count >= 1 && !lootNames.Contains(ship.strRegID))
		{
			return false;
		}
	}
	if (!string.IsNullOrEmpty(strLootATCRegions))
	{
		List<string> lootNames2 = DataHandler.GetLoot(strLootATCRegions).GetLootNames();
		Ship nearestStationRegional = CrewSim.system.GetNearestStationRegional(ship.objSS.vPosx, ship.objSS.vPosy);
		if (lootNames2.Count > 0 && (nearestStationRegional == null || !lootNames2.Contains(nearestStationRegional.strRegID)))
		{
			return false;
		}
	}
	if (!string.IsNullOrEmpty(strCTTest))
	{
		if (ship.ShipCO == null)
		{
			return false;
		}
		CondTrigger condTrigger = DataHandler.GetCondTrigger(strCTTest);
		if (!condTrigger.Triggered(ship.ShipCO))
		{
			return false;
		}
	}
	return true;
}
*/