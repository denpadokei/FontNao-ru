using FontNao_ru.Models;
using HarmonyLib;
using System;
using TMPro;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(typeof(TMP_Text))]
    [HarmonyPatch(nameof(TMP_Text.text), MethodType.Setter)]
    internal class TextMeshProUGUIPatch
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(TMP_Text __instance)
        {
            try {
                if (__instance is TextMeshProUGUI text && (text.font.name == "Teko-Medium SDF" || text.font.name == "Teko-Medium SDF Numbers Monospaced Curved") && FontLoader.MainFont && text.font != FontLoader.MainFont) {
                    text.font = FontLoader.MainFont;
                }
            }
            catch (Exception) {
            }
        }
    }
}
