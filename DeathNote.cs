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
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class DeathNoteBase : BaseUnityPlugin
    {
        private const string modGUID = "Snowlance.DeathNote";
        private const string modName = "DeathNote";
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

            LoggerInstance = PluginInstance.Logger;
            LoggerInstance.LogDebug($"Plugin {modName} loaded successfully.");

            // Loading assets
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DNAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "DNAssetBundle/DNAssetBundle"));
            LoggerInstance.LogDebug($"Got DNAssetBundle at: {Path.Combine(sAssemblyLocation, "DNAssetBundle/DNAssetBundle")}");
            if (DNAssetBundle == null)
            {
                LoggerInstance.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }

            // Registering item
            int iRarity = 10;
            LoggerInstance.LogDebug("Getting item");
            Item DeathNote = DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNotePrefab.prefab"); // TODO: LOAD PROPER ASSET HERE, NEED PATHING TO THE PREFAB OR death_note.asset file
            if (DeathNote == null)
            {
                LoggerInstance.LogError("DeathNote Item not found...");
            }
            LoggerInstance.LogDebug("pass1");
            LethalLib.Modules.Utilities.FixMixerGroups(DeathNote.spawnPrefab); // TODO: CRASHING HERE
            LoggerInstance.LogDebug("pass2");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            LoggerInstance.LogDebug("pass3");
            LethalLib.Modules.Items.RegisterScrap(DeathNote, iRarity, LethalLib.Modules.Levels.LevelTypes.All);
            LoggerInstance.LogDebug("pass4");

            //configVolume = Config.Bind("Volume", "MusicVolume", 1f, "Volume of the music. Must be between 0 and 1.");

            harmony.PatchAll();
            
            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }
    }
}
