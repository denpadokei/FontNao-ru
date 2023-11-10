using BeatSaberMarkupLanguage;
using FontNao_ru.Models;
using HarmonyLib;
using IPA.Utilities.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(typeof(BeatSaberMarkupLanguage.Plugin), nameof(BeatSaberMarkupLanguage.Plugin.OnStart))]
    internal class LoadAndSetUpFontFallbacksAsyncPatch
    {
        [HarmonyPostfix]
        public static async void Postfix()
        {
            await UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                await FontLoader.CreateChatFont();
            });
        }
    }
}
