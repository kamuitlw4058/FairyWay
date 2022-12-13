using System;
using System.IO;
using FairyWay;
using GameFramework.Event;
using ILRuntime.Runtime.Enviorment;
using UnityEditor;

namespace FairyWay.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]

    public static class ILRuntimeCrossBinding
    {
        private static readonly Type GeneratorType = typeof(GameEventArgs);
        private static readonly string OutputPath = $"Assets/GameMain/Scripts/Main/GF/GFCustom/ILRuntime/Adapter/{GeneratorType}Adapter.cs";

        [MenuItem("FallenWing/ILRuntime/Generate Cross Binding Adapter")]
        private static void GenerateCrossBindAdapter()
        {
            //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
            //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题

            using (var sw = new StreamWriter(OutputPath))
            {
                sw.WriteLine(CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(GeneratorType, "FairyWay"));
            }

            AssetDatabase.Refresh();
        }
    }
}