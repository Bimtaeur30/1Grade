using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.Managers
{
    public class CursorManager : MonoBehaviour
    {
        private static CursorManager _instance;

        //다른 오브젝트의 OnEnable이 이 스크립트의 Awake보다 먼저 돌 수 있다.
        //그때도 null이 아니도록 아직 대입 전이면 직접 찾아온다.
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