using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace FairyWay.Editor
{
    [CreateAssetMenu(fileName = "BuildPlayerOptionsOverview", menuName = "FallenWing/BuildTools/BuildPlayerOptionsOverview", order = 1)]
    public class BuildPlayerOptionsOverview : ScriptableObject
    {
        // [ReadOnly]
        public List<BuildPlayerOptionsItem> AllPlayerOptions;

        public UnityEditor.BuildPlayerOptions GetBuildPlayerOptions(BuildTargetGroup buildTargetGroup)
        {
            return AllPlayerOptions.Find(options => options.TargetGroup == buildTargetGroup).ConvertToUnityBuildPlayerOptions();
        }

        [Serializable]
        public class BuildPlayerOptionsItem
        {
            /// <summary>
            ///   <para>The Scenes to be included in the build. If empty, the currently open Scene will be built. Paths are relative to the project folder (AssetsMyLevelsMyScene.unity).</para>
            /// </summary>
            public string[] Scenes;

            /// <summary>
            ///   <para>The path where the application will be built.</para>
            /// </summary>
            public string LocationPathName;

            /// <summary>
            ///   <para>The path to an manifest file describing all of the asset bundles used in the build (optional).</para>
            /// </summary>
            public string AssetBundleManifestPath;

            /// <summary>
            ///   <para>The BuildTargetGroup to build.</para>
            /// </summary>
            public BuildTargetGroup TargetGroup;

            /// <summary>
            ///   <para>The BuildTarget to build.</para>
            /// </summary>
            public BuildTarget Target;

            /// <summary>
            ///   <para>Additional BuildOptions, like whether to run the built player.</para>
            /// </summary>
            public BuildOptions Options;

            /// <summary>
            ///   <para>User-specified preprocessor defines used while compiling assemblies for the player.</para>
            /// </summary>
            public string[] ExtraScriptingDefines;

            public BuildPlayerOptionsItem(UnityEditor.BuildPlayerOptions buildPlayerOptions)
            {
                Scenes = buildPlayerOptions.scenes;
                LocationPathName = buildPlayerOptions.locationPathName;
                AssetBundleManifestPath = buildPlayerOptions.assetBundleManifestPath;
                TargetGroup = buildPlayerOptions.targetGroup;
                Target = buildPlayerOptions.target;
                Options = buildPlayerOptions.options;
                ExtraScriptingDefines = buildPlayerOptions.extraScriptingDefines;
            }

            public UnityEditor.BuildPlayerOptions ConvertToUnityBuildPlayerOptions()
            {
                return new UnityEditor.BuildPlayerOptions
                {
                    scenes = Scenes,
                    locationPathName = LocationPathName,
                    assetBundleManifestPath = AssetBundleManifestPath,
                    targetGroup = TargetGroup,
                    target = Target,
                    options = Options,
                    extraScriptingDefines = ExtraScriptingDefines
                };
            }
        }
    }
}