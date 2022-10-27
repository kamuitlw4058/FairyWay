using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

namespace FairyWay.Main
{
    /// <summary>
    /// 适配ProCamera2D相机
    /// </summary>
    [RequireComponent(typeof(ProCamera2D))] //必须要有相机组件
    public class AdaptationProCamera2D : AdaptationBase //继承AdaptationBase实现OnScreenResize
    {
        protected override void Start()
        {
            base.Start(); //必须要调用父类的方法注册 //获取到相机组件
    
            OnScreenResize();
        }
    
        /// <summary>
        /// 具体的实现逻辑
        /// </summary>
        protected override void OnScreenResize()
        {
            // if(GameEntry.Fight != null)
            //     GameEntry.Fight.GameView?.ResizeCamera();
        }
    }
}