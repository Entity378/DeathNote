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
        //public VisualElement root;
        public VisualElement veMain;
        public int timeRemaining = 40;
        private bool shinigamiEyesActivated = false;
        private bool verifying = false;

        //private List<PlayerToDie> PlayersToDie;


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

            //PlayersToDie = new List<PlayerToDie>();

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

            // TODO: Testing stuff
            txtTimeOfDeath.style.display = DisplayStyle.Flex;
        }

        private void Update()
        {
            if (veMain.style.display == DisplayStyle.Flex && Keyboard.current.escapeKey.wasPressedThisFrame) { HideUI(); }
            // TODO: do coroutines in here instead?
            // TODO: Testing stuff
            if (verifying)
            {
                string time = HUDManager.Instance.clockNumber.text;
                float normalizedTimeOfDay = TimeOfDay.Instance.normalizedTimeOfDay;
                float globalTime = TimeOfDay.Instance.globalTime;
                float totalTime = TimeOfDay.Instance.totalTime;
                float numberofhours = TimeOfDay.Instance.numberOfHours;

                lblResult.text = time + " || " + normalizedTimeOfDay.ToString("F20") + " || " + globalTime + " || " + totalTime + " || " + numberofhours;
            }
            
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

        private void ResetUI() // TODO: might not be needed
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

        private void StartKillTimer()
        {
            throw new NotImplementedException();
        }
        private IEnumerator StartKillTimerCoroutine()
        {
            throw new NotImplementedException();
        }

        private void StartProgressBarTimer()
        {
            throw new NotImplementedException();
            StartCoroutine(StartProgressBarTimerCoroutine());
        }
        private IEnumerator StartProgressBarTimerCoroutine()
        {
            throw new NotImplementedException();
        }

        private bool testisNormalized = false;
        private void BtnSubmitOnClick()
        {
            logger.LogDebug("BtnSubmitOnClick");

            //string time = HUDManager.Instance.clockNumber.text;
            //float normalizedTimeOfDay = TimeOfDay.Instance.normalizedTimeOfDay;

            if (testisNormalized)
            {
                txtTimeOfDeath.value = TimeToClock(float.Parse(txtTimeOfDeath.text), TimeOfDay.Instance.numberOfHours);
                testisNormalized = false;
            }
            else
            {
                txtTimeOfDeath.value = ClockToTime(txtTimeOfDeath.text).ToString();
                testisNormalized = true;
            }

            return;

            /*if (!string.IsNullOrEmpty(time))
            {
                //string myString = "Your string\n with new lines and spaces";
                //myString = myString.Replace(" ", ""); // Removes all spaces
                //myString = myString.Replace("\n", ""); // Removes all newlines


                // time = "00:00:00";
                // timeNormalized = is a float that goes up and is a number between 0 and 1;
                // globalTime is a float that starts at 100 and goes up
                // total time is a constant that shows the total time in a day
                // number of hours is a constant that shows the number of hours in a day
                lblResult.text = time + " || " + normalizedTimeOfDay + " || " + globalTime + " || " + totalTime + " || " + numberofhours;
                //if (time == "00:00:00") { ShowResults("Time's up!"); } // TODO: TEMP FOR TESTING
                
                // normalizedTimeOfDay = currentDayTime / totalTime;

            }*/

            return; // TODO: TEMP FOR TESTING

            if (verifying)
            {
                // TODO: For when you press submit the second time, locking it in and adding it to the second page
                verifying = false;
                return;
            }

            ShowResults("Searching for player to kill...", 5f, true); // TODO: TEMP FOR TESTING DELETE ME
            
            PlayerControllerB playerToDie = StartOfRound.Instance.allPlayerScripts.ToList().Where(x => x.playerUsername.ToLower() == txtPlayerUsername.text.ToLower()).FirstOrDefault();

            if (playerToDie != null) // TODO: TEMP FOR TESTING CHANGE BACK TO !=
            {
                if (playerToDie.isPlayerDead) { ShowResults("Player is already dead"); return; }
                logger.LogDebug($"Found player to kill: {playerToDie.playerUsername}"); // TODO: DISABLED FOR TESTING
                ShowResults($"Found player to kill: {playerToDie.playerUsername}");
                
                txtPlayerUsername.isReadOnly = true;
                dpdnDeathType.style.display = DisplayStyle.Flex;
                dpdnDeathType.index = 0;
                txtTimeOfDeath.style.display = DisplayStyle.Flex;
                txtTimeOfDeath.value = "";
                pbRemainingTime.style.display = DisplayStyle.Flex;
                pbRemainingTime.value = 0;
                
                verifying = true;
                StartProgressBarTimer();
                // TODO: Continue here
            }
            else
            {
                ShowResults("Could not find player to kill");
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

        public string TimeToClock(float timeNormalized, float numberOfHours) // THIS WORKS
        {
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

        public float ClockToTime(string timeString) // doesnt work
        {
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
            float normalizedTime = totalMinutes / totalTime; // Assuming 24-hour day

            return normalizedTime;
        } // TODO: THIS IS THE SOLUTION: // normalizedTimeOfDay = currentDayTime / totalTime;


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
