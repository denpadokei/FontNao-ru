﻿using FontNao_ru.Models;
using HarmonyLib;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if ((__instance.font.name == "Teko-Medium SDF" || __instance.font.name == "Teko-Medium SDF Numbers Monospaced Curved") && FontLoader.MainFont && __instance.font != FontLoader.MainFont) {
                    __instance.font = FontLoader.MainFont;
                }
            }
            catch (Exception) {
            }
        }
    }
}