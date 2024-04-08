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
            
            LoggerInstance.LogDebug("Setting up UI");
            //// Getting UI
            // Setting script
            UIControllerScript uiController = DeathNote.spawnPrefab.AddComponent<UIControllerScript>();
            if (uiController == null) { LoggerInstance.LogError("uiController not found."); return; }
            //uiController.enabled = true; // TODO: might not be necessary
            LoggerInstance.LogDebug("Got UIControllerScript");

            // Setting UiDocument
            LoggerInstance.LogDebug("Getting UIDocument");
            UIDocument uiDocument = DeathNote.spawnPrefab.GetComponent<UIDocument>();
            if (uiDocument == null) { LoggerInstance.LogError("uiDocument not found."); return; }
            
            LoggerInstance.LogDebug("Getting visual tree asset");
            uiDocument.visualTreeAsset = DNAssetBundle.LoadAsset<VisualTreeAsset>("Assets/DeathNote/DeathnoteUI.uxml");
            if (uiDocument.visualTreeAsset == null) { LoggerInstance.LogError("visualTreeAsset not found."); return; }
            VisualElement root = uiDocument.visualTreeAsset.Instantiate();
            if (root == null) { LoggerInstance.LogError("root not found."); return; }

            if (uiDocument.rootVisualElement == null) { LoggerInstance.LogError("uiDocument.rootVisualElement not found."); return; }
            LoggerInstance.LogDebug("Adding root");
            //root.style.display = DisplayStyle.None;
            uiDocument.rootVisualElement.Add(root);
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            uiController.root = root;
            LoggerInstance.LogDebug("Got root");

            


            // Register Scrap
            int iRarity = 1000;
            NetworkPrefabs.RegisterNetworkPrefab(DeathNote.spawnPrefab);
            Utilities.FixMixerGroups(DeathNote.spawnPrefab);
            Items.RegisterScrap(DeathNote, iRarity, Levels.LevelTypes.All); // TODO: Get Item from here and not from AssetBundle; may need to readd panel settings and remove when hiding the ui
            //DeathNote.spawnPrefab.AddComponent<PanelSettings>(); // TODO: remove when hiding the ui> use something like this for panel settings???

            NetworkHandler.Init();

            //exampleConfigforlater = Config.Bind("", "", , "");

            harmony.PatchAll();
            
            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }
    }
}
