using System;
using System.Reflection;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
// using LitJson;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using Version = GameFramework.Version;

namespace FairyWay
{
    public class ProcedureGetConfig : ProcedureBase
    {
        private bool m_GetConfigComplete;
        private bool m_StartInit;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_GetConfigComplete = false;
            m_StartInit = false;

            // GFEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            // GFEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            // GetServerConfig();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            // GFEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            // GFEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!m_GetConfigComplete || m_StartInit)
            {
                return;
            }

            //编辑器资源模式下跳过检查版本和更新资源流程
            //不更新模式下跳过检查更新流程，直接初始化资源
            if (GFEntry.Base.EditorResourceMode)
                ChangeState<ProcedureILRuntime>(procedureOwner);
            else if (GFEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                Log.Warning("本包是不可更新测试包！");
                GFEntry.Resource.InitResources(() => { ChangeState<ProcedureILRuntime>(procedureOwner); });
                m_StartInit = true;
            }
            else
                ChangeState<ProcedureCheckVersion>(procedureOwner);
        }
        /// <summary> web请求获取版本和平台配置 </summary>
        private void GetServerConfig()
        {

            string deviceId = SystemInfo.deviceUniqueIdentifier;
            string deviceName = SystemInfo.deviceName;
            string deviceModel = SystemInfo.deviceModel;
            string processorType = SystemInfo.processorType;
            string processorCount = SystemInfo.processorCount.ToString();
            string memorySize = SystemInfo.systemMemorySize.ToString();
            string operatingSystem = SystemInfo.operatingSystem;
            string iOSGeneration = string.Empty;
            string iOSSystemVersion = string.Empty;
            string iOSVendorIdentifier = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
            iOSGeneration = UnityEngine.iOS.Device.generation.ToString();
            iOSSystemVersion = UnityEngine.iOS.Device.systemVersion;
            iOSVendorIdentifier = UnityEngine.iOS.Device.vendorIdentifier ?? string.Empty;
#endif
            string gameVersion = Version.GameVersion;
            string platform = Application.platform.ToString();
            string language = GFEntry.Localization.Language.ToString();
            string unityVersion = Application.unityVersion;
            string installMode = Application.installMode.ToString();
            string sandboxType = Application.sandboxType.ToString();
            string screenWidth = Screen.width.ToString();
            string screenHeight = Screen.height.ToString();
            string screenDpi = Screen.dpi.ToString();
            string screenOrientation = Screen.orientation.ToString();
            string screenResolution = Utility.Text.Format("{0} x {1} @ {2}Hz",
                Screen.currentResolution.width.ToString(), Screen.currentResolution.height.ToString(),
                Screen.currentResolution.refreshRate.ToString());
            string useWifi = (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork).ToString();

            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("device_id", deviceId);
            wwwForm.AddField("device_name", deviceName);
            wwwForm.AddField("device_model", deviceModel);
            wwwForm.AddField("processor_type", processorType);
            wwwForm.AddField("processor_count", processorCount);
            wwwForm.AddField("memory_size", memorySize);
            wwwForm.AddField("operating_system", operatingSystem);
            wwwForm.AddField("ios_generation", iOSGeneration);
            wwwForm.AddField("ios_system_version", iOSSystemVersion);
            wwwForm.AddField("ios_vendor_identifier", iOSVendorIdentifier);
            wwwForm.AddField("game_version", gameVersion);
            wwwForm.AddField("game_platform", platform);
            wwwForm.AddField("language", language);
            wwwForm.AddField("unity_version", unityVersion);
            wwwForm.AddField("install_mode", installMode);
            wwwForm.AddField("sandbox_type", sandboxType);
            wwwForm.AddField("screen_width", screenWidth);
            wwwForm.AddField("screen_height", screenHeight);
            wwwForm.AddField("screen_dpi", screenDpi);
            wwwForm.AddField("screen_orientation", screenOrientation);
            wwwForm.AddField("screen_resolution", screenResolution);
            wwwForm.AddField("use_wifi", useWifi);
            m_GetConfigComplete = true;

            // Log.Info("获取平台配置 Request: " + GFEntry.BuiltinData.GetConfigUrl);
            // GFEntry.WebRequest.AddWebRequest(GFEntry.BuiltinData.GetConfigUrl, wwwForm, this);
        }

        // private void OnWebRequestSuccess(object sender, GameEventArgs e)
        // {
        //     WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
        //     if (ne.UserData != this)
        //     {
        //         return;
        //     }

        //     try
        //     {
        //         var responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
        //         Log.Info("获取平台配置 Response: " + responseJson);

        //         var serverConfigInfo = JsonMapper.ToObject<ServerConfigInfo>(responseJson);
        //         if (serverConfigInfo == null || serverConfigInfo.Status == false)
        //         {
        //             Log.Error("解析平台配置失败");
        //             GFEntry.BuiltinData.ShowConfirmWindow(GFEntry.BuiltinData.GetConfigFailureText, GetServerConfig);
        //             return;
        //         }

        //         GFEntry.ServerData.ServerConfigInfo = serverConfigInfo;
        //         GFEntry.ServerData.InitResUrl();
        //         GFEntry.ServerData.InitTimeOffset();

        //         m_GetConfigComplete = true;
        //     }
        //     catch (Exception exception)
        //     {
        //         Log.Error($"解析平台配置失败 {exception}");
        //         GFEntry.BuiltinData.ShowConfirmWindow(GFEntry.BuiltinData.GetConfigFailureText, GetServerConfig);
        //     }
        // }

        // private void OnWebRequestFailure(object sender, GameEventArgs e)
        // {
        //     WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
        //     if (ne.UserData != this)
        //     {
        //         return;
        //     }

        //     Log.Error("获取平台配置失败 message '{0}'.", ne.ErrorMessage);
        //     GFEntry.BuiltinData.ShowConfirmWindow(GFEntry.BuiltinData.GetConfigFailureText, GetServerConfig);
        // }
    }
}
