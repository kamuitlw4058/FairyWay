using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace FairyWay
{
    public class ProcedureILRuntime : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //主工程的常态流程，只负责处理启动热更DLL
            GFEntry.ILRuntime.LoadHotfixDLL();
        }
    }
}
