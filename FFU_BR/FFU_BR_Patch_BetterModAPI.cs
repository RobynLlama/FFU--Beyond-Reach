using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FFU_Beyond_Reach;
using LitJson;
using MonoMod;
using Ostranauts.Core;
using Ostranauts.Ships.Rooms;
using Ostranauts.Tools;
using Ostranauts.Trading;
using UnityEngine;

public partial class patch_JsonModInfo : JsonModInfo
{
    public Dictionary<string, string[]> removeIds { get; set; }
    public Dictionary<string, Dictionary<string, string[]>> changesMap { get; set; }
}

public static partial class patch_DataHandler
{
    public static string strModsPath = string.Empty;
    public static Dictionary<string, Dictionary<string, List<string>>> dictChangesMap;
    public static List<string> listLockedCOs = new List<string>();
    [MonoModReplace]
    public static void Init()
    {
        // Early Access Build Info
        try
        {
            Debug.Log("#Info# Getting build info.");
            TextAsset textAsset = (TextAsset)Resources.Load("version", typeof(TextAsset));
            DataHandler.strBuild = "Early Access Build: " + textAsset.text;
            Debug.Log(DataHandler.strBuild);
        }
        catch (Exception ex)
        {
            Debug.Log("" + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
        }

        // Custom Mod Configuration
        FFU_BR_Defs.InitConfig();
        List<ModInformation> modQueuedPaths = [];

        // Initializing Data Variables
        DataHandler.strAssetPath = Application.streamingAssetsPath + "/";
        DataHandler.dictImages = new Dictionary<string, Texture2D>();
        DataHandler.dictColors = new Dictionary<string, Color>();
        DataHandler.dictHTMLColors = new Dictionary<string, string>();
        DataHandler.dictJsonColors = new Dictionary<string, JsonColor>();
        DataHandler.dictLights = new Dictionary<string, JsonLight>();
        DataHandler.dictShips = new Dictionary<string, JsonShip>();
        DataHandler.dictShipImages = new Dictionary<string, Dictionary<string, Texture2D>>();
        DataHandler.dictConds = new Dictionary<string, JsonCond>();
        DataHandler.dictItemDefs = new Dictionary<string, JsonItemDef>();
        DataHandler.dictCTs = new Dictionary<string, CondTrigger>();
        DataHandler.dictCOs = new Dictionary<string, JsonCondOwner>();
        DataHandler.dictDataCoCollections = new Dictionary<string, DataCoCollection>();
        DataHandler.dictCOSaves = new Dictionary<string, JsonCondOwnerSave>();
        DataHandler.dictInteractions = new Dictionary<string, JsonInteraction>();
        DataHandler.dictLoot = new Dictionary<string, Loot>();
        DataHandler.dictProductionMaps = new Dictionary<string, JsonProductionMap>();
        DataHandler.dictMarketConfigs = new Dictionary<string, JsonMarketActorConfig>();
        DataHandler.dictCargoSpecs = new Dictionary<string, JsonCargoSpec>();
        DataHandler.dictGasRespires = new Dictionary<string, JsonGasRespire>();
        DataHandler.dictPowerInfo = new Dictionary<string, JsonPowerInfo>();
        DataHandler.dictGUIPropMaps = new Dictionary<string, Dictionary<string, string>>();
        DataHandler.dictNamesFirst = new Dictionary<string, string>();
        DataHandler.dictNamesLast = new Dictionary<string, string>();
        DataHandler.dictNamesRobots = new Dictionary<string, string>();
        DataHandler.dictNamesFull = new Dictionary<string, string>();
        DataHandler.dictNamesShip = new Dictionary<string, string>();
        DataHandler.dictNamesShipAdjectives = new Dictionary<string, string>();
        DataHandler.dictNamesShipNouns = new Dictionary<string, string>();
        DataHandler.dictManPages = new Dictionary<string, string[]>();
        DataHandler.dictHomeworlds = new Dictionary<string, JsonHomeworld>();
        DataHandler.dictCareers = new Dictionary<string, JsonCareer>();
        DataHandler.dictLifeEvents = new Dictionary<string, JsonLifeEvent>();
        DataHandler.dictPersonSpecs = new Dictionary<string, JsonPersonSpec>();
        DataHandler.dictShipSpecs = new Dictionary<string, JsonShipSpec>();
        DataHandler.dictTraitScores = new Dictionary<string, int[]>();
        DataHandler.dictRoomSpec = new Dictionary<string, RoomSpec>();
        DataHandler.dictStrings = new Dictionary<string, string>();
        DataHandler.dictSlotEffects = new Dictionary<string, JsonSlotEffects>();
        DataHandler.dictSlots = new Dictionary<string, JsonSlot>();
        DataHandler.dictTickers = new Dictionary<string, JsonTicker>();
        DataHandler.dictCondRules = new Dictionary<string, CondRule>();
        DataHandler.dictMaterials = new Dictionary<string, Material>();
        DataHandler.dictAudioEmitters = new Dictionary<string, JsonAudioEmitter>();
        DataHandler.dictCrewSkins = new Dictionary<string, string>();
        DataHandler.dictAds = new Dictionary<string, JsonAd>();
        DataHandler.dictHeadlines = new Dictionary<string, JsonHeadline>();
        DataHandler.dictMusicTags = new Dictionary<string, List<string>>();
        DataHandler.dictMusic = new Dictionary<string, JsonMusic>();
        DataHandler.dictComputerEntries = new Dictionary<string, JsonComputerEntry>();
        DataHandler.dictCOOverlays = new Dictionary<string, JsonCOOverlay>();
        DataHandler.dictDataCOs = new Dictionary<string, DataCO>();
        DataHandler.dictLedgerDefs = new Dictionary<string, JsonLedgerDef>();
        DataHandler.dictPledges = new Dictionary<string, JsonPledge>();
        DataHandler.dictJobitems = new Dictionary<string, JsonJobItems>();
        DataHandler.dictJobs = new Dictionary<string, JsonJob>();
        DataHandler.dictSettings = new Dictionary<string, JsonUserSettings>();
        DataHandler.dictModList = new Dictionary<string, JsonModList>();
        DataHandler.dictModInfos = new Dictionary<string, JsonModInfo>();
        DataHandler.aModPaths = new List<string>();
        DataHandler.dictInstallables2 = new Dictionary<string, JsonInstallable>();
        DataHandler.dictAIPersonalities = new Dictionary<string, JsonAIPersonality>();
        DataHandler.dictTransit = new Dictionary<string, JsonTransit>();
        DataHandler.dictPlotManager = new Dictionary<string, JsonPlotManagerSettings>();
        DataHandler.dictStarSystems = new Dictionary<string, JsonStarSystemSave>();
        DataHandler.dictParallax = new Dictionary<string, JsonParallax>();
        DataHandler.dictContext = new Dictionary<string, JsonContext>();
        DataHandler.dictChargeProfiles = new Dictionary<string, JsonChargeProfile>();
        DataHandler.dictWounds = new Dictionary<string, JsonWound>();
        DataHandler.dictAModes = new Dictionary<string, JsonAttackMode>();
        DataHandler.dictPDAAppIcons = new Dictionary<string, JsonPDAAppIcon>();
        DataHandler.dictZoneTriggers = new Dictionary<string, JsonZoneTrigger>();
        DataHandler.dictTips = new Dictionary<string, JsonTip>();
        DataHandler.dictCrimes = new Dictionary<string, JsonCrime>();
        DataHandler.dictPlots = new Dictionary<string, JsonPlot>();
        DataHandler.dictPlotBeats = new Dictionary<string, JsonPlotBeat>();
        DataHandler.dictRaceTracks = new Dictionary<string, JsonRaceTrack>();
        DataHandler.dictRacingLeagues = new Dictionary<string, JsonRacingLeague>();
        DataHandler.dictInfoNodes = new Dictionary<string, JsonInfoNode>();
        DataHandler.dictInstallables = new Dictionary<string, JsonInstallable>();
        DataHandler.dictIAOverrides = new Dictionary<string, JsonInteractionOverride>();
        DataHandler.dictPlotBeatOverrides = new Dictionary<string, JsonPlotBeatOverride>();
        DataHandler.dictJsonVerbs = new Dictionary<string, JsonVerbs>();
        DataHandler.dictVerbs = new Dictionary<string, string[]>();
        DataHandler.dictJsonTokens = new Dictionary<string, JsonCustomTokens>();
        DataHandler.listCustomTokens = new List<string>();
        DataHandler.dictSimple = new Dictionary<string, JsonSimple>();
        DataHandler.dictGUIPropMapUnparsed = new Dictionary<string, JsonGUIPropMap>();
        DataHandler.mapCOs = new Dictionary<string, CondOwner>();

        // Initializing Modded Variables
        dictChangesMap = new Dictionary<string, Dictionary<string, List<string>>>();

        // Initializing Object Reader
        if ((bool)ObjReader.use)
        {
            ObjReader.use.scaleFactor = new Vector3(0.0625f, 0.0625f, 0.0625f);
            ObjReader.use.objRotation = new Vector3(90f, 0f, 180f);
        }
        DataHandler._interactionObjectTracker = new InteractionObjectTracker();

        // Loading User Settings
        DataHandler.dictSettings["DefaultUserSettings"] = new JsonUserSettings();
        DataHandler.dictSettings["DefaultUserSettings"].Init();
        if (File.Exists(Application.persistentDataPath + "/settings.json"))
        {
            DataHandler.JsonToData(Application.persistentDataPath + "/settings.json", DataHandler.dictSettings);
        }
        else
        {
            Debug.LogWarning("WARNING: settings.json not found. Resorting to default values.");
            DataHandler.dictSettings["UserSettings"] = new JsonUserSettings();
            DataHandler.dictSettings["UserSettings"].Init();
        }
        if (!DataHandler.dictSettings.ContainsKey("UserSettings") || DataHandler.dictSettings["UserSettings"] == null)
        {
            Debug.LogError("ERROR: Malformed settings.json. Resorting to default values.");
            DataHandler.dictSettings["UserSettings"] = new JsonUserSettings();
            DataHandler.dictSettings["UserSettings"].Init();
        }
        DataHandler.dictSettings["DefaultUserSettings"].CopyTo(DataHandler.GetUserSettings());
        DataHandler.dictSettings.Remove("DefaultUserSettings");
        DataHandler.SaveUserSettings();

        // Mod List Initialization
        bool isModded = false;
        DataHandler.strModFolder = DataHandler.dictSettings["UserSettings"].strPathMods;
        if (DataHandler.strModFolder == null || DataHandler.strModFolder == string.Empty)
        {
            DataHandler.strModFolder = Path.Combine(Application.dataPath, "Mods/");
        }
        strModsPath = DataHandler.strModFolder.Replace("loading_order.json", string.Empty);
        string directoryName = Path.GetDirectoryName(DataHandler.strModFolder);
        directoryName = Path.Combine(directoryName, "loading_order.json");

        // Creating Mod Placeholder
        JsonModInfo coreModInfo = new JsonModInfo();
        coreModInfo.strName = "core";
        DataHandler.dictModInfos["core"] = coreModInfo;

        // Mod List Loading Routine
        bool isConsoleExists = ConsoleToGUI.instance != null;
        if (isConsoleExists)
        {
            ConsoleToGUI.instance.LogInfo("Attempting to load " + directoryName + "...");
        }

        // Proceed With Mod List Loading
        if (File.Exists(directoryName))
        {
            if (isConsoleExists)
            {
                ConsoleToGUI.instance.LogInfo("loading_order.json found. Beginning mod load.");
            }
            DataHandler.JsonToData(directoryName, DataHandler.dictModList);
            JsonModList newModList = null;
            if (DataHandler.dictModList.TryGetValue("Mod Loading Order", out newModList))
            {
                if (newModList.aIgnorePatterns != null)
                {
                    for (int i = 0; i < newModList.aIgnorePatterns.Length; i++)
                    {
                        newModList.aIgnorePatterns[i] = DataHandler.PathSanitize(newModList.aIgnorePatterns[i]);
                    }
                }
                string[] aLoadOrder = newModList.aLoadOrder;

                // Go Through Each Mod Entry
                foreach (string aLoadEntry in aLoadOrder)
                {
                    isModded = true;

                    // Handle Dedicated/Invalid Settings
                    if (aLoadEntry == "core")
                    {
                        modQueuedPaths.Add(new(coreModInfo, DataHandler.strAssetPath, "core"));
                        continue;
                    }
                    if (aLoadEntry == null || aLoadEntry == string.Empty)
                    {
                        Debug.LogError("ERROR: Invalid mod folder specified: " + aLoadEntry + "; Skipping...");
                        continue;
                    }

                    // Prepare Mod Information
                    string aLoadPath = aLoadEntry.TrimStart(Path.DirectorySeparatorChar);
                    aLoadPath = aLoadEntry.TrimStart(Path.AltDirectorySeparatorChar);
                    aLoadPath += "/";
                    string modFolderPath = Path.GetDirectoryName(DataHandler.strModFolder);
                    modFolderPath = Path.Combine(modFolderPath, aLoadPath);
                    Dictionary<string, JsonModInfo> modInfoJson = new Dictionary<string, JsonModInfo>();
                    string modInfoPath = Path.Combine(modFolderPath, "mod_info.json");

                    // Start Mod Loading Routine
                    if (File.Exists(modInfoPath))
                    {
                        DataHandler.JsonToData(modInfoPath, modInfoJson);
                    }
                    if (modInfoJson.Count < 1)
                    {
                        JsonModInfo altModInfo = new JsonModInfo();
                        altModInfo.strName = aLoadEntry;
                        modInfoJson[altModInfo.strName] = altModInfo;
                        Debug.LogWarning("WARNING: Missing mod_info.json in folder: " + aLoadEntry + "; Using default name: " + altModInfo.strName);
                    }
                    using (Dictionary<string, JsonModInfo>.ValueCollection.Enumerator modEnum = modInfoJson.Values.GetEnumerator())
                    {
                        if (modEnum.MoveNext())
                        {
                            JsonModInfo modCurrent = modEnum.Current;
                            DataHandler.dictModInfos[modCurrent.strName] = modCurrent;

                            // Queue Mod's Path For Loading
                            modQueuedPaths.Add(new(modCurrent, modFolderPath, aLoadEntry));

                            if (isConsoleExists)
                            {
                                ConsoleToGUI.instance.LogInfo("Loading mod: " + modCurrent.strName + " from directory: " + aLoadEntry);
                            }
                        }
                    }
                }

                // Sync Load All Mod Data
                SyncLoadMods(modQueuedPaths, newModList.aIgnorePatterns);
            }
        }

        // Default Non-Modded Loading
        if (!isModded)
        {
            if (isConsoleExists)
            {
                ConsoleToGUI.instance.LogInfo("No loading_order.json found. Beginning default game data load from " + DataHandler.strAssetPath);
            }
            JsonModList cleanModList = new JsonModList();
            cleanModList.strName = "Default";
            cleanModList.aLoadOrder = new string[1] { "core" };
            cleanModList.aIgnorePatterns = new string[0];
            DataHandler.dictModList["Mod Loading Order"] = cleanModList;
            modQueuedPaths.Add(new(coreModInfo, DataHandler.strAssetPath, "core"));

            // Sync Load Core Data Only
            SyncLoadMods(modQueuedPaths, cleanModList.aIgnorePatterns);
        }

        // Loaded Data Post-Processing
        DataHandler.PostModLoad();
        DataHandler.bLoaded = true;
        if (DataHandler.DataHandlerInitComplete != null)
        {
            DataHandler.DataHandlerInitComplete();
        }
    }

    private static void SyncLoadMods(List<ModInformation> refQueuedPaths, string[] aIgnorePatterns)
    {
        //Use a reaper pattern for destroying invalid mods from the list
        List<ModInformation> reapedMods = [];

        // Parse Validate Data Paths
        foreach (var queuedPath in refQueuedPaths)
        {
            if (!Directory.Exists(queuedPath.DataDir))
            {
                if (queuedPath.Mod.strName == "core")
                    continue;

                reapedMods.Add(queuedPath);
            }
        }

        //Skip checking for modinfo.json because we already have it

        foreach (var reapedItem in reapedMods)
        {
            Debug.Log($"Mod reaped do to invalid structure: {reapedItem.Mod.strName}");
            refQueuedPaths.Remove(reapedItem);
        }

        // List All Valid Data Paths
        bool isConsoleExists = ConsoleToGUI.instance != null;
        int numConsoleErrors = 0;
        if (isConsoleExists)
        {
            numConsoleErrors = ConsoleToGUI.instance.ErrorCount;
            ConsoleToGUI.instance.LogInfo("Begin loading data from these paths:");
            foreach (var dataPath in refQueuedPaths) ConsoleToGUI.instance.LogInfo(dataPath.DataDir);
        }

        // Create CO Changes Map
        foreach (var mod in refQueuedPaths)
        {
            string modName = mod.Mod.strName;
            if (mod.Mod is not patch_JsonModInfo refModInfo)
            {
                ConsoleToGUI.instance.LogInfo($"Skipping CO changes map for {mod.Mod.strName} because it is not a patch_JsonModInfo");
                continue;
            }

            if (refModInfo.strName == "core") continue;
            if (refModInfo.changesMap != null)
            {
                foreach (var changeMap in refModInfo.changesMap)
                {
                    if (!dictChangesMap.ContainsKey(changeMap.Key))
                        dictChangesMap[changeMap.Key] = new Dictionary<string, List<string>>();
                    if (changeMap.Value != null)
                    {
                        foreach (var subMap in changeMap.Value)
                        {
                            bool IsInverse = subMap.Key.StartsWith(FFU_BR_Defs.SYM_INV);
                            string subMapKey = IsInverse ? subMap.Key.Substring(1) : subMap.Key;
                            if (subMapKey != FFU_BR_Defs.OPT_DEL)
                            {
                                if (!dictChangesMap[changeMap.Key].ContainsKey(subMapKey))
                                    dictChangesMap[changeMap.Key][subMapKey] = new List<string>();
                                if (IsInverse && !dictChangesMap[changeMap.Key][subMapKey].Contains(FFU_BR_Defs.FLAG_INVERSE))
                                    dictChangesMap[changeMap.Key][subMapKey].Add(FFU_BR_Defs.FLAG_INVERSE);
                                if (subMap.Value != null && subMap.Value.Length > 0)
                                {
                                    if (subMap.Value[0] != FFU_BR_Defs.OPT_DEL)
                                    {
                                        foreach (var subMapEntry in subMap.Value)
                                        {
                                            if (subMapEntry.StartsWith(FFU_BR_Defs.OPT_REM))
                                            {
                                                string cleanEntry = subMapEntry.Substring(1);
                                                string targetEntry = dictChangesMap[changeMap.Key][subMapKey]
                                                    .Find(x => x.StartsWith(cleanEntry));
                                                if (!string.IsNullOrEmpty(targetEntry))
                                                {
                                                    dictChangesMap[changeMap.Key][subMapKey].Remove(targetEntry);
                                                    continue;
                                                }
                                            }
                                            else if (subMapEntry.StartsWith(FFU_BR_Defs.OPT_MOD))
                                            {
                                                string cleanEntry = subMapEntry.Substring(1);
                                                string lookupKey = cleanEntry.Contains(FFU_BR_Defs.SYM_DIV) ? cleanEntry.Split(FFU_BR_Defs.SYM_DIV[0])[0] :
                                                    cleanEntry.Contains(FFU_BR_Defs.SYM_EQU) ? cleanEntry.Split(FFU_BR_Defs.SYM_EQU[0])[0] : cleanEntry;
                                                string targetEntry = dictChangesMap[changeMap.Key][subMapKey]
                                                    .Find(x => x.StartsWith(lookupKey));
                                                if (!string.IsNullOrEmpty(targetEntry))
                                                {
                                                    int targetIdx = dictChangesMap[changeMap.Key][subMapKey].IndexOf(targetEntry);
                                                    dictChangesMap[changeMap.Key][subMapKey][targetIdx] = cleanEntry;
                                                    continue;
                                                }
                                            }
                                            else dictChangesMap[changeMap.Key][subMapKey].Add(subMapEntry);
                                        }
                                    }
                                    else dictChangesMap[changeMap.Key].Remove(subMapKey);
                                }
                            }
                            else
                            {
                                dictChangesMap.Remove(changeMap.Key);
                                if (changeMap.Value.Count > 1)
                                    dictChangesMap[changeMap.Key] = new Dictionary<string, List<string>>();
                                else break;
                            }
                        }
                    }
                }
            }
        }
        if (FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.ModdedDump) Debug.Log($"Dynamic Changes Map (Dump): {JsonMapper.ToJson(dictChangesMap)}");

        // Sync Load Mods Data
        foreach (var item in refQueuedPaths) DataHandler.aModPaths.Insert(0, item.ModDir);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "colors/", DataHandler.dictJsonColors, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "lights/", DataHandler.dictLights, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "gasrespires/", DataHandler.dictGasRespires, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "powerinfos/", DataHandler.dictPowerInfo, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "guipropmaps/", DataHandler.dictGUIPropMapUnparsed, aIgnorePatterns);
        DataHandler.ParseGUIPropMaps();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "conditions/", DataHandler.dictConds, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "conditions_simple/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseConditionsSimple();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "items/", DataHandler.dictItemDefs, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "condtrigs/", DataHandler.dictCTs, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "interactions/", DataHandler.dictInteractions, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "condowners/", DataHandler.dictCOs, aIgnorePatterns);
        Dictionary<string, JsonRoomSpec> listRooms = new Dictionary<string, JsonRoomSpec>();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "rooms/", listRooms, aIgnorePatterns);
        DataHandler.ParseRoomSpecs(listRooms);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "ships/", DataHandler.dictShips, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "loot/", DataHandler.dictLoot, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "market/Production/", DataHandler.dictProductionMaps, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "market/", DataHandler.dictMarketConfigs, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "market/CargoSpecs/", DataHandler.dictCargoSpecs, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_last/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesLast);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_robots/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesRobots);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_first/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesFirst);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_full/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesFull);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "manpages/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseManPages();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "homeworlds/", DataHandler.dictHomeworlds, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "careers/", DataHandler.dictCareers, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "lifeevents/", DataHandler.dictLifeEvents, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "personspecs/", DataHandler.dictPersonSpecs, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "shipspecs/", DataHandler.dictShipSpecs, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "traitscores/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseTraitScores();
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "strings/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictStrings);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "slot_effects/", DataHandler.dictSlotEffects, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "slots/", DataHandler.dictSlots, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "tickers/", DataHandler.dictTickers, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "condrules/", DataHandler.dictCondRules, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "audioemitters/", DataHandler.dictAudioEmitters, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "crewskins/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictCrewSkins);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "ads/", DataHandler.dictAds, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "headlines/", DataHandler.dictHeadlines, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "music/", DataHandler.dictMusic, aIgnorePatterns);
        DataHandler.ParseMusic();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "cooverlays/", DataHandler.dictCOOverlays, aIgnorePatterns);
        Dictionary<string, JsonDCOCollection> listCollections = new Dictionary<string, JsonDCOCollection>();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "market/CoCollections/", listCollections, aIgnorePatterns);
        DataHandler.BuildMarketDCOCollection(listCollections);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "ledgerdefs/", DataHandler.dictLedgerDefs, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "pledges/", DataHandler.dictPledges, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "jobitems/", DataHandler.dictJobitems, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "jobs/", DataHandler.dictJobs, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_ship/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesShip);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_ship_adjectives/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesShipAdjectives);
        DataHandler.dictSimple.Clear();
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "names_ship_nouns/", DataHandler.dictSimple, aIgnorePatterns, true);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesShipNouns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "ai_training/", DataHandler.dictAIPersonalities, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "transit/", DataHandler.dictTransit, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "plot_manager/", DataHandler.dictPlotManager, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "star_systems/", DataHandler.dictStarSystems, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "parallax/", DataHandler.dictParallax, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "context/", DataHandler.dictContext, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "chargeprofiles/", DataHandler.dictChargeProfiles, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "wounds/", DataHandler.dictWounds, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "attackmodes/", DataHandler.dictAModes, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "pda_apps/", DataHandler.dictPDAAppIcons, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "zone_triggers/", DataHandler.dictZoneTriggers, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "tips/", DataHandler.dictTips, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "crime/", DataHandler.dictCrimes, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "plots/", DataHandler.dictPlots, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "plot_beats/", DataHandler.dictPlotBeats, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "racing/tracks/", DataHandler.dictRaceTracks, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "racing/leagues/", DataHandler.dictRacingLeagues, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "verbs/", DataHandler.dictJsonVerbs, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "tokens/", DataHandler.dictJsonTokens, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "info/", DataHandler.dictInfoNodes, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "installables/", DataHandler.dictInstallables, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "interaction_overrides/", DataHandler.dictIAOverrides, aIgnorePatterns);
        foreach (var mod in refQueuedPaths) SyncLoadJSONs(mod, mod.DataDir, "plot_beat_overrides/", DataHandler.dictPlotBeatOverrides, aIgnorePatterns);

        // Create Fast List of Locked COs
        foreach (var dictCO in DataHandler.dictCOs)
        {
            if (dictCO.Value.bSlotLocked) patch_DataHandler.listLockedCOs.Add(dictCO.Value.strName);
        }

        // Validate Mapped COs In Ship Templates
        if (FFU_BR_Defs.ModSyncLoading)
        {
            foreach (var dictShip in DataHandler.dictShips)
            {
                SwitchSlottedItems(dictShip.Value, true);
                RecoverMissingItems(dictShip.Value);
            }
        }

        // Finalize Mod Load Status
        foreach (var mod in refQueuedPaths)
        {
            string modName = mod.Mod.strName;
            if (mod.Mod is not patch_JsonModInfo refModInfo)
                continue;

            if (refModInfo.Status == GUIModRow.ModStatus.Missing)
            {
                refModInfo.Status = GUIModRow.ModStatus.Missing;
            }
            else if ((bool)ConsoleToGUI.instance &&
                numConsoleErrors < ConsoleToGUI.instance.ErrorCount)
            {
                refModInfo.Status = GUIModRow.ModStatus.Error;
            }
            else
            {
                refModInfo.Status = GUIModRow.ModStatus.Loaded;
            }
        }
    }

    private static void SyncLoadJSONs<TJson>(ModInformation modInfo, string strFolderPath, string subFolder, Dictionary<string, TJson> dataDict, string[] aIgnorePatterns, bool extData = false)
    {
        // Prepare Reference Data
        string modName = modInfo.Mod.strName;
        string fileType = subFolder.Remove(subFolder.Length - 1);

        // Per Mod Data Removal
        if (modInfo.Mod is patch_JsonModInfo refModInfo)
            if (refModInfo != null && refModInfo.removeIds != null && refModInfo.removeIds.ContainsKey(fileType))
            {
                foreach (string removeId in refModInfo.removeIds[fileType])
                {
                    bool wasRemoved = dataDict.Remove(removeId);
                    if (wasRemoved) Debug.Log($"Removed existing '{fileType}' entry: {removeId}");
                }
            }

        // Ignore Missing Folder
        string strSubFolderPath = Path.Combine(strFolderPath, subFolder);
        if (!Directory.Exists(strSubFolderPath)) return;

        // Parse Folder Contents
        string[] subFiles = Directory.GetFiles(strSubFolderPath, "*.json", SearchOption.AllDirectories);
        foreach (string subFile in subFiles)
        {
            string filePath = DataHandler.PathSanitize(subFile);

            // Check Ignored Patterns
            bool isIgnoredPath = false;
            if (aIgnorePatterns != null)
            {
                foreach (string ignorePattern in aIgnorePatterns)
                {
                    if (filePath.IndexOf(ignorePattern) >= 0)
                    {
                        isIgnoredPath = true;
                        break;
                    }
                }
            }

            // Data Loading Subroutine
            if (isIgnoredPath)
            {
                Debug.LogWarning("Ignore Pattern match: " + filePath + "; Skipping...");
            }
            else
            {
                SyncToData(filePath, fileType, modName != null, dataDict, extData);
            }
        }
    }

    public static void SyncToData<TJson>(string strFile, string strType, bool isMod, Dictionary<string, TJson> dataDict, bool extData)
    {
        Debug.Log("#Info# Loading JSON: " + strFile);
        bool logModded = FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.ModChanges;
        bool logRefCopy = FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.DeepCopy;
        bool logObjects = FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.ModdedDump;
        bool logExtended = FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.ExtendedDump;
        bool logContent = FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.ContentDump;
        bool logSource = FFU_BR_Defs.SyncLogging >= FFU_BR_Defs.SyncLogs.SourceDump;
        string rawDump = string.Empty;
        try
        {
            // Raw JSON to Data Array
            string dataFile = File.ReadAllText(strFile, Encoding.UTF8);
            rawDump += "Converting JSON into Array...\n";
            string[] rawData = isMod ? dataFile.Replace("\n", "").Replace("\r", "").Replace("\t", "")
                .Replace(" ", "").Split(new string[] { "},{" }, StringSplitOptions.None) : null;
            TJson[] fileData = JsonMapper.ToObject<TJson[]>(dataFile);

            // Parsing Each Data Block
            for (int i = 0; i < fileData.Length; i++)
            {
                TJson dataBlock = fileData[i];
                string rawBlock = isMod ? rawData[i] : null;
                rawDump += "Getting key: ";
                string referenceKey = null;
                string dataKey = null;

                // Validating Data Block
                PropertyInfo referenceProperty = dataBlock.GetType()?.GetProperty("strReference");
                PropertyInfo nameProperty = dataBlock.GetType()?.GetProperty("strName");
                if (nameProperty == null)
                {
                    JsonLogger.ReportProblem("strName is missing", ReportTypes.FailingString);
                    continue;
                }

                // Data Allocation Subroutine
                object referenceValue = referenceProperty?.GetValue(dataBlock, null);
                object nameValue = nameProperty.GetValue(dataBlock, null);
                referenceKey = referenceValue?.ToString();
                dataKey = nameValue.ToString();
                rawDump = rawDump + dataKey + "\n";
                if (isMod && dataDict.ContainsKey(dataKey))
                {
                    // Modify Existing Data
                    if (logObjects) Debug.Log($"Modification Data (Dump/Before): {JsonMapper.ToJson(dataDict[dataKey])}");
                    try
                    {
                        SyncDataSafe(dataDict[dataKey], dataBlock, ref rawBlock, strType, dataKey, extData, logModded);
                        if (logObjects) Debug.Log($"Modification Data (Dump/After): {JsonMapper.ToJson(dataDict[dataKey])}");
                    }
                    catch (Exception ex)
                    {
                        Exception inner = ex.InnerException;
                        Debug.LogWarning($"Modification sync for Data Block [{dataKey}] " +
                        $"has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" + (inner != null ?
                        $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                    }
                }
                else if (isMod && !dataDict.ContainsKey(dataKey))
                {
                    // Reference Deep Copy + Apply Changes
                    if (referenceKey != null && dataDict.ContainsKey(referenceKey))
                    {
                        string deepCopy = JsonMapper.ToJson(dataDict[referenceKey]);
                        if (logExtended) Debug.Log($"Reference Data (Dump/Before): {deepCopy}");
                        bool isDeepCopySuccess = false;
                        deepCopy = Regex.Replace(deepCopy, "(\"strName\":)\"[^\"]*\"", match =>
                        {
                            isDeepCopySuccess = true;
                            return $"{match.Groups[1].Value}\"{dataKey}\"";
                        });
                        if (isDeepCopySuccess)
                        {
                            TJson deepCopyBlock = JsonMapper.ToObject<TJson>(deepCopy);
                            Debug.Log($"#Info# Modified Deep Copy Created: {referenceKey} => {dataKey}");
                            try
                            {
                                SyncDataSafe(deepCopyBlock, dataBlock, ref rawBlock, strType, dataKey, extData, logRefCopy);
                                if (logExtended) Debug.Log($"Reference Data (Dump/After): {JsonMapper.ToJson(deepCopyBlock)}");
                            }
                            catch (Exception ex)
                            {
                                Exception inner = ex.InnerException;
                                Debug.LogWarning($"Reference sync for Data Block [{dataKey}] " +
                                $"has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" + (inner != null ?
                                $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                            }
                            try
                            {
                                dataDict.Add(dataKey, deepCopyBlock);
                            }
                            catch (Exception ex)
                            {
                                Exception inner = ex.InnerException;
                                Debug.LogWarning($"Reference add of new Data Block [{dataKey}] " +
                                $"has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" + (inner != null ?
                                $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(referenceKey))
                    {
                        Debug.LogWarning($"Reference key '{referenceKey}' " +
                        $"in Data Block [{dataKey}] is invalid! Ignoring.");
                    }
                    else
                    {
                        // Add New Mod Data Entry
                        if (logContent)
                            try
                            {
                                Debug.Log($"Addendum Data (Dump/Mod): {JsonMapper.ToJson(dataBlock)}");
                            }
                            catch (Exception ex)
                            {
                                Exception inner = ex.InnerException;
                                Debug.LogWarning($"Addendum Data (Dump/Mod) for Data Block " +
                                $"[{dataKey}] has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" +
                                (inner != null ? $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                            }
                        try
                        {
                            dataDict.Add(dataKey, dataBlock);
                        }
                        catch (Exception ex)
                        {
                            Exception inner = ex.InnerException;
                            Debug.LogWarning($"Modded Add of new Data Block [{dataKey}] " +
                            $"has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" + (inner != null ?
                            $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                        }
                    }
                }
                else
                {
                    // Add New Core Data Entry
                    if (logSource)
                        try
                        {
                            Debug.Log($"Addendum Data (Dump/Core): {JsonMapper.ToJson(dataBlock)}");
                        }
                        catch (Exception ex)
                        {
                            Exception inner = ex.InnerException;
                            Debug.LogWarning($"Addendum Data (Dump/Core) for Data Block " +
                            $"[{dataKey}] has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" +
                            (inner != null ? $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                        }
                    try
                    {
                        dataDict.Add(dataKey, dataBlock);
                    }
                    catch (Exception ex)
                    {
                        Exception inner = ex.InnerException;
                        Debug.LogWarning($"Core Add of new Data Block [{dataKey}] " +
                        $"has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" + (inner != null ?
                        $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                    }
                }
            }

            // Resetting Data Variables
            fileData = null;
            dataFile = null;
        }
        catch (Exception ex)
        {
            JsonLogger.ReportProblem(strFile, ReportTypes.SourceInfo);
            if (rawDump.Length > 1000)
            {
                rawDump = rawDump.Substring(rawDump.Length - 1000);
            }
            Debug.LogError(rawDump + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
        }

        // Specific File Dump
        if (strFile.IndexOf("osSGv1") >= 0)
        {
            Debug.Log(rawDump);
        }
    }

    public static void SyncDataSafe<TJson>(TJson currDataSet, TJson newDataSet, ref string rawDataSet, string dataType, string dataKey, bool extData, bool doLog = false)
    {
        Type currDataType = currDataSet.GetType();
        Type newDataType = newDataSet.GetType();

        // Iterate Over Properties
        foreach (PropertyInfo currProperty in currDataType.GetProperties())
        {
            // Ignore Forbidden Property
            if (!currProperty.CanWrite || dataType.IsForbidden(currProperty.Name)) continue;

            // New Data Property Validation
            PropertyInfo newProperty = newDataType.GetProperty(currProperty.Name);
            if (newProperty != null)
            {
                bool doCurr = false;
                string refName = currProperty.Name;
                object newValue = newProperty.GetValue(newDataSet, null);
                object currValue = currProperty.GetValue(currDataSet, null);
                if (rawDataSet.IndexOf(refName) >= 0)
                {
                    try
                    {
                        // Handle Dictionary Variables
                        if (currValue != null && newValue is IDictionary)
                            SyncRecords(ref newValue, ref currValue, ref doCurr,
                                dataKey, refName, dataType, extData, doLog);

                        // Handle Array Variables
                        else if (currValue != null && newValue is string[])
                            SyncArrays(ref newValue, ref currValue,
                                dataKey, refName, extData, doLog);

                        // Handle Simple Variables
                        else if (doLog) Debug.Log($"#Info# Data Block [{dataKey}], Property " +
                            $"[{refName}]: {currValue.Sanitized()} => {newValue.Sanitized()}");

                        // Overwrite Existing Value
                        if (doCurr) currProperty.SetValue(currDataSet, currValue, null);
                        else currProperty.SetValue(currDataSet, newValue, null);
                    }
                    catch (Exception ex)
                    {
                        Exception inner = ex.InnerException;
                        Debug.LogWarning($"Value sync for Data Block [{dataKey}], Property " +
                        $"[{refName}] has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" +
                        (inner != null ? $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                    }
                }
            }
        }

        // Iterate Over Fields
        BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.Instance;
        foreach (FieldInfo currField in currDataType.GetFields(fieldFlags))
        {
            // Ignore Forbidden Field
            if (currField.IsLiteral || dataType.IsForbidden(currField.Name)) continue;

            // New Data Field Validation
            FieldInfo newField = newDataType.GetField(currField.Name, fieldFlags);
            if (newField != null)
            {
                bool doCurr = false;
                string refName = currField.Name;
                object newValue = newField.GetValue(newDataSet);
                object currValue = currField.GetValue(currDataSet);
                if (rawDataSet.IndexOf(refName) >= 0)
                {
                    try
                    {
                        // Handle Dictionary Variables
                        if (currValue != null && newValue is IDictionary)
                            SyncRecords(ref newValue, ref currValue, ref doCurr,
                                dataKey, refName, dataType, extData, doLog);

                        // Handle Array Variables
                        else if (currValue != null && newValue is string[])
                            SyncArrays(ref newValue, ref currValue,
                                dataKey, refName, extData, doLog);

                        // Handle Simple Variables
                        else if (doLog) Debug.Log($"#Info# Data Block [{dataKey}], Field " +
                            $"[{refName}]: {currValue.Sanitized()} => {newValue.Sanitized()}");

                        // Overwrite Existing Value
                        if (doCurr) currField.SetValue(currDataSet, currValue);
                        else currField.SetValue(currDataSet, newValue);
                    }
                    catch (Exception ex)
                    {
                        Exception inner = ex.InnerException;
                        Debug.LogWarning($"Value sync for Data Block [{dataKey}], Property " +
                        $"[{refName}] has failed! Ignoring.\n{ex.Message}\n{ex.StackTrace}" +
                        (inner != null ? $"\nInner: {inner.Message}\n{inner.StackTrace}" : ""));
                    }
                }
            }
        }
    }

    public static void SyncRecords(ref object newValue, ref object currValue, ref bool isDictionary, string dataKey, string propName, string dataType, bool extData, bool doLog)
    {
        isDictionary = true;
        IDictionary newDict = (IDictionary)newValue;
        IDictionary currDict = (IDictionary)currValue;
        bool isPrimitive = currDict.Values.Cast<object>().FirstOrDefault()?.GetType().IsPrimitive ?? false;

        // Perform Dictionary Operations
        foreach (var newKey in newDict.Keys.Cast<object>().ToList())
        {
            bool doRemove = newKey.ToString().StartsWith(FFU_BR_Defs.OPT_DEL);
            bool doReplace = newKey.ToString().StartsWith(FFU_BR_Defs.OPT_MOD);
            bool hasAction = doRemove || doReplace;
            object targetKey = hasAction ? newKey.ToString().Substring(1) : newKey;

            // Existing Records Handling
            if (currDict.Contains(targetKey))
            {
                if (doRemove)
                {

                    // Record Block Removal
                    if (doLog) Debug.Log($"#Info# Property [{targetKey}] was removed " +
                        $"from Data Block [{dataKey}/{propName}]");
                    currDict.Remove(targetKey);
                }
                else if (doReplace)
                {

                    // Record Block Override
                    if (doLog) Debug.Log($"#Info# Property [{targetKey}] was replaced " +
                        $"in Data Block [{dataKey}/{propName}]");
                    currDict[targetKey] = newDict[newKey];
                }
                else if (!isPrimitive)
                {

                    // Sub-Record Handling
                    string rawDataSubSet = JsonMapper.ToJson(newDict[targetKey]);
                    SyncDataSafe(currDict[targetKey], newDict[targetKey], ref rawDataSubSet,
                        dataType, $"{dataKey}/{propName}:{targetKey}", extData, doLog);
                }
                else
                {

                    // Record Overwrite
                    if (doLog) Debug.Log($"#Info# Data Block [{dataKey}/{propName}], " +
                        $"Property [{targetKey}]: {currDict[targetKey]} => {newDict[targetKey]}");
                    currDict[targetKey] = newDict[targetKey];
                }
            }
            else
            {

                // New Record Addition
                if (doLog) Debug.Log($"#Info# Property [{targetKey}], Value [{newDict[targetKey]}] " +
                    $"was added to Data Block [{dataKey}/{propName}]");
                currDict[targetKey] = newDict[targetKey];
            }
        }
    }

    public static void SyncArrays(ref object newValue, ref object currValue, string dataKey, string propName, bool extData, bool doLog)
    {
        SyncArrayOp defaultOp = extData ? SyncArrayOp.Add : SyncArrayOp.None;
        List<string> modArray = (currValue as string[]).ToList();
        List<string> refArray = (newValue as string[]).ToList();
        List<string> origArray = new List<string>(modArray);
        bool noArrayOps = true;

        // Perform Sub-Array Operations
        foreach (var refItem in refArray)
        {
            if (string.IsNullOrEmpty(refItem)) continue;
            if (!char.IsDigit(refItem[0]) || !refItem.Contains('|')) continue;

            // Invalid Sub-Arrays Ignored
            List<string> refSubArray = refItem.Split('|').ToList();
            int.TryParse(refSubArray[0].Replace("=", ""), out int rowIndex);

            // Target Index Validation
            if (rowIndex > 0)
            {
                refSubArray.RemoveAt(0);
                List<string> modSubArray = modArray[rowIndex - 1].Split('|').ToList();
                SyncArrayOps(modSubArray, refSubArray, ref noArrayOps, dataKey, $"{propName}#{rowIndex}", doLog, defaultOp);
                if (noArrayOps) Debug.LogWarning($"You attempted to modify sub-array in Data Block " +
                    $"[{dataKey}], Property [{propName}#{rowIndex}], but performed no array operations. " +
                    $"Assume that something went horribly wrong and game is likely to crash.");
                modArray[rowIndex - 1] = string.Join("|", modSubArray.ToArray());
            }
        }

        // Perform Array Operations
        SyncArrayOps(modArray, refArray, ref noArrayOps, dataKey, propName, doLog, defaultOp);

        // Overwriting Existing Value
        if (noArrayOps)
        {
            if (doLog) Debug.Log($"#Info# Data Block [{dataKey}], " +
                $"Property [{propName}]: String[{origArray.Count}] => String[{refArray.Count}]");
            newValue = refArray.ToArray();
        }
        else newValue = modArray.ToArray();
    }

    public static void SyncArrayOps(List<string> modArray, List<string> refArray, ref bool noArrayOps, string dataKey, string propName, bool doLog, SyncArrayOp arrayOp = SyncArrayOp.None)
    {
        int opIndex = 0;
        // Array Operations Subroutine
        foreach (var refItem in refArray)
        {
            // Valid Sub-Arrays Ignored
            if (string.IsNullOrEmpty(refItem)) continue;
            if (char.IsDigit(refItem[0]) && refItem.Contains('|')) continue;

            // Get Operations Command
            if (refItem.StartsWith("--"))
            {
                switch (refItem.Substring(0, 7))
                {
                    case OP_MOD: arrayOp = SyncArrayOp.Mod; break;
                    case OP_ADD: arrayOp = SyncArrayOp.Add; break;
                    case OP_INS: arrayOp = SyncArrayOp.Ins; break;
                    case OP_DEL: arrayOp = SyncArrayOp.Del; break;
                }
                if (arrayOp == SyncArrayOp.Ins)
                {
                    int.TryParse(refItem.Replace("=", "").Substring(7), out opIndex);
                    opIndex--;
                    if (opIndex < 0)
                    {
                        Debug.LogWarning($"The '{OP_INS}' array operation in Data Block [{dataKey}], " +
                            $"Property [{propName}] received invalid index! Using [0] index.");
                        opIndex = 0;
                    }
                }
                continue;
            }

            // Operations Logical Check
            if (noArrayOps) noArrayOps = arrayOp == SyncArrayOp.None;
            if (noArrayOps) break;

            // Execute Array Operation
            string[] refVal = refItem.Split('=');
            bool isDataValue = refVal.Length == 2 && !refItem.Contains('|') && !string.IsNullOrEmpty(refVal[1]);
            if (isDataValue)
            {
                switch (arrayOp)
                {
                    case SyncArrayOp.Mod: OpModData(modArray, refItem, dataKey, propName, doLog); break;
                    case SyncArrayOp.Add: OpAddData(modArray, refItem, dataKey, propName, doLog); break;
                    case SyncArrayOp.Ins: OpInsData(modArray, ref opIndex, refItem, dataKey, propName, doLog); break;
                    case SyncArrayOp.Del: OpDelData(modArray, refItem, dataKey, propName, doLog); break;
                }
            }
            else
            {
                switch (arrayOp)
                {
                    case SyncArrayOp.Mod:
                        Debug.LogWarning($"Non-data [{refItem}] in Data Block [{dataKey}], " +
                        $"Property [{propName}] doesn't support '{OP_MOD}' operation! Ignoring."); break;
                    case SyncArrayOp.Add: OpAddSimple(modArray, refItem, dataKey, propName, doLog); break;
                    case SyncArrayOp.Ins: OpInsSimple(modArray, ref opIndex, refItem, dataKey, propName, doLog); break;
                    case SyncArrayOp.Del: OpDelSimple(modArray, refItem, dataKey, propName, doLog); break;
                }
            }
        }
    }

    public static void OpModData(List<string> modArray, string refItem, string dataKey, string propName, bool doLog)
    {
        string[] refData = refItem.Split('=');
        bool isReplaced = false;
        for (int i = 0; i < modArray.Count; i++)
        {
            string[] itemData = modArray[i].Split('=');
            if (itemData[0] == refData[0])
            {
                if (doLog) Debug.Log($"#Info# " +
                    $"Data Block [{dataKey}], Property [{propName}], Parameter " +
                    $"[{refData[0]}]: {itemData[1]} => {refData[1]}");
                modArray[i] = refItem;
                isReplaced = true;
                break;
            }
        }
        if (!isReplaced) Debug.LogWarning($"Parameter [{refData[0]}] was not " +
        $"found in Data Block [{dataKey}], Property [{propName}]! Ignoring.");
    }

    public static void OpAddData(List<string> modArray, string refItem, string dataKey, string propName, bool doLog)
    {
        string[] refData = refItem.Split('=');
        if (doLog) Debug.Log($"#Info# Parameter [{refData[0]}], Value [{refData[1]}] " +
            $"was added to Data Block [{dataKey}], Property [{propName}]");
        modArray.Add(refItem);
    }

    public static void OpInsData(List<string> modArray, ref int arrIndex, string refItem, string dataKey, string propName, bool doLog)
    {
        string[] refData = refItem.Split('=');
        if (doLog) Debug.Log($"#Info# Parameter [{refData[0]}], Value [{refData[1]}] was inserted " +
            $"into Data Block [{dataKey}], Property [{propName}] at Index [{arrIndex}]");
        if (arrIndex >= modArray.Count)
        {
            Debug.LogWarning($"Index [{arrIndex}] for Parameter [{refData[0]}] in Data " +
                $"Block [{dataKey}], Property [{propName}] is invalid! Adding instead.");
            modArray.Add(refItem);
        }
        else modArray.Insert(arrIndex, refItem);
        arrIndex++;
    }

    public static void OpDelData(List<string> modArray, string refItem, string dataKey, string propName, bool doLog)
    {
        string[] refData = refItem.Split('=');
        bool isFound = false;
        int removeIndex = 0;
        for (int i = 0; i < modArray.Count; i++)
        {
            string[] itemData = modArray[i].Split('=');
            if (itemData[0] == refData[0])
            {
                removeIndex = i;
                isFound = true;
                break;
            }
        }
        if (isFound)
        {
            if (doLog) Debug.Log($"#Info# Parameter [{refData[0]}] was removed " +
                $"from Data Block [{dataKey}], Property [{propName}]");
            modArray.RemoveAt(removeIndex);
        }
        else
        {
            Debug.LogWarning($"Parameter [{refData[0]}] was not found " +
            $"in Data Block [{dataKey}], Property [{propName}]! Ignoring.");
        }
    }

    public static void OpAddSimple(List<string> modArray, string refItem, string dataKey, string propName, bool doLog)
    {
        if (doLog) Debug.Log($"#Info# Parameter [{refItem}] was added " +
            $"to Data Block [{dataKey}], Property [{propName}]");
        modArray.Add(refItem);
    }

    public static void OpInsSimple(List<string> modArray, ref int arrIndex, string refItem, string dataKey, string propName, bool doLog)
    {
        if (doLog) Debug.Log($"#Info# Parameter [{refItem}] was inserted into " +
            $"Data Block [{dataKey}], Property [{propName}] at Index [{arrIndex}]");
        if (arrIndex >= modArray.Count)
        {
            Debug.LogWarning($"Index [{arrIndex}] for Parameter [{refItem}] in Data " +
                $"Block [{dataKey}], Property [{propName}] is invalid! Adding instead.");
            modArray.Add(refItem);
        }
        else modArray.Insert(arrIndex, refItem);
        arrIndex++;
    }

    public static void OpDelSimple(List<string> modArray, string refItem, string dataKey, string propName, bool doLog)
    {
        bool isFound = false;
        int removeIndex = 0;
        for (int i = 0; i < modArray.Count; i++)
        {
            if (modArray[i].StartsWith(refItem))
            {
                removeIndex = i;
                isFound = true;
                break;
            }
        }
        if (isFound)
        {
            if (doLog) Debug.Log($"#Info# Parameter [{refItem}] was removed " +
                $"from Data Block [{dataKey}], Property [{propName}]");
            modArray.RemoveAt(removeIndex);
        }
        else
        {
            Debug.LogWarning($"Parameter [{refItem}] was not found in " +
            $"Data Block [{dataKey}], Property [{propName}]!");
        }
    }

    public static bool IsForbidden(this string data, string property)
    {
        return property switch
        {
            "strName" or "strReference" => true,
            _ => false
        };
    }

    public static object Sanitized(this object refObject)
    {
        if (refObject is null) return "NULL";
        else if (refObject is string &&
            ((string)refObject).Length == 0) return "EMPTY";
        else return refObject;
    }

    public static bool TryGetCOValue(string strName, out JsonCondOwner refCO)
    {
        if (DataHandler.dictCOs.TryGetValue(strName, out JsonCondOwner coDict))
        {
            refCO = coDict;
            return true;
        }
        if (DataHandler.dictCOOverlays.TryGetValue(strName, out JsonCOOverlay coOver))
        {
            if (DataHandler.dictCOs.TryGetValue(coOver.strCOBase, out JsonCondOwner coOverDict))
            {
                refCO = coOverDict;
                return true;
            }
        }
        refCO = null;
        return false;
    }

    public const string OP_MOD = "--MOD--";
    public const string OP_ADD = "--ADD--";
    public const string OP_INS = "--INS--";
    public const string OP_DEL = "--DEL--";

    public enum SyncArrayOp
    {
        None,
        Mod,
        Add,
        Ins,
        Del
    }
}

// Reference Output: ILSpy v9.1.0.7988 / C# 12.0 / 2022.8

/* DataHandler.Init
public static void Init()
{
	string empty = string.Empty;
	try
	{
		Debug.Log("#Info# Getting build info.");
		TextAsset textAsset = (TextAsset)Resources.Load("version", typeof(TextAsset));
		strBuild = "Early Access Build: " + textAsset.text;
		Debug.Log(strBuild);
	}
	catch (Exception ex)
	{
		Debug.Log(empty + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
	}
	strAssetPath = Application.streamingAssetsPath + "/";
	dictImages = new Dictionary<string, Texture2D>();
	dictColors = new Dictionary<string, Color>();
	dictHTMLColors = new Dictionary<string, string>();
	dictJsonColors = new Dictionary<string, JsonColor>();
	dictLights = new Dictionary<string, JsonLight>();
	dictShips = new Dictionary<string, JsonShip>();
	dictShipImages = new Dictionary<string, Dictionary<string, Texture2D>>();
	dictConds = new Dictionary<string, JsonCond>();
	dictItemDefs = new Dictionary<string, JsonItemDef>();
	dictCTs = new Dictionary<string, CondTrigger>();
	dictCOs = new Dictionary<string, JsonCondOwner>();
	dictDataCoCollections = new Dictionary<string, DataCoCollection>();
	dictCOSaves = new Dictionary<string, JsonCondOwnerSave>();
	dictInteractions = new Dictionary<string, JsonInteraction>();
	dictLoot = new Dictionary<string, Loot>();
	dictProductionMaps = new Dictionary<string, JsonProductionMap>();
	dictMarketConfigs = new Dictionary<string, JsonMarketActorConfig>();
	dictCargoSpecs = new Dictionary<string, JsonCargoSpec>();
	dictGasRespires = new Dictionary<string, JsonGasRespire>();
	dictPowerInfo = new Dictionary<string, JsonPowerInfo>();
	dictGUIPropMaps = new Dictionary<string, Dictionary<string, string>>();
	dictNamesFirst = new Dictionary<string, string>();
	dictNamesLast = new Dictionary<string, string>();
	dictNamesRobots = new Dictionary<string, string>();
	dictNamesFull = new Dictionary<string, string>();
	dictNamesShip = new Dictionary<string, string>();
	dictNamesShipAdjectives = new Dictionary<string, string>();
	dictNamesShipNouns = new Dictionary<string, string>();
	dictManPages = new Dictionary<string, string[]>();
	dictHomeworlds = new Dictionary<string, JsonHomeworld>();
	dictCareers = new Dictionary<string, JsonCareer>();
	dictLifeEvents = new Dictionary<string, JsonLifeEvent>();
	dictPersonSpecs = new Dictionary<string, JsonPersonSpec>();
	dictShipSpecs = new Dictionary<string, JsonShipSpec>();
	dictTraitScores = new Dictionary<string, int[]>();
	dictRoomSpec = new Dictionary<string, RoomSpec>();
	dictStrings = new Dictionary<string, string>();
	dictSlotEffects = new Dictionary<string, JsonSlotEffects>();
	dictSlots = new Dictionary<string, JsonSlot>();
	dictTickers = new Dictionary<string, JsonTicker>();
	dictCondRules = new Dictionary<string, CondRule>();
	dictMaterials = new Dictionary<string, Material>();
	dictAudioEmitters = new Dictionary<string, JsonAudioEmitter>();
	dictCrewSkins = new Dictionary<string, string>();
	dictAds = new Dictionary<string, JsonAd>();
	dictHeadlines = new Dictionary<string, JsonHeadline>();
	dictMusicTags = new Dictionary<string, List<string>>();
	dictMusic = new Dictionary<string, JsonMusic>();
	dictComputerEntries = new Dictionary<string, JsonComputerEntry>();
	dictCOOverlays = new Dictionary<string, JsonCOOverlay>();
	dictDataCOs = new Dictionary<string, DataCO>();
	dictLedgerDefs = new Dictionary<string, JsonLedgerDef>();
	dictPledges = new Dictionary<string, JsonPledge>();
	dictJobitems = new Dictionary<string, JsonJobItems>();
	dictJobs = new Dictionary<string, JsonJob>();
	dictSettings = new Dictionary<string, JsonUserSettings>();
	dictModList = new Dictionary<string, JsonModList>();
	dictModInfos = new Dictionary<string, JsonModInfo>();
	aModPaths = new List<string>();
	dictInstallables2 = new Dictionary<string, JsonInstallable>();
	dictAIPersonalities = new Dictionary<string, JsonAIPersonality>();
	dictTransit = new Dictionary<string, JsonTransit>();
	dictPlotManager = new Dictionary<string, JsonPlotManagerSettings>();
	dictStarSystems = new Dictionary<string, JsonStarSystemSave>();
	dictParallax = new Dictionary<string, JsonParallax>();
	dictContext = new Dictionary<string, JsonContext>();
	dictChargeProfiles = new Dictionary<string, JsonChargeProfile>();
	dictWounds = new Dictionary<string, JsonWound>();
	dictAModes = new Dictionary<string, JsonAttackMode>();
	dictPDAAppIcons = new Dictionary<string, JsonPDAAppIcon>();
	dictZoneTriggers = new Dictionary<string, JsonZoneTrigger>();
	dictTips = new Dictionary<string, JsonTip>();
	dictCrimes = new Dictionary<string, JsonCrime>();
	dictPlots = new Dictionary<string, JsonPlot>();
	dictPlotBeats = new Dictionary<string, JsonPlotBeat>();
	dictRaceTracks = new Dictionary<string, JsonRaceTrack>();
	dictRacingLeagues = new Dictionary<string, JsonRacingLeague>();
	dictInfoNodes = new Dictionary<string, JsonInfoNode>();
	dictInstallables = new Dictionary<string, JsonInstallable>();
	dictIAOverrides = new Dictionary<string, JsonInteractionOverride>();
	dictPlotBeatOverrides = new Dictionary<string, JsonPlotBeatOverride>();
	dictJsonVerbs = new Dictionary<string, JsonVerbs>();
	dictVerbs = new Dictionary<string, string[]>();
	dictJsonTokens = new Dictionary<string, JsonCustomTokens>();
	listCustomTokens = new List<string>();
	dictSimple = new Dictionary<string, JsonSimple>();
	dictGUIPropMapUnparsed = new Dictionary<string, JsonGUIPropMap>();
	mapCOs = new Dictionary<string, CondOwner>();
	if ((bool)ObjReader.use)
	{
		ObjReader.use.scaleFactor = new Vector3(0.0625f, 0.0625f, 0.0625f);
		ObjReader.use.objRotation = new Vector3(90f, 0f, 180f);
	}
	_interactionObjectTracker = new InteractionObjectTracker();
	dictSettings["DefaultUserSettings"] = new JsonUserSettings();
	dictSettings["DefaultUserSettings"].Init();
	if (File.Exists(Application.persistentDataPath + "/settings.json"))
	{
		JsonToData(Application.persistentDataPath + "/settings.json", dictSettings);
	}
	else
	{
		Debug.LogWarning("WARNING: settings.json not found. Resorting to default values.");
		dictSettings["UserSettings"] = new JsonUserSettings();
		dictSettings["UserSettings"].Init();
	}
	if (!dictSettings.ContainsKey("UserSettings") || dictSettings["UserSettings"] == null)
	{
		Debug.LogError("ERROR: Malformed settings.json. Resorting to default values.");
		dictSettings["UserSettings"] = new JsonUserSettings();
		dictSettings["UserSettings"].Init();
	}
	dictSettings["DefaultUserSettings"].CopyTo(GetUserSettings());
	dictSettings.Remove("DefaultUserSettings");
	SaveUserSettings();
	bool flag = false;
	strModFolder = dictSettings["UserSettings"].strPathMods;
	if (strModFolder == null || strModFolder == string.Empty)
	{
		strModFolder = Path.Combine(Application.dataPath, "Mods/");
	}
	string directoryName = Path.GetDirectoryName(strModFolder);
	directoryName = Path.Combine(directoryName, "loading_order.json");
	JsonModInfo jsonModInfo = new JsonModInfo();
	jsonModInfo.strName = "Core";
	dictModInfos["core"] = jsonModInfo;
	bool flag2 = ConsoleToGUI.instance != null;
	if (flag2)
	{
		ConsoleToGUI.instance.LogInfo("Attempting to load " + directoryName + "...");
	}
	if (File.Exists(directoryName))
	{
		if (flag2)
		{
			ConsoleToGUI.instance.LogInfo("loading_order.json found. Beginning mod load.");
		}
		JsonToData(directoryName, dictModList);
		JsonModList value = null;
		if (dictModList.TryGetValue("Mod Loading Order", out value))
		{
			if (value.aIgnorePatterns != null)
			{
				for (int i = 0; i < value.aIgnorePatterns.Length; i++)
				{
					value.aIgnorePatterns[i] = PathSanitize(value.aIgnorePatterns[i]);
				}
			}
			string[] aLoadOrder = value.aLoadOrder;
			foreach (string text in aLoadOrder)
			{
				flag = true;
				if (text == "core")
				{
					LoadMod(strAssetPath, value.aIgnorePatterns, jsonModInfo);
					continue;
				}
				if (text == null || text == string.Empty)
				{
					Debug.LogError("ERROR: Invalid mod folder specified: " + text + "; Skipping...");
					continue;
				}
				string text2 = text.TrimStart(Path.DirectorySeparatorChar);
				text2 = text.TrimStart(Path.AltDirectorySeparatorChar);
				text2 += "/";
				string directoryName2 = Path.GetDirectoryName(strModFolder);
				directoryName2 = Path.Combine(directoryName2, text2);
				Dictionary<string, JsonModInfo> dictionary = new Dictionary<string, JsonModInfo>();
				string text3 = Path.Combine(directoryName2, "mod_info.json");
				if (File.Exists(text3))
				{
					JsonToData(text3, dictionary);
				}
				if (dictionary.Count < 1)
				{
					JsonModInfo jsonModInfo2 = new JsonModInfo();
					jsonModInfo2.strName = text;
					dictionary[jsonModInfo2.strName] = jsonModInfo2;
					Debug.LogWarning("WARNING: Missing mod_info.json in folder: " + text + "; Using default name: " + jsonModInfo2.strName);
				}
				using (Dictionary<string, JsonModInfo>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						JsonModInfo current = enumerator.Current;
						dictModInfos[text] = current;
						if (flag2)
						{
							ConsoleToGUI.instance.LogInfo("Loading mod: " + dictModInfos[text].strName + " from directory: " + text);
						}
					}
				}
				LoadMod(directoryName2, value.aIgnorePatterns, dictModInfos[text]);
			}
		}
	}
	if (!flag)
	{
		if (flag2)
		{
			ConsoleToGUI.instance.LogInfo("No loading_order.json found. Beginning default game data load from " + strAssetPath);
		}
		JsonModList jsonModList = new JsonModList();
		jsonModList.strName = "Default";
		jsonModList.aLoadOrder = new string[1] { "core" };
		jsonModList.aIgnorePatterns = new string[0];
		dictModList["Mod Loading Order"] = jsonModList;
		LoadMod(strAssetPath, jsonModList.aIgnorePatterns, jsonModInfo);
	}
	PostModLoad();
	bLoaded = true;
	if (DataHandlerInitComplete != null)
	{
		DataHandlerInitComplete();
	}
}
*/

/* DataHandler.LoadMod
private static void LoadMod(string strFolderPath, string[] aIgnorePatterns, JsonModInfo jmi)
{
	if (!Directory.Exists(strFolderPath + "data/"))
	{
		Debug.LogError("ERROR: Mod folder not found: " + strFolderPath + "data/");
		jmi.Status = GUIModRow.ModStatus.Missing;
		return;
	}
	bool flag = ConsoleToGUI.instance != null;
	int num = 0;
	if (flag)
	{
		num = ConsoleToGUI.instance.ErrorCount;
		ConsoleToGUI.instance.LogInfo("Begin loading data from: " + strFolderPath);
	}
	aModPaths.Insert(0, strFolderPath);
	strFolderPath += "data/";
	LoadModJsons(strFolderPath + "colors/", dictJsonColors, aIgnorePatterns);
	LoadModJsons(strFolderPath + "lights/", dictLights, aIgnorePatterns);
	LoadModJsons(strFolderPath + "gasrespires/", dictGasRespires, aIgnorePatterns);
	LoadModJsons(strFolderPath + "powerinfos/", dictPowerInfo, aIgnorePatterns);
	LoadModJsons(strFolderPath + "guipropmaps/", dictGUIPropMapUnparsed, aIgnorePatterns);
	ParseGUIPropMaps();
	LoadModJsons(strFolderPath + "conditions/", dictConds, aIgnorePatterns);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "conditions_simple/", dictSimple, aIgnorePatterns);
	ParseConditionsSimple();
	LoadModJsons(strFolderPath + "items/", dictItemDefs, aIgnorePatterns);
	LoadModJsons(strFolderPath + "condtrigs/", dictCTs, aIgnorePatterns);
	LoadModJsons(strFolderPath + "interactions/", dictInteractions, aIgnorePatterns);
	LoadModJsons(strFolderPath + "condowners/", dictCOs, aIgnorePatterns);
	Dictionary<string, JsonRoomSpec> dictionary = new Dictionary<string, JsonRoomSpec>();
	LoadModJsons(strFolderPath + "rooms/", dictionary, aIgnorePatterns);
	ParseRoomSpecs(dictionary);
	LoadModJsons(strFolderPath + "ships/", dictShips, aIgnorePatterns);
	LoadModJsons(strFolderPath + "loot/", dictLoot, aIgnorePatterns);
	LoadModJsons(strFolderPath + "market/Production/", dictProductionMaps, aIgnorePatterns);
	LoadModJsons(strFolderPath + "market/", dictMarketConfigs, aIgnorePatterns);
	LoadModJsons(strFolderPath + "market/CargoSpecs/", dictCargoSpecs, aIgnorePatterns);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_last/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesLast);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_robots/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesRobots);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_first/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesFirst);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_full/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesFull);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "manpages/", dictSimple, aIgnorePatterns);
	ParseManPages();
	LoadModJsons(strFolderPath + "homeworlds/", dictHomeworlds, aIgnorePatterns);
	LoadModJsons(strFolderPath + "careers/", dictCareers, aIgnorePatterns);
	LoadModJsons(strFolderPath + "lifeevents/", dictLifeEvents, aIgnorePatterns);
	LoadModJsons(strFolderPath + "personspecs/", dictPersonSpecs, aIgnorePatterns);
	LoadModJsons(strFolderPath + "shipspecs/", dictShipSpecs, aIgnorePatterns);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "traitscores/", dictSimple, aIgnorePatterns);
	ParseTraitScores();
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "strings/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictStrings);
	LoadModJsons(strFolderPath + "slot_effects/", dictSlotEffects, aIgnorePatterns);
	LoadModJsons(strFolderPath + "slots/", dictSlots, aIgnorePatterns);
	LoadModJsons(strFolderPath + "tickers/", dictTickers, aIgnorePatterns);
	LoadModJsons(strFolderPath + "condrules/", dictCondRules, aIgnorePatterns);
	LoadModJsons(strFolderPath + "audioemitters/", dictAudioEmitters, aIgnorePatterns);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "crewskins/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictCrewSkins);
	LoadModJsons(strFolderPath + "ads/", dictAds, aIgnorePatterns);
	LoadModJsons(strFolderPath + "headlines/", dictHeadlines, aIgnorePatterns);
	LoadModJsons(strFolderPath + "music/", dictMusic, aIgnorePatterns);
	ParseMusic();
	LoadModJsons(strFolderPath + "cooverlays/", dictCOOverlays, aIgnorePatterns);
	Dictionary<string, JsonDCOCollection> dictionary2 = new Dictionary<string, JsonDCOCollection>();
	LoadModJsons(strFolderPath + "market/CoCollections/", dictionary2, aIgnorePatterns);
	BuildMarketDCOCollection(dictionary2);
	LoadModJsons(strFolderPath + "ledgerdefs/", dictLedgerDefs, aIgnorePatterns);
	LoadModJsons(strFolderPath + "pledges/", dictPledges, aIgnorePatterns);
	LoadModJsons(strFolderPath + "jobitems/", dictJobitems, aIgnorePatterns);
	LoadModJsons(strFolderPath + "jobs/", dictJobs, aIgnorePatterns);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_ship/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesShip);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_ship_adjectives/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesShipAdjectives);
	dictSimple.Clear();
	LoadModJsons(strFolderPath + "names_ship_nouns/", dictSimple, aIgnorePatterns);
	ParseSimpleIntoStringDict(dictSimple, dictNamesShipNouns);
	LoadModJsons(strFolderPath + "ai_training/", dictAIPersonalities, aIgnorePatterns);
	LoadModJsons(strFolderPath + "transit/", dictTransit, aIgnorePatterns);
	LoadModJsons(strFolderPath + "plot_manager/", dictPlotManager, aIgnorePatterns);
	LoadModJsons(strFolderPath + "star_systems/", dictStarSystems, aIgnorePatterns);
	LoadModJsons(strFolderPath + "parallax/", dictParallax, aIgnorePatterns);
	LoadModJsons(strFolderPath + "context/", dictContext, aIgnorePatterns);
	LoadModJsons(strFolderPath + "chargeprofiles/", dictChargeProfiles, aIgnorePatterns);
	LoadModJsons(strFolderPath + "wounds/", dictWounds, aIgnorePatterns);
	LoadModJsons(strFolderPath + "attackmodes/", dictAModes, aIgnorePatterns);
	LoadModJsons(strFolderPath + "pda_apps/", dictPDAAppIcons, aIgnorePatterns);
	LoadModJsons(strFolderPath + "zone_triggers/", dictZoneTriggers, aIgnorePatterns);
	LoadModJsons(strFolderPath + "tips/", dictTips, aIgnorePatterns);
	LoadModJsons(strFolderPath + "crime/", dictCrimes, aIgnorePatterns);
	LoadModJsons(strFolderPath + "plots/", dictPlots, aIgnorePatterns);
	LoadModJsons(strFolderPath + "plot_beats/", dictPlotBeats, aIgnorePatterns);
	LoadModJsons(strFolderPath + "racing/tracks/", dictRaceTracks, aIgnorePatterns);
	LoadModJsons(strFolderPath + "racing/leagues/", dictRacingLeagues, aIgnorePatterns);
	LoadModJsons(strFolderPath + "verbs/", dictJsonVerbs, aIgnorePatterns);
	LoadModJsons(strFolderPath + "tokens/", dictJsonTokens, aIgnorePatterns);
	LoadModJsons(strFolderPath + "info/", dictInfoNodes, aIgnorePatterns);
	LoadModJsons(strFolderPath + "installables/", dictInstallables, aIgnorePatterns);
	LoadModJsons(strFolderPath + "interaction_overrides/", dictIAOverrides, aIgnorePatterns);
	LoadModJsons(strFolderPath + "plot_beat_overrides/", dictPlotBeatOverrides, aIgnorePatterns);
	if (jmi.Status == GUIModRow.ModStatus.Missing)
	{
		jmi.Status = GUIModRow.ModStatus.Missing;
	}
	else if ((bool)ConsoleToGUI.instance && num < ConsoleToGUI.instance.ErrorCount)
	{
		jmi.Status = GUIModRow.ModStatus.Error;
	}
	else
	{
		jmi.Status = GUIModRow.ModStatus.Loaded;
	}
}
*/

/* DataHandler.LoadModJsons
private static void LoadModJsons<TJson>(string strFolderPath, Dictionary<string, TJson> dict, string[] aIgnorePatterns)
{
	if (!Directory.Exists(strFolderPath))
	{
		return;
	}
	string[] files = Directory.GetFiles(strFolderPath, "*.json", SearchOption.AllDirectories);
	string[] array = files;
	foreach (string strIn in array)
	{
		string text = PathSanitize(strIn);
		bool flag = false;
		if (aIgnorePatterns != null)
		{
			foreach (string value in aIgnorePatterns)
			{
				if (text.IndexOf(value) >= 0)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Debug.LogWarning("Ignore Pattern match: " + text + "; Skipping...");
		}
		else
		{
			JsonToData(text, dict);
		}
	}
}
*/

/* DataHandler.JsonToData
public static void JsonToData<TJson>(string strFile, Dictionary<string, TJson> dict)
{
	Debug.Log("#Info# Loading json: " + strFile);
	StringBuilder stringBuilder = new StringBuilder(70);
	try
	{
		string json = File.ReadAllText(strFile, Encoding.UTF8);
		stringBuilder.AppendLine("Converting json into Array...");
		TJson[] array = JsonMapper.ToObject<TJson[]>(json);
		TJson[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			TJson val = array2[i];
			stringBuilder.Append("Getting key: ");
			string text = null;
			Type type = val.GetType();
			PropertyInfo property = type.GetProperty("strName");
			if (property == null)
			{
				JsonLogger.ReportProblem("strName is missing", ReportTypes.FailingString);
			}
			object value = property.GetValue(val, null);
			text = value.ToString();
			stringBuilder.AppendLine(text);
			if (dict.ContainsKey(text))
			{
				Debug.Log("Warning: Trying to add " + text + " twice.");
				dict[text] = val;
			}
			else
			{
				dict.Add(text, val);
			}
		}
		array = null;
		json = null;
	}
	catch (Exception ex)
	{
		JsonLogger.ReportProblem(strFile, ReportTypes.SourceInfo);
		string text2 = ((stringBuilder.Length <= 1000) ? stringBuilder.ToString() : stringBuilder.ToString(stringBuilder.Length - 1000, 1000));
		Debug.LogError(text2 + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
	}
	if (strFile.IndexOf("osSGv1") >= 0)
	{
		Debug.Log(stringBuilder);
	}
}
*/
