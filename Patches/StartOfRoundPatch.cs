using HarmonyLib;
using UnityEngine.UIElements;

namespace DeathNote.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("EndOfGame")]
        private static void EndOfGamePrefix()
        {
            DeathController.ShinigamiEyesActivated = false;
            UIControllerScript.Instance.btnActivateEyes.style.display = DisplayStyle.Flex;
            UIControllerScript.Instance.lblSEDescription.text = "You may, in exchange of half of your life, acquire the power of the Shinigami Eyes, which will enable you to see an entity's name when looking at them.";
            UIControllerScript.Instance.lblSEDescription.style.color = UnityEngine.Color.black;
            DeathController.EnemyNames.Clear();
        }
    }
}
