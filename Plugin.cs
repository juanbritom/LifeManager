using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace AddLife
{
    [BepInPlugin("zuk.digimonno.battlestatsmultiplier", "Add Life", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static ConfigEntry<double> LifeToAdd;
        public static ConfigEntry<double> LifeToRemove;
        public static ConfigEntry<string[]> ControlKeys;
        public override void Load()
        {
            //configfile
            LifeToAdd = Config.Bind("General", "Life Add Value", 1.0, "");
            LifeToRemove = Config.Bind("General", "Life Remove Value", 1.0, "");
            ControlKeys = Config.Bind("Controls", "Select Partner 1, Select partner 2, Add, Remove", new string[] { "left", "right", "up", "down" }, "");
            // Plugin startup logic
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading!");
            if (LifeToAdd.Value < 0 || LifeToRemove.Value < 0)
            {
                Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} plugin found problems within its configuration file, please check it!");
                return;
            }
            Awake();

        }
        public void Awake()
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

        }
        
        public class ButtonPresses : 
        {
        }

        [HarmonyPatch(typeof(), "_AddPartnerLifeTime")]
        public static class ModPatch
        {
        }
    }
} 