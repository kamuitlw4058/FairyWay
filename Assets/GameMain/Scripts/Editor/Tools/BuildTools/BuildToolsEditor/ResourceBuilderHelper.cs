using System;
using System.IO;
using System.Linq;
using GameFramework.Resource;
using Sirenix.Utilities;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;

namespace FairyWay.Editor
{
    public class ResourceBuilderHelper
    {
        public static bool BuildAssetBundles(Platform platform, ResourceMode resourceMode)
        {
            var controller = new ResourceBuilderController();
            if (!controller.Load())
            {
                Log.Info($"Controller Load Failed!!");
                return false;
            }

            if (controller.OutputDirectory == null)
            {
                Log.Info($"Controller OutputDirectory is null.");
                return false;
            }

            if (!Directory.Exists(controller.OutputDirectory))
            {
                Log.Info($"Controller OutputDirectory is not Exists.");
                return false;
            }

            controller.Platforms = platform;
            switch (resourceMode)
            {
                case ResourceMode.Package:
                    controller.OutputPackageSelected = true;
                    controller.OutputFullSelected = false;
                    controller.OutputPackedSelected = false;
                    break;
                case ResourceMode.Updatable:
                    controller.OutputPackageSelected = false;
                    controller.OutputFullSelected = true;
                    controller.OutputPackedSelected = true;
                    break;
            }

            controller.RefreshCompressionHelper();
            controller.RefreshBuildEventHandler();
            if (controller.BuildResources())
            {
                controller.Save();
            }
            else
            {
                Log.Info($"Controller BuildResources Failed!");
                return false;
            }

            return true;
        }
    }
}
