using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace DeathNote.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("EndOfGame")]
        private static void RefreshEnemiesListPrefix() // TODO: Make sure button highlight or display is changed back to normal when this is called
        {
            DeathController.ShinigamiEyesActivated = false;
            DeathController.EnemyNames.Clear();
        }
    }
}
