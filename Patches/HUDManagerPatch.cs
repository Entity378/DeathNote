using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DeathNote.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        [HarmonyPostfix]
        [HarmonyPatch("PingScan_performed")]
        public static void PingScan_performedPatch()
        {
            logger.LogDebug("In PingScan_performedPatch");
            if (DeathController.ShinigamiEyesActivated)
            {
                logger.LogDebug("In PingScan_performedPatch: ShinigamiEyesActivated");
                Dictionary<RectTransform, ScanNodeProperties> scan_nodes = HUDManager.Instance.scanNodes;

                foreach (var scan_node in scan_nodes)
                {
                    EnemyAI enemy = scan_node.Value.gameObject.GetComponentInParent<EnemyAI>();
                    logger.LogDebug($"Got enemy from scannode:{enemy.thisEnemyIndex}: {enemy.enemyType.enemyName}");
                    string name = DeathController.EnemyNames.Where(x => int.Parse(x.Substring(x.Length - 1)) == enemy.thisEnemyIndex).First();
                    

                    logger.LogDebug("In PingScan_performedPatch: name: " + name);

                    if (name != null)
                    {
                        scan_node.Value.headerText = name;
                        scan_node.Value.subText = enemy.enemyType.enemyName;
                    }
                }
            }
        }
    }
}
