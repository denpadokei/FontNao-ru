using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using FontNao_ru.Models;
using HarmonyLib;
using HMUI;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FontNao_ru.Patch
{
    /// <summary>
    /// メインの曲リストのみ適応
    /// </summary>
    [HarmonyPatch(typeof(LevelListTableCell))]
    [HarmonyPatch(nameof(LevelListTableCell.SetDataFromLevelAsync), MethodType.Normal)]
    internal class LevelListTableCellSetDataFromLevelAsyncPatch
    {

        [HarmonyPrefix]
        public static void HarmonyPrefix(ref TextMeshProUGUI ____songNameText)
        {
            if (____songNameText) {
                ____songNameText.enableAutoSizing = true;
                ____songNameText.fontSizeMax = 4f;
                ____songNameText.fontSizeMin = 2.5f;
            }
        }
    }

    /// <summary>
    /// BSMLのリストでバグる
    /// </summary>
    [HarmonyPatch(typeof(LevelListTableCell))]
    [HarmonyPatch(nameof(LevelListTableCell.enabled), MethodType.Setter)]
    internal class LevelListTableCellEnablePatch
    {
        [HarmonyPrefix]
        public static void HarmonyPrefix(ref TextMeshProUGUI ____songNameText)
        {
            if (____songNameText) {
                ____songNameText.enableWordWrapping = false;
                ____songNameText.overflowMode = TextOverflowModes.Ellipsis;
            }
        }
    }
}
