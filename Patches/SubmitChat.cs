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
using System.Linq;
using System.Reflection;

namespace QuickRestart.Patches
{
    
    [HarmonyPatch(typeof(HUDManager))]
    public class SubmitChat
    {
        // TODO: make this not ugly
        [HarmonyPatch("SubmitChat_performed")]
        [HarmonyPrefix]
        private static bool Prefix(HUDManager __instance)
        {

            /*if (!context.performed)
            {
                return true;
            }
            if (string.IsNullOrEmpty(__instance.chatTextField.text))
            {
                return true;
            }
            
            if (local == null)
            {
                return true;
            }
            StartOfRound manager = local.playersManager;
            if (manager == null)
            {
                return true;
            }*/
            
            LoggerInstance.LogDebug("start");
            PlayerControllerB local = GameNetworkManager.Instance.localPlayerController;
            LoggerInstance.LogDebug($"Got player: {local.playerUsername}");
            LoggerInstance.LogDebug("pass 1");
            string text = __instance.chatTextField.text;
            LoggerInstance.LogDebug($"ChatSays '{text}'");
            LoggerInstance.LogDebug("pass 2");
            text = text.ToLower();
            LoggerInstance.LogDebug("pass 3");
            GrabbableObject heldObject = local.currentlyHeldObjectServer;
            if (heldObject == null) { return true; }

            LoggerInstance.LogDebug("pass 3.5");
            if (heldObject.itemProperties.itemName == "Death Note" && text.Contains("/deathnote "))
            {
                LoggerInstance.LogDebug("pass 4");
                string[] parts = text.Split(" ", 2);
                if (parts.Length >= 2)
                {
                    LoggerInstance.LogDebug("pass 5");
                    string name = parts[1];
                    LoggerInstance.LogDebug($"PlayertoDie: '{name}'");

                    PlayerControllerB playerToDie = StartOfRound.Instance.allPlayerScripts.ToList().Where(x => x.playerUsername.ToLower() == name).FirstOrDefault();
                    if (playerToDie != null)
                    {
                        LoggerInstance.LogDebug("found player to kill!!!!!!"); // IT WORKS UP TO HERE!
                    }
                    else
                    {
                        LoggerInstance.LogDebug("Player not found...");
                        SendChatMessage("Username is incorrect...");
                    }
                }
                else
                {
                    return true;
                }
            }

            /*if (DeathNoteBase.PluginInstance.verifying)
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
            }*/
            return true;
        }

        private static void SendChatMessage(string message)
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