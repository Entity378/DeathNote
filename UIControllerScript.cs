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
using TMPro;

namespace DeathNote
{
    public class UIControllerScript : MonoBehaviour
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public static UIControllerScript Instance { get; private set; }
        public VisualElement veMain;
        public int timeRemaining = 40; // TODO: get this from config
        private bool verifying = false;

        

        //private List<PlayerToDie> PlayersToDie;

        public Label lblResult;
        public TextField txtPlayerUsername;
        public Button btnSubmit;
        public DropdownField dpdnDeathType;
        public TextField txtTimeOfDeath;
        public ProgressBar pbRemainingTime;
        public Label lblSEDescription;
        public Button btnActivateEyes;

        private void Start()
        {
            logger.LogDebug("UIControllerScript: Start()");

            if (Instance == null)
            {
                Instance = this;
            }

            //PlayersToDie = new List<PlayerToDie>();

            // Get UIDocument
            logger.LogDebug("Getting UIDocument");
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null) { logger.LogError("uiDocument not found."); return; }

            // Get VisualTreeAsset
            logger.LogDebug("Getting visual tree asset");
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

            // Find elements
            lblResult = root.Q<Label>("lblResult");
            if (lblResult == null) { logger.LogError("lblResult not found."); return; }

            txtPlayerUsername = root.Q<TextField>("txtPlayerUsername");
            if (txtPlayerUsername == null) { logger.LogError("txtPlayerUsername not found."); return; }

            btnSubmit = root.Q<Button>("btnSubmit");
            if (btnSubmit == null) { logger.LogError("btnSubmit not found."); return; }

            dpdnDeathType = root.Q<DropdownField>("dpdnDeathType");
            if (dpdnDeathType == null) { logger.LogError("dpdnDeathType not found."); return; }
            dpdnDeathType.choices = DeathController.GetCauseOfDeathsAsStrings();
            dpdnDeathType.index = 0;

            txtTimeOfDeath = root.Q<TextField>("txtTimeOfDeath");
            if (txtTimeOfDeath == null) { logger.LogError("txtTimeOfDeath not found."); return; }

            pbRemainingTime = root.Q<ProgressBar>("pbRemainingTime");
            if (pbRemainingTime == null) { logger.LogError("pbRemainingTime not found."); return; }
            pbRemainingTime.highValue = timeRemaining;

            lblSEDescription = root.Q<Label>("lblSEDescription");
            if (lblSEDescription == null) { logger.LogError("lblSEDescription not found."); return; }

            btnActivateEyes = root.Q<Button>("btnActivateEyes");
            if (btnActivateEyes == null) { logger.LogError("btnActivateEyes not found."); return; }

            txtTimeOfDeath = root.Q<TextField>("txtTimeOfDeath");
            if (txtTimeOfDeath == null) { logger.LogError("txtTimeOfDeath not found."); return; }

            pbRemainingTime = root.Q<ProgressBar>("pbRemainingTime");
            if (pbRemainingTime == null) { logger.LogError("pbRemainingTime not found."); return; }
            pbRemainingTime.highValue = timeRemaining;

            lblSEDescription = root.Q<Label>("lblSEDescription");
            if (lblSEDescription == null) { logger.LogError("lblSEDescription not found."); return; }

            btnActivateEyes = root.Q<Button>("btnActivateEyes");
            if (btnActivateEyes == null) { logger.LogError("btnActivateEyes not found."); return; }

            logger.LogDebug("Got Controls for UI");

            // Add event handlers
            btnSubmit.clickable.clicked += BtnSubmitOnClick;
            btnActivateEyes.RegisterCallback<ClickEvent>(BtnActivateEyesOnClick);
            txtPlayerUsername.RegisterCallback<KeyUpEvent>(txtPlayerUsernameOnValueChanged);

