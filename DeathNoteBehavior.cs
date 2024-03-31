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

                PlayerControllerB player = DeathNoteBase.PlayerToDie;
                if (player != null)
                {
                    if (player.isPlayerDead) { DeathNoteBase.SendChatMessage("This player is already dead..."); }
                    NetworkHandler.clientMessage.SendServer(player.actualClientId);
                    DeathNoteBase.SendChatMessage($"{player.playerUsername} has died of a heart attack");
                }
                else
                {
                    DeathNoteBase.SendChatMessage("A name was unspecified or doesnt exist. (Use /deathnote playerusername)");
                }
            }
        }
    }
}