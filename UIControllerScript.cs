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

namespace DeathNote
{
    public class UIControllerScript : MonoBehaviour
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public VisualElement root;
        private Item deathNoteItem;

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

        public void Start()
        {
            logger.LogMessage("UIControllerScript: Start()");

            deathNoteItem = DeathNoteBase.DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNoteItem.asset");
            if (deathNoteItem == null) { logger.LogError("deathNoteItem is null!"); return; }
            root = deathNoteItem.spawnPrefab.GetComponent<UIDocument>().rootVisualElement;
            if (root == null) { logger.LogError("root is null!"); return; }
            HideUI();

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

            dpdnDeathType.choices = new List<string>() { "Abandoned", "Blast", "Bludgeoning", "Crushing", "Drowning", "Electrocution", "Gravity", "Gunshots", "Kicking", "Mauling", "Strangulation", "Suffocation" };
            logger.LogDebug("Got Controls for UI");

            btnSubmit.clicked += BtnSubmitOnClick;
            logger.LogDebug("UIControllerScript: btnSubmit.clicked += BtnSubmitOnClick;");
            txtPlayerUsername.RegisterCallback<KeyUpEvent>(txtPlayerUsernameOnValueChanged);
            root.RegisterCallback<KeyUpEvent>(rootOnKeyUpEvent);
            btnActivateEyes.clicked += BtnActivateEyesOnClick;
            HideUI(); // this dont work as expected??????
            logger.LogDebug("UIControllerScript: End()");
            // TODO: code gets to here, but UI keeps showing when item is spawned in the game
        }

        public void Update()
        {
            root.style.display = DisplayStyle.None;
        }

        public void ShowUI()
        {
            if (root == null) { logger.LogError("root is null!"); return; }
            root.style.display = DisplayStyle.Flex;
        }

        public void HideUI()
        {
            if (root == null) { logger.LogError("root is null!"); return; }
            root.style.display = DisplayStyle.None;
            root.style.visibility = Visibility.Hidden;
        }

        public void ResetUI()
        {

            Item DeathNote = DeathNoteBase.DNAssetBundle.LoadAsset<Item>("Assets/DeathNote/DeathNoteItem.asset");
            UIDocument uiDocument = DeathNote.spawnPrefab.GetComponent<UIDocument>();

            root.Clear(); // Remove current UI
            root = uiDocument.visualTreeAsset.Instantiate(); // Instantiate new UI
            uiDocument.rootVisualElement.Add(root); // Add new UI
        }

        private void txtPlayerUsernameOnValueChanged(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                BtnActivateEyesOnClick();
            }
        }

        private void rootOnKeyUpEvent(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape && root.style.display == DisplayStyle.Flex)
            {
                root.style.display = DisplayStyle.None;
            }
        }

        private void BtnActivateEyesOnClick()
        {
            throw new NotImplementedException();
        }

        private void BtnSubmitOnClick()
        {
            throw new NotImplementedException();
        }
    }
}
