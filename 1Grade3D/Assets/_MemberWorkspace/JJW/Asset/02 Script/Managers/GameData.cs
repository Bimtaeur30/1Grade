using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance { get; private set; }

        public int CurrentLevel { get; set; } = 1;
        public int Money { get; set; } = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}