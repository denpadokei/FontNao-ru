using BeatSaberMarkupLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

namespace FontNao_ru.Models
{
    internal class FontLoader : ComponentSingleton<FontLoader>
    {
        private Material s_noGlow;
        public Material UINoGlowMaterial
        {
            get
            {
                try {
                    return this.s_noGlow ?? (this.s_noGlow = BeatSaberUI.MainUIFontMaterial);
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        private Shader s_mainUIFontMaterialShader;
        public Shader MainUIFontMaterialShader
        {
            get
            {
                try {
                    return this.s_mainUIFontMaterialShader ?? (this.s_mainUIFontMaterialShader = !BeatSaberUI.MainTextFont ? null : BeatSaberUI.MainTextFont.material.shader);
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        // DaNike to the rescue 
        public bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font)
        {
            if (FontManager.TryGetTMPFontByFamily(family, out font)) {
                font.material.shader = this.MainUIFontMaterialShader;
                return true;
            }

            return false;
        }

        public void Awake()
        {
            this.MainFontPath = Path.Combine(this.FontAssetPath, "Main");
            this.FallBackFontPath = Path.Combine(this.FontAssetPath, "FallBack");
        }

        public void OnDestroy()
        {
            if (this.MainFont) {
                Destroy(this.MainFont);
            }
            foreach (var font in this.FallBackFonts) {
                if (font) {
                    Destroy(font);
                }
            }
            this.IsInitialized = false;
        }

        public IEnumerator Start()
        {
            while (!this.IsInitialized) {
                yield return this.CreateFont();
            }
        }

        private string FontAssetPath { get; } = Path.Combine(Environment.CurrentDirectory, "UserData", "FontNao_ru", "FontAssets");
        private string MainFontPath { get; set; }
        private string FallBackFontPath { get; set; }

        public bool IsInitialized { get; private set; } = false;

        private TMP_FontAsset _mainFont = null;

        public TMP_FontAsset MainFont
        {
            get
            {
                try {
                    if (!this._mainFont) {
                        return null;
                    }
                    if (this._mainFont.material.shader != this.MainUIFontMaterialShader) {
                        this._mainFont.material.shader = this.MainUIFontMaterialShader;
                    }
                    if (!this._mainFont.material.shaderKeywords.Any()) {
                        this._mainFont.material.shaderKeywords = this.UINoGlowMaterial.shaderKeywords;
                    }
                    return this._mainFont;
                }
                catch (Exception) {
                    return null;
                }
            }
            private set => this._mainFont = value;
        }

        private List<TMP_FontAsset> _fallbackFonts = new List<TMP_FontAsset>();
        public List<TMP_FontAsset> FallBackFonts
        {
            get
            {
                try {

                    foreach (var font in this._fallbackFonts) {
                        if (font.material.shader != this.MainUIFontMaterialShader) {
                            font.material.shader = this.MainUIFontMaterialShader;
                        }
                        if (!font.material.shaderKeywords.Any()) {
                            font.material.shaderKeywords = this.UINoGlowMaterial.shaderKeywords;
                        }
                    }
                    return this._fallbackFonts;
                }
                catch (Exception) {
                    return this._fallbackFonts;
                }
            }
            private set => this._fallbackFonts = value;
        }

        public IEnumerator CreateFont()
        {
            this.IsInitialized = false;
            yield return new WaitWhile(() => !this.MainUIFontMaterialShader || !this.UINoGlowMaterial);
            try {
                if (this.MainFont != null) {
                    GameObject.Destroy(this.MainFont);
                }
                foreach (var font in this.FallBackFonts) {
                    if (font != null) {
                        GameObject.Destroy(font);
                    }
                }
                if (!Directory.Exists(this.MainFontPath)) {
                    _ = Directory.CreateDirectory(this.MainFontPath);
                }
                if (!Directory.Exists(this.FallBackFontPath)) {
                    _ = Directory.CreateDirectory(this.FallBackFontPath);
                }
                AssetBundle bundle = null;
                foreach (var filename in Directory.EnumerateFiles(this.MainFontPath, "*.assets", SearchOption.TopDirectoryOnly)) {
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
                            this.MainFont = asset;
                            bundle.Unload(false);
                            break;
                        }
                    }
                }
                this._fallbackFonts.Clear();
                foreach (var fallbackFontPath in Directory.EnumerateFiles(this.FallBackFontPath, "*.assets")) {
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
                            this._fallbackFonts.Add(asset);
                        }
                    }
                    bundle.Unload(false);
                }
                if (this.MainFont) {
                    this.MainFont.fallbackFontAssetTable.AddRange(this.FallBackFonts);
                }
            }
            catch (Exception e) {
                Plugin.Error(e);
                yield break;
            }
            this.IsInitialized = true;
        }
    }
}
