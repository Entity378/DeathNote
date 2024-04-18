using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using DeathNote;
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

        public static LethalServerMessage<string[]> serverMessage = new LethalServerMessage<string[]>("message");
        public static LethalClientMessage<string[]> clientMessage = new LethalClientMessage<string[]>("message");

        public static void Init()
        {
            clientMessage.OnReceived += RecieveFromServer;
            serverMessage.OnReceived += RecieveFromClient;
        }

        private static void RecieveFromServer(string[] info)
        {
            logger.LogDebug("In RecieveFromServer");
            PlayerControllerB playerToDie = GameNetworkManager.Instance.localPlayerController;
            playerToDie.KillPlayer(new Vector3()); // TODO: This makes player go into the floor???
            playerToDie.causeOfDeath = DeathController.GetCauseOfDeathFromString(info[1]);
        }

        private static void RecieveFromClient(string[] info, ulong clientID)
        {
            logger.LogDebug($"In RecieveFromClient");
            serverMessage.SendClient(info, ulong.Parse(info[0]));
        }
    }
}
