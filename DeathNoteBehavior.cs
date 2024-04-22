using BepInEx.Logging;
using DeathNote;
using GameNetcodeStuff;
using LethalNetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace DeathNote
{
    internal class DeathNoteBehavior : PhysicsProp
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                logger.LogDebug("Using item works!");

                /*List<EnemyAI> enemies = RoundManager.Instance.SpawnedEnemies;
                logger.LogDebug($"Got {enemies.Count} enemies");
                foreach (EnemyAI enemy in enemies)
                {
                    ScanNodeProperties enemyNode = enemy.gameObject.GetComponentInChildren<ScanNodeProperties>(); // TODO: Maybe i can get the reverse and grab the enemyai object from the scan node. then just patch scannode_performed to show the name in real time with a cool translate effect
                    logger.LogDebug($"{enemy.thisEnemyIndex}: {enemy.enemyType}, {enemyNode.creatureScanID}, {enemyNode.nodeType}, {enemyNode.headerText}, {enemyNode.subText}");
                }// TODO: need to access the name that pops up when scanning the enemy
                
                

                logger.LogDebug("\nScanned nodes:");
                Dictionary<RectTransform, ScanNodeProperties> scannodes = HUDManager.Instance.scanNodes;

                foreach (var scanNode in scannodes)
                {
                    EnemyAI enemy = scanNode.Value.gameObject.GetComponentInParent<EnemyAI>(); // THIS WORKS
                    logger.LogDebug($"{scanNode.Value.creatureScanID}, {scanNode.Value.nodeType}, {scanNode.Value.headerText}, {scanNode.Value.subText} || {enemy.thisEnemyIndex}, {enemy.enemyType}");
                }
                return;*/
                // Testing end
                
                UIControllerScript uiController = GetComponent<UIControllerScript>();
                logger.LogDebug("Got veMain");
                logger.LogMessage(uiController.veMain.style.display.ToString());
                if (uiController.veMain.style.display == null)
                {
                    logger.LogDebug("veMain.style.display is null");
                    return;
                }
                
                if (uiController.veMain.style.display == DisplayStyle.None)
                {
                    logger.LogDebug("Showing UI");
                    uiController.ShowUI();
                }
            }
        }
    }
}