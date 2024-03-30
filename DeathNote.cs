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

namespace DeathNoteMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    [BepInDependency("LethalNetworkAPI")]
    public class DeathNoteBase : BaseUnityPlugin
    {
        private const string modGUID = "Snowlance.DeathNote";
        private const string modName = "DeathNote";
        private const string modVersion = "0.1.0";

        public static AssetBundle DNAssetBundle;
        public static PlayerControllerB PlayerToDie;
        public static DeathNoteBase PluginInstance { get; private set; } = null!;
        public static ManualLogSource LoggerInstance;
        private readonly Harmony harmony = new Harmony(modGUID);

        //public static ConfigEntry<float> configVolume;

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
                LoggerInstance.LogError("Failed to load custom assets.");
                return;
            }

            // Registering item
            int iRarity = 1000;
            LoggerInstance.LogDebug("Getting item");

            Item DeathNote = DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNoteItem.asset");
            DeathNoteBehavior script = DeathNote.spawnPrefab.AddComponent<DeathNoteBehavior>();

            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = DeathNote;

            NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            Utilities.FixMixerGroups(DeathNote.spawnPrefab);
            Items.RegisterScrap(DeathNote, iRarity, Levels.LevelTypes.All);

            NetworkHandler.Init();

            //exampleConfigforlater = Config.Bind("", "", , "");

            harmony.PatchAll();
            
            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }

        public static void SendChatMessage(string message)
        {
            MethodInfo chat = AccessTools.Method(typeof(HUDManager), "AddChatMessage");
            chat?.Invoke(HUDManager.Instance, new object[] { message, "" });
        }
    }
}
