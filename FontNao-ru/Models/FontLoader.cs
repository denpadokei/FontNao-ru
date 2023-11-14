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
        public static Material UINoGlowMaterial => s_noGlow ?? (s_noGlow = BeatSaberUI.MainUIFontMaterial);

        private static Shader s_mainUIFontMaterialShader;
        public static Shader MainUIFontMaterialShader => s_mainUIFontMaterialShader ?? (s_mainUIFontMaterialShader = !BeatSaberUI.MainTextFont ? null : BeatSaberUI.MainTextFont.material.shader);

        // DaNike to the rescue 
        public static bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font)
        {
            if (FontManager.TryGetTMPFontByFamily(family, out font)) {
                font.material.shader = MainUIFontMaterialShader;
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
                if (_mainFont.material.shader != MainUIFontMaterialShader) {
                    _mainFont.material.shader = MainUIFontMaterialShader;
                }
                if (!_mainFont.material.shaderKeywords.Any()) {
                    _mainFont.material.shaderKeywords = UINoGlowMaterial.shaderKeywords;
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
                    if (font.material.shader != MainUIFontMaterialShader) {
                        font.material.shader = MainUIFontMaterialShader;
                    }
                    if (!font.material.shaderKeywords.Any()) {
                        font.material.shaderKeywords = UINoGlowMaterial.shaderKeywords;
                    }
                }
                return _fallbackFonts;
            }
            private set => _fallbackFonts = value;
        }

        public static async Task CreateChatFont()
        {
            IsInitialized = false;
            while (MainUIFontMaterialShader == null) {
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
            foreach (var filename in Directory.EnumerateFiles(MainFontPath, "*.assets", SearchOption.TopDirectoryOnly)) {
                using (var fs = File.OpenRead(filename)) {
                    bundle = AssetBundle.LoadFromStream(fs);
                }
                if (bundle != null) {
                    break;
                }
            }
            if (bundle != null) {
                foreach (var bundleItem in bundle.GetAllAssetNames()) {
                    var asset = bundle.LoadAsset<TMP_FontAsset>(Path.GetFileNameWithoutExtension(bundleItem));
                    if (asset != null) {
                        Plugin.Info($"Main {asset.name} is Load.");
                        MainFont = asset;
                        bundle.Unload(false);
                        break;
                    }
                }
            }
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
                        Plugin.Info($"Fallback {asset.name} is Load.");
                        _fallbackFonts.Add(asset);
                    }
                }
                bundle.Unload(false);
            }
            MainFont.fallbackFontAssetTable = FallBackFonts.ToList();
            IsInitialized = true;
        }
    }
}
