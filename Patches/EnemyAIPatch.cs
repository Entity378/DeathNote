using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

namespace DeathNote.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatch
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void StartPostFix(EnemyAI __instance)
        {
            string enemyName = __instance.enemyType.enemyName + new Random().Next(100, 1000).ToString() + "-" + __instance.thisEnemyIndex;
            DeathController.EnemyNames.Add(enemyName);
        }
    }
}
