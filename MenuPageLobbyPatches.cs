using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoSteamIdJoin
{
    [HarmonyPatch(typeof(MenuPageLobby))]
    public class MenuPageLobbyPatches
    {
        private static MenuPageLobby currentInstance;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostFix(MenuPageLobby __instance)
        {
            RepoSteamIdJoin.Logger.LogInfo("MenuPageLobbyPatches started!");
            currentInstance = __instance;
            ChangeChatHelpText("Press <u><b>T</b></u> to chat | Chat <u>/c</u> to copy Lobby ID");
        }

        public static void ChangeChatHelpText(string targetText)
        {
            currentInstance.chatPromptText.text = targetText;
        }
    }
}
