using System.IO;
using FairyWay;
using ILRuntime.Runtime.Enviorment;
using UnityEditor;

namespace FairyWay.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public static class ILRuntimeCLRBinding
    {
        private const string OUTPUT_PATH = "Assets/GameMain/Scripts/Main/GF/GFCustom/ILRuntime/Generated";

        [MenuItem("FallenWing/ILRuntime/Generate CLR Binding Code by Analysis")]
        private static void GenerateCLRBindingByAnalysis()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            var appDomain = new AppDomain();
            using (var fs = new FileStream(AssetUtility.GetHotfixDLLEditorAsset(), FileMode.Open, FileAccess.Read))
            {
                appDomain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                InitILRuntime(appDomain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appDomain, OUTPUT_PATH);
            }

            AssetDatabase.Refresh();
        }

        private static void InitILRuntime(AppDomain appDomain)
        {
            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            ILRuntimeHelper.RegisterCrossBindingAdaptor(appDomain);
            ILRuntimeHelper.RegisterValueTypeBinder(appDomain);
        }
    }
}