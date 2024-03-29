using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using LethalLib.Modules;
using LethalLib;

namespace DeathNoteMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class DeathNoteBase : BaseUnityPlugin
    {
        private const string modGUID = "Snowlance.DeathNote";
        private const string modName = "DeathNote";
        private const string modVersion = "0.1.0";

        public static AssetBundle DNAssetBundle;

        public static DeathNoteBase PluginInstance { get; private set; } = null!;
        public static ManualLogSource LoggerInstance;
        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConfigEntry<float> configVolume;

        private void Awake()
        {
            if (PluginInstance == null)
            {
                PluginInstance = this;
            }

            LoggerInstance = PluginInstance.Logger;
            LoggerInstance.LogDebug($"Plugin {modName} loaded successfully.");

            // Loading assets
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DNAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "mod_assets"));
            LoggerInstance.LogDebug($"Got DNAssetBundle at: {Path.Combine(sAssemblyLocation, "mod_assets")}");
            if (DNAssetBundle == null)
            {
                LoggerInstance.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }

            // Registering item
            int iRarity = 1000;
            LoggerInstance.LogDebug("Getting item");

            Item DeathNote = DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNoteItem.asset");
            NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            Utilities.FixMixerGroups(DeathNote.spawnPrefab);
            Items.RegisterScrap(DeathNote, iRarity, Levels.LevelTypes.All); // TODO: ITEM SPAWNS IN GAME BUT ITS HALFWAY IN THE FLOOR NO MATTER WHAT I DO, REFOLLOW TUTORIAL

            //configVolume = Config.Bind("Volume", "MusicVolume", 1f, "Volume of the music. Must be between 0 and 1.");

            harmony.PatchAll();
            
            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }
    }
}
