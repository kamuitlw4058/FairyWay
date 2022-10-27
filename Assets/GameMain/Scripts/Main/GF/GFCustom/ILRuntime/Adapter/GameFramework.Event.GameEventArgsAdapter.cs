using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace SteamClient
{   
    public class GameEventArgsAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32> mget_Id_0 = new CrossBindingFunctionInfo<System.Int32>("get_Id");
        static CrossBindingMethodInfo mClear_1 = new CrossBindingMethodInfo("Clear");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(GameFramework.Event.GameEventArgs);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : GameFramework.Event.GameEventArgs, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void Clear()
            {
                mClear_1.Invoke(this.instance);
            }

            public override System.Int32 Id
            {
            get
            {
                return mget_Id_0.Invoke(this.instance);

            }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

