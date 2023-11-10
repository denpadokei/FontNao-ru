//using HarmonyLib;
//using SongCore.HarmonyPatches;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TMPro;
//using static IPA.Logging.Logger;

//namespace FontNao_ru.Patch
//{
//    [HarmonyPatch(typeof(LevelListTableCellSetDataFromLevel), nameof(LevelListTableCellSetDataFromLevel.Postfix))]
//    internal class LevelListTableCellSetDataFromLevelPatch
//    {
//        public static void Postfix()
//        {
//            var sb = new StringBuilder();
//            sb.Append(level.songName);
//            sb.Append(" <size=80%>");
//            sb.Append(level.songSubName);
//            sb.Append("</size>");
//            Plugin.Info(sb.ToString());
//            Plugin.Info(level.songAuthorName);
//        }
//    }
//}
