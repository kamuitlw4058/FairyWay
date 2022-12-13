using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using GFResource = UnityGameFramework.Editor.ResourceTools.Resource;
using Object = UnityEngine.Object;

namespace FairyWay.Editor.ResourceTools
{
    /// <summary>
    /// Resource 规则编辑器，支持按规则配置自动生成 ResourceCollection.xml
    /// </summary>
    public class ResourceRuleEditor : OdinEditorWindow
    {
        private readonly string m_ConfigurationPath = "Assets/GameMain/StreamRes/Configs/ResourceRuleEditor.asset";
        private ResourceRuleEditorData m_Configuration;
        private ResourceCollection m_ResourceCollection;

        private ReorderableList m_RuleList;
        private Vector2 m_ScrollPosition = Vector2.zero;

        private string m_SourceAssetExceptTypeFilter = "t:Script";
        private string[] m_SourceAssetExceptTypeFilterGUIDArray;

        private string m_SourceAssetExceptLabelFilter = "l:ResourceExclusive";
        private string[] m_SourceAssetExceptLabelFilterGUIDArray;

        private Action<string> RefreshResourceCallback;

        // [MenuItem("Game Framework/Resource Tools/Resource Rule Editor", false, 50)]
        // static void Open()
        // {
        //     ResourceRuleEditor window = GetWindow<ResourceRuleEditor>(true, "Resource Rule Editor", true);
        //     window.minSize = new Vector2(1555f, 420f);
        // }

        [MenuItem("FallenWing/BuildTools/Resource Rule Editor", false, 50)]
        public static void OpenWindow()
        {
            var window = GetWindow<ResourceRuleEditor>();
            window.minSize = new Vector2(1555f, 420f);
            window.titleContent = new GUIContent("资源规则编辑器");
        }

        protected override void OnGUI()
        {
            if (m_Configuration == null)
            {
                Load();
            }

            if (m_RuleList == null)
            {
                InitRuleListDrawer();
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("添加规则", EditorStyles.toolbarButton))
                {
                    Add();
                }

                if (GUILayout.Button("保存", EditorStyles.toolbarButton))
                {
                    Save();
                }

                if (GUILayout.Button("刷新 ResourceCollection.xml", EditorStyles.toolbarButton))
                {
                    RefreshResourceCollection();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                OnListElementLabelGUI();
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginVertical();
            {
                GUILayout.Space(30);

                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
                {
                    m_RuleList.DoLayoutList();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            if (GUI.changed)
                EditorUtility.SetDirty(m_Configuration);
        }

        private void Load()
        {
            m_Configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_ConfigurationPath);
            if (m_Configuration == null)
            {
                m_Configuration = ScriptableObject.CreateInstance<ResourceRuleEditorData>();
            }
        }

        private T LoadAssetAtPath<T>(string path) where T : Object
        {
#if UNITY_5
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
#endif
        }

        private void InitRuleListDrawer()
        {
            m_RuleList = new ReorderableList(m_Configuration.rules, typeof(ResourceRule));
            m_RuleList.drawElementCallback = OnListElementGUI;
            m_RuleList.drawHeaderCallback = OnListHeaderGUI;
            m_RuleList.draggable = true;
            m_RuleList.elementHeight = 22;
            m_RuleList.onAddCallback = (list) => Add();
        }

        private void Add()
        {
            string path = SelectFolder();
            if (!string.IsNullOrEmpty(path))
            {
                var rule = new ResourceRule();
                rule.assetsDirectoryPath = path;
                m_Configuration.rules.Add(rule);
            }
        }

        private void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;

            ResourceRule rule = m_Configuration.rules[index];
            rect.y++;

            Rect r = rect;
            r.width = 16;
            r.height = 18;
            rule.valid = EditorGUI.Toggle(r, rule.valid);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMax + 425;
            float assetBundleNameLength = r.width;
            rule.name = EditorGUI.TextField(r, rule.name);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 100;
            rule.loadType = (LoadType)EditorGUI.EnumPopup(r, rule.loadType);

            r.xMin = r.xMax + GAP + 15;
            r.xMax = r.xMin + 30;
            rule.packed = EditorGUI.Toggle(r, rule.packed);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.fileSystem = EditorGUI.TextField(r, rule.fileSystem);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.groups = EditorGUI.TextField(r, rule.groups);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.variant = EditorGUI.TextField(r, rule.variant);
            if (!string.IsNullOrEmpty(rule.variant))
            {
                rule.variant = rule.variant.ToLower();
            }

            r.xMin = r.xMax + GAP;
            r.width = assetBundleNameLength - 15;
            GUI.enabled = false;
            rule.assetsDirectoryPath = EditorGUI.TextField(r, rule.assetsDirectoryPath);
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                var path = SelectFolder();
                if (path != null)
                    rule.assetsDirectoryPath = path;
            }

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.filterType = (ResourceFilterType)EditorGUI.EnumPopup(r, rule.filterType);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            rule.searchPatterns = EditorGUI.TextField(r, rule.searchPatterns);
        }

