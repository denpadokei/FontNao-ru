using HarmonyLib;
using HMUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;

namespace FontNao_ru.Patch
{
    /// <summary>
    /// BSMLのリストでバグるので共通化
    /// </summary>
    [HarmonyPatch(typeof(TableView))]
    internal class LevelListTableCellEnablePatch
    {
        [HarmonyTargetMethod]
        public static MethodBase Method()
        {
            return typeof(TableView).GetMethod(nameof(TableView.RefreshCells), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPostfix]
        public static void HarmonyPrefix(TableView __instance)
        {
            try {
                foreach (var cell in __instance._visibleCells) {
                    if (cell is LevelListTableCell levelCell) {
                        levelCell._songNameText.enableAutoSizing = true;
                        levelCell._songNameText.fontSizeMax = 4f;
                        levelCell._songNameText.fontSizeMin = 2.5f;
                        levelCell._songNameText.enableWordWrapping = false;
                        levelCell._songNameText.overflowMode = TextOverflowModes.Ellipsis;
                    }
                }
            }
            catch (Exception e) {
                Plugin.Error(e);
            }
        }
    }
}
