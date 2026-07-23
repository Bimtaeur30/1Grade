using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public enum SceneType { MainMenu, InGame, GameClear }

    [Serializable]
    public class SceneEntry
    {
        public SceneType sceneType;
        public string sceneName;
    }

    public class SceneFlowManager : MonoBehaviour
    {
        public static SceneFlowManager Instance { get; private set; }
        
        [SerializeField] private SceneEntry[] sceneEntries;
        
        public event Action<SceneType> OnSceneChanged;

        public SceneType CurrentScene {get; private set; } = SceneType.MainMenu;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void GoToScene(SceneType sceneType)
        {
            string sceneName = GetSceneName(sceneType);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("씬 이름이 없어ㅓㅓ");
                return;
            }
            
            SceneManager.LoadScene(sceneName);
            CurrentScene = sceneType;
            OnSceneChanged?.Invoke(sceneType);
        }

        private string GetSceneName(SceneType sceneType)
        {
            foreach (var entry in sceneEntries)
            {
                if(entry.sceneType == sceneType) return entry.sceneName;
            }
            return null;
        }
    }
    
    
}