            logger.LogDebug("UIControllerScript: Start() complete");
            txtPlayerUsername.value = "PLAYER #0"; // TODO: Testing, remove later
        }

        private void Update()
        {
            if (veMain.style.display == DisplayStyle.Flex && Keyboard.current.escapeKey.wasPressedThisFrame) { HideUI(); }
        }

        public void ShowUI()
        {
            logger.LogDebug("Showing UI");
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
            veMain.style.display = DisplayStyle.None;

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            StartOfRound.Instance.localPlayerUsingController = false;
            IngamePlayerSettings.Instance.playerInput.ActivateInput();
            StartOfRound.Instance.localPlayerController.disableLookInput = false;
        }

        private void ResetUI1() // TODO: might not be needed
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

        private void ResetUI()
        {
            txtPlayerUsername.value = "";
            txtPlayerUsername.isReadOnly = false;
            dpdnDeathType.style.display = DisplayStyle.None;
            dpdnDeathType.index = 0;
            txtTimeOfDeath.style.display = DisplayStyle.None;
            txtTimeOfDeath.value = "";
            pbRemainingTime.style.display = DisplayStyle.Flex;
            pbRemainingTime.value = 0;

            verifying = false;
        }

        private void StartKillTimer(DeathController deathController) // TODO: MAIN TIMER, WILL BE A LOT
        {
            deathController.causeOfDeath = DeathController.GetCauseOfDeathFromString(dpdnDeathType.value);
            // TODO: Make sure all these changes work and continue here, get timeofdeath next
        }
        private IEnumerator StartKillTimerCoroutine()
        {
            throw new NotImplementedException();
        }

        private void StartProgressBarTimer(DeathController deathController)
        {
            StartCoroutine(StartProgressBarTimerCoroutine(deathController));
        }
        private IEnumerator StartProgressBarTimerCoroutine(DeathController deathController)
        {
            while (pbRemainingTime.value < pbRemainingTime.highValue)
            {
                pbRemainingTime.value = TimeOfDay.Instance.currentDayTime;
                lblResult.text = $"{pbRemainingTime.lowValue} || {pbRemainingTime.value} || {pbRemainingTime.highValue}";
                yield return new WaitForSeconds(Time.deltaTime);
            }

            StartKillTimer(deathController);
        }

        private void BtnSubmitOnClick()
        {
            // normalizedTime = currentDayTime / totalTime;
            logger.LogDebug("BtnSubmitOnClick");

            if (verifying)
            {


                // TODO: For when you press submit the second time, locking it in and adding it to the second page. make sure to parse everything and showresults if they are wrong
                verifying = false;
                return;
            }

            ShowResults("Searching for player to kill...", 0.5f); // TODO: TEMP FOR TESTING DELETE ME
            
            PlayerControllerB playerToDie = StartOfRound.Instance.allPlayerScripts.ToList().Where(x => x.playerUsername.ToLower() == txtPlayerUsername.text.ToLower()).FirstOrDefault();

            if (playerToDie != null) // TODO: TEMP FOR TESTING CHANGE BACK TO !=
            {
                if (playerToDie.isPlayerDead) { ShowResults("Player is already dead"); return; }
                logger.LogDebug($"Found player to kill: {playerToDie.playerUsername}");
                ShowResults($"Found player to kill: {playerToDie.playerUsername}");

                DeathController deathController = new DeathController();
                deathController.PlayerToDie = playerToDie;
                
                txtPlayerUsername.isReadOnly = true;
                dpdnDeathType.style.display = DisplayStyle.Flex;
                dpdnDeathType.index = 0;
                txtTimeOfDeath.style.display = DisplayStyle.Flex;
                txtTimeOfDeath.value = "";
                pbRemainingTime.style.display = DisplayStyle.Flex;

                pbRemainingTime.lowValue = 0;
                //pbRemainingTime.lowValue = TimeOfDay.Instance.currentDayTime; // TODO: This keeps starting at 75% rather than the beginning
                pbRemainingTime.value = pbRemainingTime.lowValue;
                pbRemainingTime.highValue = TimeOfDay.Instance.currentDayTime + timeRemaining;

                txtTimeOfDeath.value = NormalizedToClock(pbRemainingTime.highValue / TimeOfDay.Instance.totalTime);
                verifying = true;
                StartProgressBarTimer(deathController); // TODO: implement


                // TODO: Continue here?
            }
            else
            {
                ShowResults("Could not find player to kill");
            }
        }

        private void txtPlayerUsernameOnValueChanged(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                BtnSubmitOnClick();
            }
        }

        public string NormalizedToClock(float timeNormalized) // THIS WORKS
        {
            int numberOfHours = TimeOfDay.Instance.numberOfHours;
            int num = (int)(timeNormalized * (60f * numberOfHours)) + 360;
            logger.LogDebug($"num: {num}");
            int num2 = (int)Mathf.Floor(num / 60);
            logger.LogDebug($"num2: {num2}");

            string amPM = "AM";
            if (num2 >= 24)
            {
                return "12:00AM";
            }
            if (num2 < 12)
            {
                amPM = "AM";
            }
            else
            {
                amPM = "PM";
            }
            if (num2 > 12)
            {
                num2 %= 12;
                logger.LogDebug($"num2 changed: {num2}");
            }
            int num3 = num % 60;
            logger.LogDebug($"num3: {num3}");
            string text = $"{num2:00}:{num3:00}".TrimStart('0') + amPM;
            return text;
        }

        public float ClockToNormalized(string timeString) // THIS WORKS DONT TOUCH FOR THE LOVE OF GOD
        {
            timeString = timeString.ToUpper().Replace(" ", "").Replace("\n", "");

            int numberOfHours = TimeOfDay.Instance.numberOfHours;
            float lengthOfHours = TimeOfDay.Instance.lengthOfHours;
            float totalTime = TimeOfDay.Instance.totalTime;

            int startHour = (24 - numberOfHours);

            // Split the time string into hours, minutes, and AM/PM
            string[] timeParts = timeString.Split(':');
            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1].Substring(0, 2)); // Extract minutes
            string amPm = timeParts[1].Substring(2); // Extract AM/PM

            // Convert hours to 24-hour format
            if (amPm == "PM")
            {
                hours += 12;
            }
            else if (amPm == "AM" && hours == 12)
            {
                hours = 0;
            }


            // Calculate the total number of minutes
            hours = hours - startHour;
            float totalMinutes = (hours * lengthOfHours) + minutes;

            // Convert total minutes to normalized time
            // normalizedTime = currentDayTime / totalTime;
            float normalizedTime = totalMinutes / totalTime; // Assuming 24-hour day

            return normalizedTime;
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
            if (verifying)
            {
                verifying = false;
            }
            else { verifying = true; }

            logger.LogDebug("BtnActivateEyesOnClick");
            //throw new NotImplementedException();
        }
    }
}
