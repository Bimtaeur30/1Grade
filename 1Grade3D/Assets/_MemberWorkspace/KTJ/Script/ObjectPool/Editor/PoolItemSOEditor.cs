using GGMLib.ObjectPool.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGMLib.ObjectPool.Editor
{
    [CustomEditor(typeof(PoolItemSO))]
    public class PoolItemSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset viewAsset = default;

        private TextField _nameField;
        private ObjectField _prefabField;
        private Button _changeButton;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            viewAsset.CloneTree(root);
            //DrawDefaultInspector(); //기본 인스펙터 그려주고.
            //InspectorElement.FillDefaultInspector(root, serializedObject, this);
            
            _nameField = root.Q<TextField>("pooling-name");
            _prefabField = root.Q<ObjectField>("prefab-field");
            _changeButton = root.Q<Button>("change-btn");

            _changeButton.clicked += HandleChangeButtonClick;
            _nameField.RegisterCallback<KeyDownEvent>(HandleKeyDownEvent);
            _prefabField.RegisterValueChangedCallback(HandlePrefabChangeEvent);
            
            return root;
        }

        private void HandlePrefabChangeEvent(ChangeEvent<Object> evt)
        {
            if (evt.newValue == null) return; //기존 프리팹을 빼버렸다면 아무것도 하지마라.
            
            GameObject newObject = evt.newValue as GameObject; 
            Debug.Assert(newObject != null, "프리팹은 반드시 게임오브젝트여야 합니다.");
            PoolItemSO poolItemSO = target as PoolItemSO;

            if (!newObject.TryGetComponent(out IPoolable poolable))
            {
                poolItemSO.prefab = null;
                EditorUtility.SetDirty(poolItemSO);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Error", "Poolable 콤포넌트를 찾을 수 없습니다.", "OK");
                return;
            }
            
            poolable.PoolItem = poolItemSO;
            EditorUtility.SetDirty(newObject); //프리팹을 dirty처리 
            AssetDatabase.SaveAssets();
        }

        private void HandleChangeButtonClick()
        {
            string newName = _nameField.text;
            if (string.IsNullOrEmpty(newName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a valid name.", "OK");
                return;
            }
            string assetPath = AssetDatabase.GetAssetPath(target);
            
            string message = AssetDatabase.RenameAsset(assetPath, newName);
            if (string.IsNullOrEmpty(message))
            {
                target.name = newName;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", message, "OK");
            }
        }

        private void HandleKeyDownEvent(KeyDownEvent evt)
        {
            if(evt.keyCode == KeyCode.Return)
                HandleChangeButtonClick();
        }
    }
}