using BepInEx;
using BepInEx.Logging;
using DeathNote;
using GameNetcodeStuff;
using LethalLib.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
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
//using TMPro;

namespace DeathNote
{
    public class DeathController : MonoBehaviour
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public static bool ShinigamiEyesActivated = false;

        public UIControllerScript ui;

        public PlayerControllerB PlayerToDie;
        public string causeOfDeathString;
        public string detailsString;

        public string TimeOfDeathString;
        public float TimeOfDeath;

        public static List<string> Details = new List<string> { "Heart Attack", "Decapitation", "Coil Decapitation", "Seizure", "Disappearance", "Mask", "Burn" }; // disapearance is 4


        public static List<string> GetCauseOfDeathsAsStrings()
        {
            List<string> deathType = new List<string>();

            deathType.Add(CauseOfDeath.Unknown.ToString());
            deathType.Add(CauseOfDeath.Abandoned.ToString());
            deathType.Add(CauseOfDeath.Blast.ToString());
            deathType.Add(CauseOfDeath.Bludgeoning.ToString());
            deathType.Add(CauseOfDeath.Burning.ToString());
            deathType.Add(CauseOfDeath.Crushing.ToString());
            deathType.Add(CauseOfDeath.Drowning.ToString());
            deathType.Add(CauseOfDeath.Electrocution.ToString());
            deathType.Add(CauseOfDeath.Fan.ToString());
            deathType.Add(CauseOfDeath.Gravity.ToString());
            deathType.Add(CauseOfDeath.Gunshots.ToString());
            deathType.Add(CauseOfDeath.Kicking.ToString());
            deathType.Add(CauseOfDeath.Mauling.ToString());
            deathType.Add(CauseOfDeath.Stabbing.ToString());
            deathType.Add(CauseOfDeath.Strangulation.ToString());
            deathType.Add(CauseOfDeath.Suffocation.ToString());

            return deathType;
        }

        // deathAnimations:
        // 0 = normal
        // 1 = decapitation
        // 2 = coilhead decapitation

        public static CauseOfDeath GetCauseOfDeathFromString(string causeOfDeathString) // TODO: might need to add Unknown to list as default value
        {
            CauseOfDeath _causeOfDeath = CauseOfDeath.Unknown;
            
            switch (causeOfDeathString.ToLower())
            {
                case "abandoned":
                    _causeOfDeath = CauseOfDeath.Abandoned;
                    break;
                case "blast":
                    _causeOfDeath = CauseOfDeath.Blast;
                    break;
                case "bludgeoning":
                    _causeOfDeath = CauseOfDeath.Bludgeoning;
                    break;
                case "burning":
                    _causeOfDeath = CauseOfDeath.Burning;
                    break;
                case "crushing":
                    _causeOfDeath = CauseOfDeath.Crushing;
                    break;
                case "drowniing":
                    _causeOfDeath = CauseOfDeath.Drowning;
                    break;
                case "electrocution":
                    _causeOfDeath = CauseOfDeath.Electrocution;
                    break;
                case "fan":
                    _causeOfDeath = CauseOfDeath.Fan;
                    break;
                case "gravity":
                    _causeOfDeath = CauseOfDeath.Gravity;
                    break;
                case "gunshots":
                    _causeOfDeath = CauseOfDeath.Gunshots;
                    break;
                case "kicking":
                    _causeOfDeath = CauseOfDeath.Kicking;
                    break;
                case "mauling":
                    _causeOfDeath = CauseOfDeath.Mauling;
                    break;
                case "stabbing":
                    _causeOfDeath = CauseOfDeath.Stabbing;
                    break;
                case "strangulation":
                    _causeOfDeath = CauseOfDeath.Strangulation;
                    break;
                case "suffocation":
                    _causeOfDeath = CauseOfDeath.Suffocation;
                    break;
                case "unknown":
                    _causeOfDeath = CauseOfDeath.Unknown;
                    break;
            }

            logger.LogDebug($"Got cause of death: {_causeOfDeath}");
            return _causeOfDeath;
        }

        /*public static string GetDetailsAsIndex(string _details)
        {
            int index = GetCauseOfDeathsAsStrings().IndexOf(_details);
        }*/

        public IEnumerator StartKillTimerCoroutine()
        {
            logger.LogDebug("In StartKillTimerCoroutine");
            yield return new WaitForSeconds(1f);
            Label lblPlayerToDie = new Label();
            //lblPlayerToDie.style.unityFont = DeathNoteBase.DNAssetBundle.LoadAsset<Font>("Assets/DeathNote/Death Note.ttf");
            lblPlayerToDie.text = $"{PlayerToDie.playerUsername}: {causeOfDeathString}, {ui.TimeToClock(TimeOfDeath)}";
            lblPlayerToDie.style.color = Color.red;

            ProgressBar pbTimeToDie = new ProgressBar();
            pbTimeToDie.name = "pbTimeToDie";
            pbTimeToDie.lowValue = 0;
            pbTimeToDie.highValue = TimeOfDeath - TimeOfDay.Instance.currentDayTime;
            pbTimeToDie.style.display = DisplayStyle.Flex;
            pbTimeToDie.title = "Remaining Time";

            ui.svRight.Add(lblPlayerToDie);
            ui.svRight.Add(pbTimeToDie);

            if (pbTimeToDie == null) { logger.LogError("pbTimeToDie is null"); }

            float elapsedTime = 0f;

            while (pbTimeToDie.value < pbTimeToDie.highValue)
            {
                elapsedTime += Time.deltaTime * TimeOfDay.Instance.globalTimeSpeedMultiplier;
                pbTimeToDie.value = Mathf.Lerp(pbTimeToDie.lowValue, pbTimeToDie.highValue, elapsedTime / pbTimeToDie.highValue);
                yield return null;
            }

            KillPlayer();
            ui.svRight.Remove(lblPlayerToDie);
            ui.svRight.Remove(pbTimeToDie);
        }

        public void KillPlayer()
        {
            logger.LogDebug($"Killing player {PlayerToDie.playerUsername}: {causeOfDeathString}, {TimeOfDeathString}");

            // TODO: continue here with death animations

            string[] info = { PlayerToDie.actualClientId.ToString(), causeOfDeathString, detailsString };
            NetworkHandler.clientMessage.SendServer(info);
        }

        public static void GetEnemy()
        {
            // TODO: implement, might not be needed
            //public List<EnemyAI> SpawnedEnemies = new List<EnemyAI>();
        }

        public static List<SpawnableEnemyWithRarity> GetEnemies()
        {
            logger.LogDebug("Getting enemies");
            List<SpawnableEnemyWithRarity> enemies = new List<SpawnableEnemyWithRarity>();
            enemies = GameObject.Find("Terminal")
                .GetComponentInChildren<Terminal>()
                .moonsCatalogueList
                .SelectMany(x => x.Enemies.Concat(x.DaytimeEnemies).Concat(x.OutsideEnemies))
                .Where(x => x != null && x.enemyType != null && x.enemyType.name != null)
                .GroupBy(x => x.enemyType.name, (k, v) => v.First())
                .ToList();

            logger.LogDebug($"Enemy types: {enemies.Count}");
            return enemies;
        }
    }
}
