using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.IO;

namespace Plugins
{
    public class CustomPackageManager : EditorWindow
    {
        const string editorPrefKey = "CustomPackages/CustomPackageManager";

        string localPath;

        List<PackageData> packageList;
        int selectedPackage;
        bool edit = false;

        [MenuItem("Plugins/Package Importer")]
        public static void ShowWindow()
        {
            CustomPackageManager wnd = GetWindow<CustomPackageManager>();

            wnd.titleContent = new GUIContent("Package Importer");
            wnd.minSize = new Vector2(600, 250);

            wnd.Show();
        }

        public void OnEnable()
        {
            edit = false;

            VisualElement root = rootVisualElement;

            GetLocalPath();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path.Combine(localPath, "CustomPackageManager.uxml"));
            VisualElement uxmlRoot = visualTree.CloneTree();
            root.Add(uxmlRoot);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(localPath, "CustomPackageManager.uss"));
            root.styleSheets.Add(styleSheet);

            var darkThemeStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(localPath, "DarkTheme.uss"));
            root.styleSheets.Add(darkThemeStyleSheet);

            SelectPackage(0);
            RefreshPackages();
            SetupControls();

            ShowViewPanel();
        }

        private void OnGUI()
        {
            rootVisualElement.Q<VisualElement>("panelContainer").style.height = new
                StyleLength(position.height);
        }

        void SetupControls()
        {
            Button addButton = rootVisualElement.Q<Button>("addButton");

            addButton.clickable.clicked += () =>
            {
                string path = EditorUtility.OpenFilePanelWithFilters("Add package", "", new string[] { "UnityPackage", "unitypackage" });
                packageList.Add(new PackageData() { pathToPackage = path, name = Path.GetFileName(path), info = "Description" });
                SavePackageList();
                RefreshPackages();
            };

            Button editButton = rootVisualElement.Q<Button>("editButton");

            editButton.clickable.clicked += () =>
            {
                ToggleEdit();
                SavePackageList();
                RefreshPackages();
            };

            Button removeButton = rootVisualElement.Q<Button>("removeButton");

            removeButton.clickable.clicked += () =>
            {
                packageList.RemoveAt(selectedPackage);
                if (selectedPackage > 0) selectedPackage--;
                SavePackageList();
                ToggleEdit();
                RefreshPackages();
            };

            Button importButton = rootVisualElement.Q<Button>("importButton");

            importButton.clickable.clicked += () =>
            {
                ImportPackage();
            };

            TextField searchField = rootVisualElement.Q<TextField>("searchBar");
            searchField.RegisterCallback<KeyDownEvent>(e => 
            { 
                if (e.keyCode == KeyCode.Return)
                {
                    PopulatePackageList(searchField.value);
                }
            });
        }

        void RefreshPackages()
        {
            LoadPackageList();
            PopulatePackageList();
            UpdateRightPanel();
        }

        void PopulatePackageList(string search = null)
        {
            ListView packageListView = rootVisualElement.Q<ListView>("listView");
            packageListView.Clear();

            for (int i = 0; i < packageList.Count; i++)
            {
                int elementID = i;

                if (search != null)
                {
                    if (!packageList[i].name.ToLower().StartsWith(search)) continue;
                }

                Box listContainer = new Box();
                listContainer.name = "listItem";
                listContainer.style.height = 30;
                listContainer.AddManipulator(new Clickable(() =>
                {
                    SelectPackage(elementID);
                    PopulatePackageList();
                    UpdateRightPanel();
                }));

                Label listLabel = new Label(packageList[i].name);
                listLabel.style.unityTextAlign = TextAnchor.LowerLeft;
                listLabel.style.marginLeft = 3;
                listLabel.style.height = 23;

                Box separator = new Box();
                separator.style.width = new StyleLength(StyleKeyword.Auto);
                separator.style.height = 2;
                separator.style.bottom = 0;
                separator.style.marginTop = new StyleLength(StyleKeyword.Auto);
                separator.AddToClassList("separator");

                listContainer.Add(listLabel);
                listContainer.Add(separator);

                packageListView.Insert(packageListView.childCount, listContainer);

                if (elementID == selectedPackage)
                {
                    listContainer.style.backgroundColor = new StyleColor(new Color32(48, 48, 48, 255));
                }
            }
        }

        void LoadPackageList()
        {
            if (!EditorPrefs.HasKey(editorPrefKey))
            {
                SavePackageList();
            }

            string json = EditorPrefs.GetString(editorPrefKey);
            var wrapper = JsonUtility.FromJson<PackageListWrapper>(json);
            packageList = wrapper.packageList;
        }

        void SavePackageList()
        {
            if (packageList == null)
                packageList = new List<PackageData>();

            var wrapper = new PackageListWrapper() { packageList = packageList };
            string json = JsonUtility.ToJson(wrapper);
            EditorPrefs.SetString(editorPrefKey, json);
        }

        void GetLocalPath()
        {
            MonoScript myScript = MonoScript.FromScriptableObject(this);
            localPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(myScript));
        }

        void SelectPackage(int elementID)
        {
            selectedPackage = elementID;
        }

        void ImportPackage()
        {
            PackageData package = packageList[selectedPackage];

            if (File.Exists(package.pathToPackage))
            {
                AssetDatabase.ImportPackage(package.pathToPackage, true);
            }
        }

        void UpdateRightPanel()
        {
            if (selectedPackage >= packageList.Count) return;

            var nameLabel = rootVisualElement.Q<Label>("packageName");
            nameLabel.text = packageList[selectedPackage].name;

            var infoLabel = rootVisualElement.Q<Label>("packageInfo");
            infoLabel.text = packageList[selectedPackage].info;
        }

        void ToggleEdit()
        {
            edit = !edit;
            if (edit) EditSelectedPackage();
            else FinishEdit();

            Button editButton = rootVisualElement.Q<Button>("editButton");
            if (edit) editButton.text = "Save";
            else editButton.text = "Edit";
        }

        public void EditSelectedPackage()
        {
            ShowEditPanel();

            var nameField = rootVisualElement.Q<TextField>("packageNameField");
            nameField.value = packageList[selectedPackage].name;

            var infoField = rootVisualElement.Q<TextField>("packageInfoField");
            infoField.value = packageList[selectedPackage].info;

            var pathField = rootVisualElement.Q<TextField>("packagePathField");
            pathField.value = packageList[selectedPackage].pathToPackage;
        }

        public void FinishEdit()
        {
            ShowViewPanel();

            PackageData package = packageList[selectedPackage];

            var nameField = rootVisualElement.Q<TextField>("packageNameField");
            package.name = nameField.value;

            var infoField = rootVisualElement.Q<TextField>("packageInfoField");
            package.info = infoField.value;

            var pathField = rootVisualElement.Q<TextField>("packagePathField");
            package.pathToPackage = pathField.value;

            packageList[selectedPackage] = package;
        }

        void ShowViewPanel()
        {
            var viewPanel = rootVisualElement.Q<VisualElement>("viewPanel");
            viewPanel.style.visibility = Visibility.Visible;
            viewPanel.style.display = DisplayStyle.Flex;
            viewPanel.SetEnabled(true);

            var editPanel = rootVisualElement.Q<VisualElement>("editPanel");
            editPanel.style.visibility = Visibility.Hidden;
            editPanel.style.display = DisplayStyle.None;
            editPanel.SetEnabled(false);
        }

        void ShowEditPanel()
        {
            var viewPanel = rootVisualElement.Q<VisualElement>("viewPanel");
            viewPanel.style.visibility = Visibility.Hidden;
            viewPanel.style.display = DisplayStyle.None;
            viewPanel.SetEnabled(false);

            var editPanel = rootVisualElement.Q<VisualElement>("editPanel");
            editPanel.style.visibility = Visibility.Visible;
            editPanel.style.display = DisplayStyle.Flex;
            editPanel.SetEnabled(true);
        }

        [System.Serializable]
        struct PackageListWrapper
        {
            public List<PackageData> packageList;
        }

        [System.Serializable]
        public struct PackageData
        {
            public string pathToPackage;
            public string name;
            public string info;
        }
    }
}