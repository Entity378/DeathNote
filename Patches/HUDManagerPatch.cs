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
            // Private List<ScanNodeProperties> nodesOnScreen = new List<ScanNodeProperties>();
            if (DeathController.ShinigamiEyesActivated)
            {
                List<ScanNodeProperties> nodes = HUDManager.Instance.nodesOnScreen;
                logger.LogDebug($"Got {nodes.Count} nodes");

                foreach (var node in nodes) // TODO: Continue testing here
                {
                    logger.LogDebug($"{node.nodeType} {node.headerText} {node.subText}");

                    /*EnemyAI enemy = node.Value.gameObject.GetComponentInParent<EnemyAI>();
                    logger.LogDebug($"Got enemy from scannode:{enemy.thisEnemyIndex}: {enemy.enemyType.enemyName}");
                    string name = DeathController.EnemyNames.Where(x => int.Parse(x.Substring(x.Length - 1)) == enemy.thisEnemyIndex).First();
                    
                    logger.LogDebug("passed name check");
                    logger.LogDebug("In PingScan_performedPatch: name: " + name);

                    if (name != null)
                    {
                        //scan_node.Value.headerText = name;
                        //scan_node.Value.subText = enemy.enemyType.enemyName;
                        //logger.LogDebug(scan_node.Value.headerText + " " + scan_node.Value.subText);
                    }*/
                }

                HUDManager.Instance.UpdateScanNodes(StartOfRound.Instance.localPlayerController);
            }
        }
    }
}
