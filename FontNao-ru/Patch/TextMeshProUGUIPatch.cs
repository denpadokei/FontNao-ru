using FontNao_ru.Models;
using HarmonyLib;
using System;
using TMPro;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(typeof(TextMeshProUGUI))]
    [HarmonyPatch(nameof(TextMeshProUGUI.Awake), MethodType.Normal)]
    internal class TextMeshProUGUIPatch
    {
        [HarmonyPostfix]
        public static void HarmonyPost(TextMeshProUGUI __instance, TMP_FontAsset ___m_fontAsset)
        {
            try {
                if ((___m_fontAsset.name == "Teko-Medium SDF" || ___m_fontAsset.name == "Teko-Medium SDF Numbers Monospaced Curved") && FontLoader.MainFont && ___m_fontAsset != FontLoader.MainFont) {
                    __instance.font = FontLoader.MainFont;
                }
            }
            catch (Exception) {
            }
        }
    }
}
