using UnityEngine;

namespace FairyWay
{
    /// <summary>
    /// 屏幕适配组件的基类
    /// </summary>
    public abstract class AdaptationBase : MonoBehaviour
    {
        /// <summary>
        /// 在Start方法中把具体的适配逻辑注册到管理器中
        /// </summary>
        protected virtual void Start()
        {
            UnityGameFramework.Runtime.GameEntry.GetComponent<AdaptationComponent>().OnScreenResize += OnScreenResize;
        }

        /// <summary>
        /// 在OnDestroy方法中把具体的适配逻辑从管理器注销
        /// </summary>
        protected virtual void OnDestroy()
        {
            var adaptationComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<AdaptationComponent>();
            if (adaptationComponent)
                adaptationComponent.OnScreenResize -= OnScreenResize;
        }

        /// <summary>
        /// 具体的适配逻辑交给子类去实现
        /// </summary>
        protected abstract void OnScreenResize();
    }
}