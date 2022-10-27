using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

namespace FairyWay.Main
{
    /// <summary>
    /// 适配世界地图相机
    /// </summary>
    [RequireComponent(typeof(Camera))] //必须要有相机组件
    public class AdaptationWorldCamera : AdaptationBase //继承AdaptationBase实现OnScreenResize
    {
        private Camera m_Camera;

        private float m_DefaultPositionZ = -4.3f;
        
        protected override void Start()
        {
            base.Start(); //必须要调用父类的方法注册 //获取到相机组件

            m_Camera = GetComponent<Camera>();
        }
    
        /// <summary>
        /// 具体的实现逻辑
        /// </summary>
        protected override void OnScreenResize()
        {
            //适配处理
            //实际宽高比小于设计宽高比（ipad）
            if (GFEntry.Adaptation.ActualRatio < GFEntry.Adaptation.DefaultRatio)
            {
                //保持横向视野不低于16比9时的，拉远镜头
                //warning:经验数值，并没有经过计算
                m_Camera.transform.SetPositionZ(0.89f * m_DefaultPositionZ * (GFEntry.Adaptation.DefaultRatio / GFEntry.Adaptation.ActualRatio));
            }
            //手机
            else
            {
                m_Camera.transform.SetPositionZ(m_DefaultPositionZ);
            }
        }
    }
}