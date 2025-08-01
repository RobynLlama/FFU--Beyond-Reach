﻿using FFU_Beyond_Reach;
using MonoMod;
using Ostranauts.Ships.AIPilots.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Ostranauts.Ships.Commands {
    public partial class patch_HoldStationAutoPilot : HoldStationAutoPilot {
        [MonoModIgnore] public patch_HoldStationAutoPilot(IAICharacter pilot) : base(pilot) { }
        public CommandCode RunCommand() {
            Ship shipStationKeepingTarget = base.ShipUs.shipStationKeepingTarget;
            if (shipStationKeepingTarget == null || shipStationKeepingTarget.objSS == null || base.ShipUs == null) {
                return CommandCode.Cancelled;
            }
            if (shipStationKeepingTarget.bDestroyed) {
                base.ShipUs.shipStationKeepingTarget = CrewSim.system.GetShipByRegID(shipStationKeepingTarget.strRegID);
                if (base.ShipUs.shipStationKeepingTarget == null) {
                    return CommandCode.Cancelled;
                }
                shipStationKeepingTarget = base.ShipUs.shipStationKeepingTarget;
            } else if ((base.ShipUs.IsDocked() && !(FFU_BR_Defs.TowBraceAllowsKeep && base.ShipUs.TowBraceSecured())) || shipStationKeepingTarget.HideFromSystem) {
                return CommandCode.Finished;
            }
            GUIOrbitDraw gUIOrbitDraw = null;
            GUIDockSys ds = null;
            Dictionary<string, string> dictionary = null;
            if (CrewSim.goUI != null) {
                gUIOrbitDraw = CrewSim.goUI.GetComponent<GUIOrbitDraw>();
                ds = CrewSim.goUI.GetComponent<GUIDockSys>();
            }
            if (gUIOrbitDraw != null) {
                dictionary = gUIOrbitDraw.GetPropMap();
            }
            if (PauseDuringPlayerInput(gUIOrbitDraw, ds)) {
                return CommandCode.Ongoing;
            }
            float num = 0f;
            float num2 = 0f;
            float num3 = 0f;
            float num4 = (float)(shipStationKeepingTarget.objSS.vVelX - base.ShipUs.objSS.vVelX);
            float num5 = (float)(shipStationKeepingTarget.objSS.vVelY - base.ShipUs.objSS.vVelY);
            float num6 = (float)(shipStationKeepingTarget.objSS.vPosx - base.ShipUs.objSS.vPosx);
            float num7 = (float)(shipStationKeepingTarget.objSS.vPosy - base.ShipUs.objSS.vPosy);
            float num8 = Mathf.Cos(base.ShipUs.objSS.fRot);
            float num9 = Mathf.Sin(base.ShipUs.objSS.fRot);
            float num10 = 0f;
            num10 += Mathf.Atan2(num6 * num8 + num7 * num9, (0f - num6) * num9 + num7 * num8) * (-0.2f + -0.3f / Time.timeScale);
            num10 -= base.ShipUs.objSS.fW * 0.3f;
            float num11 = 1f / Time.timeScale;
            num3 = MathUtils.Clamp(num10, 0f - num11, num11);
            float num12 = Mathf.Abs(num4);
            float num13 = Mathf.Abs(num5);
            if ((double)num12 <= 1E-11 && (double)num13 <= 1E-11 && Mathf.Abs(num3) <= 0.05f && base.ShipUs.objSS.fW == 0f) {
                base.ShipUs.Maneuver(0f, 0f, 0f, 0, 1E-10f);
                return CommandCode.Ongoing;
            }
            if ((double)num12 > 1E-11 && num12 > num13) {
                num = ((!(num4 < 0f)) ? 1f : (-1f));
            } else {
                base.ShipUs.objSS.vVelX = shipStationKeepingTarget.objSS.vVelX;
            }
            if ((double)num13 > 1E-11 && num13 > num12) {
                num2 = ((!(num5 < 0f)) ? 1f : (-1f));
            } else {
                base.ShipUs.objSS.vVelY = shipStationKeepingTarget.objSS.vVelY;
            }
            float num14 = num * num8 + num2 * num9;
            float num15 = 0f - (num * num9 - num2 * num8);
            if (dictionary != null) {
                _engineMode = ((!dictionary.TryGetValue("nKnobEngineMode", out var value)) ? 1 : int.Parse(value));
                _throttleSld = ((!dictionary.TryGetValue("slidThrottle", out value)) ? 0.25f : float.Parse(value));
            }
            base.ShipUs.Maneuver(num14 * _throttleSld, num15 * _throttleSld, num3 * _throttleSld, 0, CrewSim.TimeElapsedScaled(), (Ship.EngineMode)_engineMode);
            return CommandCode.Ongoing;
        }
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* HoldStationAutoPilot.RunCommand
public override CommandCode RunCommand()
{
	Ship shipStationKeepingTarget = base.ShipUs.shipStationKeepingTarget;
	if (shipStationKeepingTarget == null || shipStationKeepingTarget.objSS == null || base.ShipUs == null)
	{
		return CommandCode.Cancelled;
	}
	if (shipStationKeepingTarget.bDestroyed)
	{
		base.ShipUs.shipStationKeepingTarget = CrewSim.system.GetShipByRegID(shipStationKeepingTarget.strRegID);
		if (base.ShipUs.shipStationKeepingTarget == null)
		{
			return CommandCode.Cancelled;
		}
		shipStationKeepingTarget = base.ShipUs.shipStationKeepingTarget;
	}
	else if (base.ShipUs.IsDocked() || shipStationKeepingTarget.HideFromSystem)
	{
		return CommandCode.Finished;
	}
	GUIOrbitDraw gUIOrbitDraw = null;
	GUIDockSys ds = null;
	Dictionary<string, string> dictionary = null;
	if (CrewSim.goUI != null)
	{
		gUIOrbitDraw = CrewSim.goUI.GetComponent<GUIOrbitDraw>();
		ds = CrewSim.goUI.GetComponent<GUIDockSys>();
	}
	if (gUIOrbitDraw != null)
	{
		dictionary = gUIOrbitDraw.GetPropMap();
	}
	if (PauseDuringPlayerInput(gUIOrbitDraw, ds))
	{
		return CommandCode.Ongoing;
	}
	float num = 0f;
	float num2 = 0f;
	float num3 = 0f;
	float num4 = (float)(shipStationKeepingTarget.objSS.vVelX - base.ShipUs.objSS.vVelX);
	float num5 = (float)(shipStationKeepingTarget.objSS.vVelY - base.ShipUs.objSS.vVelY);
	float num6 = (float)(shipStationKeepingTarget.objSS.vPosx - base.ShipUs.objSS.vPosx);
	float num7 = (float)(shipStationKeepingTarget.objSS.vPosy - base.ShipUs.objSS.vPosy);
	float num8 = Mathf.Cos(base.ShipUs.objSS.fRot);
	float num9 = Mathf.Sin(base.ShipUs.objSS.fRot);
	float num10 = 0f;
	num10 += Mathf.Atan2(num6 * num8 + num7 * num9, (0f - num6) * num9 + num7 * num8) * (-0.2f + -0.3f / Time.timeScale);
	num10 -= base.ShipUs.objSS.fW * 0.3f;
	float num11 = 1f / Time.timeScale;
	num3 = MathUtils.Clamp(num10, 0f - num11, num11);
	float num12 = Mathf.Abs(num4);
	float num13 = Mathf.Abs(num5);
	if ((double)num12 <= 1E-11 && (double)num13 <= 1E-11 && Mathf.Abs(num3) <= 0.05f && base.ShipUs.objSS.fW == 0f)
	{
		base.ShipUs.Maneuver(0f, 0f, 0f, 0, 1E-10f);
		return CommandCode.Ongoing;
	}
	if ((double)num12 > 1E-11 && num12 > num13)
	{
		num = ((!(num4 < 0f)) ? 1f : (-1f));
	}
	else
	{
		base.ShipUs.objSS.vVelX = shipStationKeepingTarget.objSS.vVelX;
	}
	if ((double)num13 > 1E-11 && num13 > num12)
	{
		num2 = ((!(num5 < 0f)) ? 1f : (-1f));
	}
	else
	{
		base.ShipUs.objSS.vVelY = shipStationKeepingTarget.objSS.vVelY;
	}
	float num14 = num * num8 + num2 * num9;
	float num15 = 0f - (num * num9 - num2 * num8);
	if (dictionary != null)
	{
		_engineMode = ((!dictionary.TryGetValue("nKnobEngineMode", out var value)) ? 1 : int.Parse(value));
		_throttleSld = ((!dictionary.TryGetValue("slidThrottle", out value)) ? 0.25f : float.Parse(value));
	}
	base.ShipUs.Maneuver(num14 * _throttleSld, num15 * _throttleSld, num3 * _throttleSld, 0, CrewSim.TimeElapsedScaled(), (Ship.EngineMode)_engineMode);
	return CommandCode.Ongoing;
}
*/