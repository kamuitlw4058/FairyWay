using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameFramework.Resource;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace FairyWay
{
    /// <summary> ILRuntime组件，负责启动热更工程，维护其生命周期以及提供热更工程一些ILR相关函数。并提供了切换为反射执行版本的功能 </summary>
    public class ILRuntimeComponent : GameFrameworkComponent
    {
        [NonSerialized]
        public bool IsHotfixStart;

        private const string TYPE_FULL_NAME = "FairyWay.Hotfix.GameBridge";

        private object m_GameBridgeInstance;
        public IType ILRuntimeBridge;
        private IMethod m_Start;
        private IMethod m_Update;
        private IMethod m_Shutdown;

        private FileStream m_FsDLL;
        private FileStream m_FsPDB;


        //仅在编辑器下提供反射执行方式，方便调试
        /// <summary> 是否反射执行 </summary>
        public bool IsReflection { get; private set; }
        private const string IS_REFLECTION = "Develop.IsReflection";
        public Assembly ReflectionAssembly;
        public Type ReflectionBridge;
        private MethodInfo m_RStart;
        private MethodInfo m_RUpdate;
        private MethodInfo m_RShutdown;

#if UNITY_EDITOR
        [Button("$ButtonText", 150)]
        [DisableInPlayMode]
        private void ChangeReflection()
        {
            if (CheckReflection)
                PlayerPrefs.SetInt(IS_REFLECTION, 0);
            else
                PlayerPrefs.SetInt(IS_REFLECTION, DateTime.Now.DayOfYear);

            PlayerPrefs.Save();
        }
        private static string ButtonText => (CheckReflection ? "当前是反射执行（调试）" : "当前是解释执行") +
                                            "\n\n正常开发时应使用ILRuntime解释执行，当需要调试时可以切换为反射执行。\n" +
                                            "当需要调试仅在ILR环境才会出现的问题时，应使用解释执行并使用VS下的ILR调试插件。\n" +
                                            "为了保证开发环境和真实运行环境的一致，在调试完成后应立即切回解释执行模式。\n" +
                                            "即使你忘记切换，也会在第二天自动切回解释执行模式。\n\n" +
                                            (CheckReflection ? "点击切换为解释执行" : "点击切换为反射执行（调试）");
#endif
        private static bool CheckReflection => PlayerPrefs.GetInt(IS_REFLECTION, 0) == DateTime.Now.DayOfYear;

        /// <summary> ILRuntime入口对象 </summary>
        public AppDomain AppDomain
        {
            get;
            private set;
        }

        private void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                IsReflection = CheckReflection;
            else
                IsReflection = false;
        }

        private void Update()
        {
            if (IsReflection)
            {
                m_RUpdate?.Invoke(m_GameBridgeInstance, new object[] { Time.deltaTime, Time.unscaledDeltaTime });
            }
            else
            {
                if (m_Update != null)
                {
                    //无gc方式调用
                    using (var ctx = AppDomain.BeginInvoke(m_Update))
                    {
                        ctx.PushObject(m_GameBridgeInstance);
                        ctx.PushFloat(Time.deltaTime);
                        ctx.PushFloat(Time.unscaledDeltaTime);
                        ctx.Invoke();
                    }
                }
            }
        }

        private void OnDestroy()
        {
            IsHotfixStart = false;

            if (IsReflection)
            {
                m_RShutdown?.Invoke(m_GameBridgeInstance, null);
            }
            else
            {
                if (m_Shutdown != null)
                {
                    using (var ctx = AppDomain.BeginInvoke(m_Shutdown))
                    {
                        ctx.PushObject(m_GameBridgeInstance);
                        ctx.Invoke();
                    }
                }

                if (m_FsDLL != null)
                {
                    m_FsDLL.Close();
                    m_FsDLL.Dispose();
                    Log.Info("hotfix dll文件流释放");
                }

                if (m_FsPDB != null)
                {
                    m_FsPDB.Close();
                    m_FsPDB.Dispose();
                    Log.Info("hotfix pdb文件流释放");
                }
            }
        }

        /// <summary> 获取所有热更新层类的Type对象 </summary>
        public List<Type> GetHotfixTypes()
        {
            if (IsReflection)
                return ReflectionAssembly.GetTypes().ToList();
            else
                return AppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
        }

        /// <summary> 加载热更新DLL </summary>
        public void LoadHotfixDLL()
        {
            AppDomain = new AppDomain();

            if (IsReflection)
            {
                //反射执行
                Log.Info("本次运行是反射执行！");
                var bytes = File.ReadAllBytes(AssetUtility.GetHotfixDLLEditorAsset());
                var symbol = File.ReadAllBytes(AssetUtility.GetHotfixPDBEditorAsset());
                ReflectionAssembly = Assembly.Load(bytes, symbol);

                HotfixStart();
            }
            else
            {
                //ILR解释执行
                Log.Info("本次运行是解释执行！");
#if UNITY_EDITOR
                //编辑器模式下，直接到Library目录加载Unity自动编译的dll。必须用文件流形式加载，因unity接口无法加载非assets目录下的文件
                m_FsDLL = new FileStream(AssetUtility.GetHotfixDLLEditorAsset(), FileMode.Open, FileAccess.Read);
                Log.Info("hotfix dll加载完毕");
                m_FsPDB = new FileStream(AssetUtility.GetHotfixPDBEditorAsset(), FileMode.Open, FileAccess.Read);
                Log.Info("hotfix pdb加载完毕");
                AppDomain.LoadAssembly(m_FsDLL, m_FsPDB, new PdbReaderProvider());

                //启动调试服务器
                AppDomain.DebugService.StartDebugService(56000);

                //设置Unity主线程ID 这样就可以用Profiler看性能消耗了
                AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

                ILRuntimeHelper.InitILRuntime(AppDomain);

                HotfixStart();
#else
                //发布模式下加载DLL.bytes，会在打包时自动拷贝到目录
                GameEntry.Resource.LoadAsset(AssetUtility.GetHotfixDLLByteAsset(), new LoadAssetCallbacks(
                    (assetName, asset, duration, userData) =>
                    {
                        var dllAsset = (TextAsset) asset;
                        var dll = dllAsset.bytes;
                        Log.Info("hotfix dll加载完毕");

                        AppDomain.LoadAssembly(new MemoryStream(dll));
                        ILRuntimeHelper.InitILRuntime(AppDomain);

                        HotfixStart();
                    },
                    (assetName, status, errorMessage, userData) =>
                    {
                        Log.Error("hotfix dll加载失败 error message '{0}'.", errorMessage);
                    }));
#endif
            }
        }

        /// <summary> 开始执行热更新层代码 </summary>
        private void HotfixStart()
        {
            IsHotfixStart = true;

            if (IsReflection)
            {
                ReflectionBridge = ReflectionAssembly.GetType(TYPE_FULL_NAME);
                m_GameBridgeInstance = ReflectionAssembly.CreateInstance(TYPE_FULL_NAME);

                m_RStart = ReflectionBridge.GetMethod("Start", BindingFlags.Public | BindingFlags.Instance);
                m_RUpdate = ReflectionBridge.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
                m_RShutdown = ReflectionBridge.GetMethod("Shutdown", BindingFlags.Public | BindingFlags.Instance);

                m_RStart?.Invoke(m_GameBridgeInstance, null);
            }
            else
            {
                ILRuntimeBridge = AppDomain.LoadedTypes[TYPE_FULL_NAME];

                m_GameBridgeInstance = ((ILType)ILRuntimeBridge).Instantiate();
                m_Start = ILRuntimeBridge.GetMethod("Start", 0);
                m_Update = ILRuntimeBridge.GetMethod("Update", 2);
                m_Shutdown = ILRuntimeBridge.GetMethod("Shutdown", 0);

                using (var ctx = AppDomain.BeginInvoke(m_Start))
                {
                    ctx.PushObject(m_GameBridgeInstance);
                    ctx.Invoke();
                }
            }
        }
    }
}