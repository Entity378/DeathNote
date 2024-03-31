using BepInEx.Logging;
using DeathNoteMod;
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeathNote
{
    public static class DeathController
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        static PlayerControllerB playerToDie;
        static string causeOfDeathString;


        public static List<string> GetCauseOfDeathsAsStrings()
        {
            List<string> deathType = new List<string>();

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

        public static void KillPlayerWithCauseOfDeath()
        {
            CauseOfDeath causeOfDeath;

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
            }
        }
    }
}
