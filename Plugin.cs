using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;

namespace LifeManager
{
    [BepInPlugin("zuk.digimonno.LifeManager", "Life Manager", "1.0.0-beta")]
    public class Plugin : BasePlugin
    {
        public static ConfigEntry<double> LifeToAdd;
        public static ConfigEntry<double> LifeToRemove;
        public static ConfigEntry<KeyCode> ControlKeysLeft;
        public static ConfigEntry<KeyCode> ControlKeysRight;
        public static ConfigEntry<KeyCode> ControlKeysUp;
        public static ConfigEntry<KeyCode> ControlKeysDown;
        public static KeyCode[] keys;
        public override void Load()
        {
            //configfile
            LifeToAdd = Config.Bind("General", "Life Add Value", 10.0, "");
            LifeToRemove = Config.Bind("General", "Life Remove Value", 10.0, "");
            ControlKeysLeft = Config.Bind("Controls", "Select Partner 1", KeyCode.LeftArrow, "");
            ControlKeysRight = Config.Bind("Controls", "Select Partner 2", KeyCode.RightArrow, "");
            ControlKeysUp = Config.Bind("Controls", "Add Life", KeyCode.UpArrow, "");
            ControlKeysDown = Config.Bind("Controls", "Remove Life", KeyCode.DownArrow, "");
            keys = new KeyCode[]{ ControlKeysLeft.Value, ControlKeysRight.Value, ControlKeysUp.Value, ControlKeysDown.Value };
            // Plugin startup logic

            if (LifeToAdd.Value < 0 || LifeToRemove.Value < 0)
            {
                Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} plugin found problems within its configuration file, please check it!");
                return;
            }
            Awake();
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded!");
        }

        [HarmonyPatch(typeof(MainGameManager), "Update")]
        public static class upMod
        {
            //fazer segurando botao do partner e apertando cima/baixo
            [HarmonyPrefix]
            public static bool Update_Patcher()
            {             
                if (Input.GetKey(keys[0]))
                {
                    if (Input.GetKey(keys[2]))
                    {
                        new CScenarioScript()._AddPartnerLifeTime(AppInfo.PARTNER_NO.Left, (float)LifeToAdd.Value);
                    }
                    if (Input.GetKey(keys[3]))
                    {
                        new CScenarioScript()._AddPartnerLifeTime(AppInfo.PARTNER_NO.Left, -(float)LifeToRemove.Value);
                    }
                }
                if (Input.GetKey(keys[1]))
                {
                    if (Input.GetKey(keys[2]))
                    {
                        new CScenarioScript()._AddPartnerLifeTime(AppInfo.PARTNER_NO.Right, (float)LifeToAdd.Value);
                    }
                    if (Input.GetKey(keys[3]))
                    {
                        new CScenarioScript()._AddPartnerLifeTime(AppInfo.PARTNER_NO.Right, -(float)LifeToRemove.Value);
                    }
                }

                return true;
            }
        }

        public void Awake()
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

        }
    }
} 