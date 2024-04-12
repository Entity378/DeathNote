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

                UIControllerScript uiController = GetComponent<UIControllerScript>();
                VisualElement veMain = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("veMain");
                logger.LogDebug("Got veMain");
                logger.LogMessage(veMain.style.display.ToString());
                
                if (veMain.style.display == DisplayStyle.None)
                {
                    logger.LogDebug("Showing UI");
                    uiController.ShowUI();
                }
                
                //List<SpawnableEnemyWithRarity> enemies = DeathController.GetEnemies();

                /*PlayerControllerB player = DeathController.PlayerToDie;
                if (player != null)
                {
                    if (player.isPlayerDead) { ChatController.SendChatMessage("This player is already dead..."); }
                    NetworkHandler.clientMessage.SendServer(player.actualClientId);
                    ChatController.SendChatMessage($"{player.playerUsername} has died of a heart attack");
                }
                else
                {
                    ChatController.SendChatMessage("A name was unspecified or doesnt exist. (Use /deathnote playerusername)");
                }*/
            }
        }
    }
}