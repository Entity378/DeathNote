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
            logger.LogDebug($"enemy names: {DeathController.EnemyNames.ToString()}");

            EnemyAI enemy = RoundManager.Instance.SpawnedEnemies.Last<EnemyAI>();
            logger.LogDebug($"enemy: {enemy.enemyType.enemyName}");

            string enemyName = enemy.enemyType + new Random().Next(100, 1000).ToString() + "-" + enemy.thisEnemyIndex;
            logger.LogDebug($"enemyName: {enemyName}");
            DeathController.EnemyNames.Add(enemyName);

            /* // TODO: Debug log from test, getting 0 index for each enemy, may need to patch a different method or wait
             [Debug  : DeathNote] enemy names: System.Collections.Generic.List`1[System.String]
            [Debug  : DeathNote] enemy: Crawler
            [Debug  : DeathNote] enemyName: Crawler (EnemyType)743-0
            [Debug  : DeathNote] In PingScan_performedPatch
            [Debug  : DeathNote] In PingScan_performedPatch: ShinigamiEyesActivated
            [Debug  : DeathNote] In PingScan_performedPatch
            [Debug  : DeathNote] In PingScan_performedPatch: ShinigamiEyesActivated
            [Debug  : DeathNote] In PingScan_performedPatch
            [Debug  : DeathNote] In PingScan_performedPatch: ShinigamiEyesActivated
            [Debug  : DeathNote] Got enemy from scannode:28: Puffer
            [Debug  : DeathNote] enemy names: System.Collections.Generic.List`1[System.String]
            [Debug  : DeathNote] enemy: Crawler
            [Debug  : DeathNote] enemyName: Crawler (EnemyType)884-0
            [Debug  : DeathNote] enemy names: System.Collections.Generic.List`1[System.String]
            [Debug  : DeathNote] enemy: Hoarding bug
            [Debug  : DeathNote] enemyName: HoarderBug (EnemyType)632-0
             */

            // TODO: Find a way to publicize the game assembly so i can access scannodes in HudManager.Instance.scannodes

            /*if (UIControllerScript.Instance == null) { return true; }
            if (UIControllerScript.Instance.veMain == null) { logger.LogError("veMain is null!"); return true; }
            if (UIControllerScript.Instance.veMain.style.display == DisplayStyle.Flex) { return false; }
            return true;*/
        }
    }
}
