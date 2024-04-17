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