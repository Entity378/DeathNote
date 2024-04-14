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
    public static class DeathController
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

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

        public static CauseOfDeath GetCauseOfDeath(string causeOfDeathString) // TODO: might need to add Unknown to list as default value
        {
            CauseOfDeath causeOfDeath = CauseOfDeath.Unknown;
            
            switch (causeOfDeathString.ToLower())
            {
                case "abandoned":
                    causeOfDeath = CauseOfDeath.Abandoned;
                    break;
                case "blast":
                    causeOfDeath = CauseOfDeath.Blast;
                    break;
                case "bludgeoning":
                    causeOfDeath = CauseOfDeath.Bludgeoning;
                    break;
                case "crushing":
                    causeOfDeath = CauseOfDeath.Crushing;
                    break;
                case "drowniing":
                    causeOfDeath = CauseOfDeath.Drowning;
                    break;
                case "electrocution":
                    causeOfDeath = CauseOfDeath.Electrocution;
                    break;
                case "gravity":
                    causeOfDeath = CauseOfDeath.Gravity;
                    break;
                case "gunshots":
                    causeOfDeath = CauseOfDeath.Gunshots;
                    break;
                case "kicking":
                    causeOfDeath = CauseOfDeath.Kicking;
                    break;
                case "mauling":
                    causeOfDeath = CauseOfDeath.Mauling;
                    break;
                case "strangulation":
                    causeOfDeath = CauseOfDeath.Strangulation;
                    break;
                case "suffocation":
                    causeOfDeath = CauseOfDeath.Suffocation;
                    break;
                case "unknown":
                    causeOfDeath = CauseOfDeath.Unknown;
                    break;
            }

            logger.LogDebug($"Got cause of death: {causeOfDeath}");
            return causeOfDeath;
        }

        public static void KillPlayer(PlayerControllerB playerToDie, CauseOfDeath causeOfDeath)
        {
            playerToDie.causeOfDeath = causeOfDeath;
            NetworkHandler.clientMessage.SendServer(playerToDie.actualClientId);
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
