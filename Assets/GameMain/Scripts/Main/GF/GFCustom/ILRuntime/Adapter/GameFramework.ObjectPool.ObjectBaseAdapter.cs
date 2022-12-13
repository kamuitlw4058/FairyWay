using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace FairyWay
{
    public class ObjectBaseAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Boolean> mget_CustomCanReleaseFlag_0 = new CrossBindingFunctionInfo<System.Boolean>("get_CustomCanReleaseFlag");
        static CrossBindingMethodInfo mClear_1 = new CrossBindingMethodInfo("Clear");
        static CrossBindingMethodInfo mOnSpawn_2 = new CrossBindingMethodInfo("OnSpawn");
        static CrossBindingMethodInfo mOnUnspawn_3 = new CrossBindingMethodInfo("OnUnspawn");
        static CrossBindingMethodInfo<System.Boolean> mRelease_4 = new CrossBindingMethodInfo<System.Boolean>("Release");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(GameFramework.ObjectPool.ObjectBase);
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

        public class Adapter : GameFramework.ObjectPool.ObjectBase, CrossBindingAdaptorType
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
                if (mClear_1.CheckShouldInvokeBase(this.instance))
                    base.Clear();
                else
                    mClear_1.Invoke(this.instance);
            }

            protected override void OnSpawn()
            {
                if (mOnSpawn_2.CheckShouldInvokeBase(this.instance))
                    base.OnSpawn();
                else
                    mOnSpawn_2.Invoke(this.instance);
            }

            protected override void OnUnspawn()
            {
                if (mOnUnspawn_3.CheckShouldInvokeBase(this.instance))
                    base.OnUnspawn();
                else
                    mOnUnspawn_3.Invoke(this.instance);
            }

            protected override void Release(System.Boolean isShutdown)
            {
                mRelease_4.Invoke(this.instance, isShutdown);
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

