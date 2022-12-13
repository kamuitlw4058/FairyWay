using System;
using FairyGUI;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace FairyWay
{
    /// <summary> 屏幕适配组件，负责对各种分辨率下需要进行调整尺寸的相机/Canvas等进行适配处理 </summary>
    public class AdaptationComponent : GameFrameworkComponent
    {
        public int DefaultWidth = 1920; //默认设计宽度
        public int DefaultHeight = 1080; //默认设计高度

        [HideInInspector] public float DefaultRatio; //默认设计的宽高比
        [HideInInspector] public float ActualRatio; //实际屏幕的宽高比
        [HideInInspector] public bool WideMode; //是否是横屏模式
        [HideInInspector] public Vector2 ScreenSize; //屏幕实际分辨率

        public Action OnScreenResize; //屏幕适配时的回调

        private Vector2 m_TempSize; //临时变量,用于监听屏幕是否发生改变

        private void Update()
        {
            //屏幕大小改变时重新计算适配
            m_TempSize.Set(Screen.width, Screen.height);
            if (ScreenSize != m_TempSize)
            {
                Debug.Log("宽>>" + GRoot.inst.width + "高>>" + GRoot.inst.height);
                ScreenSize = m_TempSize;
                RefreshData();
            }
        }

        /// <summary>
        /// 初始化适配相关的数据
        /// </summary>
        public void Init()
        {
            DefaultRatio = DefaultWidth * 1.0f / DefaultHeight;
            ActualRatio = Screen.width * 1.0f / Screen.height;

            WideMode = ActualRatio > 1; //宽如果大于高的话，那么就是横屏模式

            m_TempSize = new Vector2(Screen.width, Screen.height);

            RefreshData();
        }

        /// <summary>
        /// 刷新数据，调用回调
        /// </summary>
        private void RefreshData()
        {
            //刷新实际宽高比
            ActualRatio = Screen.width * 1.0f / Screen.height;
            WideMode = ActualRatio > 1;

            //调用适配回调
            OnScreenResize?.Invoke();
        }

        private void OnDestroy()
        {

        }
    }
}