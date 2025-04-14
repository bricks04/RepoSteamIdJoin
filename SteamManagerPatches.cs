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

namespace RepoSteamIdJoin
{
    [HarmonyPatch(typeof(SteamManager))]
    public class SteamManagerPatches
    {
        public static string currentLobbyId = "";

        public static Dictionary<ulong, int> playerJoinPair = new Dictionary<ulong, int>();
        public static int maxPermittableJoins = 1;

        public static bool CheckPlayerJoin(ulong checkPlayerId)
        {
            if (playerJoinPair.TryGetValue(checkPlayerId, out int value))
            {
                RepoSteamIdJoin.Logger.LogWarning("User " + checkPlayerId.ToString() + " has a record in playerJoinPair!");
                if (value >= maxPermittableJoins)
                {
                    RepoSteamIdJoin.Logger.LogWarning("User " + checkPlayerId.ToString() + " has " + value.ToString() + " joins, more then the permittable joins!");
                    return false;
                }
                else
                {
                    playerJoinPair[checkPlayerId] = value + 1;
                }
            }
            else
            {
                RepoSteamIdJoin.Logger.LogInfo("User " + checkPlayerId.ToString() + " has no record in playerJoinPair");
                playerJoinPair.Add(checkPlayerId, 1);
            }
            return true;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostFix(SteamManager __instance)
        {
            RepoSteamIdJoin.Logger.LogInfo("SteamManager awoken!");

        }

        [HarmonyPatch("OnLobbyMemberJoined")]
        [HarmonyPostfix]
        private static void OnLobbyMemberJoinedPostFix(SteamManager __instance, Lobby _lobby, Friend _friend)
        {
            RepoSteamIdJoin.Logger.LogInfo("Logging a join from id " + _friend.Id.ToString());
            if (__instance.currentLobby.IsOwnedBy(SteamClient.SteamId))
            {
                if (CheckPlayerJoin(_friend.Id.Value))
                {
                    RepoSteamIdJoin.Logger.LogInfo("User is permitted to join the lobby!");
                }
                else
                {
                    RepoSteamIdJoin.Logger.LogWarning("User is not permitted to join the lobby!");
                }
            }
            //d60e54
        }

        [HarmonyPatch("LeaveLobby")]
        [HarmonyPostfix]
        private static void UpdatePostFix(SteamManager __instance)
        {

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
            //Try enforce application of new SteamManager Instance to make sure that future calls are done to the most up to date one?
            RepoSteamIdJoin.Logger.LogInfo(SteamManager.instance.currentLobby);
            RepoSteamIdJoin.Logger.LogInfo(SteamManager.instance.currentLobby.Id);
            currentLobbyId = _lobby.Id.ToString();
        }

        [HarmonyPatch("UnlockLobby")]
        [HarmonyPostfix]
        public static void UnlockLobbyPostFix(SteamManager __instance)
        {
            RepoSteamIdJoin.Logger.LogInfo("Beginning UnlockLobby Postfix");
            // __instance.currentLobby.SetInvisible();
            RepoSteamIdJoin.Logger.LogInfo(SteamManager.instance.currentLobby);
            RepoSteamIdJoin.Logger.LogInfo(SteamManager.instance.currentLobby.Id);
        }

        [HarmonyPatch("LeaveLobby")]
        [HarmonyPostfix]
        private static void LeaveLobbyPostFix(SteamManager __instance)
        {
            //currentInstance = __instance;
        }

        [HarmonyPatch("HostLobby")]
        [HarmonyPostfix]
        public static void HostLobby()
        {
            //joinedALobbyBefore = true;
        }

        public static void CopyLobbyId()
        {
            GUIUtility.systemCopyBuffer = currentLobbyId;
            RepoSteamIdJoin.Logger.LogInfo("Copied the current lobby ID!");
        }

        //public static async void RequestGameLobbyJoin(ulong lobbyId)
        public static void RequestGameLobbyJoin(ulong lobbyId)
        {
            SteamManager.instance.OnGameLobbyJoinRequested(new Lobby(lobbyId), new SteamId());

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
    }
}
