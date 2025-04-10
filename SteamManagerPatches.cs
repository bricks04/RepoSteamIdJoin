using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Steamworks.Data;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.ComponentModel.Design;
using static RepoSteamIdJoin.MenuManagerPatches;
using static UnityEngine.InputSystem.InputRemoting;
using System.Text.RegularExpressions;

namespace RepoSteamIdJoin
{
    [HarmonyPatch(typeof(SteamManager))]
    public class SteamManagerPatches
    {
        static SteamManager currentInstance;

        public static string currentLobbyId = "";
        public static bool joinedALobbyBefore = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostFix(SteamManager __instance)
        {
            RepoSteamIdJoin.Logger.LogInfo("SteamManager awoken!");
            currentInstance = __instance;
        }

        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void OnEnablePostFix(SteamManager __instance)
        {
            SteamMatchmaking.OnChatMessage += ProcessChatSignal;
        }

        [HarmonyPatch("LeaveLobby")]
        [HarmonyPostfix]
        private static void UpdatePostFix(SteamManager __instance)
        {
            if (currentInstance != __instance)
            {
                currentInstance = __instance;
            }
        }
        
        [HarmonyPatch("OnGameLobbyJoinRequested")]
        [HarmonyPrefix]
        private static void Start_Prefix(SteamManager __instance)
        {
            // Code to execute for each PlayerController *before* Start() is called.
            RepoSteamIdJoin.Logger.LogDebug($"{__instance} Start Prefix");
        }

        [HarmonyPatch("OnLobbyCreated")]
        [HarmonyPostfix]
        private static void OnLobbyCreatedPostFix(SteamManager __instance, Result _result, Lobby _lobby)
        {
            RepoSteamIdJoin.Logger.LogInfo("Beginning HostLobby postfix");
            RepoSteamIdJoin.Logger.LogInfo("Your lobby code is : " + _lobby.Id.ToString());
            RepoSteamIdJoin.displayedLobbyId = _lobby.Id.ToString();
            //GUIUtility.systemCopyBuffer = _lobby.Id.ToString();
            currentLobbyId = _lobby.Id.ToString();
        }

        [HarmonyPatch("UnlockLobby")]
        [HarmonyPostfix]
        public static void UnlockLobbyPostFix(SteamManager __instance)
        {
            RepoSteamIdJoin.Logger.LogInfo("Beginning UnlockLobby Postfix");
            // __instance.currentLobby.SetInvisible();
        }

        [HarmonyPatch("HostLobby")]
        [HarmonyPostfix]
        public static void HostLobby()
        {
            joinedALobbyBefore = true;
        }

        public static void CopyLobbyId()
        {
            GUIUtility.systemCopyBuffer = currentLobbyId;
            RepoSteamIdJoin.Logger.LogInfo("Copied the current lobby ID!");
        }

        //public static async void RequestGameLobbyJoin(ulong lobbyId)
        public static bool RequestGameLobbyJoin(ulong lobbyId)
        {
            if (!joinedALobbyBefore) 
            {
                currentInstance.OnGameLobbyJoinRequested(new Lobby(lobbyId), new SteamId());
                joinedALobbyBefore = true;
                return true;
            }
            else
            {
                RepoSteamIdJoin.Logger.LogError("You have already attempted to join a lobby this session! Please restart the game to prevent any unintentional behaviour from occurring. ");
                return false;
            }

            // Call OnGameLobbyJoinRequested
            //Debug.Log("Steam: Game lobby join requested: " + lobbyId.ToString());
            //await SteamMatchmaking.JoinLobbyAsync(lobbyId);
            //if (RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu)
            //{
            //    foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
            //    {
            //        player.OutroStartRPC();
            //    }
            //    RunManager.instance.lobbyJoin = true;
            //    RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.LobbyMenu);
            //}
            //currentInstance.joinLobby = true;
        }

        public static void TryToKick(string user)
        {
            RepoSteamIdJoin.Logger.LogInfo(currentInstance.currentLobby.Id);
            RepoSteamIdJoin.Logger.LogInfo(SteamClient.SteamId);
            if (currentInstance.currentLobby.IsOwnedBy(SteamClient.SteamId))
            {
                RepoSteamIdJoin.Logger.LogInfo(currentInstance.currentLobby.Members);
                foreach (Friend member in currentInstance.currentLobby.Members)
                {
                    RepoSteamIdJoin.Logger.LogInfo(member.Name);
                    if (member.Name == user)
                    {
                        RepoSteamIdJoin.Logger.LogInfo("Found the user to kick!");
                        currentInstance.currentLobby.SendChatString("~k" + member.Id.ToString());
                    }
                }
                RepoSteamIdJoin.Logger.LogInfo("List over");
            }
            else
            {
                RepoSteamIdJoin.Logger.LogWarning("Attempted to kick, but is not the lobby owner.");
            }
        }

        private static void ProcessChatSignal(Lobby lobby, Friend friend, string arg3)
        {
            RepoSteamIdJoin.Logger.LogInfo("Recieved signal!");
            RepoSteamIdJoin.Logger.LogInfo(arg3);
            if (Regex.IsMatch(arg3, "^~k"))
            {
                string targetToKick = arg3.Substring(2);
                RepoSteamIdJoin.Logger.LogInfo(targetToKick);
                if (SteamClient.SteamId.ToString() == targetToKick)
                {
                    RepoSteamIdJoin.Logger.LogWarning("I have been requested to be kicked!");
                    currentInstance.LeaveLobby();
                }
            }
            
        }
    }
}
