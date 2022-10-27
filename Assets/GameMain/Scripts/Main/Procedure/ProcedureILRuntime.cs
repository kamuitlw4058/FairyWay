using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace FairyWay.Main
{
    public class ProcedureILRuntime : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
        #if STEAM_CLIENT
            base.OnEnter(procedureOwner);

            //主工程的常态流程，只负责处理启动热更DLL
            GameEntry.ILRuntime.LoadHotfixDLL();
        #endif
        }
    }
}
