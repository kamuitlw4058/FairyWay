using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public static class PlatformUtils
    {
        public static Platform GetCurrentPlatform()
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

        public static void SwitchPlatform(Platform platform)
        {
            if (platform == GetCurrentPlatform())
            {
                return;
            }

            switch (platform)
            {
                case Platform.Android:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    break;
                case Platform.Windows64:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    break;
                case Platform.MacOS:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
                    break;
                case Platform.IOS:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                    break;
            }
        }


        public static ValueDropdownList<Platform> GetAllSupportPlatform()
        {
            return new ValueDropdownList<Platform> { Platform.Android, Platform.Windows64, Platform.IOS };
        }


        public static void SwitchMainScene()
        {
            if (SceneManager.GetActiveScene().name != "Client")
            {
                EditorSceneManager.OpenScene($"{CommonConstant.SCENE_DIR}/Client.unity");
            }
        }
    }
}