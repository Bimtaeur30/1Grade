using System;
using System.Collections.Generic;
using System.IO;
using GGMLib.ObjectPool.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGMLib.ObjectPool.Editor
{
    public class ObjectPoolEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualAsset = default;
        [SerializeField] private PoolManagerSO poolManager = default;
        [SerializeField] private VisualTreeAsset itemUIAsset = default;

        private string _rootFolder;
        private Button _createBtn;
        private ScrollView _itemView;

        private List<PoolItemView> _itemList;
        private PoolItemView _currentItem;
        
        private UnityEditor.Editor _cachedEditor;
        private VisualElement _inspector;
        
        [MenuItem("Tools/Object Pool")]
        public static void ShowWindow()
        {
            ObjectPoolEditor wnd = GetWindow<ObjectPoolEditor>();
            wnd.titleContent = new GUIContent("ObjectPoolEditor");
        }

        #region Helper method

        private string GetCurrentDirectory()
        {
            string scriptFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            return Path.GetDirectoryName(scriptFilePath); //이 스크립트 파일이 있는 경로를 알아내서 가져온다.
        }

        private void InitializeRootFolder()
        {
            string dirName = GetCurrentDirectory();
            DirectoryInfo parentDir = Directory.GetParent(dirName); //현재 파일의 부모 경로
            Debug.Assert(parentDir != null, $"부모 경로가 없습니다. 경로를 체크하세요 : {dirName}");
            
            string dataPath = Application.dataPath;
            _rootFolder = parentDir.FullName.Replace('\\', '/'); //역슬래시를 전부 슬래시로 치환
            if (_rootFolder.StartsWith(dataPath)) //절대경로로 나왔다면 앞에 잘라내서 상대 경로로 변환
            {
                _rootFolder = "Assets" + _rootFolder.Substring(dataPath.Length);
            }
        }
        #endregion

        public void CreateGUI()
        {
            InitializeRootFolder(); //루트 경로를 입력해두고
            
            VisualElement root = rootVisualElement;

            if (visualAsset == null)
            {
                string dirName = GetCurrentDirectory();
                visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{dirName}/ObjectPoolEditor.uxml");
            }
            visualAsset.CloneTree(root);

            InitializeItems(root);
            GenerateItemUI();
        }

        private void InitializeItems(VisualElement root)
        {
            _createBtn = root.Q<Button>("create-btn");
            _createBtn.clicked += HandleCreateItem;
            _itemView = root.Q<ScrollView>("item-view");
            
            _itemView.Clear(); //전체 비워주고
            _itemList = new List<PoolItemView>(); //새로 리스트 할당
            _inspector = root.Q<VisualElement>("inspector-view");
        }

        private void GenerateItemUI()
        {
            _itemView.Clear();
            _itemList.Clear();
            _inspector.Clear();

            if (poolManager == null)
            {
                string filePath = $"{_rootFolder}/PoolManager.asset";
                poolManager = AssetDatabase.LoadAssetAtPath<PoolManagerSO>(filePath);
                if (poolManager == null)  //파일로 찾았는데도 없다면 새거 하나 만들어.
                {
                    Debug.LogWarning("풀매니저 에셋이 없어서 새로 하나 만들어줍니다.");
                    poolManager = ScriptableObject.CreateInstance<PoolManagerSO>();
                    AssetDatabase.CreateAsset(poolManager, filePath);
                }
            }

            if (itemUIAsset == null) //마찬가지로 PoolItemView.uxml이 없다면 로드해준다.
            {
                string dirName = GetCurrentDirectory();
                itemUIAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{dirName}/PoolItemView.uxml");
                Debug.Assert(itemUIAsset != null, "PoolItemView.uxml 파일이 존재하지 않습니다.");
            }

            foreach (PoolItemSO itemSO in poolManager.itemList)
            {
                TemplateContainer itemContainer = itemUIAsset.Instantiate();
                PoolItemView itemView = new PoolItemView(itemContainer, itemSO); //데이터 바인딩한 클래스
                
                _itemView.Add(itemContainer); //스크롤뷰에 넣는다.
                _itemList.Add(itemView); //리스트에도 넣는다.

                itemView.Name = itemSO.poolName;
                itemView.IsEmpty = itemSO.prefab == null;
                itemView.IsActive = false;

                itemView.OnSelect += HandleSelectItem;
                itemView.OnDelete += HandleDeleteItem;
            }
        }
        
        private void HandleCreateItem()
        {
            Guid itemGuid = Guid.NewGuid();
            PoolItemSO newItem = ScriptableObject.CreateInstance<PoolItemSO>();
            newItem.poolName = itemGuid.ToString(); //고유한 이름을 붙여준다.

            if (!Directory.Exists($"{_rootFolder}/Items"))
            {
                Directory.CreateDirectory($"{_rootFolder}/Items");
            }
            
            AssetDatabase.CreateAsset(newItem, $"{_rootFolder}/Items/{newItem.poolName}.asset");
            
            poolManager.itemList.Add(newItem);
            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();
            
            GenerateItemUI(); //아이템 UI새로 생성.
        }
        
        private void HandleSelectItem(PoolItemView viewUI)
        {
            if (_currentItem != null)
            {
                _currentItem.IsActive = false;
            }
            _currentItem = viewUI;
            _currentItem.IsActive = true;
            
            _inspector.Clear();
            UnityEditor.Editor.CreateCachedEditor(_currentItem.ItemSO, null, ref _cachedEditor);
            VisualElement inspectorElement = _cachedEditor.CreateInspectorGUI(); //인스펙터 GUI용 VisualElement를 만든다.
            
            SerializedObject so = new SerializedObject(_currentItem.ItemSO); //현재 대상을 직렬화.
            inspectorElement.Bind(so);
            inspectorElement.TrackSerializedObjectValue(so, trackObject =>
            {
                viewUI.Name = trackObject.FindProperty("poolName").stringValue;
                viewUI.IsEmpty = trackObject.FindProperty("prefab").objectReferenceValue == null;
            });
            
            _inspector.Add(inspectorElement);
        }

        private void HandleDeleteItem(PoolItemView viewUI)
        {
            poolManager.itemList.Remove(viewUI.ItemSO);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(viewUI.ItemSO));
            EditorUtility.SetDirty(poolManager); //삭제 완료
            AssetDatabase.SaveAssets();

            if (viewUI == _currentItem)
            {
                _currentItem = null; //현재 선택된 것이 삭제된 것이면
                _inspector.Clear(); //인스펙터토 클리어.
            }
            
            GenerateItemUI(); //삭제후 새롭게 리스트 갱신.
        }
    }
}
