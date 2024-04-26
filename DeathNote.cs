using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using LethalLib.Modules;
using LethalLib;
using GameNetcodeStuff;
using DeathNote;
using UnityEngine.UIElements;

namespace DeathNote
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    [BepInDependency("LethalNetworkAPI")]
    public class DeathNoteBase : BaseUnityPlugin
    {
        private const string modGUID = "Snowlance.DeathNote";
        private const string modName = "DeathNote";
        private const string modVersion = "0.3.2";

        public static AssetBundle? DNAssetBundle;

        public static DeathNoteBase PluginInstance { get; private set; } = null!;
        public static ManualLogSource LoggerInstance;
        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConfigEntry<int> configRarity;
        public static ConfigEntry<bool> configTimer;
        public static ConfigEntry<int> configTimerLength;
        public static ConfigEntry<bool> configAllowEarlySubmit;
        public static ConfigEntry<bool> configShowPlayerList;

        public static ConfigEntry<bool> configShinigamiEyes;
        public static ConfigEntry<bool> configPermanentEyes;

        public static ConfigEntry<bool> configAlwaysShowPlayerNames;
        public static ConfigEntry<bool> configShowEnemyNames;
        public static ConfigEntry<bool> configShowUnkillableEnemyNames; // can break

        //public static ConfigEntry<string> configCustomNames;

        private void Awake()
        {
            if (PluginInstance == null)
            {
                PluginInstance = this;
            }

            LoggerInstance = PluginInstance.Logger;
            LoggerInstance.LogDebug($"Plugin {modName} loaded successfully.");

            configRarity = Config.Bind("General", "Rarity", 5, "Rarity of the death note.");
            configTimer = Config.Bind("General", "Timer", true, "If picking the death details should have a time limit.\nWhen you enter a name and click submit, you'll have x in-game seconds to fill in a time and details before it adds the name to the book.");
            configTimerLength = Config.Bind("General", "Timer Length", 40, "Timer length. 40 is lore accurate.");
            configAllowEarlySubmit = Config.Bind("General", "Allow Early Submit", true, "Allows you to click submit again to add it to the book early. Turn this off if you want a cooldown mechanic.");
            
            configShowPlayerList = Config.Bind("Accessibility", "Show PlayerList", false, "Show a dropdown list of players under the name input to select from instead.");

            configShinigamiEyes = Config.Bind("Shinigami Eyes", "Shinigami Eyes", true, "Allows you to trade half of your max health for the ability to see certain entity names (configurable in Names section).\nEnemy names require you to scan them.");
            configPermanentEyes = Config.Bind("Shinigami Eyes", "Permanent Eyes", false, "Makes Shinigami Eyes permanent. Disabling this will reset the ability at the end of every round.");

            configAlwaysShowPlayerNames = Config.Bind("Names", "AlwaysShowPlayerNames", false, "Always shows player names above their head. Disabling this will only show player names when you have the Shinigami Eyes.");
            configShowEnemyNames = Config.Bind("Names", "ShowEnemyNames", true, "Allows you to see enemy names when scanning them if you have the Shinigami Eyes.");
            configShowUnkillableEnemyNames = Config.Bind("Names", "ShowUnkillableEnemyNames", false, "Allows you to see the names of enemies that are immortal. WARNING: Killing them can break things or cause bugs.");

            // Loading assets
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DNAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "mod_assets"));
            LoggerInstance.LogDebug($"Got DNAssetBundle at: {Path.Combine(sAssemblyLocation, "mod_assets")}");
            if (DNAssetBundle == null)
            {
                LoggerInstance.LogError("Failed to load custom assets.");
                return;
            }

            // Getting item
            LoggerInstance.LogDebug("Getting item");
            Item DeathNote = DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNoteItem.asset");
            LoggerInstance.LogDebug($"Got item: {DeathNote.name}");
            
            // Assign behavior script
            DeathNoteBehavior script = DeathNote.spawnPrefab.AddComponent<DeathNoteBehavior>();

            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = DeathNote;

            // Assign UIControllerScript
            LoggerInstance.LogDebug("Setting up UI");
            UIControllerScript uiController = DeathNote.spawnPrefab.AddComponent<UIControllerScript>(); // WHY TF IS THIS BREAKING NOW
            if (uiController == null) { LoggerInstance.LogError("uiController not found."); return; }
            LoggerInstance.LogDebug("Got UIControllerScript");

            // Register Scrap
            int iRarity = configRarity.Value;
            NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            Utilities.FixMixerGroups(DeathNote.spawnPrefab);
            Items.RegisterScrap(DeathNote, iRarity, Levels.LevelTypes.All);
            
            NetworkHandler.Init();

            harmony.PatchAll();
            
            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }

        private void Update()
        {
            if (StartOfRound.Instance != null && DeathController.ShinigamiEyesActivated)
            {
                PlayerControllerB localPlayer = StartOfRound.Instance.localPlayerController;
                if (localPlayer.health > (DeathController.HalfHealth))
                {
                    localPlayer.DamagePlayer(localPlayer.health - DeathController.HalfHealth, false, true, CauseOfDeath.Unknown, -1); // TODO: Test this more
                }
            }
        }
    }
}