        private string SelectFolder()
        {
            string dataPath = Application.dataPath;
            string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(dataPath))
                {
                    return "Assets/" + selectedPath.Substring(dataPath.Length + 1);
                }
                else
                {
                    ShowNotification(new GUIContent("Can not be outside of 'Assets/'!"), 2);
                }
            }

            return null;
        }

        private void OnListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "Rules");
        }

        private void OnListElementLabelGUI()
        {
            Rect rect = new Rect();
            const float GAP = 5;
            GUI.enabled = false;

            Rect r = new Rect(0, 20, rect.width, rect.height);
            r.width = 45;
            r.height = 18;
            EditorGUI.TextField(r, "Active");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMax + 415;
            float assetBundleNameLength = r.width;
            EditorGUI.TextField(r, "Name");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 100;
            EditorGUI.TextField(r, "Load Type");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 50;
            EditorGUI.TextField(r, "Packed");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "File System");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "Groups");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "Variant");

            r.xMin = r.xMax + GAP;
            r.width = assetBundleNameLength + 50;
            EditorGUI.TextField(r, "AssetDirectory");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "Filter Type");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 250;
            EditorGUI.TextField(r, "Patterns");
            GUI.enabled = true;
        }

        public void Save()
        {
            if (LoadAssetAtPath<ResourceRuleEditorData>(m_ConfigurationPath) == null)
            {
                AssetDatabase.CreateAsset(m_Configuration, m_ConfigurationPath);
            }
            else
            {
                EditorUtility.SetDirty(m_Configuration);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region Refresh ResourceCollection.xml

        public bool RefreshResourceCollection(Action<string> callback = null)
        {
            RefreshResourceCallback = callback;
            if (m_Configuration == null)
            {
                Load();
            }

            m_SourceAssetExceptTypeFilterGUIDArray = AssetDatabase.FindAssets(m_SourceAssetExceptTypeFilter);
            m_SourceAssetExceptLabelFilterGUIDArray = AssetDatabase.FindAssets(m_SourceAssetExceptLabelFilter);
            AnalysisResourceFilters();
            if (SaveCollection())
            {
                Debug.Log("Refresh ResourceCollection.xml success");
                return true;
            }
            else
            {
                Debug.Log("Refresh ResourceCollection.xml fail");
                return false;
            }
        }

        private GFResource[] GetResources()
        {
            return m_ResourceCollection.GetResources();
        }

        private bool HasResource(string name, string variant)
        {
            return m_ResourceCollection.HasResource(name, variant);
        }

        private bool AddResource(string name, string variant, string fileSystem,
            LoadType loadType, bool packed, string[] resourceGroups)
        {
            return m_ResourceCollection.AddResource(name, variant, fileSystem, loadType,
                packed, resourceGroups);
        }

        private bool RenameResource(string oldName, string oldVariant,
            string newName, string newVariant)
        {
            return m_ResourceCollection.RenameResource(oldName, oldVariant,
                newName, newVariant);
        }

        private bool AssignAsset(string assetGuid, string resourceName, string resourceVariant)
        {
            if (m_ResourceCollection.AssignAsset(assetGuid, resourceName, resourceVariant))
            {
                return true;
            }
            else
            {
                Debug.LogError($"AssignAsset出错，请检查原因！resourceName:{resourceName}");
                RefreshResourceCallback?.Invoke(resourceName);
            }

            return false;
        }

        private void AnalysisResourceFilters()
        {
            m_ResourceCollection = new ResourceCollection();
            List<string> signedAssetBundleList = new List<string>();

            foreach (ResourceRule resourceRule in m_Configuration.rules)
            {
                if (resourceRule.variant == "")
                    resourceRule.variant = null;

                if (resourceRule.valid)
                {
                    switch (resourceRule.filterType)
                    {
                        case ResourceFilterType.Root:
                            {
                                if (string.IsNullOrEmpty(resourceRule.name))
                                {
                                    string relativeDirectoryName =
                                        resourceRule.assetsDirectoryPath.Replace("Assets/", "");
                                    ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                        Utility.Path.GetRegularPath(relativeDirectoryName));
                                }
                                else
                                {
                                    ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                        resourceRule.name);
                                }
                            }
                            break;

                        case ResourceFilterType.Children:
                            {
                                string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                                for (int i = 0; i < patterns.Length; i++)
                                {
                                    FileInfo[] assetFiles =
                                        new DirectoryInfo(resourceRule.assetsDirectoryPath).GetFiles(patterns[i],
                                            SearchOption.AllDirectories);
                                    foreach (FileInfo file in assetFiles)
                                    {
                                        if (file.Name.Contains("DS_Store"))
                                        {
                                            continue;
                                        }

                                        if (CheckFileIgnore(file))
                                            continue;

                                        string relativeAssetName = file.FullName.Substring(Application.dataPath.Length + 1);
                                        string relativeAssetNameWithoutExtension =
                                            Utility.Path.GetRegularPath(
                                                relativeAssetName.Substring(0, relativeAssetName.IndexOf('.')));

                                        string assetName = Path.Combine("Assets", relativeAssetName);
                                        string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                                        if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                                        {
                                            ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                                relativeAssetNameWithoutExtension, assetGUID);
                                        }
                                    }
                                }
                            }
                            break;

                        case ResourceFilterType.ChildrenFoldersOnly:
                            {
                                DirectoryInfo[] assetDirectories =
                                    new DirectoryInfo(resourceRule.assetsDirectoryPath).GetDirectories();
                                foreach (DirectoryInfo directory in assetDirectories)
                                {
                                    string relativeDirectoryName =
                                        directory.FullName.Substring(Application.dataPath.Length + 1);

                                    ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                        Utility.Path.GetRegularPath(relativeDirectoryName), string.Empty,
                                        directory.FullName);
                                }
                            }
                            break;

                        case ResourceFilterType.ChildrenFilesOnly:
                            {
                                DirectoryInfo[] assetDirectories =
                                    new DirectoryInfo(resourceRule.assetsDirectoryPath).GetDirectories();
                                foreach (DirectoryInfo directory in assetDirectories)
                                {
                                    string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                                    for (int i = 0; i < patterns.Length; i++)
                                    {
                                        FileInfo[] assetFiles =
                                            new DirectoryInfo(directory.FullName).GetFiles(patterns[i],
                                                SearchOption.AllDirectories);
                                        foreach (FileInfo file in assetFiles)
                                        {
                                            if (file.Name.Contains("DS_Store"))
                                            {
                                                continue;
                                            }

                                            if (CheckFileIgnore(file))
                                                continue;

                                            string relativeAssetName =
                                                file.FullName.Substring(Application.dataPath.Length + 1);
                                            string relativeAssetNameWithoutExtension =
                                                Utility.Path.GetRegularPath(
                                                    relativeAssetName.Substring(0, relativeAssetName.IndexOf('.')));

                                            string assetName = Path.Combine("Assets", relativeAssetName);
                                            string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                                            if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                                            {
                                                ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                                    relativeAssetNameWithoutExtension, assetGUID);
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case ResourceFilterType.TopFilesOnly:
                            {
                                var directory = new DirectoryInfo(resourceRule.assetsDirectoryPath);
                                string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                                for (int i = 0; i < patterns.Length; i++)
                                {
                                    FileInfo[] assetFiles =
                                        new DirectoryInfo(directory.FullName).GetFiles(patterns[i],
                                            SearchOption.TopDirectoryOnly);
                                    foreach (FileInfo file in assetFiles)
                                    {
                                        if (file.Name.Contains("DS_Store"))
                                        {
                                            continue;
                                        }

                                        if (CheckFileIgnore(file))
                                            continue;

                                        string relativeAssetName =
                                            file.FullName.Substring(Application.dataPath.Length + 1);
                                        string relativeAssetNameWithoutExtension =
                                            Utility.Path.GetRegularPath(
                                                relativeAssetName.Substring(0, relativeAssetName.IndexOf('.')));

                                        string assetName = Path.Combine("Assets", relativeAssetName);
                                        string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                                        if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                                        {
                                            ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                                relativeAssetNameWithoutExtension, assetGUID);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void ApplyResourceFilter(ref List<string> signedResourceList, ResourceRule resourceRule,
            string resourceName, string singleAssetGUID = "", string childDirectoryPath = "")
        {
            if (!signedResourceList.Contains(Path.Combine(resourceRule.assetsDirectoryPath, resourceName)))
            {
                signedResourceList.Add(Path.Combine(resourceRule.assetsDirectoryPath, resourceName));

                foreach (GFResource oldResource in GetResources())
                {
                    if (oldResource.Name == resourceName)
                    {
                        RenameResource(oldResource.Name, oldResource.Variant,
                            resourceName, resourceRule.variant);
                        break;
                    }
                }

                if (!HasResource(resourceName, resourceRule.variant))
                {

                    if (string.IsNullOrEmpty(resourceRule.fileSystem))
                    {
                        resourceRule.fileSystem = null;
                    }

                    AddResource(resourceName, resourceRule.variant, resourceRule.fileSystem,
                        resourceRule.loadType, resourceRule.packed,
                        resourceRule.groups.Split(';', ',', '|'));
                }

                switch (resourceRule.filterType)
                {
                    case ResourceFilterType.Root:
                    case ResourceFilterType.ChildrenFoldersOnly:
                        string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                        if (childDirectoryPath == "")
                        {
                            childDirectoryPath = resourceRule.assetsDirectoryPath;
                        }

                        for (int i = 0; i < patterns.Length; i++)
                        {
                            FileInfo[] assetFiles =
                                new DirectoryInfo(childDirectoryPath).GetFiles(patterns[i],
                                    SearchOption.AllDirectories);
                            foreach (FileInfo file in assetFiles)
                            {
                                if (file.Name.Contains("DS_Store"))
                                {
                                    continue;
                                }

                                if (CheckFileIgnore(file))
                                    continue;

                                string assetName = Path.Combine("Assets",
                                    file.FullName.Substring(Application.dataPath.Length + 1));

                                string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                                if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                                {
                                    AssignAsset(assetGUID, resourceName,
                                        resourceRule.variant);
                                }
                            }
                        }

                        break;

                    case ResourceFilterType.Children:
                    case ResourceFilterType.ChildrenFilesOnly:
                    case ResourceFilterType.TopFilesOnly:
                        {
                            AssignAsset(singleAssetGUID, resourceName,
                                    resourceRule.variant);
                        }
                        break;
                }
            }
        }

        private bool SaveCollection()
        {
            return m_ResourceCollection.Save();
        }

        private static bool CheckFileIgnore(FileInfo file)
        {
            if (file.Extension.Contains("meta"))
                return true;
            else if (file.Name.StartsWith(".git"))
                return true;

            return false;
        }

        #endregion
    }
}