using UnityEngine;

namespace FairyWay
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GFEntry : MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
        }
    }
}
