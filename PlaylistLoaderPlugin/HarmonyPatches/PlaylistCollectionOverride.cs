﻿using System;
using System.Collections;
using HarmonyLib;
using PlaylistLoaderLite.UI;
using UnityEngine;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace PlaylistLoaderLite.HarmonyPatches
{
    /// <summary>
    /// This is a patch of the method <see cref="PlaylistsViewController.SetData(IAnnotatedBeatmapLevelCollection[], int, bool)"/>
    /// TODO: Remove this or replace it with your own.
    /// </summary>
    [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionsViewController), "SetData",
        new Type[] { // Specify the types of SetDataFromLevelAsync's parameters here.
        typeof(IAnnotatedBeatmapLevelCollection[]), typeof(int), typeof(bool)})]
    public class PlaylistCollectionOverride
    {
        private static IAnnotatedBeatmapLevelCollection[] loadedPlaylists;
        
        /// <summary>
        /// Adds this plugin's name to the beginning of the author text in the song list view.
        /// </summary>
        internal static void Prefix(ref IAnnotatedBeatmapLevelCollection[] annotatedBeatmapLevelCollections)
        {
            // Check if annotatedBeatmapLevelCollections is empty (Versus Tab)
            if (annotatedBeatmapLevelCollections.Length == 0)
                return;
            // Checks if this is the playlists view
            if (annotatedBeatmapLevelCollections[0].GetType().Equals(typeof(UserFavoritesPlaylistSO)))
            {
                IAnnotatedBeatmapLevelCollection[] tempplaylists = new IAnnotatedBeatmapLevelCollection[annotatedBeatmapLevelCollections.Length + loadedPlaylists.Length];
                for (int i = 0; i < annotatedBeatmapLevelCollections.Length; i++)
                {
                    tempplaylists[i] = annotatedBeatmapLevelCollections[i];
                }
                int j = 0;
                for (int i = annotatedBeatmapLevelCollections.Length; i < tempplaylists.Length; i++)
                {
                    tempplaylists[i] = loadedPlaylists[j++];
                }
                annotatedBeatmapLevelCollections = tempplaylists;
            }
        }

        public static IEnumerator RefreshPlaylists()
        {
            PluginUI.instance.ArePlaylistsLoaded = false;
            PluginUI.instance.ArePlaylistsLoading = true;
            yield return LoadPlaylistScript.Load();
            loadedPlaylists = LoadPlaylistScript.Current.ToArray();
            PluginUI.instance.ArePlaylistsLoaded = true;
            PluginUI.instance.ArePlaylistsLoading = false;
            //return loadedPlaylists.Length;
        }
    }
}
