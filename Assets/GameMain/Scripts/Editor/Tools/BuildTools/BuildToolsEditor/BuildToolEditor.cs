using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FairyWay.Editor.ResourceTools;
using FairyWay.Main;
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
    public class BuildToolEditor : OdinEditorWindow
    {

        [LabelText("选择平台")]
        [ValueDropdown("GetAllSupportPlatform")]
        public Platform SelectPlatform;

        [LabelText("资源模式")]
        [ValueDropdown("ResourceModeDropdown")]
        public ResourceMode ResourceMode;

        private string m_LastBuildName;
        private string m_LastBuildVersion;

        [LabelText("构建平台名")] //[ValueDropdown("ServerPlatformDropdown")]
        public string ServerPlatform;

        public ValueDropdownList<ResourceMode> ResourceModeDropdown()
        {
            return new ValueDropdownList<ResourceMode> { ResourceMode.Package, ResourceMode.Updatable };
        }




        [MenuItem("FallenWing/BuildTools/打包汇总", false, 6)]
        public static void ShowWindow()
        {
            if (SceneManager.GetActiveScene().name != "Client" && !EditorUtility.DisplayDialog("提示", "将要打开SteamClient场景", "确定", "取消"))
            {
                return;
            }

            PlatformUtils.SwitchMainScene();
            var window = GetWindow<BuildToolEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 700);
            window.titleContent = new GUIContent("打包面板");
            window.Show();
            window.SelectPlatform = PlatformUtils.GetCurrentPlatform();
            window.InitGameFrameworkData();
        }


        public ValueDropdownList<Platform> GetAllSupportPlatform()
        {
            return PlatformUtils.GetAllSupportPlatform();
        }



        [Button("一键打包", ButtonSizes.Gigantic)]
        public void BuildAll()
        {
            var path = GetBuildPath();
            Debug.Log($"Build Path:{path}");
            // BuildGamePackage(GetBuildPath());
        }

        private string GetBuildPath()
        {
            var key = $"Build{SelectPlatform}Path";
            var path = PlayerPrefs.GetString(key);
            var folder = string.Empty;
            var selectDir = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                switch (SelectPlatform)
                {
                    case Platform.Windows64:
                        folder = path;
                        break;
                    case Platform.Android:
                        folder = Path.GetDirectoryName(path);
                        break;
                    case Platform.IOS:
                        folder = path;
                        break;
                }
            }

            // SetBuildInfo();
            switch (SelectPlatform)
            {
                case Platform.Android:
                    path = EditorUtility.SaveFilePanel("生成游戏目录", folder, m_LastBuildName, "apk");
                    break;
                case Platform.Windows64:
                    selectDir = EditorUtility.SaveFolderPanel("生成游戏目录", folder, "");
                    path = string.IsNullOrEmpty(selectDir) ? string.Empty : $"{selectDir}/{m_LastBuildName}/{PlayerSettings.productName}.exe";
                    break;
                case Platform.IOS:
                    selectDir = EditorUtility.SaveFolderPanel("生成XCode目录", folder, "");
                    path = selectDir;
                    break;
            }

            if (!string.IsNullOrEmpty(path))
            {
                switch (SelectPlatform)
                {
                    case Platform.Windows64:
                        PlayerPrefs.SetString(key, selectDir);
                        break;
                    case Platform.Android:
                        PlayerPrefs.SetString(key, path);
                        break;
                    case Platform.IOS:
                        PlayerPrefs.SetString(key, path);
                        break;
                }

                PlayerPrefs.Save();
            }

            return path;
        }


        private void SetBuildInfo()
        {
            // m_LastBuildVersion = PlayerSettings.bundleVersion.Replace('.', '_') + "_" +
            //                      ResourceBuilderHelper.GetResourceBuilderController().InternalResourceVersion;
            m_LastBuildVersion = "1";
            ServerPlatform = "local";
            m_LastBuildName = "Client" + m_LastBuildVersion + '_' + ServerPlatform + '_' + DateTime.Now.ToFileTime();
        }




        private void BuildGamePackage(string path, bool isBat = false, bool buildAb = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // BindSpriteAtlasAndCancelOverride();
            PlatformUtils.SwitchPlatform(SelectPlatform);
            PlatformUtils.SwitchMainScene();
            SetGameFrameworkData();
            // SetSymbol();
            BindScript();
            //  也没有使用。
            // CriFileSystemHelper.BuildCpk();

            // 热更部分生成。
            // var errorMsg = HotfixHelper.BuildHotfixReleaseDll();
            // if (!string.IsNullOrEmpty(errorMsg))
            // {
            //     m_FailureData = string.Empty;
            //     CollectFailureData(errorMsg);
            //     WriteFailureData();
            //     throw new Exception("Build dll failed");
            // }

            AssetDatabase.Refresh();
            if (!BuildResourceRule())
            {
                throw new Exception("Build resource failed");
            }

            // BuildAssetBundlesByResourceBuilder();
            // if (buildAb)
            // {
            //     WriteAbInfo();
            //     return;
            // }



            BuildGame(path, isBat);
        }


        // 从场景中获取到服务器相关的信息。
        private void InitGameFrameworkData()
        {
            var scene = SceneManager.GetActiveScene();
            var gameEntry = scene.GetRootGameObjects().ToList().Find(g => g.name == "GFEntry");
            // var builtinData = gameEntry.transform.Find("Customs/BuiltinData").GetComponent<BuiltinDataComponent>();
            var resource = gameEntry.transform.Find("GameFramework/Resource").GetComponent<ResourceComponent>();
            // ServerURL = builtinData.ServerUrl;
            // ServerPlatform = builtinData.BuiltinPlatform;
            // BuildWith37 = HasSymbolInPlatform(SelectPlatform, SYMBOL_ANDROID_37);
            ResourceMode = resource.m_ResourceMode;
        }

        // 设置服务器相关信息并保存。
        private void SetGameFrameworkData()
        {
            // var scene = SceneManager.GetActiveScene();
            // var gameEntry = scene.GetRootGameObjects().ToList().Find(g => g.name == "GameEntry");
            // var builtinData = gameEntry.transform.Find("Customs/BuiltinData").GetComponent<BuiltinDataComponent>();
            // builtinData.ServerUrl = ServerURL;
            // builtinData.BuiltinPlatform = ServerPlatform;
            // var resource = gameEntry.transform.Find("GameFramework/Resource").GetComponent<ResourceComponent>();
            // resource.m_ResourceMode = ResourceMode;
            // EditorUtility.SetDirty(resource);
            // EditorSceneManager.SaveScene(scene);
        }


        // 热更域的更新。
        private void BindScript()
        {
            var path = "Assets/GameMain/Scripts/Main/Custom/ILRuntime/Generated";
            //用新的分析热更dll调用引用来生成绑定代码
            // var appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            // using (var fs = new FileStream(AssetUtility.GetHotfixDLLEditorAsset(), FileMode.Open, FileAccess.Read))
            // {
            //     appDomain.LoadAssembly(fs);

            //     //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            //     ILRuntimeHelper.RegisterCrossBindingAdaptor(appDomain);
            //     ILRuntimeHelper.RegisterValueTypeBinder(appDomain);
            //     ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appDomain, path);
            // }

            // AssetDatabase.Refresh();
        }


        // private void CollectFailureData(string msg)
        // {
        //     m_FailureData = string.IsNullOrEmpty(m_FailureData) ? msg : $"{m_FailureData} {msg} ";
        // }


        private bool BuildResourceRule()
        {
            var resourceRuleEditor = CreateInstance<ResourceRuleEditor>();
            // m_FailureData = string.Empty;
            resourceRuleEditor.RefreshResourceCollection();
            // WriteFailureData();
            // if (!string.IsNullOrEmpty(m_FailureData))
            // {
            //     return false;
            // }

            resourceRuleEditor.Save();
            DestroyImmediate(resourceRuleEditor);
            return true;
        }

        [Button("打资源包", ButtonSizes.Gigantic)]
        public void BuildAssetBundlesByResourceBuilder()
        {
            if (ResourceBuilderHelper.BuildAssetBundles(SelectPlatform, ResourceMode))
            {
                Debug.Log("打包完成");
            }
            else
            {
                throw new Exception("ab包打包失败，检查配置");
            }
        }


        private void BuildGame(string path, bool isBat = false)
        {
            var targetGroup = BuildTargetGroup.Unknown;
            switch (SelectPlatform)
            {
                case Platform.Android:
                    // PlayerSettings.keystorePass = m_BuildKeystorePassword;
                    // PlayerSettings.keyaliasPass = m_BuildKeystorePassword;
                    targetGroup = BuildTargetGroup.Android;
                    break;
                case Platform.Windows64:
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case Platform.IOS:
                    targetGroup = BuildTargetGroup.iOS;
                    break;
            }

            var option = GetBuildPlayerOptionsFromGameConfig(targetGroup);
            option.locationPathName = path;
            try
            {
                BuildPipeline.BuildPlayer(option);
            }
            catch
            {
                Debug.LogError("生成游戏包出错");
                return;
            }

            // if (!isBat)
            // {
            //     var dirPath = Path.GetDirectoryName(path);
            //     if (!string.IsNullOrEmpty(dirPath))
            //     {
            //         var proc = new Process { StartInfo = { FileName = dirPath } };
            //         proc.Start();
            //     }
            // }
            // else
            // {
            //     WriteApkInfo();
            // }

            // UpdateBuildVersion();
        }


        UnityEditor.BuildPlayerOptions GetBuildPlayerOptionsFromGameConfig(BuildTargetGroup targetGroup)
        {
            return AssetDatabase.FindAssets("t:BuildPlayerOptionsOverview")
                .Select(guid => AssetDatabase.LoadAssetAtPath<BuildPlayerOptionsOverview>(AssetDatabase.GUIDToAssetPath(guid))).ToList()[0]
                .GetBuildPlayerOptions(targetGroup);
        }


    }
}

