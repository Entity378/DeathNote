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

        public static AssetBundle? DNAssetBundle;
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

            

            // Getting item
            LoggerInstance.LogDebug("Getting item");
            Item DeathNote = DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNoteItem.asset");
            
            // Assign script
            DeathNoteBehavior script = DeathNote.spawnPrefab.AddComponent<DeathNoteBehavior>();

            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = DeathNote;

            //// Getting UI
            // Setting script
            UIControllerScript uiController = DeathNote.spawnPrefab.AddComponent<UIControllerScript>();

            // Setting UiDocument
            UIDocument uiDocument = DeathNote.spawnPrefab.AddComponent<UIDocument>();
            //uiDocument.visualTreeAsset = DNAssetBundle.LoadAsset<VisualTreeAsset>("Assets/DeathNote/DeathnoteUI.uxml");
            
            VisualTreeAsset visualTreeAsset = DNAssetBundle.LoadAsset<VisualTreeAsset>("Assets/DeathNote/DeathnoteUI.uxml");
            //VisualElement root = uiDocument.visualTreeAsset.Instantiate();
            VisualElement root = visualTreeAsset.Instantiate();
            uiDocument.rootVisualElement.Add(root);

            // Get buttons
            uiController.lblResult = root.Q<Label>("lblResult");
            if(uiController.lblResult == null) { LoggerInstance.LogError("lblResult not found."); }
            uiController.txtPlayerUsername = root.Q<TextField>("txtPlayerUsername");
            if (uiController.txtPlayerUsername == null) { return; }
            uiController.dpdnDeathType = root.Q<DropdownField>("dpdnDeathType");
            if (uiController.dpdnDeathType == null) { return; }
            // TODO: Continue here



            // Register Scrap
            int iRarity = 5;
            NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            Utilities.FixMixerGroups(DeathNote.spawnPrefab);
            Items.RegisterScrap(DeathNote, iRarity, Levels.LevelTypes.All);

            NetworkHandler.Init();

            //exampleConfigforlater = Config.Bind("", "", , "");

            harmony.PatchAll();
            
            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }
    }
}
