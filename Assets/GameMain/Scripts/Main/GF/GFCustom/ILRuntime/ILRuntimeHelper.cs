using System;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using LitJson;
// using Spine;
// using Lockstep.Math;
using UnityEngine;
using UnityEngine.Events;
using UnityGameFramework.Runtime;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace FairyWay
{
    public static unsafe class ILRuntimeHelper
    {
        public static void InitILRuntime(AppDomain appDomain)
        {
            //注册LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);
            //注册CLR重定向方法
            RegisterCLRMethodRedirection(appDomain);

            //适配委托
            RegisterDelegate(appDomain);

            //注册跨域继承适配器
            RegisterCrossBindingAdaptor(appDomain);

            //注册值类型绑定
            RegisterValueTypeBinder(appDomain);

            //注册CLR绑定代码，必须置于最后
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);
        }
        public static void RegisterCLRMethodRedirection(AppDomain appDomain)
        {
            //重定向Log.Error日志打印，添加热更dll内堆栈，暂时只写了参数为单个string类型的
            //warning:会影响GF调试器查看日志的效果
            var mi = typeof(Log).GetMethod("Error", new System.Type[] { typeof(string) });
            appDomain.RegisterCLRMethodRedirection(mi, Log_11);
        }

        public static void RegisterDelegate(AppDomain appDomain)
        {
            appDomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.ObjectBaseAdapter.Adapter, System.Boolean>();
            appDomain.DelegateManager.RegisterMethodDelegate<FairyGUI.GObject, System.Object>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.ObjectBaseAdapter.Adapter, FairyWay.ObjectBaseAdapter.Adapter, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector2, System.Boolean>();
            appDomain.DelegateManager.RegisterMethodDelegate<object>();
            appDomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<IDictionary<string, UnityEngine.Object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
            appDomain.DelegateManager.RegisterFunctionDelegate<object, bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<FairyGUI.GObject>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.String, System.String, System.Type, FairyGUI.PackageItem>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Texture>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.AudioClip>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, object, float, object>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, GameFramework.Resource.LoadResourceStatus, string, object>();
            appDomain.DelegateManager.RegisterFunctionDelegate<string, string, Type, object>();
            appDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.String>();
            appDomain.DelegateManager.RegisterMethodDelegate<Google.Protobuf.IMessage>();
            // appDomain.DelegateManager.RegisterMethodDelegate<Spine.Unity.SkeletonAnimation>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Int32, FairyGUI.GObject>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.IMessageAdapter.Adaptor>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Playables.PlayableDirector>();
            appDomain.DelegateManager.RegisterMethodDelegate<FairyGUI.EventContext>();
            appDomain.DelegateManager.RegisterFunctionDelegate<GameFramework.ObjectPool.ObjectPoolBase, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int32>();
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, ILTypeInstance>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, GameFramework.Event.GameEventArgs>();
            // appDomain.DelegateManager.RegisterMethodDelegate<Spine.TrackEntry>();
            appDomain.DelegateManager.RegisterMethodDelegate<FairyGUI.GTweener>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Video.VideoPlayer>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.IMessageAdapter.Adaptor, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>, System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Char, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyGUI.EventContext, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.IMessageAdapter.Adaptor, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.IMessageAdapter.Adaptor, FairyWay.IMessageAdapter.Adaptor, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.Int64, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyWay.IMessageAdapter.Adaptor, System.Int64>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<System.Int32>>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.String>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32, System.Int32>();
            appDomain.DelegateManager.RegisterFunctionDelegate<FairyGUI.GObject, FairyGUI.GObject, System.Int32>();
            // appDomain.DelegateManager.RegisterFunctionDelegate<SteamClient.FightHeroData, System.Boolean>();

            // appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<SteamClient.FightHeroData>>((act) =>
            // {
            //     return new System.Predicate<SteamClient.FightHeroData>((obj) =>
            //     {
            //         return ((Func<SteamClient.FightHeroData, System.Boolean>)act)(obj);
            //     });
            // });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<FairyGUI.GObject>>((act) =>
            {
                return new System.Comparison<FairyGUI.GObject>((x, y) =>
                {
                    return ((Func<FairyGUI.GObject, FairyGUI.GObject, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int64>>((act) =>
            {
                return new System.Comparison<System.Int64>((x, y) =>
                {
                    return ((Func<System.Int64, System.Int64, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<FairyWay.IMessageAdapter.Adaptor>>((act) =>
            {
                return new System.Comparison<FairyWay.IMessageAdapter.Adaptor>((x, y) =>
                {
                    return ((Func<FairyWay.IMessageAdapter.Adaptor, FairyWay.IMessageAdapter.Adaptor, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>>>((act) =>
            {
                return new System.Comparison<System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>>((x, y) =>
                {
                    return ((Func<System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>, System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction>((act) =>
            {
                return new UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<float>>((act) =>
            {
                return new UnityAction<float>((arg0) =>
                {
                    ((Action<float>)act)(arg0);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<EventHandler<GameFramework.Event.GameEventArgs>>((act) =>
            {
                return new EventHandler<GameFramework.Event.GameEventArgs>((sender, e) =>
                {
                    ((Action<object, GameFramework.Event.GameEventArgs>)act)(sender, e);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.EventCallback0>((act) =>
            {
                return new FairyGUI.EventCallback0(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.UIPackage.CreateObjectCallback>((act) =>
            {
                return new FairyGUI.UIPackage.CreateObjectCallback((result) =>
                {
                    ((Action<FairyGUI.GObject>)act)(result);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.UIPackage.LoadResourceAsync>((act) =>
            {
                return new FairyGUI.UIPackage.LoadResourceAsync((name, extension, type, item) =>
                {
                    ((Action<System.String, System.String, System.Type, FairyGUI.PackageItem>)act)(name, extension, type, item);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<GameFramework.Resource.LoadAssetSuccessCallback>((act) =>
            {
                return new GameFramework.Resource.LoadAssetSuccessCallback((assetName, asset, duration, userData) =>
                {
                    ((Action<string, object, float, object>)act)(assetName, asset, duration, userData);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<GameFramework.Resource.LoadAssetFailureCallback>((act) =>
            {
                return new GameFramework.Resource.LoadAssetFailureCallback((assetName, status, errorMessage, userData) =>
                {
                    ((Action<string, GameFramework.Resource.LoadResourceStatus, string, object>)act)(assetName, status, errorMessage, userData);
                });
            });

            // appDomain.DelegateManager.RegisterDelegateConvertor<Spine.AnimationState.TrackEntryDelegate>((act) =>
            // {
            //     return new Spine.AnimationState.TrackEntryDelegate((trackEntry) =>
            //     {
            //         ((Action<Spine.TrackEntry>)act)(trackEntry);
            //     });
            // });

            appDomain.DelegateManager.RegisterDelegateConvertor<GameFramework.Resource.InitResourcesCompleteCallback>((act) =>
            {
                return new GameFramework.Resource.InitResourcesCompleteCallback(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.PlayCompleteCallback>((act) =>
            {
                return new FairyGUI.PlayCompleteCallback(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.TransitionHook>((act) =>
            {
                return new FairyGUI.TransitionHook(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.TimerCallback>((act) =>
            {
                return new FairyGUI.TimerCallback((param) =>
                {
                    ((Action<System.Object>)act)(param);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.ListItemRenderer>((act) =>
            {
                return new FairyGUI.ListItemRenderer((index, item) =>
                {
                    ((Action<System.Int32, FairyGUI.GObject>)act)(index, item);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.EventCallback1>((act) =>
            {
                return new FairyGUI.EventCallback1((context) =>
                {
                    ((Action<FairyGUI.EventContext>)act)(context);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<GameFramework.ObjectPool.ObjectPoolBase>>((act) =>
            {
                return new System.Predicate<GameFramework.ObjectPool.ObjectPoolBase>((obj) =>
                {
                    return ((Func<GameFramework.ObjectPool.ObjectPoolBase, System.Boolean>)act)(obj);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Int32>>((act) =>
            {
                return new System.Predicate<System.Int32>((obj) =>
                {
                    return ((Func<System.Int32, System.Boolean>)act)(obj);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Converter<System.String, System.Int32>>((act) =>
            {
                return new System.Converter<System.String, System.Int32>((input) =>
                {
                    return ((Func<System.String, System.Int32>)act)(input);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.GTweenCallback1>((act) =>
            {
                return new FairyGUI.GTweenCallback1((tweener) =>
                {
                    ((Action<FairyGUI.GTweener>)act)(tweener);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.GTweenCallback>((act) =>
            {
                return new FairyGUI.GTweenCallback(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
            {
                return new System.Comparison<System.Int32>((x, y) =>
                {
                    return ((Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<FairyWay.IMessageAdapter.Adaptor>>((act) =>
            {
                return new System.Predicate<FairyWay.IMessageAdapter.Adaptor>((obj) =>
                {
                    return ((Func<FairyWay.IMessageAdapter.Adaptor, System.Boolean>)act)(obj);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<FairyWay.ObjectBaseAdapter.Adapter>>((act) =>
            {
                return new System.Comparison<FairyWay.ObjectBaseAdapter.Adapter>((x, y) =>
                {
                    return ((Func<FairyWay.ObjectBaseAdapter.Adapter, FairyWay.ObjectBaseAdapter.Adapter, System.Int32>)act)(x, y);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UnityEngine.Vector2>>((act) =>
            {
                return new System.Predicate<UnityEngine.Vector2>((obj) =>
                {
                    return ((Func<UnityEngine.Vector2, System.Boolean>)act)(obj);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<FairyWay.ObjectBaseAdapter.Adapter>>((act) =>
            {
                return new System.Predicate<FairyWay.ObjectBaseAdapter.Adapter>((obj) =>
                {
                    return ((Func<FairyWay.ObjectBaseAdapter.Adapter, System.Boolean>)act)(obj);
                });
            });
        }

        public static void RegisterCrossBindingAdaptor(AppDomain appDomain)
        {
            // appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            // appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
            // appDomain.RegisterCrossBindingAdaptor(new IMessageAdapter());
            // appDomain.RegisterCrossBindingAdaptor(new ObjectBaseAdapter());//注意非完全自动生成
        }

        public static void RegisterValueTypeBinder(AppDomain appDomain)
        {
            // appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            // appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            // appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
        }


        //CLR重定向
        //编写重定向方法对于刚接触ILRuntime的朋友可能比较困难，比较简单的方式是通过CLR绑定生成绑定代码，然后在这个基础上改，比如下面这个代码是从UnityEngine_Debug_Binding里面复制来改的
        //如何使用CLR绑定请看相关教程和文档
        private static unsafe StackObject* Log_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

            //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
            object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            //所有非基础类型都得调用Free来释放托管堆栈
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            //我们在输出信息后面加上DLL堆栈
            Log.Error(message + "\nILStack:\n" + stacktrace);

            return __ret;
        }

    }
}

