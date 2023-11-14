using BeatSaberMarkupLanguage;
using FontNao_ru.Models;
using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(typeof(LevelListTableCell))]
    [HarmonyPatch(nameof(LevelListTableCell.SetDataFromLevelAsync), 0)]
    internal class LebelBarPatch
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(ref TextMeshProUGUI ____songNameText)
        {
            ____songNameText.enableAutoSizing = true;
            ____songNameText.fontSizeMax = 4f;
            ____songNameText.fontSizeMin = 2.5f;
            ____songNameText.enableWordWrapping = false;
            ____songNameText.overflowMode = TextOverflowModes.Ellipsis;
        }
    }
}
