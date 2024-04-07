using BepInEx.Logging;
using DeathNoteMod;
using GameNetcodeStuff;
using LethalNetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DeathNote
{
    internal class DeathNoteBehavior : PhysicsProp
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            //DeathController.SpawnEnemyWithDeathType();
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                logger.LogDebug("Using item works!");

                UIControllerScript uiController = base.GetComponent<UIControllerScript>();
                if (uiController == null) { logger.LogError("UIControllerScript does not exist!"); }

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