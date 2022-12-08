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
    public static class PlatformUtils
    {
        public static ValueDropdownList<Platform> GetAllSupportPlatform()
        {
            return new ValueDropdownList<Platform> { Platform.Android, Platform.Windows64, Platform.IOS};
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