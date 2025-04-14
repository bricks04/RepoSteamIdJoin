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
using System.Text.RegularExpressions;

namespace RepoSteamIdJoin
{
    [HarmonyPatch(typeof(ChatManager))]
    public class ChatManagerPatches
    {
        [HarmonyPatch("MessageSend")]
        [HarmonyPrefix]
        private static bool MessageSendPreFix(ChatManager __instance, bool _possessed)
        {
            if (!_possessed)
            {
                if (__instance.chatMessage == "/c")
                {
                    // Press <u><b>T</b></u> to chat | Chat <u>/c</u> to copy Lobby ID
                    CopyLobbyId();
                    return false;
                }
                else if (__instance.chatMessage == "/clear")
                {
                    // Clear the user tracking dict
                    SteamManagerPatches.playerJoinPair = new Dictionary<ulong, int>();
                    return false;
                }
                else if (__instance.chatMessage == "/track")
                {
                    // Enable the tracker
                    SteamManagerPatches.maxPermittableJoins = 1;
                    return false;
                }
                else if (__instance.chatMessage == "/notrack")
                {
                    // Disable the tracker
                    SteamManagerPatches.maxPermittableJoins = 0;
                    return false;
                }
                else if (Regex.IsMatch(__instance.chatMessage, "^\\/joins\\s[0-9]{1,2}$"))
                {
                    // Set the permittable joins to a specific number
                    RepoSteamIdJoin.Logger.LogInfo("Trying to change permittable joins to " + __instance.chatMessage.Substring(7));
                    if (int.TryParse(__instance.chatMessage.Substring(7), out int result))
                    {
                        SteamManagerPatches.maxPermittableJoins = result;
                    }
                    else
                    {
                        RepoSteamIdJoin.Logger.LogWarning("Entered value did not parse correctly!");
                    }
                    return false;
                }
                // Novelty functions below
                else if (__instance.chatMessage == "/truth")
                {
                    __instance.chatMessage = "Ollie is the cutest!";
                    return true;
                }
                else if (__instance.chatMessage == "balls")
                {
                    return true;
                }
            }

            return true;
        }
    }
}
