using BeatSaberMarkupLanguage;
using BS_Utils.Utilities;
using FontNao_ru.Models;
using HarmonyLib;
using IPA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace FontNao_ru
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        // TODO: If using Harmony, uncomment and change YourGitHub to the name of your GitHub account, or use the form "com.company.project.product"
        //       You must also add a reference to the Harmony assembly in the Libs folder.
        // public const string HarmonyId = "com.github.YourGitHub.FontNao_ru";
        // internal static readonly HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(HarmonyId);

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        private Harmony harmony;

        public static void Error(Exception log, [CallerMemberName] string mem = null, [CallerLineNumber] int line = 0)
        {
            try {
                Log.Error($"[{mem}({line})]{log}");
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public static void Info(string message, [CallerMemberName] string mem = null, [CallerLineNumber] int line = 0)
        {
            try {
                Log.Info($"[{mem}({line})]{message}");
            }
            catch (Exception e) {
                Log.Error($"[{mem}({line})]{e}");
            }
        }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public Plugin(IPALogger logger)
        {
            Instance = this;
            Plugin.Log = logger;
            Plugin.Log?.Debug("Logger initialized.");
        }

        private void BSEvents_lateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            _ = FontLoader.Instance.StartCoroutine(ChangeFonts());
        }

        private static readonly HashSet<TMP_FontAsset> s_fonts = new HashSet<TMP_FontAsset>();

        private static IEnumerator ChangeFonts()
        {
            yield return new WaitWhile(() => !FontLoader.Instance || !FontLoader.Instance.IsInitialized);
            var tmp = FontLoader.Instance.FallBackFonts.ToList();
            foreach (var fontAsset in Resources.FindObjectsOfTypeAll<TMP_FontAsset>()) {
                Info(fontAsset.name);
                try {
                    if (fontAsset == FontLoader.Instance.MainFont) {
                        if (FontLoader.Instance.MainFont.fallbackFontAssetTable == null) {
                            FontLoader.Instance.MainFont.fallbackFontAssetTable = new List<TMP_FontAsset> { BeatSaberUI.MainTextFont };
                        }
                        else {
                            FontLoader.Instance.MainFont.fallbackFontAssetTable.Add(BeatSaberUI.MainTextFont);
                        }
                        _ = s_fonts.Add(fontAsset);
                        Info($"{fontAsset.name} is Main font.");
                        continue;
                    }
                    if (FontLoader.Instance.FallBackFonts.Any()) {
                        if (s_fonts.Contains(fontAsset)) {
                            Info($"{fontAsset.name} aleady seted.");
                            continue;
                        }
                        var newFallBack = new List<TMP_FontAsset>();
                        var oldFallback = fontAsset.fallbackFontAssetTable?.ToList();
                        newFallBack.AddRange(tmp);
                        if (oldFallback != null) {
                            newFallBack.AddRange(oldFallback);
                        }
                        Info($"{fontAsset.name} Set fallback.");
                        if (fontAsset.fallbackFontAssetTable == null) {
                            fontAsset.fallbackFontAssetTable = newFallBack;
                        }
                        else {
                            fontAsset.fallbackFontAssetTable.Clear();
                            fontAsset.fallbackFontAssetTable.AddRange(newFallBack);
                        }
                        _ = s_fonts.Add(fontAsset);
                    }
                }
                catch (Exception e) {
                    Error(e);
                }
            }
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Plugin.Log?.Debug("Config loaded");
        }
        */
        #endregion

        #region Disableable

        /// <summary>
        /// Called when the plugin is enabled (including when the game starts if the plugin is enabled).
        /// </summary>
        [OnEnable]
        public void OnEnable()
        {
            BSEvents.lateMenuSceneLoadedFresh += this.BSEvents_lateMenuSceneLoadedFresh;
            this.ApplyHarmonyPatches();
        }

        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit. It is important to clean up any Harmony patches, GameObjects, and Monobehaviours here.
        /// The game should be left in a state as if the plugin was never started.
        /// Methods marked [OnDisable] must return void or Task.
        /// </summary>
        [OnDisable]
        public void OnDisable()
        {
            this.RemoveHarmonyPatches();
            BSEvents.lateMenuSceneLoadedFresh -= this.BSEvents_lateMenuSceneLoadedFresh;
        }

        /*
        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit.
        /// Return Task for when the plugin needs to do some long-running, asynchronous work to disable.
        /// [OnDisable] methods that return Task are called after all [OnDisable] methods that return void.
        /// </summary>
        [OnDisable]
        public async Task OnDisableAsync()
        {
            await LongRunningUnloadTask().ConfigureAwait(false);
        }
        */
        #endregion

        // Uncomment the methods in this section if using Harmony
        #region Harmony

        /// <summary>
        /// Attempts to apply all the Harmony patches in this assembly.
        /// </summary>
        internal void ApplyHarmonyPatches()
        {
            try {
                Plugin.Log?.Debug("Applying Harmony patches.");
                this.harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex) {
                Plugin.Log?.Error("Error applying Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }

        /// <summary>
        /// Attempts to remove all the Harmony patches that used our HarmonyId.
        /// </summary>
        internal void RemoveHarmonyPatches()
        {
            try {
                // Removes all patches with this HarmonyId
                this.harmony?.UnpatchSelf();
            }
            catch (Exception ex) {
                Plugin.Log?.Error("Error removing Harmony patches: " + ex.Message);
                Plugin.Log?.Debug(ex);
            }
        }

        #endregion
    }
}
