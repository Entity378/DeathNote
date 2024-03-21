using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using static LethalLib.Modules.ContentLoader;

namespace DeathNoteMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class DeathNoteBase : BaseUnityPlugin
    {
        private const string modGUID = "Snowlance.DeathNoteMod";
        private const string modName = "DeathNoteMod";
        private const string modVersion = "0.1.0";

        public static AssetBundle DNAssetBundle;

        public static DeathNoteBase PluginInstance { get; private set; } = null!;
        public static ManualLogSource LoggerInstance { get; private set; }
        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConfigEntry<float> configVolume;

        private void Awake()
        {
            if (PluginInstance == null)
            {
                PluginInstance = this;
            }

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DNAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "DNAssetBundle/DNAssetBundle"));
            if (DNAssetBundle == null)
            {
                LoggerInstance.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }

            int iRarity = 10;
            Item DeathNote = DNAssetBundle.LoadAsset<Item>("directory/to/itemdataasset.asset");
            LethalLib.Modules.Utilities.FixMixerGroups(DeathNote.spawnPrefab);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(DeathNote, iRarity, LethalLib.Modules.Levels.LevelTypes.All);

            LoggerInstance = PluginInstance.Logger;
            LoggerInstance.LogDebug($"Plugin {modName} loaded successfully.");

            configVolume = Config.Bind("Volume", "MusicVolume", 1f, "Volume of the music. Must be between 0 and 1.");

            harmony.PatchAll();

            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
            
        }
    }
}
