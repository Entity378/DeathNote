using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using DeathNote;
using BepInEx.Logging;
using UnityEngine.InputSystem.XR;
using LethalLib.Modules;
using System.Linq;

namespace DeathNote
{
    public class UIControllerScript : MonoBehaviour
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public static UIControllerScript Instance { get; private set; }
        //public VisualElement root;
        public VisualElement veMain;

        public Label lblResult;
        public TextField txtPlayerUsername;
        public Button btnSubmit;
        public DropdownField dpdnDeathType;
        public TextField txtTimeOfDeath;
        public ProgressBar pbRemainingTime;
        //Label lblInstructions;
        public Label lblInstruction1;
        public Label lblInstruction2;
        public Label lblInstruction3;
        //Label lblSETitle;
        public Label lblSEDescription;
        public Label lblSEWarning;
        public Button btnActivateEyes;

        private void Start()
        {
            logger.LogDebug("UIControllerScript: Start()");

            if (Instance == null)
            {
                Instance = this;
            }


            // Get UIDocument
            logger.LogDebug("Getting UIDocument");
            UIDocument uiDocument = GetComponent<UIDocument>(); // TODO: might need to enable or something
            if (uiDocument == null) { logger.LogError("uiDocument not found."); return; }

            // Get VisualTreeAsset
            logger.LogDebug("Getting visual tree asset");
            //uiDocument.visualTreeAsset = DNAssetBundle.LoadAsset<VisualTreeAsset>("Assets/DeathNote/DeathnoteUI.uxml");
            if (uiDocument.visualTreeAsset == null) { logger.LogError("visualTreeAsset not found."); return; }
            
            // Instantiate root
            VisualElement root = uiDocument.visualTreeAsset.Instantiate();
            if (root == null) { logger.LogError("root is null!"); return; }
            logger.LogDebug("Adding root");
            uiDocument.rootVisualElement.Add(root);
            if (uiDocument.rootVisualElement == null) { logger.LogError("uiDocument.rootVisualElement not found."); return; }
            logger.LogDebug("Got root");
            root = uiDocument.rootVisualElement;

            veMain = uiDocument.rootVisualElement.Q<VisualElement>("veMain");
            veMain.style.display = DisplayStyle.None;
            if (veMain == null) { logger.LogError("veMain not found."); return; }
            //logger.LogMessage($"display: {veMain.style.display}");

            // Find elements
            lblResult = root.Q<Label>("lblResult");
            if (lblResult == null) { logger.LogError("lblResult not found."); return; }
            txtPlayerUsername = root.Q<TextField>("txtPlayerUsername");
            if (txtPlayerUsername == null) { logger.LogError("txtPlayerUsername not found."); return; }
            btnSubmit = root.Q<Button>("btnSubmit");
            if (btnSubmit == null) { logger.LogError("btnSubmit not found."); return; }
            dpdnDeathType = root.Q<DropdownField>("dpdnDeathType");
            if (dpdnDeathType == null) { logger.LogError("dpdnDeathType not found."); return; }
            txtTimeOfDeath = root.Q<TextField>("txtTimeOfDeath");
            if (txtTimeOfDeath == null) { logger.LogError("txtTimeOfDeath not found."); return; }
            pbRemainingTime = root.Q<ProgressBar>("pbRemainingTime");
            if (pbRemainingTime == null) { logger.LogError("pbRemainingTime not found."); return; }
            lblInstruction1 = root.Q<Label>("lblInstruction1");
            if (lblInstruction1 == null) { logger.LogError("lblInstruction1 not found."); return; }
            lblInstruction2 = root.Q<Label>("lblInstruction2");
            if (lblInstruction2 == null) { logger.LogError("lblInstruction2 not found."); return; }
            lblInstruction3 = root.Q<Label>("lblInstruction3");
            if (lblInstruction3 == null) { logger.LogError("lblInstruction3 not found."); return; }
            lblSEDescription = root.Q<Label>("lblSEDescription");
            if (lblSEDescription == null) { logger.LogError("lblSEDescription not found."); return; }
            lblSEWarning = root.Q<Label>("lblSEWarning");
            if (lblSEWarning == null) { logger.LogError("lblSEWarning not found."); return; }
            btnActivateEyes = root.Q<Button>("btnActivateEyes");
            if (btnActivateEyes == null) { logger.LogError("btnActivateEyes not found."); return; }

            // Set up controls
            dpdnDeathType.choices = new List<string>() { "Abandoned", "Blast", "Bludgeoning", "Crushing", "Drowning", "Electrocution", "Gravity", "Gunshots", "Kicking", "Mauling", "Strangulation", "Suffocation" };
            logger.LogDebug("Got Controls for UI");

            // Add event handlers
            btnSubmit.RegisterCallback<ClickEvent>(BtnSubmitOnClick);
            btnActivateEyes.RegisterCallback<ClickEvent>(BtnActivateEyesOnClick);
            txtPlayerUsername.RegisterCallback<KeyUpEvent>(txtPlayerUsernameOnValueChanged);

            logger.LogDebug("UIControllerScript: Start() complete");
        }

        private void Update()
        {
            if (veMain.style.display == DisplayStyle.Flex && Keyboard.current.escapeKey.wasPressedThisFrame) { HideUI(); }
            //Instance = this;
        }

        public void ShowUI()
        {
            logger.LogDebug("Showing UI");
            //VisualElement veMain = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("veMain");
            veMain.style.display = DisplayStyle.Flex;

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            StartOfRound.Instance.localPlayerUsingController = false;
        }

        public void HideUI()
        {
            logger.LogDebug("Hiding UI");
            //VisualElement veMain = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("veMain");
            veMain.style.display = DisplayStyle.None;

            UnityEngine.Cursor.lockState = CursorLockMode.Locked; // TODO: patch when escape is pressed to open the quick menu so it doesnt open the pause menu when in the ui
            UnityEngine.Cursor.visible = false;
            StartOfRound.Instance.localPlayerUsingController = false;
            // playerActions.Movement.Enable();
        }

        private void ResetUI()
        {
            logger.LogDebug("ResetUI");
            throw new NotImplementedException();
            // TODO: implement uiDocument.enabled = false;?

            /*UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null) { logger.LogError("uiDocument is null!"); return; }
            VisualElement veMain = GetComponent<UIDocument>().rootVisualElement;
            if (veMain == null) { logger.LogError("veMain is null!"); return; }

            veMain.Clear(); // Remove current UI
            veMain = uiDocument.visualTreeAsset.Instantiate(); // Instantiate new UI
            uiDocument.rootVisualElement.Add(veMain); // Add new UI*/
        }

        private void BtnSubmitOnClick(ClickEvent evt)
        {
            logger.LogDebug("BtnSubmitOnClick");
            throw new NotImplementedException();
        }

        private void BtnActivateEyesOnClick(ClickEvent evt)
        {
            logger.LogDebug("BtnActivateEyesOnClick");
            throw new NotImplementedException();
        }

        private void txtPlayerUsernameOnValueChanged(KeyUpEvent evt)
        {
            logger.LogDebug($"txtPlayerUsernameOnValueChanged: {evt.keyCode}");
            if (evt.keyCode == KeyCode.Return)
            {
                throw new NotImplementedException();
                // TODO: implement
            }
        }
    }
}
