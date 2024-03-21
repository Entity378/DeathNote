using BepInEx;
using DeathNoteMod;
using static DeathNoteMod.DeathNoteBase;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Diagnostics.CodeAnalysis;
using System;

/*namespace QuickRestart.Patches
{
    
    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    public class SubmitChat
    {
        // TODO: make this not ugly
        private static bool Prefix(HUDManager __instance, ref InputAction.CallbackContext context)
        {
            return;
            if (!context.performed)
            {
                return true;
            }
            if (string.IsNullOrEmpty(__instance.chatTextField.text))
            {
                return true;
            }
            PlayerControllerB local = GameNetworkManager.Instance.localPlayerController;
            if (local == null)
            {
                return true;
            }
            StartOfRound manager = local.playersManager;
            if (manager == null)
            {
                return true;
            }
            string text = __instance.chatTextField.text;
            if (DeathNoteBase.PluginInstance.verifying)
            {
                if (text == "CONFIRM")
                {
                    ResetTextbox(__instance, local);
                    if (!local.isInHangarShipRoom || !manager.inShipPhase || manager.travellingToNewLevel)
                    {
                        PluginInstance.SendChatMessage("Cannot restart, ship must be in orbit.");
                        return false;
                    }
                    PluginInstance.AcceptRestart(manager);
                    return false;
                }
                if (text == "DENY")
                {
                    ResetTextbox(__instance, local);
                    PluginInstance.DeclineRestart();
                    return false;
                }
                return true;
            }
            if (text == "/restart")
            {
                ResetTextbox(__instance, local);
                if (!GameNetworkManager.Instance.isHostingGame)
                {
                    PluginInstance.SendChatMessage("Only the host can restart.");
                    return false;
                }
                if (!local.isInHangarShipRoom || !manager.inShipPhase || manager.travellingToNewLevel)
                {
                    PluginInstance.SendChatMessage("Cannot restart, ship must be in orbit.");
                    return false;
                }
                if (PluginInstance.bypassConfirm)
                {
                    PluginInstance.AcceptRestart(manager);
                }
                else
                {
                    PluginInstance.ConfirmRestart();
                }
                return false;
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
}*/