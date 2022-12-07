using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;


using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;

namespace FairyWay.Editor
{
    public class BuildToolEditor : OdinEditorWindow
    {

        [LabelText("选择平台")] [ValueDropdown("GetAllSupportPlatform")]
        public Platform SelectPlatform;

        private string m_LastBuildName;
        private string m_LastBuildVersion;

        [LabelText("构建平台名")] //[ValueDropdown("ServerPlatformDropdown")]
        public string ServerPlatform;



        
        [MenuItem("FallenWing/打包汇总", false, 6)]
        public static void ShowWindow()
        {
            if (SceneManager.GetActiveScene().name != "Client" && !EditorUtility.DisplayDialog("提示", "将要打开SteamClient场景", "确定", "取消"))
            {
                return;
            }

            SwitchMainScene();
            var window = GetWindow<BuildToolEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 700);
            window.titleContent = new GUIContent("打包面板");
            window.Show();
            window.SelectPlatform = window.GetCurrentPlatform();
            // window.InitGameFrameworkData();
        }


     


        public ValueDropdownList<Platform> GetAllSupportPlatform()
        {
            return new ValueDropdownList<Platform> { Platform.Android, Platform.Windows64, Platform.IOS};
        }


        private static void SwitchMainScene()
        {
            if (SceneManager.GetActiveScene().name != "Client")
            {
                EditorSceneManager.OpenScene("Assets/GameMain/Scenes/Client.unity");
            }
        }

        private Platform GetCurrentPlatform()
        {
            var platform = Platform.Windows64;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    platform = Platform.Android;
                    break;
                case BuildTarget.StandaloneWindows64:
                    platform = Platform.Windows64;
                    break;
                case BuildTarget.iOS:
                    platform = Platform.IOS;
                    break;
            }

            return platform;
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
            ServerPlatform= "local";
            m_LastBuildName = "Client" + m_LastBuildVersion + '_' + ServerPlatform + '_' + DateTime.Now.ToFileTime();
        }



        // private void BuildGamePackage(string path, bool isBat = false, bool buildAb = false)
        // {
        //     if (string.IsNullOrEmpty(path))
        //     {
        //         return;
        //     }

        //     BindSpriteAtlasAndCancelOverride();
        //     SwitchPlatform();
        //     SwitchMainScene();
        //     SetGameFrameworkData();
        //     SetSymbol();
        //     BindScript();
        //     CriFileSystemHelper.BuildCpk();
        //     var errorMsg = HotfixHelper.BuildHotfixReleaseDll();
        //     if (!string.IsNullOrEmpty(errorMsg))
        //     {
        //         m_FailureData = string.Empty;
        //         CollectFailureData(errorMsg);
        //         WriteFailureData();
        //         throw new Exception("Build dll failed");
        //     }

        //     AssetDatabase.Refresh();
        //     if (!BuildResourceRule())
        //     {
        //         throw new Exception("Build resource failed");
        //     }

        //     BuildAssetBundlesByResourceBuilder();
        //     if (buildAb)
        //     {
        //         WriteAbInfo();
        //         return;
        //     }

        //     BuildGame(path, isBat);
        // }

    }
}
