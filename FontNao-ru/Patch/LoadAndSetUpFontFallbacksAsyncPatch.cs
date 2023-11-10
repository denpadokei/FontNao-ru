using BeatSaberMarkupLanguage;
using FontNao_ru.Models;
using HarmonyLib;
using IPA.Utilities.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(nameof(BeatSaberMarkupLanguage.Plugin.OnStart))]
    internal class LoadAndSetUpFontFallbacksAsyncPatch
    {
        [HarmonyPostfix]
        public static async void Postfix()
        {
            await UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                await FontLoader.CreateChatFont();
                var mainfont = BeatSaberUI.MainTextFont;
                var tmp = FontLoader.FallBackFonts.ToList();
                tmp.AddRange(mainfont.fallbackFontAssetTable.ToList());
                mainfont.fallbackFontAssetTable.Clear();
                mainfont.fallbackFontAssetTable.AddRange(tmp);
            });
        }
    }
}
