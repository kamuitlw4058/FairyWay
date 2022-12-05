// using FairyGUI;
using LitJson;
// using Lockstep.Math;
using UnityEngine;
using UnityGameFramework.Runtime;
// using static FairyGUI.UIContentScaler;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace FairyWay.Main
{
    public class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //设置帧率
            Application.targetFrameRate = 60;
        
            //json
            InitJsonConverter();

            //debug
            InitDebugger();
            #if STEAM_CLIENT
                //初始化适配组件
                GameEntry.Adaptation.Init();
                
                ChangeState<ProcedureGetConfig>(procedureOwner);

                GRoot.inst.SetContentScaleFactor(Screen.width, Screen.height, ScreenMatchMode.MatchWidthOrHeight);
            #endif
        }
        private void InitDebugger()
        {
            Debug.Log($"GFEntry.Debugger:{GFEntry.Debugger}");
            Debug.Log($"GFEntry.Setting:{GFEntry.Setting}");
            GFEntry.Debugger.ActiveWindow = GFEntry.Setting.GetBool(Constant.Setting.Debugger, false);
            GFEntry.Debugger.WindowScale = 2.0f;
        }
    
        private void InitJsonConverter()
        {
            //初始化LitJson
            JsonMapper.Clear();
            
            //注册自定义类型转换委托

            //string -> int
            JsonMapper.RegisterImporter<string, int>(int.Parse);
            
            //string -> bool
            JsonMapper.RegisterImporter<string, bool>(bool.Parse);
            
            //int -> LFloat
            // JsonMapper.RegisterImporter<int, LFloat>(input => new LFloat(true, input));

            
            //export LFloat
            // JsonMapper.RegisterExporter<LFloat>((lfloat, writer) => { writer.Write(lfloat._val); });
        }
      
    }
}