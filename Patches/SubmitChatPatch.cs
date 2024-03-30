using DeathNoteMod;
using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;
using UnityEngine.EventSystems;
using static DeathNoteMod.DeathNoteBase;

namespace QuickRestart.Patches
{

    [HarmonyPatch(typeof(HUDManager))]
    public class SubmitChatPatch
    {
        // TODO: make this not ugly
        [HarmonyPatch("SubmitChat_performed")]
        [HarmonyPrefix]
        private static bool Prefix(HUDManager __instance)
        {
            PlayerControllerB local = GameNetworkManager.Instance.localPlayerController;

            string text = __instance.chatTextField.text.ToLower();
            GrabbableObject heldObject = local.currentlyHeldObjectServer;

            if (heldObject == null) { return true; }

            if (heldObject.itemProperties.itemName == "Death Note" && text.Contains("/deathnote "))
            {
                string[] parts = text.Split(" ", 2);
                if (parts.Length >= 2)
                {
                    string name = parts[1];
                    LoggerInstance.LogDebug($"Getting player: '{name}'");
                    PlayerControllerB playerToDie = StartOfRound.Instance.allPlayerScripts.ToList().Where(x => x.playerUsername.ToLower() == name).FirstOrDefault();
                    
                    if (playerToDie != null)
                    {
                        LoggerInstance.LogDebug($"Found player to kill: {playerToDie.playerUsername}"); // IT WORKS UP TO HERE!
                        PlayerToDie = playerToDie;

                        ResetTextbox(__instance, local);
                        return false;
                    }
                    else
                    {
                        LoggerInstance.LogDebug("Player not found...");
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