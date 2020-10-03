using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;
using SongCore;
using System.Collections;
using PlaylistLoaderLite.HarmonyPatches;
using System;
using IPA.Logging;

namespace PlaylistLoaderLite.UI
{
    public class PluginUI : PersistentSingleton<PluginUI>
    {
        public MenuButton _refreshButton;
        internal static ProgressBar _progressBar;
        const int MESSAGE_TIME = 5;

        public bool ArePlaylistsLoaded { get; internal set; }
        public bool ArePlaylistsLoading { get; internal set; }

        internal void Setup()
        {
            _refreshButton = new MenuButton("Refresh Playlists", "Refresh Songs & Playlists", RefreshButtonPressed, true);
            MenuButtons.instance.RegisterButton(_refreshButton);
            LaunchLoadPlaylists();
        }

        internal void LaunchLoadPlaylists()
        {
            StartCoroutine(LaunchLoadPlaylistsFlow());
        }

        internal void RefreshButtonPressed()
        {
            StartCoroutine(RefreshButtonFlow());
        }

        internal IEnumerator LaunchLoadPlaylistsFlow()
        {
            // Wait for SongCore plugin to load
            yield return new WaitUntil(() => Loader.Instance != null);
            _progressBar = ProgressBar.Create();
            StartCoroutine(RefreshButtonFlow());
        }

        internal IEnumerator RefreshButtonFlow()
        {
            if (!Loader.AreSongsLoading)
                Loader.Instance.RefreshSongs();
            yield return new WaitUntil(() => Loader.AreSongsLoaded == true);
            StartCoroutine(PlaylistCollectionOverride.RefreshPlaylists());
            yield return new WaitWhile(() => !this.ArePlaylistsLoaded || this.ArePlaylistsLoading);
            _progressBar.enabled = true;
            _progressBar.ShowMessage(string.Format("\n{0} playlists loaded.", LoadPlaylistScript.Current.Count), MESSAGE_TIME);
        }
    }
}