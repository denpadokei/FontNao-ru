using HarmonyLib;
using System;
using TMPro;
using static TMPro.TMP_Text;

namespace FontNao_ru.Patch
{
    [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.StringToCharArray))]
    internal class StringToCharArrayPatch
    {
        public static bool Prefix(TMP_Text __instance, ref string sourceText, ref UnicodeChar[] charBuffer)
        {
            if (sourceText == null) {
                charBuffer[0].unicode = 0;
                return false;
            }

            if (charBuffer == null) {
                charBuffer = new UnicodeChar[8];
            }
            try {
                __instance.m_styleStack.SetDefault(0);
                var writeIndex = 0;
                for (var i = 0; i < sourceText.Length; i++) {
                    if (__instance.m_inputSource == TextInputSources.Text && sourceText[i] == '\\' && sourceText.Length > i + 1) {
                        switch (sourceText[i + 1]) {
                            case 'U':
                                if (sourceText.Length > i + 9) {
                                    if (writeIndex == charBuffer.Length) {
                                        __instance.ResizeInternalArray(ref charBuffer);
                                    }

                                    charBuffer[writeIndex].unicode = __instance.GetUTF32(sourceText, i + 2);
                                    charBuffer[writeIndex].stringIndex = i;
                                    charBuffer[writeIndex].length = 10;
                                    i += 9;
                                    writeIndex++;
                                    continue;
                                }

                                break;
                            case '\\':
                                if (__instance.m_parseCtrlCharacters && sourceText.Length > i + 2) {
                                    if (writeIndex + 2 > charBuffer.Length) {
                                        __instance.ResizeInternalArray(ref charBuffer);
                                    }

                                    charBuffer[writeIndex].unicode = sourceText[i + 1];
                                    charBuffer[writeIndex + 1].unicode = sourceText[i + 2];
                                    i += 2;
                                    writeIndex += 2;
                                    continue;
                                }

                                break;
                            case 'n':
                                if (__instance.m_parseCtrlCharacters) {
                                    if (writeIndex == charBuffer.Length) {
                                        __instance.ResizeInternalArray(ref charBuffer);
                                    }

                                    charBuffer[writeIndex].unicode = 10;
                                    charBuffer[writeIndex].stringIndex = i;
                                    charBuffer[writeIndex].length = 1;
                                    i++;
                                    writeIndex++;
                                    continue;
                                }

                                break;
                            case 'r':
                                if (__instance.m_parseCtrlCharacters) {
                                    if (writeIndex == charBuffer.Length) {
                                        __instance.ResizeInternalArray(ref charBuffer);
                                    }

                                    charBuffer[writeIndex].unicode = 13;
                                    charBuffer[writeIndex].stringIndex = i;
                                    charBuffer[writeIndex].length = 1;
                                    i++;
                                    writeIndex++;
                                    continue;
                                }

                                break;
                            case 't':
                                if (__instance.m_parseCtrlCharacters) {
                                    if (writeIndex == charBuffer.Length) {
                                        __instance.ResizeInternalArray(ref charBuffer);
                                    }

                                    charBuffer[writeIndex].unicode = 9;
                                    charBuffer[writeIndex].stringIndex = i;
                                    charBuffer[writeIndex].length = 1;
                                    i++;
                                    writeIndex++;
                                    continue;
                                }

                                break;
                            case 'u':
                                if (sourceText.Length > i + 5) {
                                    if (writeIndex == charBuffer.Length) {
                                        __instance.ResizeInternalArray(ref charBuffer);
                                    }

                                    charBuffer[writeIndex].unicode = __instance.GetUTF16(sourceText, i + 2);
                                    charBuffer[writeIndex].stringIndex = i;
                                    charBuffer[writeIndex].length = 6;
                                    i += 5;
                                    writeIndex++;
                                    continue;
                                }

                                break;
                        }
                    }

                    if (char.IsHighSurrogate(sourceText[i]) && char.IsLowSurrogate(sourceText[i + 1])) {
                        if (writeIndex == charBuffer.Length) {
                            __instance.ResizeInternalArray(ref charBuffer);
                        }

                        charBuffer[writeIndex].unicode = char.ConvertToUtf32(sourceText[i], sourceText[i + 1]);
                        charBuffer[writeIndex].stringIndex = i;
                        charBuffer[writeIndex].length = 2;
                        i++;
                        writeIndex++;
                        continue;
                    }

                    if (sourceText[i] == '<' && __instance.m_isRichText) {
                        if (__instance.IsTagName(ref sourceText, "<BR>", i)) {
                            if (writeIndex == charBuffer.Length) {
                                __instance.ResizeInternalArray(ref charBuffer);
                            }

                            charBuffer[writeIndex].unicode = 10;
                            charBuffer[writeIndex].stringIndex = i;
                            charBuffer[writeIndex].length = 1;
                            writeIndex++;
                            i += 3;
                            continue;
                        }

                        if (__instance.IsTagName(ref sourceText, "<STYLE=", i)) {
                            if (__instance.ReplaceOpeningStyleTag(ref sourceText, i, out var srcOffset, ref charBuffer, ref writeIndex)) {
                                i = srcOffset;
                                continue;
                            }
                        }
                        else if (__instance.IsTagName(ref sourceText, "</STYLE>", i)) {
                            _ = __instance.ReplaceClosingStyleTag(ref sourceText, i, ref charBuffer, ref writeIndex);
                            i += 7;
                            continue;
                        }
                    }

                    if (writeIndex == charBuffer.Length) {
                        __instance.ResizeInternalArray(ref charBuffer);
                    }

                    charBuffer[writeIndex].unicode = sourceText[i];
                    charBuffer[writeIndex].stringIndex = i;
                    charBuffer[writeIndex].length = 1;
                    writeIndex++;
                }

                if (writeIndex == charBuffer.Length) {
                    __instance.ResizeInternalArray(ref charBuffer);
                }

                charBuffer[writeIndex].unicode = 0;
            }
            catch (Exception e) {
                foreach (var item in sourceText) {
                    Plugin.Info($"0x{Convert.ToInt32(item):X8}");
                }
                Plugin.Error(e);
            }
            return false;
        }
    }
}

[HarmonyPatch(typeof(TMP_Text))]
[HarmonyPatch(nameof(TMP_Text.text), MethodType.Setter)]
public class TMPTextTextSetPatch
{
    [HarmonyPrefix]
    public static void Prefix(TMP_Text __instance, ref string __0)
    {
        if (!string.IsNullOrEmpty(__0)) {
            __0 = __0.Replace('\ud835', ' ');
        }
    }
}
