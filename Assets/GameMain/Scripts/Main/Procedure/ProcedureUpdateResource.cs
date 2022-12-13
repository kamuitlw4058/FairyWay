using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace FairyWay
{
    public class ProcedureUpdateResource : ProcedureBase
    {
#if STEAM_CLIENT
        private bool m_UpdateAllComplete;
        private int m_UpdateCount;
        private long m_UpdateTotalZipLength;
        private int m_UpdateSuccessCount;
        private readonly List<UpdateLengthData> m_UpdateLengthData = new List<UpdateLengthData>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_UpdateAllComplete = false;
            m_UpdateCount = 0;
            m_UpdateTotalZipLength = 0L;
            m_UpdateSuccessCount = 0;
            m_UpdateLengthData.Clear();

            GameEntry.Event.Subscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            GameEntry.Event.Subscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            GameEntry.Event.Subscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            GameEntry.Event.Subscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

            GameEntry.Resource.CheckResources(OnCheckResourcesComplete);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            GameEntry.Event.Unsubscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            GameEntry.Event.Unsubscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            GameEntry.Event.Unsubscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_UpdateAllComplete)
            {
                return;
            }

            ChangeState<ProcedureILRuntime>(procedureOwner);
        }

        /// <summary> 开始更新资源 </summary>
        private void StartUpdateResources()
        {
            GameEntry.BuiltinData.ShowProgress();
            
            GameEntry.Resource.UpdateResources(OnUpdateResourcesComplete);
        }

        /// <summary> 更新资源完成 </summary>
        private void ProcessUpdateResourcesComplete()
        {
            m_UpdateAllComplete = true;
        }
        
        /// <summary> 通知进度刷新 </summary>
        private void RefreshProgress()
        {
            long updateSuccessLength = 0L;
            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                updateSuccessLength += m_UpdateLengthData[i].Length;
            }

            GameEntry.BuiltinData.SetProgress(m_UpdateSuccessCount, m_UpdateCount, updateSuccessLength, m_UpdateTotalZipLength,
                GameEntry.Download.CurrentSpeed);
        }

        /// <summary> 检查资源回调 </summary>
        private void OnCheckResourcesComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            m_UpdateCount = updateCount;
            m_UpdateTotalZipLength = updateTotalZipLength;
            if (updateCount <= 0)
            {
                //没有需要更新的文件，直接完成
                ProcessUpdateResourcesComplete();
                return;
            }

            Log.Info("检查资源完成, '{0}'个资源需要更新, zip大小：'{1}', 解压后大小：'{2}', '{3}'个资源将被移除", updateCount.ToString(),
                updateTotalZipLength.ToString(), updateTotalLength.ToString(), removedCount.ToString());

            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //流量下载提示弹窗
                var lengthString = $"{(updateTotalZipLength / 1024f / 1024f):F2} MB";
                var text = Utility.Text.Format(GameEntry.BuiltinData.CarrierDataNetworkText, lengthString);
                GameEntry.BuiltinData.ShowConfirmWindow(text, StartUpdateResources);
                return;
            }

            StartUpdateResources();
        }

        /// <summary> 更新资源组完成回调 </summary>
        private void OnUpdateResourcesComplete(GameFramework.Resource.IResourceGroup resourceGroup, bool result)
        {
            if (result)
            {
                //因资源没有分组，所以一个组完成则直接完成更新流程，若分组则需调整逻辑
                Log.Info("更新资源成功");
                ProcessUpdateResourcesComplete();
            }
            else
            {
                Log.Error("更新资源结束，遇到了错误");
            }
        }

        /// <summary>  </summary>
        private void OnResourceUpdateStart(object sender, GameEventArgs e)
        {
            ResourceUpdateStartEventArgs ne = (ResourceUpdateStartEventArgs)e;

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    Log.Warning("Update resource '{0}' is invalid.", ne.Name);
                    m_UpdateLengthData[i].Length = 0;
                    RefreshProgress();
                    return;
                }
            }

            m_UpdateLengthData.Add(new UpdateLengthData(ne.Name));
        }

        private void OnResourceUpdateChanged(object sender, GameEventArgs e)
        {
            ResourceUpdateChangedEventArgs ne = (ResourceUpdateChangedEventArgs)e;

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    m_UpdateLengthData[i].Length = ne.CurrentLength;
                    RefreshProgress();
                    return;
                }
            }

            Log.Warning("Update resource '{0}' is invalid.", ne.Name);
        }

        private void OnResourceUpdateSuccess(object sender, GameEventArgs e)
        {
            ResourceUpdateSuccessEventArgs ne = (ResourceUpdateSuccessEventArgs)e;
            Log.Info("更新资源 '{0}' 成功.", ne.Name);

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    m_UpdateLengthData[i].Length = ne.CompressedLength;
                    m_UpdateSuccessCount++;
                    RefreshProgress();
                    return;
                }
            }

            Log.Warning("Update resource '{0}' is invalid.", ne.Name);
        }

        private void OnResourceUpdateFailure(object sender, GameEventArgs e)
        {
            ResourceUpdateFailureEventArgs ne = (ResourceUpdateFailureEventArgs)e;
            if (ne.RetryCount >= ne.TotalRetryCount)
            {
                Log.Error("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
                return;
            }
            else
            {
                Log.Info("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
            }

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    m_UpdateLengthData.Remove(m_UpdateLengthData[i]);
                    RefreshProgress();
                    return;
                }
            }

            Log.Warning("Update resource '{0}' is invalid.", ne.Name);
        }

        private class UpdateLengthData
        {
            public UpdateLengthData(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public int Length { get; set; }
        }
#endif
    }
}
