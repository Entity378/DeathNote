using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace DeathNote
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class DeathNoteBase : BaseUnityPlugin
    {
        private const string modGUID = "Snowlance.DeathNote";
        private const string modName = "DeathNote";
        private const string modVersion = "0.1.0";

        public static DeathNoteBase PluginInstance { get; private set; } = null!;
        public static ManualLogSource LoggerInstance { get; private set; }
        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConfigEntry<float> configVolume;

        private void Awake()
        {
            if (PluginInstance == null)
            {
                PluginInstance = this;
            }

            LoggerInstance = PluginInstance.Logger;
            LoggerInstance.LogDebug($"Plugin {modName} loaded successfully.");

            configVolume = Config.Bind("Volume", "MusicVolume", 1f, "Volume of the music. Must be between 0 and 1.");

            harmony.PatchAll();

            LoggerInstance.LogInfo($"{modGUID} v{modVersion} has loaded!");
        }
    }
}
