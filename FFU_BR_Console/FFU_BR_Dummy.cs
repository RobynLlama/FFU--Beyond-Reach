using MonoMod;

namespace FFU_Beyond_Reach {
    [MonoModIgnore] public partial class FFU_BR_Defs {
        public static SyncLogs SyncLogging = 0;
        public static ActLogs ActLogging = 0;
        public static bool DynamicRandomRange;
        public static int MaxLogTextSize;
        public static bool ModSyncLoading;
        public static bool EnableCodeFixes;
        public static bool ModifyUpperLimit;
        public static float BonusUpperLimit;
        public static float SuitOxygenNotify;
        public static float SuitPowerNotify;
        public static bool ShowEachO2Battery;
        public static bool StrictInvSorting;
        public static bool AltTempEnabled;
        public static string AltTempSymbol;
        public static float AltTempMult;
        public static float AltTempShift;
        public static bool TowBraceAllowsKeep;
        public static bool OrgInventoryMode;
        public static float[] OrgInventoryTweaks;
        public static bool BetterInvTransfer;
        public static bool QuickBarPinning;
        public static float[] QuickBarTweaks;
        public static bool NoSkillTraitCost;
        public static bool AllowSuperChars;
        public static float SuperCharMultiplier;
        public static string[] SuperCharacters;
        public enum SyncLogs {
            None,
            ModChanges,
            DeepCopy,
            ModdedDump,
            ExtendedDump,
            ContentDump,
            SourceDump,
        }
        public enum ActLogs {
            None,
            Interactions,
            Runtime
        }
    }
}