using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Diagnostics.Metrics;
using UnityEngine;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;

namespace LifeManager
{
    [BepInPlugin("zuk.digimonno.LifeManager", "Life Manager", "1.1.1-beta")]
    public class Plugin : BasePlugin
    {
        public static ConfigEntry<double> LifeToAdd;
        public static ConfigEntry<double> LifeToRemove;
        public static ConfigEntry<bool> AddAgeOnLifeRemove;
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
            AddAgeOnLifeRemove = Config.Bind("General", "Add to Age when removing Life?", true, "Useful to trigger digievolution");
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

        [HarmonyPatch(typeof(CScenarioScript), "Update")]
        public class upMod
        {
            //fazer segurando botao do partner e apertando cima/baixo
            [HarmonyPostfix]
            public static void Update_Patcher(CScenarioScript __instance)
            {
                if (Input.GetKey(keys[0])) //left
                {
                    if (Input.GetKey(keys[2])) //up
                    {
                        __instance._AddPartnerLifeTime(AppInfo.PARTNER_NO.Left, (float)LifeToAdd.Value);
                    }
                    if (Input.GetKey(keys[3])) //down
                    {
                        if (AddAgeOnLifeRemove.Value) { 
                        __instance._GetPartnerDigimonData(AppInfo.PARTNER_NO.Left).m_time_from_age += (float)LifeToRemove.Value;
                        __instance._AddPartnerLifeTime(AppInfo.PARTNER_NO.Left, -(float)LifeToRemove.Value);
                        __instance._GetPartnerCtrl(AppInfo.PARTNER_NO.Left)._UpdateAge();
                        }
                        else
                        {
                            __instance._AddPartnerLifeTime(AppInfo.PARTNER_NO.Left, -(float)LifeToRemove.Value);
                        }
                    }
                }
                if (Input.GetKey(keys[1])) //right
                {
                    if (Input.GetKey(keys[2])) //up
                    {
                        __instance._AddPartnerLifeTime(AppInfo.PARTNER_NO.Right, (float)LifeToAdd.Value);
                    }
                    if (Input.GetKey(keys[3])) //down
                    {
                        if (AddAgeOnLifeRemove.Value)
                        {
                            __instance._GetPartnerDigimonData(AppInfo.PARTNER_NO.Right).m_time_from_age+=(float)LifeToRemove.Value;
                            __instance._AddPartnerLifeTime(AppInfo.PARTNER_NO.Right, -(float)LifeToRemove.Value);
                            __instance._GetPartnerCtrl(AppInfo.PARTNER_NO.Right)._UpdateAge();
                        }
                        else
                        {
                            __instance._AddPartnerLifeTime(AppInfo.PARTNER_NO.Right, -(float)LifeToRemove.Value);
                        }
                        
                    }
                }

            }
        }

        public void Awake()
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

        }
    }
} 