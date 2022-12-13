using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FairyWay;
using FairyWay.Editor.ResourceTools;
using GameFramework.Resource;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;

namespace FairyWay.Editor
{
    public class BuildPlayerOptionsEditor : OdinEditorWindow
    {

        private readonly string m_ConfigurationPath = "Assets/GameMain/StreamRes/Configs/BuildPlayerOptionsOverview.asset";
        private BuildPlayerOptionsOverview m_Overview;
        private void LoadOrCreate()
        {
            if (m_Overview != null)
            {
                return;
            }
            m_Overview = AssetDatabaseUtils.LoadAssetAtPath<BuildPlayerOptionsOverview>(m_ConfigurationPath);
            if (m_Overview == null)
            {
                m_Overview = ScriptableObject.CreateInstance<BuildPlayerOptionsOverview>();
                AssetDatabase.CreateAsset(m_Overview, m_ConfigurationPath);
            }

        }


        [MenuItem("FallenWing/BuildTools/创建默认BuildPlayerOptions", false)]
        public static void CreateDefaultBuildPlayerOptionsEditor()
        {
            var editor = CreateInstance<BuildPlayerOptionsEditor>();
            editor.LoadOrCreate();
            editor.CreateDefaultBuildPlayerOptions();
        }

        public void CreateDefaultBuildPlayerOptions()
        {
            m_Overview.AllPlayerOptions.Clear();
            var options = new List<UnityEditor.BuildPlayerOptions>();
            var platforms = new List<Platform> { Platform.Android, Platform.Windows64, Platform.IOS };
            foreach (var platform in platforms)
            {
                PlatformUtils.SwitchPlatform(Platform.Android);
                var option = GetDefaultSetBuildPlayerOptions(BuildTarget.Android, BuildTargetGroup.Android);
                m_Overview.AllPlayerOptions.Add(new BuildPlayerOptionsOverview.BuildPlayerOptionsItem(option));
            }
            EditorUtility.SetDirty(m_Overview);
            AssetDatabase.SaveAssets();
        }

        public static UnityEditor.BuildPlayerOptions GetDefaultSetBuildPlayerOptions(BuildTarget buildTarget, BuildTargetGroup targetGroup, bool askForLocation = false,
BuildPlayerOptions defaultOptions = new BuildPlayerOptions())
        {
            EditorUserBuildSettings.selectedStandaloneTarget = buildTarget;
            EditorUserBuildSettings.selectedBuildTargetGroup = targetGroup;
            EditorUserBuildSettings.SetBuildLocation(buildTarget, "test");
            var method = typeof(BuildPlayerWindow.DefaultBuildMethods).GetMethod(
                "GetBuildPlayerOptionsInternal",
                BindingFlags.NonPublic | BindingFlags.Static);

            // invoke internal method
            if (method != null)
            {
                return (BuildPlayerOptions)method.Invoke(null, new object[] { askForLocation, defaultOptions });
            }

            return defaultOptions;
        }

        public UnityEditor.BuildPlayerOptions GetBuildPlayerOptions(BuildTargetGroup targetGroup)
        {
            return m_Overview.GetBuildPlayerOptions(targetGroup);
        }

        public static UnityEditor.BuildPlayerOptions GetBuildPlayerOptionsFromGameConfig(BuildTargetGroup targetGroup)
        {
            var editor = CreateInstance<BuildPlayerOptionsEditor>();
            editor.LoadOrCreate();
            editor.CreateDefaultBuildPlayerOptions();
            return editor.GetBuildPlayerOptions(targetGroup);
        }




    }
}