using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using DeathNoteMod;
using GameNetcodeStuff;
using LethalNetworkAPI;
using UnityEngine;

namespace DeathNote
{
    internal static class NetworkHandler
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        public static PlayerControllerB CurrentClient
        {
            get
            {
                return StartOfRound.Instance.localPlayerController;
            }
        }

        public static LethalServerEvent serverEvent = new LethalServerEvent(identifier: "event");
        public static LethalClientEvent clientEvent = new LethalClientEvent(identifier: "event");

        public static LethalServerMessage<ulong> serverMessage = new LethalServerMessage<ulong>("message");
        public static LethalClientMessage<ulong> clientMessage = new LethalClientMessage<ulong>("message");

        public static void Init()
        {
            clientEvent.OnReceived += RecieveFromServer;
            serverMessage.OnReceived += RecieveFromClient;
        }

        private static void RecieveFromServer()
        {
            logger.LogDebug("In RecieveFromServer");
            PlayerControllerB playerToDie = GameNetworkManager.Instance.localPlayerController;
            playerToDie.KillPlayer(new Vector3()); // ???
            ChatController.SendChatMessage("You have died from a sudden heart attack");
        }

        private static void RecieveFromClient(ulong playerToDieId, ulong clientID)
        {
            logger.LogDebug($"In RecieveFromClient: {playerToDieId}");
            serverEvent.InvokeClient(playerToDieId);
        }
    }
}
