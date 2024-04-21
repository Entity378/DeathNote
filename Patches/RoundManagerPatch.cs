using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;

namespace DeathNote.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        [HarmonyPostfix]
        [HarmonyPatch("SpawnEnemyFromVent")]
        public static void SpawnEnemyFromVentPatch()
        {
            EnemyAI enemy = RoundManager.Instance.SpawnedEnemies.Last<EnemyAI>();

            string enemyName = enemy.enemyType + new Random().Next(1000).ToString() + "-" + enemy.thisEnemyIndex;
            DeathController.EnemyNames.Add(enemyName);
            
            // TODO: Find a way to publicize the game assembly so i can access scannodes in HudManager.Instance.scannodes

            /*if (UIControllerScript.Instance == null) { return true; }
            if (UIControllerScript.Instance.veMain == null) { logger.LogError("veMain is null!"); return true; }
            if (UIControllerScript.Instance.veMain.style.display == DisplayStyle.Flex) { return false; }
            return true;*/
        }
    }
}
