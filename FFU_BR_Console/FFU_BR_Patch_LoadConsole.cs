public partial class patch_ConsoleResolver : ConsoleResolver {
    public static extern bool orig_ResolveString(ref string strInput);
    public static bool ResolveString(ref string strInput) {
        strInput = strInput.Trim();
        string[] array = strInput.Split(' ');
        array[0] = array[0].ToLower();
        switch (array[0]) {
            case "findcondcos": return KeywordFindCondCOs(ref strInput);
            case "repairship": return KeywordRepairShip(ref strInput);
            case "openinventory": return KeywordOpenInventory(ref strInput);
            case "triggerinfo": return KeywordCondTrigInfo(ref strInput);
            case "triggertest": return KeywordCondTrigTest(ref strInput);
            default: return orig_ResolveString(ref strInput);
        }
    }

    private extern static bool orig_KeywordHelp(ref string strInput, string[] strings);
    private static bool KeywordHelp(ref string strInput, string[] strings) {
        if (strings.Length == 1) {
            strInput += "\nWelcome to the Ostranauts console.";
            strInput += "\nAvailable Commands:";
            strInput += "\nhelp";
            strInput += "\necho";
            strInput += "\ncrewsim";
            strInput += "\naddcond";
            strInput += "\ngetcond (FFU modified)";
            strInput += "\nspawn";
            strInput += "\nunlockdebug";
            strInput += "\nbugform";
            strInput += "\nclear";
            strInput += "\nverify";
            strInput += "\nkill";
            strInput += "\naddcrew";
            strInput += "\naddnpc";
            strInput += "\ndamageship";
            strInput += "\nbreakinship";
            strInput += "\noxygen";
            strInput += "\nmeteor";
            strInput += "\nlookup";
            strInput += "\ntoggle";
            strInput += "\nship";
            strInput += "\nrel";
            strInput += "\nsummon";
            strInput += "\nskywalk";
            strInput += "\nplot";
            strInput += "\nmeatstate";
            strInput += "\nrename";
            strInput += "\nfindcondcos (FFU only)";
            strInput += "\nrepairship (FFU only)";
            strInput += "\nopeninventory (FFU only)";
            strInput += "\ntriggerinfo (FFU only)";
            strInput += "\ntriggertest (FFU only)";
            strInput += "\n\ntype command name after help to see more details about command";
            strInput += "\n";
            return true;
        }
        if (strings.Length == 2) {
            switch (strings[1]) {
                case "getcond":
                    strInput += "\ngetcond lists a condition's value on a condowner";
                    strInput += "\ne.g. 'getcond Joshu IsHuman'";
                    strInput += "\nwill find condowner with name/friendlyName/ID of 'Joshu'";
                    strInput += "\nwill check for all condtions including partial string 'IsHuman'";
                    strInput += "\nif condowner is valid and conditions are found, it will list their current names and values on Joshu";
                    strInput += "\n<i>spaces within names must be replaced with underscores: '_'</i>";
                    strInput += "\n'getcond *' will list all stats that condowner has";
                    strInput += "\n'getcond-NUM' will execute command for NUM parent of the condowner";
                    strInput += "\n'getcond *coParents' will list all parents of the condowner";
                    strInput += "\n'getcond *coRules' will list all rules attached to the condowner";
                    strInput += "\n'getcond *coTickers' will list all tickers attached to the condowner";
                    strInput += "\n";
                    return true;
                case "findcondcos":
                    strInput += "\nfindcondcos lists all condowner templates that meet listed conditions";
                    strInput += "\nworks from main menu and can support any amount of conditions";
                    strInput += "\n'IsCondition' will only list templates that have that condition";
                    strInput += "\n'!IsCondition' will only list templates that don't have that condition";
                    strInput += "\n'using both will only list templates that meet both conditions";
                    strInput += "\ne.g. 'findcondcos IsScrap !IsFlexible'";
                    return true;
                case "repairship":
                    strInput += "\nrepairship zeroes all StatDamage values on all IsInstalled condowners of all loaded ships";
                    return true;
                case "openinventory":
                    strInput += "\nopeninventory opens inventory window from the perspective of the selected condowner";
                    strInput += "\ne.g. select target with right mouse button and enter 'openinventory' command";
                    return true;
                case "triggerinfo":
                    strInput += "\ntriggerinfo shows rules of condition trigger either as JSON (0) or as friendly (1) text";
                    strInput += "\ne.g. triggerinfo TIsYourConditionTriggerName 0";
                    strInput += "\nor triggerinfo TIsYourConditionTriggerName 1";
                    return true;
                case "triggertest":
                    strInput += "\ntriggertest fires condition trigger against template or selected object and logs outcome";
                    strInput += "\n<i>works only if there is initialized CrewSim instance (i.e. only when game was loaded)</i>";
                    strInput += "\ne.g. triggertest TIsYourConditionTriggerName [them] (only in-game and with selected object)";
                    strInput += "\ne.g. triggertest TIsYourConditionTriggerName ItmYourCondownerName";
                    return true;
                default: return orig_KeywordHelp(ref strInput, strings);
            }
        }
        return false;
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* ConsoleResolver.ResolveString
public static bool ResolveString(ref string strInput)
{
	strInput.Trim();
	string[] array = strInput.Split(' ');
	array[0] = array[0].ToLower();
	switch (array[0])
	{
	case "help":
		if (KeywordHelp(ref strInput, array))
		{
			return true;
		}
		return false;
	case "echo":
		strInput = strInput + "\n" + strInput.Remove(0, 5);
		return true;
	case "unlockdebug":
		return KeywordUnlockDebug(ref strInput, array);
	case "crewsim":
		return KeywordCrewSim(ref strInput, array);
	case "addcond":
		return KeywordAddCond(ref strInput, array);
	case "getcond":
		return KeywordGetCond(ref strInput, array);
	case "bugform":
		return KeywordBugForm(ref strInput);
	case "spawn":
		return KeywordSpawn(ref strInput, array);
	case "verify":
		return KeywordVerify(ref strInput);
	case "kill":
		return KeywordKill(ref strInput, array);
	case "addcrew":
		return KeywordAddCrew(ref strInput, array, makeCrew: true);
	case "addnpc":
		return KeywordAddCrew(ref strInput, array);
	case "damageship":
		return KeywordDamageShip(ref strInput, array);
	case "breakinship":
		return KeywordBreakInShip(ref strInput, array);
	case "meteor":
		return KeywordMeteor(ref strInput, array);
	case "oxygen":
		return KeywordOxygen(ref strInput, array);
	case "toggle":
		return KeywordToggle(ref strInput, array);
	case "ship":
		return KeywordShip(ref strInput, array);
	case "shipvis":
		return KeywordShipVis(ref strInput, array);
	case "lookup":
		return KeywordLookup(ref strInput, array);
	case "plot":
		return KeywordPlot(ref strInput, array);
	case "summon":
		return KeywordSummon(ref strInput, array);
	case "rel":
		return KeywordRelationship(ref strInput, array);
	case "skywalk":
		return KeywordSkywalk(ref strInput, array);
	case "detach":
		return KeywordDetach(ref strInput, array);
	case "attach":
		return KeywordAttach(ref strInput, array);
	case "meatstate":
		return KeywordMeatState(ref strInput, array);
	case "priceflips":
		return KeywordPriceFlips(ref strInput, array);
	case "rename":
		return KeywordRename(ref strInput, array);
	case "clear":
	case "clr":
		return KeywordClear(ref strInput, array);
	default:
		strInput += "\nFailed to recognise command.";
		return false;
	}
}
*/

/* ConsoleResolver.KeywordHelp
private static bool KeywordHelp(ref string strInput, string[] strings)
{
	if (strings.Length == 1)
	{
		strInput += "\nWelcome to the Ostranauts console.";
		strInput += "\nAvailable Commands:";
		strInput += "\nhelp";
		strInput += "\necho";
		strInput += "\ncrewsim";
		strInput += "\naddcond";
		strInput += "\ngetcond";
		strInput += "\nspawn";
		strInput += "\nunlockdebug";
		strInput += "\nbugform";
		strInput += "\nclear";
		strInput += "\nverify";
		strInput += "\nkill";
		strInput += "\naddcrew";
		strInput += "\naddnpc";
		strInput += "\ndamageship";
		strInput += "\nbreakinship";
		strInput += "\noxygen";
		strInput += "\nmeteor";
		strInput += "\nlookup";
		strInput += "\ntoggle";
		strInput += "\nship";
		strInput += "\nrel";
		strInput += "\nsummon";
		strInput += "\nskywalk";
		strInput += "\nplot";
		strInput += "\nmeatstate";
		strInput += "\nrename";
		strInput += "\n\ntype command name after help to see more details about command";
		strInput += "\n";
		return true;
	}
	if (strings.Length == 2)
	{
		switch (strings[1])
		{
		case "help":
			strInput += "\nhelp explains which commands are available and what they do";
			strInput += "\n<i>you seem to have figured this one out already</i>";
			strInput += "\n";
			return true;
		case "echo":
			strInput += "\necho repeats the text given.";
			strInput += "\nthis serves as a test of the console command resolver itself";
			strInput += "\ne.g. 'echo hello world' returns 'hello world'";
			strInput += "\n";
			return true;
		case "crewsim":
			strInput += "\ncrewsim shows how long crewsim has been running for";
			strInput += "\n<i>will not find an instance if in main menu</i>";
			strInput += "\n";
			return true;
		case "addcond":
			strInput += "\naddcond adds a condition to a condowner";
			strInput += "\ne.g. 'addcond Joshu IsHuman 1.0'";
			strInput += "\nwill find condowner with name/friendlyName/ID of 'Joshu'";
			strInput += "\nwill check the validity of the condtion 'IsHuman'";
			strInput += "\nif both are valid it will add 1.0 IsHuman to Joshu";
			strInput += "\n<i>spaces within names must be replaced with underscores: '_'</i>";
			strInput += "\n";
			return true;
		case "getcond":
			strInput += "\ngetcond lists a condition's value on a condowner";
			strInput += "\ne.g. 'getcond Joshu IsHuman'";
			strInput += "\nwill find condowner with name/friendlyName/ID of 'Joshu'";
			strInput += "\nwill check for all condtions including partial string 'IsHuman'";
			strInput += "\nif condowner is valid and conditions are found, it will list their current names and values on Joshu";
			strInput += "\n<i>spaces within names must be replaced with underscores: '_'</i>";
			strInput += "\n";
			return true;
		case "unlockdebug":
			strInput += "\nunlockdebug unlocks special debug hotkeys";
			strInput += "\nalso allows for the debug overlay to be enabled";
			strInput = strInput + "\nDebug overlay hotkey is: " + GUIActionKeySelector.commandDebug.KeyName;
			return true;
		case "bugform":
			strInput += "\ndisplays a link to the form to submit bugs";
			return true;
		case "clear":
			strInput += "\nclears the log of past lines";
			strInput += "\nleaving blank will clear all previous lines";
			strInput += "\noptional param number will clear that many previous lines from the bottom of the log";
			strInput += "\ne.g. 'clear 5'";
			return true;
		case "spawn":
			strInput += "\nspawns a given loot into the game";
			strInput += "\ntries to put it into the player's inventory or onto the ground next to the player";
			strInput += "\ne.g. 'spawn ItmAICargo01'";
			return true;
		case "verify":
			strInput += "\nverifies game json files";
			return true;
		case "kill":
			strInput += "\nkills given CO";
			strInput += "\nadds Death condition to CO with matching name (Replace spaces in human names with _)";
			strInput += "\ne.g. 'kill Joshu_Lastname'";
			return true;
		case "addcrew":
			strInput += "\nadds randomly selected crew to current ship";
			strInput += "\ncan spawn multiple random crew at once with optional number at the end";
			strInput += "\ne.g. 'addcrew 3'";
			return true;
		case "addnpc":
			strInput += "\nadds a randomly selected npc to current ship";
			strInput += "\ncan spawn multiple random npcs at once with optional number at the end";
			strInput += "\ne.g. 'addnpc 3'";
			return true;
		case "meteor":
			strInput += "\nhits the ship with meteor";
			strInput += "\ncan spawn multiple meteors at once with optional number at the end";
			strInput += "\ne.g. 'meteor 4'";
			return true;
		case "damageship":
			strInput += "\nadds random damage to all tiles on current ship";
			strInput += "\nnumber specifies max amount of random damage to apply";
			strInput += "\ne.g. 'damageship 0.3' = random upto 30% damage to all items";
			return true;
		case "breakinship":
			strInput += "\nperforms standard derelict break-in pass on current ship";
			strInput += "\nnumber specifies max amount of random damage to apply";
			strInput += "\ne.g. 'damageship 0.3' = up to 30% damage to all items, 30% of valuables removed, etc.";
			return true;
		case "oxygen":
			strInput += "\nadds oxygen to all people on current ship";
			strInput += "\nnumber specifies amount of oxygen to add to each person";
			strInput += "\ne.g. 'oxygen 1.5'";
			return true;
		case "lookup":
			strInput += "\nallows the user to look up ships and plots";
			strInput += "\nuse the followup keyword 'ships' or 'plots' to get data";
			strInput += "\ne.g. 'lookup ships'";
			return true;
		case "toggle":
			strInput += "\nallows the user to toggle certain game settings";
			strInput += "\ne.g. 'toggle aoshow' turns ambient occlusion on or off";
			strInput += "\ne.g. 'toggle aozoom' swaps ambient occlusion between screen space and world space";
			strInput += "\ne.g. 'toggle aospread 11.25' allows numeric tuning of ambient occlusion spread/width";
			strInput += "\ne.g. 'toggle aointensity 0.66' allows numeric tuning of how dark ambient occlusion appears";
			return true;
		case "ship":
			strInput += "\nallows the user to spawn in and teleport to a specific ship";
			strInput += "\ne.g. 'ship Volatile Aero' will spawn in the Volatile Aero ship";
			return true;
		case "rel":
			strInput += "\nallows the user to set social relationships between humans";
			strInput += "\ne.g. 'rel condition elvis_presley' adds a condition to the player/selected NPC's relationship with Elvis Presley";
			strInput += "\ne.g. 'rel condition elvis_presley joanna_dark' adds a condition to the relationship between Elvis Presley and Joanna Dark";
			return true;
		case "plot":
			strInput += "\nallows the user to fast forward through a specific named plot, spawning appropriate quest givers and passing quest flags";
			strInput += "\ne.g. 'plot Messenger' starts the Messenger quest chain, spawning a Messenger questgiver on the current ship";
			return true;
		case "skywalk":
			strInput += "\nallows the user to teleport to a named ship ID";
			strInput += "\ne.g. 'skywalk OKLG' teleports the player to K-LEG.";
			return true;
		case "summon":
			strInput += "\nallows the user to summon an existing NPC from another ship to the current ship";
			strInput += "\ne.g. 'summon elvis_presley' teleports Elvis Presley to the current ship.";
			return true;
		case "meatstate":
			strInput += "\nallows player to check and change the current state of meat in their game";
			strInput += "\nuse alone to check status or put numeric value after keyword to change value";
			strInput += "\n0 = Inert (Meat does nothing).";
			strInput += "\n1 = Dormant (Meat does nothing until fought).";
			strInput += "\n2 = Spread (Meat spreads and fights).";
			strInput += "\n3 = Decay (Meat spreads fights and slowly dies).";
			strInput += "\n4 = Eradicate (Meat quickly dies).";
			strInput += "\n5 = Hell (Meat gets 2 actions instead of 1).";
			return true;
		case "rename":
			strInput += "\nallows player to rename selected items";
			strInput += "\ne.g. 'rename Naiba' will change the item's friendly and short names to 'Naiba'";
			return true;
		default:
			strInput += "\ncannot give help, command name not recognised";
			strInput += "\n";
			return true;
		}
	}
	return false;
}
*/