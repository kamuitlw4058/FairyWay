using System;
using System.IO;
using System.Linq;
using GameFramework.Resource;
using Sirenix.Utilities;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace FairyWay.Editor
{
    public class ResourceBuilderHelper
    {
        public static bool BuildAssetBundles(Platform platform, ResourceMode resourceMode)
        {
            var controller = new ResourceBuilderController();
            if (!controller.Load() || controller.OutputDirectory == null || !Directory.Exists(controller.OutputDirectory))
            {
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
                return false;
            }

            return true;
        }
    }
}
