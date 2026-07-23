using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Scan
{
    public class ScanUICanvas : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new(0f, 1.2f, 0f);

        private Transform cam;
        private Transform target;

        private void Awake()
        {
            cam = Camera.main.transform;
            gameObject.SetActive(false);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            gameObject.SetActive(target != null);
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            transform.position = target.position + offset;
            transform.forward = cam.forward;
        }
    }
}