using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class CursorManager : MonoBehaviour
    {
        private static CursorManager _instance;

        public static CursorManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindFirstObjectByType<CursorManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Confined;
        }
    }
}