using BepInEx.Logging;
using DeathNoteMod;
using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;

namespace DeathNote
{

    [HarmonyPatch(typeof(HUDManager))]
    public class ChatController
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;
        private static bool verifyingCOS = false;

        // TODO: make this not ugly
        [HarmonyPatch("SubmitChat_performed")]
        [HarmonyPrefix]
        private static bool Prefix(HUDManager __instance)
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            string text = __instance.chatTextField.text.ToLower();
            GrabbableObject heldObject = localPlayer.currentlyHeldObjectServer;

            if (heldObject == null) { return true; }

            if (heldObject.itemProperties.itemName == "Death Note" && text.Contains("/deathnote "))
            {
                string[] parts = text.Split(" ", 2);
                if (parts.Length >= 2)
                {
                    string name = parts[1];
                    logger.LogDebug($"Getting player: '{name}'");
                    PlayerControllerB playerToDie = StartOfRound.Instance.allPlayerScripts.ToList().Where(x => x.playerUsername.ToLower() == name).FirstOrDefault();

                    if (playerToDie != null)
                    {
                        logger.LogDebug($"Found player to kill: {playerToDie.playerUsername}"); // IT WORKS UP TO HERE!
                        DeathController.PlayerToDie = playerToDie;

                        ResetTextbox(__instance, localPlayer);
                        return false;
                    }
                    else
                    {
                        logger.LogDebug("Player not found...");
                        SendChatMessage("A name was unspecified or doesnt exist. (Use /deathnote playerusername)");
                    }
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public static void SendChatMessage(string message)
        {
            MethodInfo chat = AccessTools.Method(typeof(HUDManager), "AddChatMessage");
            chat?.Invoke(HUDManager.Instance, new object[] { message, "" });
        }

        private static void ResetTextbox(HUDManager manager, PlayerControllerB local)
        {
            local.isTypingChat = false;
            manager.chatTextField.text = "";
            EventSystem.current.SetSelectedGameObject(null);
            manager.PingHUDElement(manager.Chat, 2f, 1f, 0.2f);
            manager.typingIndicator.enabled = false;
        }
    }
}