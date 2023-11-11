using FontNao_ru.Models;
using HarmonyLib;
using IPA.Utilities.Async;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(typeof(BeatSaberMarkupLanguage.Plugin), nameof(BeatSaberMarkupLanguage.Plugin.OnStart))]
    internal class LoadAndSetUpFontFallbacksAsyncPatch
    {
        [HarmonyPostfix]
        public static async void Postfix()
        {
            _ = await UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                await FontLoader.CreateChatFont();
            });
        }
    }
}
