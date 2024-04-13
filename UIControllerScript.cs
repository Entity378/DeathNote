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
using GameNetcodeStuff;
using System.Collections;

namespace DeathNote
{
    public class UIControllerScript : MonoBehaviour
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public static UIControllerScript Instance { get; private set; }
        //public VisualElement root;
        public VisualElement veMain;
        public int timeRemaining = 40;
        private bool shinigamiEyesActivated = false;
        private bool verifying = false;

        public Label lblResult;
        public TextField txtPlayerUsername;
        public Button btnSubmit;
        public DropdownField dpdnDeathType;
        public TextField txtTimeOfDeath;
        public ProgressBar pbRemainingTime;
        //Label lblSETitle;
        public Label lblSEDescription;
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
            pbRemainingTime.highValue = timeRemaining;
            lblSEDescription = root.Q<Label>("lblSEDescription");
            if (lblSEDescription == null) { logger.LogError("lblSEDescription not found."); return; }
            btnActivateEyes = root.Q<Button>("btnActivateEyes");
            if (btnActivateEyes == null) { logger.LogError("btnActivateEyes not found."); return; }

            // Set up controls
            dpdnDeathType.choices = new List<string>() { "Abandoned", "Blast", "Bludgeoning", "Crushing", "Drowning", "Electrocution", "Gravity", "Gunshots", "Kicking", "Mauling", "Strangulation", "Suffocation" };
            logger.LogDebug("Got Controls for UI");

            // Add event handlers
            btnSubmit.clickable.clicked += BtnSubmitOnClick;
            btnActivateEyes.RegisterCallback<ClickEvent>(BtnActivateEyesOnClick);
            txtPlayerUsername.RegisterCallback<KeyUpEvent>(txtPlayerUsernameOnValueChanged);

            logger.LogDebug("UIControllerScript: Start() complete");
        }

        private void Update()
        {
            if (veMain.style.display == DisplayStyle.Flex && Keyboard.current.escapeKey.wasPressedThisFrame) { HideUI(); }
            // TODO: do coroutines in here instead?
        }

        public void ShowUI()
        {
            logger.LogDebug("Showing UI");
            //VisualElement veMain = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("veMain");
            veMain.style.display = DisplayStyle.Flex;

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            StartOfRound.Instance.localPlayerUsingController = false;
            IngamePlayerSettings.Instance.playerInput.DeactivateInput();
            StartOfRound.Instance.localPlayerController.disableLookInput = true;
        }

        public void HideUI()
        {
            logger.LogDebug("Hiding UI");
            //VisualElement veMain = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("veMain");
            veMain.style.display = DisplayStyle.None;

            UnityEngine.Cursor.lockState = CursorLockMode.Locked; // TODO: patch when escape is pressed to open the quick menu so it doesnt open the pause menu when in the ui
            UnityEngine.Cursor.visible = false;
            StartOfRound.Instance.localPlayerUsingController = false;
            IngamePlayerSettings.Instance.playerInput.ActivateInput();
            StartOfRound.Instance.localPlayerController.disableLookInput = false;
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

        private void BtnSubmitOnClick()
        {
            if (verifying)
            {
                // TODO: For when you press submit the second time, locking it in and adding it to the second page

                return;
            }

            logger.LogDebug("BtnSubmitOnClick");
            ShowResults("Searching for player to kill...", 5f, true); // TODO: TEMP FOR TESTING DELETE ME

            string name = txtPlayerUsername.text;
            logger.LogDebug($"Got name: {name}");
            
            DeathController.PlayerToDie = StartOfRound.Instance.allPlayerScripts.ToList().Where(x => x.playerUsername.ToLower() == name).FirstOrDefault();

            if (DeathController.PlayerToDie == null) // TODO: TEMP FOR TESTING CHANGE BACK TO !=
            {
                //if (DeathController.PlayerToDie.isPlayerDead) { return; } // TODO: DISABLED FOR TESTING
                //logger.LogDebug($"Found player to kill: {DeathController.PlayerToDie.playerUsername}"); // TODO: DISABLED FOR TESTING
                //DeathController.KillPlayer();
                //ShowResults($"Found player to kill: {DeathController.PlayerToDie.playerUsername}\nThey will die from a heart attack in 40 seconds.");
                txtPlayerUsername.isReadOnly = true;
                dpdnDeathType.style.display = DisplayStyle.Flex;
                txtTimeOfDeath.style.display = DisplayStyle.Flex;
                pbRemainingTime.style.display = DisplayStyle.Flex;
                
                verifying = true;
                // TODO: Continue here
            }
        }

        private void txtPlayerUsernameOnValueChanged(KeyUpEvent evt) // THIS IS DONE
        {
            //logger.LogDebug($"txtPlayerUsernameOnValueChanged: {evt.keyCode}");
            if (evt.keyCode == KeyCode.Return)
            {
                BtnSubmitOnClick();
            }
        }

        public void ShowResults(string message, float duration = 3f, bool flash = false)
        {
            lblResult.text = message;
            StartCoroutine(ShowResultsCoroutine(message, duration, flash));
        }
        private IEnumerator ShowResultsCoroutine(string message, float duration, bool flash)
        {
            float endTime = Time.time + duration;
            bool isRed = true;

            if (flash == true)
            {
                while (Time.time < endTime)
                {
                    if (isRed)
                    {
                        lblResult.style.color = Color.black;
                    }
                    else
                    {
                        lblResult.style.color = Color.red;
                    }

                    isRed = !isRed;

                    yield return new WaitForSeconds(0.75f);
                }
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }


            lblResult.style.color = Color.red;
            lblResult.text = "";
        }

        private void BtnActivateEyesOnClick(ClickEvent evt)
        {
            logger.LogDebug("BtnActivateEyesOnClick");
            throw new NotImplementedException();
        }
    }
}
