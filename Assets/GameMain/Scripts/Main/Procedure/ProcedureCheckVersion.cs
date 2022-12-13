using System;
using System.IO;
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
    public class ProcedureCheckVersion : ProcedureBase
    {
        private bool m_LatestVersionComplete;
        // private VersionInfo m_VersionInfo;
        private UpdateVersionListCallbacks m_UpdateVersionListCallbacks;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
#if STEAM_CLIENT
            //设置下载Version List回调
            m_UpdateVersionListCallbacks = new UpdateVersionListCallbacks(OnUpdateVersionListSuccess, OnUpdateVersionListFailure);
#endif
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
#if STEAM_CLIENT
            m_LatestVersionComplete = false;
            m_VersionInfo = null;
            
            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            
            RequestVersion();

#endif
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
#if STEAM_CLIENT
            GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            base.OnLeave(procedureOwner, isShutdown);
#endif
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
#if STEAM_CLIENT
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_LatestVersionComplete)
            {
                return;
            }

            
            ChangeState<ProcedureUpdateResource>(procedureOwner);
#endif
        }

        private void RequestVersion()
        {
#if STEAM_CLIENT
            //根据服务器下发的
            资源版本地址和自身系统决定获取versionList的路径
            //TODO:可能修改成php下发等，因此后缀最好也由服务器下发而非写死
            var versionListUrl = Utility.Path.GetRegularPath(Path.Combine(GameEntry.ServerData.ResUrl, GetPlatformPath(), "version.json"));
            
            GameEntry.WebRequest.AddWebRequest(versionListUrl, this);
#endif
        }

        /// <summary> 检查Version List </summary>
        private void UpdateVersion()
        {
#if STEAM_CLIENT
            //使用服务器端的最新资源版本号和本地进行比对
            if (GameEntry.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) == CheckVersionListResult.Updated)
            {
                //已是最新Version List，标记完成
                Log.Info($"Version List已是最新，版本 '{m_VersionInfo.InternalResourceVersion}'");
                m_LatestVersionComplete = true;
            }
            else
            {
                //需要更新Version List
                Log.Info($"需要更新Version List，目标版本 '{m_VersionInfo.InternalResourceVersion}'");
                GameEntry.Resource.UpdateVersionList(m_VersionInfo.VersionListLength, m_VersionInfo.VersionListHashCode,
                    m_VersionInfo.VersionListZipLength, m_VersionInfo.VersionListZipHashCode,
                    m_UpdateVersionListCallbacks);
            }
#endif
        }
#if STEAM_CLIENT
        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            try
            {
                var responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
                m_VersionInfo = JsonMapper.ToObject<VersionInfo>(responseJson);
                if (m_VersionInfo == null)
                {
                    Log.Error("解析版本信息失败");
                    GameEntry.BuiltinData.ShowConfirmWindow(GameEntry.BuiltinData.RequestVersionFailureText, RequestVersion);
                    return;
                }

                Log.Info($"最新游戏包版本为 '{m_VersionInfo.LatestGameVersion}', 本地游戏包版本为 '{Version.GameVersion}'.");

                if (!Version.GameVersion.Equals(m_VersionInfo.LatestGameVersion) && m_VersionInfo.ForceGameUpdate)
                {
                    //强更提示弹窗
                    GameEntry.BuiltinData.ShowConfirmWindow(GameEntry.BuiltinData.ForceUpdateText,
                        () => { Application.OpenURL(m_VersionInfo.ForceGameUpdateUrl); });
                    return;
                }

                //设置资源下载地址
                GameEntry.Resource.UpdatePrefixUri = Utility.Path.GetRegularPath(Path.Combine(GameEntry.ServerData.ResUrl, GetPlatformPath()));

                UpdateVersion();
            }
            catch (Exception exception)
            {
                Log.Error("请求版本信息失败 message '{0}'.", exception);
                GameEntry.BuiltinData.ShowConfirmWindow(GameEntry.BuiltinData.RequestVersionFailureText, RequestVersion);
            }
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("请求版本信息失败 message '{0}'.", ne.ErrorMessage);
            GameEntry.BuiltinData.ShowConfirmWindow(GameEntry.BuiltinData.RequestVersionFailureText, RequestVersion);
        }

        private void OnUpdateVersionListSuccess(string downloadPath, string downloadUri)
        {
            m_LatestVersionComplete = true;
            Log.Info("获取Version List成功，地址：'{0}'", downloadUri);
        }

        private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
        {
            Log.Error("获取Version List失败，地址：'{0}' , message '{1}'.", downloadUri, errorMessage);
            
            GameEntry.BuiltinData.ShowConfirmWindow(GameEntry.BuiltinData.RequestVersionFailureText, RequestVersion);
        }

        private string GetPlatformPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                default:
                    return "Windows64";
            }
        }
#endif
    }
}
