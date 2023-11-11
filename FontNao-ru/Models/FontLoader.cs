using BeatSaberMarkupLanguage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FontNao_ru.Models
{
    internal static class FontLoader
    {
        private static Material s_noGlow;
        public static Material UINoGlowMaterial => s_noGlow ?? (s_noGlow = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault());

        private static Shader s_tmpNoGlowFontShader;
        public static Shader TMPNoGlowFontShader => s_tmpNoGlowFontShader ?? (s_tmpNoGlowFontShader = !BeatSaberUI.MainTextFont ? null : BeatSaberUI.MainTextFont.material.shader);

        // DaNike to the rescue 
        public static bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font)
        {
            if (FontManager.TryGetTMPFontByFamily(family, out font)) {
                font.material.shader = TMPNoGlowFontShader;
                return true;
            }

            return false;
        }
        private static readonly string FontAssetPath = Path.Combine(Environment.CurrentDirectory, "UserData", "FontNao_ru", "FontAssets");
        private static readonly string MainFontPath = Path.Combine(FontAssetPath, "Main");
        private static readonly string FallBackFontPath = Path.Combine(FontAssetPath, "FallBack");

        public static bool IsInitialized { get; private set; } = false;

        private static TMP_FontAsset _mainFont = null;

        public static TMP_FontAsset MainFont
        {
            get
            {
                if (!_mainFont) {
                    return null;
                }
                if (_mainFont.material.shader != TMPNoGlowFontShader) {
                    _mainFont.material.shader = TMPNoGlowFontShader;
                }
                return _mainFont;
            }
            private set => _mainFont = value;
        }

        private static List<TMP_FontAsset> _fallbackFonts = new List<TMP_FontAsset>();
        public static List<TMP_FontAsset> FallBackFonts
        {
            get
            {
                foreach (var font in _fallbackFonts) {
                    if (font.material.shader != TMPNoGlowFontShader) {
                        font.material.shader = TMPNoGlowFontShader;
                    }
                }
                return _fallbackFonts;
            }
            private set => _fallbackFonts = value;
        }

        public static async Task CreateChatFont()
        {
            IsInitialized = false;
            while (TMPNoGlowFontShader == null) {
                await Task.Delay(10);
            }
            if (MainFont != null) {
                GameObject.Destroy(MainFont);
            }
            foreach (var font in FallBackFonts) {
                if (font != null) {
                    GameObject.Destroy(font);
                }
            }
            if (!Directory.Exists(MainFontPath)) {
                _ = Directory.CreateDirectory(MainFontPath);
            }
            if (!Directory.Exists(FallBackFontPath)) {
                _ = Directory.CreateDirectory(FallBackFontPath);
            }
            AssetBundle bundle = null;
            _fallbackFonts.Clear();
            foreach (var fallbackFontPath in Directory.EnumerateFiles(FallBackFontPath, "*.assets")) {
                using (var fs = File.OpenRead(fallbackFontPath)) {
                    bundle = AssetBundle.LoadFromStream(fs);
                }
                if (bundle == null) {
                    continue;
                }
                foreach (var bundleItem in bundle.GetAllAssetNames()) {
                    var asset = bundle.LoadAsset<TMP_FontAsset>(Path.GetFileNameWithoutExtension(bundleItem));
                    if (asset != null) {
                        Plugin.Info($"{asset.name} is Load");
                        _fallbackFonts.Add(asset);
                    }
                }
                bundle.Unload(false);
            }
            IsInitialized = true;
        }
    }
}
