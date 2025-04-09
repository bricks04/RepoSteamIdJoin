using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace RepoSteamIdJoin
{
    [BepInPlugin("bricks04.RepoSteamIdJoin", "RepoSteamIdJoin", "1.0")]
    public class RepoSteamIdJoin : BaseUnityPlugin
    {
        // To Do List
        // Change the current SteamId Join from a hardcoded variable to a changable variable VIA config
        // Add override code that changes lobby mode to public
        // Add method to copy the Lobby Id to clipboard
        // Add UI to copy the Lobby Id to clipboard
        // Add UI to paste the LobbyId and join lobby so you dont have to use config
        internal static RepoSteamIdJoin Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }

        public static string displayedLobbyId = "";

        public static ulong joiningId = 000;

        private void Awake()
        {
            Instance = this;

            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;

            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        internal void Unpatch()
        {
            Harmony?.UnpatchSelf();
        }

        private void Update()
        {
            // Code that runs every frame goes here
        }

        //Menu Controls
    }
}