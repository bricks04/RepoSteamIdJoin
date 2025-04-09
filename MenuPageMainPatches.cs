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
using static RepoSteamIdJoin.MenuManagerPatches;
using System.Security.Cryptography;


namespace RepoSteamIdJoin
{
    [HarmonyPatch(typeof(MenuPageMain))]
    public class MenuPageMainPatches
    {
        public static AssetBundle HoloCheckUIAssets;
        public static GameObject holoCheckUI;
        public static GameObject instantiatedUI;

        public static GameObject uiTarget;

        public static bool attemptedToJoin = false;

        private static int easter = 0;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostFix()
        {
            RepoSteamIdJoin.Logger.LogInfo("MenuPageMainPatches awoken!");
            RepoSteamIdJoin.Logger.LogInfo("Clipboard contains : ");
            RepoSteamIdJoin.Logger.LogInfo(GUIUtility.systemCopyBuffer);
            if (ulong.TryParse(GUIUtility.systemCopyBuffer, out ulong result))
            {
                RepoSteamIdJoin.Logger.LogInfo(result);
                RepoSteamIdJoin.Logger.LogInfo("Parse was successful!");
            }
            else
            {
                RepoSteamIdJoin.Logger.LogWarning("Parse was unsuccessful!");
            }
            // Load external asset pack that contains basic UI stuff, and instantiate them in the same way MoreCompanyAssets does
            //string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //
            //AssetBundle proposedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "holocheckassetbundle"));
            //if (proposedAssetBundle == null)
            //{
            //    RepoSteamIdJoin.Logger.LogError("Failed to load custom assets. If custom assets were previously loaded, you can safely ignore this error. "); // ManualLogSource for your plugin
            //    return;
            //}
            //else
            //{
            //    //HoloCheckUIAssets = proposedAssetBundle;
            //    //RepoSteamIdJoin.Logger.LogInfo("Custom assets loaded!");
            //    //holoCheckUI = HoloCheckUIAssets.LoadAsset<GameObject>("HoloCheckUI");
            //    //instantiatedUI = GameObject.Instantiate(holoCheckUI);
            //    //instantiatedUI.transform.SetAsFirstSibling();
            //
            //    uiTarget = GameObject.Find("UI/HUD/HUD Canvas/HUD/Menu Holder/Menu Page Main(Clone)");
            //    RepoSteamIdJoin.Logger.LogInfo(uiTarget.name);
            //    
            //}
        }

        [HarmonyPatch("ButtonEventJoinGame")]
        [HarmonyPrefix]
        private static bool ButtonEventJoinGamePreFix()
        {
            RepoSteamIdJoin.Logger.LogInfo("MenuPageMainPatches awoken!");
            RepoSteamIdJoin.Logger.LogInfo("Clipboard contains : ");
            RepoSteamIdJoin.Logger.LogInfo(GUIUtility.systemCopyBuffer);
            if (ulong.TryParse(GUIUtility.systemCopyBuffer, out ulong result))
            {
                RepoSteamIdJoin.Logger.LogInfo(result);
                RepoSteamIdJoin.Logger.LogInfo("Parse was successful!");
                if (attemptedToJoin)
                {
                    if (RandomNumberGenerator.GetInt32(99) > 90)
                    {
                        GeneratePopUp("Balls", "Humongous balls", "Ok");
                    }
                    else
                    {
                        GeneratePopUp("ERROR", "Please restart your game before rejoining lobbies", "Ok");
                    }
                }
                else
                {
                    RequestGameLobbyJoin(result);
                    attemptedToJoin = true;
                }
            }
            else
            {
                RepoSteamIdJoin.Logger.LogWarning("Parse was unsuccessful!");
                // RequestGameLobbyJoin(RepoSteamIdJoin.joiningId);
                easter += 1;
                if (RandomNumberGenerator.GetInt32(99) > 90)
                {
                    GeneratePopUp("Balls", "Humongous balls", "Ok");
                }
                else
                {
                    GeneratePopUp("ERROR", "Clipboard does not contain a LobbyID", "wait what is this");
                }
            }

            return false;
        }

    }
}
