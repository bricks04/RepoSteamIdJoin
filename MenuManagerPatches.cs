using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Steamworks.Data;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using static RepoSteamIdJoin.SteamManagerPatches;
using System.IO;
using System.Reflection;
using Color = UnityEngine.Color;

namespace RepoSteamIdJoin
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatches
    {
        public static MenuManager currentInstance;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostFix(MenuManager __instance)
        {
            currentInstance = __instance;
            RepoSteamIdJoin.Logger.LogInfo(currentInstance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void UpdatePrefix(MenuManager __instance)
        {
            if (currentInstance != __instance)
            {
                currentInstance = __instance;
            }
            
            //GeneratePopUp("Balls", "Humongous balls", "wait what is this");
        }

        public static void GeneratePopUp(string headerText, string bodyText, string buttonText)
        {
            currentInstance.PagePopUp(headerText, Color.yellow, bodyText, buttonText);
        }
    }
}
