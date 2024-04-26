using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.UIElements;
using static DeathNote.DeathNoteBase;

namespace DeathNote.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        private static ManualLogSource logger = DeathNoteBase.LoggerInstance;

        [HarmonyPostfix]
        [HarmonyPatch("EndOfGame")]
        private static void EndOfGamePrefix()
        {
            try
            {
                if (configShinigamiEyes.Value && !configPermanentEyes.Value) // get configs from tester and recreate error and fix it
                {
                    logger.LogDebug("In EndOfGamePrefix");

                    DeathController.ShinigamiEyesActivated = false;
                    UIControllerScript.Instance.btnActivateEyes.style.display = DisplayStyle.Flex;
                    UIControllerScript.Instance.lblSEDescription.text = "You may, in exchange of half of your life, acquire the power of the Shinigami Eyes, which will enable you to see an entity's name when looking at them.\nThis will reset at the end of the round.";
                    UIControllerScript.Instance.lblSEDescription.style.color = UnityEngine.Color.black;
                    DeathController.EnemyNames.Clear();
                }
            }
            catch (System.Exception)
            {
                logger.LogError("Error in EndOfGamePrefix");
                return;
            }

            /*if (configShinigamiEyes.Value && !configPermanentEyes.Value) // get configs from tester and recreate error and fix it
            {
                logger.LogDebug("In EndOfGamePrefix");

                DeathController.ShinigamiEyesActivated = false;
                UIControllerScript.Instance.btnActivateEyes.style.display = DisplayStyle.Flex;
                UIControllerScript.Instance.lblSEDescription.text = "You may, in exchange of half of your life, acquire the power of the Shinigami Eyes, which will enable you to see an entity's name when looking at them.\nThis will reset at the end of the round.";
                UIControllerScript.Instance.lblSEDescription.style.color = UnityEngine.Color.black;
                DeathController.EnemyNames.Clear();
            }*/
        }
    }
}
