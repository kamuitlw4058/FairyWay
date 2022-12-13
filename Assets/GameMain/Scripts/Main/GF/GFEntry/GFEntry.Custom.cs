using System.Collections.Generic;
// using SteamClient.Service;
using UnityEngine;
using UnityEngine.Events;

namespace FairyWay
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GFEntry : MonoBehaviour
    {
        public static AdaptationComponent Adaptation { get; private set; }
        public static ILRuntimeComponent ILRuntime { get; private set; }
#if STEAM_CLIENT
        public static BuiltinDataComponent BuiltinData { get; private set; }

       

        public static QualityComponent Quality { get; private set; }

       

        public static GameConfigComponent GameConfig { get; private set; }

        public static ServerDataComponent ServerData { get; private set; }

        public static FightComponent Fight { get; private set; }

        public static CameraComponent Camera { get; private set; }

        public static FGUIComponent FGUI { get; private set; }

        public static TimelineComponent Timeline { get; private set; }

        public static MaterialComponent Material { get; private set; }

        public static VideoComponent Video { get; private set; }

        public static CriVideoComponent CriVideo { get; private set; }

        public static CRIComponent CRI { get; private set; }

        public static PostProcessComponent PostProcess { get; private set; }

        public static ErrorReportComponent ErrorReport { get; private set; }

        public static SdkComponent Sdk { get; private set; }
        public static MainCityComponent MainCity { get; private set; }
        public static ChapterComponent Chapter { get; private set; }
        public static Dictionary<int, string> PropIDToIcon { get; set; }
        
        public static IdIndex clientGeneric = new IdIndex();
#endif
        /*public static List<MainCityBuildClass> buildClassList = new List<MainCityBuildClass>();
        public static List<MainCityBuildClass> BuildClassList{ get { return buildClassList; }set{BuildClassList = value;}}*/

        private static void InitCustomComponents()
        {
            Adaptation = UnityGameFramework.Runtime.GameEntry.GetComponent<AdaptationComponent>();
            ILRuntime = UnityGameFramework.Runtime.GameEntry.GetComponent<ILRuntimeComponent>();
#if STEAM_CLIENT
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
            Quality = UnityGameFramework.Runtime.GameEntry.GetComponent<QualityComponent>();
            
            GameConfig = UnityGameFramework.Runtime.GameEntry.GetComponent<GameConfigComponent>();
            ServerData = UnityGameFramework.Runtime.GameEntry.GetComponent<ServerDataComponent>();
            Fight = UnityGameFramework.Runtime.GameEntry.GetComponent<FightComponent>();
            Camera = UnityGameFramework.Runtime.GameEntry.GetComponent<CameraComponent>();
            FGUI = UnityGameFramework.Runtime.GameEntry.GetComponent<FGUIComponent>();
            Timeline = UnityGameFramework.Runtime.GameEntry.GetComponent<TimelineComponent>();
            Material = UnityGameFramework.Runtime.GameEntry.GetComponent<MaterialComponent>();
            Video = UnityGameFramework.Runtime.GameEntry.GetComponent<VideoComponent>();
            CriVideo = UnityGameFramework.Runtime.GameEntry.GetComponent<CriVideoComponent>();
            CRI = UnityGameFramework.Runtime.GameEntry.GetComponent<CRIComponent>();
            PostProcess = UnityGameFramework.Runtime.GameEntry.GetComponent<PostProcessComponent>();
            ErrorReport = UnityGameFramework.Runtime.GameEntry.GetComponent<ErrorReportComponent>();
            Sdk = UnityGameFramework.Runtime.GameEntry.GetComponent<SdkComponent>();
            MainCity = UnityGameFramework.Runtime.GameEntry.GetComponent<MainCityComponent>();
            Chapter = UnityGameFramework.Runtime.GameEntry.GetComponent<ChapterComponent>();
#endif
        }
    }
}