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
            
            LoggerInstance.LogDebug("Setting up UI");
            //// Getting UI
            // Setting script
            UIControllerScript uiController = DeathNote.spawnPrefab.AddComponent<UIControllerScript>();
            uiController.enabled = true; // TODO: might not be necessary
            LoggerInstance.LogDebug("Got UIControllerScript");

            // Setting UiDocument
            LoggerInstance.LogDebug("Getting UIDocument");
            //UIDocument uiDocument = DeathNote.spawnPrefab.AddComponent<UIDocument>();
            UIDocument uiDocument = DeathNote.spawnPrefab.GetComponent<UIDocument>();
            if (uiDocument == null) { LoggerInstance.LogError("uiDocument not found."); return; }
            
            LoggerInstance.LogDebug("Getting visual tree asset");
            //uiDocument.visualTreeAsset = DNAssetBundle.LoadAsset<VisualTreeAsset>("Assets/DeathNote/DeathnoteUI.uxml");
            VisualTreeAsset visualTreeAsset = DNAssetBundle.LoadAsset<VisualTreeAsset>("Assets/DeathNote/DeathnoteUI.uxml");
            if (visualTreeAsset == null) { LoggerInstance.LogError("visualTreeAsset not found."); return; }
            //VisualElement root = uiDocument.visualTreeAsset.Instantiate();
            VisualElement root = visualTreeAsset.Instantiate();
            if (root == null) { LoggerInstance.LogError("root not found."); return; }
            
            LoggerInstance.LogDebug("Adding root");
            uiDocument.rootVisualElement.Add(root); // this is returning an error, nullreferenceexception
            LoggerInstance.LogDebug("Got root");

            // Get buttons
            uiController.lblResult = root.Q<Label>("lblResult");
            if(uiController.lblResult == null) { LoggerInstance.LogError("lblResult not found."); return; }
            uiController.txtPlayerUsername = root.Q<TextField>("txtPlayerUsername");
            if (uiController.txtPlayerUsername == null) { LoggerInstance.LogError("txtPlayerUsername not found."); return; }
            uiController.btnSubmit = root.Q<Button>("btnSubmit");
            if (uiController.btnSubmit == null) { LoggerInstance.LogError("btnSubmit not found."); return; }
            uiController.dpdnDeathType = root.Q<DropdownField>("dpdnDeathType");
            if (uiController.dpdnDeathType == null) { LoggerInstance.LogError("dpdnDeathType not found."); return; }
            uiController.txtTimeOfDeath = root.Q<TextField>("txtTimeOfDeath");
            if (uiController.txtTimeOfDeath == null) { LoggerInstance.LogError("txtTimeOfDeath not found."); return; }
            uiController.pbRemainingTime = root.Q<ProgressBar>("pbRemainingTime");
            if (uiController.pbRemainingTime == null) { LoggerInstance.LogError("pbRemainingTime not found."); return; }
            uiController.lblInstructions1 = root.Q<Label>("lblInstructions1");
            if (uiController.lblInstructions1 == null) { LoggerInstance.LogError("lblInstructions1 not found."); return; }
            uiController.lblInstructions2 = root.Q<Label>("lblInstructions2");
            if (uiController.lblInstructions2 == null) { LoggerInstance.LogError("lblInstructions2 not found."); return; }
            uiController.lblInstructions3 = root.Q<Label>("lblInstructions3");
            if (uiController.lblInstructions3 == null) { LoggerInstance.LogError("lblInstructions3 not found."); return; }
            uiController.lblSEDescription = root.Q<Label>("lblSEDescription");
            if (uiController.lblSEDescription == null) { LoggerInstance.LogError("lblSEDescription not found."); return; }
            uiController.lblSEWarning = root.Q<Label>("lblSEWarning");
            if (uiController.lblSEWarning == null) { LoggerInstance.LogError("lblSEWarning not found."); return; }
            uiController.btnActivateEyes = root.Q<Button>("btnActivateEyes");
            if (uiController.btnActivateEyes == null) { LoggerInstance.LogError("btnActivateEyes not found."); return; }

            uiController.Init();
            LoggerInstance.LogDebug("Got Controls for UI");

            



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
