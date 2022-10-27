using System;
using Google.Protobuf;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;


namespace SteamClient
{
    public class IMessageAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get { return typeof(IMessage); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adaptor); }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        public class Adaptor : MyAdaptor, IMessage
        {
            public Adaptor(AppDomain appdomain, ILTypeInstance instance) : base(appdomain, instance)
            {
            }

            protected override AdaptHelper.AdaptMethod[] GetAdaptMethods()
            {
                AdaptHelper.AdaptMethod[] methods =
                {
                    new AdaptHelper.AdaptMethod {Name = "MergeFrom", ParamCount = 1},
                    new AdaptHelper.AdaptMethod {Name = "WriteTo", ParamCount = 1},
                    new AdaptHelper.AdaptMethod {Name = "CalculateSize", ParamCount = 0},
                };
                return methods;
            }

            public void MergeFrom(CodedInputStream input)
            {
                Invoke(0, input);
            }

            public void WriteTo(CodedOutputStream output)
            {
                Invoke(1, output);
            }

            public int CalculateSize()
            {
                return (int) Invoke(2);
            }
        }
    }

    public static class AdaptHelper
    {
        public class AdaptMethod
        {
            public string Name;
            public int ParamCount;
            public IMethod Method;
        }


        public static IMethod GetMethod(this ILType type, AdaptMethod m)
        {
            if (m.Method != null)
                return m.Method;

            m.Method = type.GetMethod(m.Name, m.ParamCount);
            if (m.Method == null)
            {
                string baseClass = "";
                if (type.FirstCLRBaseType != null)
                {
                    baseClass = type.FirstCLRBaseType.FullName;
                }
                else if (type.FirstCLRInterface != null)
                {
                    baseClass = type.FirstCLRInterface.FullName;
                }

                throw new Exception(string.Format("can't find the method: {0}.{1}:{2}, paramCount={3}", type.FullName,
                    m.Name, baseClass, m.ParamCount));
            }

            return m.Method;
        }
    }

    public abstract class MyAdaptor : CrossBindingAdaptorType
    {
        protected AppDomain AppDomain { get; set; }
        protected ILTypeInstance _instance;
        private AdaptHelper.AdaptMethod[] _methods;

        protected abstract AdaptHelper.AdaptMethod[] GetAdaptMethods();

        public ILTypeInstance ILInstance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        protected object Invoke(int index, params object[] p)
        {
            if (_methods == null)
                _methods = GetAdaptMethods();

            var m = _instance.Type.GetMethod(_methods[index]);
            return AppDomain.Invoke(m, _instance, p);
        }

        protected MyAdaptor(AppDomain appdomain, ILTypeInstance instance)
        {
            AppDomain = appdomain;
            _instance = instance;
        }
    }
}