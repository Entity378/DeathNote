using BepInEx;
using BepInEx.Logging;
using DeathNote;
using GameNetcodeStuff;
using LethalLib.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DeathNote
{
    public class DeathController
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public static bool ShinigamiEyesActivated = false;

        public PlayerControllerB PlayerToDie;
        public CauseOfDeath causeOfDeath;

        public string TimeOfDeathString;
        public float TimeOfDeath;

        public static List<string> GetCauseOfDeathsAsStrings()
        {
            List<string> deathType = new List<string>();

            deathType.Add(CauseOfDeath.Unknown.ToString());
            deathType.Add(CauseOfDeath.Abandoned.ToString());
            deathType.Add(CauseOfDeath.Blast.ToString());
            deathType.Add(CauseOfDeath.Bludgeoning.ToString());
            deathType.Add(CauseOfDeath.Crushing.ToString());
            deathType.Add(CauseOfDeath.Drowning.ToString());
            deathType.Add(CauseOfDeath.Electrocution.ToString());
            deathType.Add(CauseOfDeath.Gravity.ToString());
            deathType.Add(CauseOfDeath.Gunshots.ToString());
            deathType.Add(CauseOfDeath.Kicking.ToString());
            deathType.Add(CauseOfDeath.Mauling.ToString());
            deathType.Add(CauseOfDeath.Strangulation.ToString());
            deathType.Add(CauseOfDeath.Suffocation.ToString());

            return deathType;
        }

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
                case "crushing":
                    _causeOfDeath = CauseOfDeath.Crushing;
                    break;
                case "drowniing":
                    _causeOfDeath = CauseOfDeath.Drowning;
                    break;
                case "electrocution":
                    _causeOfDeath = CauseOfDeath.Electrocution;
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

        public void KillPlayer()
        {
            PlayerToDie.causeOfDeath = causeOfDeath;
            NetworkHandler.clientMessage.SendServer(PlayerToDie.actualClientId);
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
