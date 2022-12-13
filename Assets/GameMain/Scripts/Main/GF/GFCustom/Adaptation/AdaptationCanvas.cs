using UnityEngine;
using UnityEngine.UI;

namespace FairyWay
{
    /// <summary>
    /// 屏幕改变时适配UICanvas组件
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))] //要有CanvasScaler才能绑定脚本
    public class AdaptationCanvas : AdaptationBase
    {
        private CanvasScaler m_CanvasScaler; //获取到CanvasScaler组件

        protected override void Start()
        {
            base.Start(); //必须要调用父类的方法注册

            Init();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Init()
        {
            m_CanvasScaler = GetComponent<CanvasScaler>();

            //为CanvasScaler赋初值（仅仅是以防万一，在Inspector已经赋值，以下步骤可以略过）

            var adaptationComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<AdaptationComponent>();//start函数中调用，GameEntry的静态初始化可能还未完成

            //缩放模式为按照屏幕尺寸缩放
            m_CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            //默认分辨率为默认的设计分辨率
            m_CanvasScaler.referenceResolution = new Vector2(adaptationComponent.DefaultWidth, adaptationComponent.DefaultHeight);
            //适配模式为按照屏幕宽高适配
            m_CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            //默认按照高度适配
            m_CanvasScaler.matchWidthOrHeight = 1;
        }

        /// <summary>
        /// 实现适配的具体逻辑
        /// </summary>
        protected override void OnScreenResize()
        {
            if (GFEntry.Adaptation.WideMode)
            {
                //宽高比大于等于默认宽高比,手机模式（按照高度来适配）
                if (GFEntry.Adaptation.ActualRatio >= GFEntry.Adaptation.DefaultRatio)
                {
                    m_CanvasScaler.matchWidthOrHeight = 1;
                }
                //宽高比小于等于默认宽高比，pad模式（按照宽度来适配）
                else
                {
                    m_CanvasScaler.matchWidthOrHeight = 0;
                }
            }
            else
            {
                //TODO 竖屏模式的适配暂时先不考虑
            }
        }
    }
}
