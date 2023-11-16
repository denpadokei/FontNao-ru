using BeatSaberMarkupLanguage;
using FontNao_ru.Models;
using HarmonyLib;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FontNao_ru.Patch
{
    /// <summary>
    /// 基本処理
    /// </summary>
    [HarmonyPatch(typeof(TextMeshProUGUI))]
    [HarmonyPatch(nameof(TextMeshProUGUI.Awake), MethodType.Normal)]
    internal class TextMeshProUGUIPatch
    {
        [HarmonyPrefix]
        public static void HarmonyPre(TextMeshProUGUI __instance, ref TMP_FontAsset ___m_fontAsset)
        {
            try {
                if (___m_fontAsset == null || ((___m_fontAsset.name == "Teko-Medium SDF" || ___m_fontAsset.name == "Teko-Medium SDF Numbers Monospaced Curved") && FontLoader.Instance.MainFont && ___m_fontAsset != FontLoader.Instance.MainFont)) {
                    ___m_fontAsset = FontLoader.Instance.MainFont;
                }
                else if (___m_fontAsset.name == "Teko-Medium SDF" || ___m_fontAsset.name == "Teko-Medium SDF Numbers Monospaced Curved") {
                    IEnumerator SetFont()
                    {
                        yield return new WaitWhile(() => !FontLoader.Instance.IsInitialized);
                        if (FontLoader.Instance.MainFont) {
                            __instance.font = FontLoader.Instance.MainFont;
                        }
                    }
                    _ = FontLoader.Instance.StartCoroutine(SetFont());
                }
            }
            catch (Exception) {
                IEnumerator SetFont()
                {
                    yield return new WaitWhile(() => !FontLoader.Instance.IsInitialized);
                    if (FontLoader.Instance.MainFont) {
                        __instance.font = FontLoader.Instance.MainFont;
                    }
                }
                _ = FontLoader.Instance.StartCoroutine(SetFont());
            }
        }
    }

    /// <summary>
    /// 一部BSMLの置換
    /// </summary>
    [HarmonyPatch(typeof(TMP_Text))]
    [HarmonyPatch(nameof(TMP_Text.font), MethodType.Setter)]
    internal class FontSetterPatch
    {
        [HarmonyPrefix]
        public static void HarmonyPre(TMP_Text __instance, ref TMP_FontAsset __0)
        {
            if (__0 == FontLoader.Instance.MainFont) {
                return;
            }
            if (__instance is TextMeshProUGUI text) {
                IEnumerator SetFont()
                {
                    yield return new WaitWhile(() => !FontLoader.Instance.IsInitialized);
                    if (FontLoader.Instance.MainFont && (text.font == BeatSaberUI.MainTextFont || text.font.name == "Teko-Medium SDF" || text.font.name == "Teko-Medium SDF Numbers Monospaced Curved")) {
                        text.font = FontLoader.Instance.MainFont;
                    }
                }
                _ = FontLoader.Instance.StartCoroutine(SetFont());
            }
        }
    }
}
