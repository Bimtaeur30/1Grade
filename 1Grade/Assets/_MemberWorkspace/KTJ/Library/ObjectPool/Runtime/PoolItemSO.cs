using System;
using UnityEngine;

namespace GGMLib.ObjectPool.Runtime
{
    [CreateAssetMenu(fileName = "Pool Item", menuName = "Lib/ObjectPool/PoolItem")]
    public class PoolItemSO : ScriptableObject
    {
        [HideInInspector] public string poolName;
        public GameObject prefab;
        public int initCount;

        private void OnValidate()
        {
            if (prefab != null && !prefab.TryGetComponent(out IPoolable _))
            {
                Debug.LogError($"Poolable component가 있는 프리팹만 넣을 수 있습니다.");
                prefab = null;
            }
        }
    }
}