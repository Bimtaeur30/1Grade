using System.Collections.Generic;
using UnityEngine;

namespace GGMLib.ObjectPool.Runtime
{
    public class Pool
    {
        private readonly Stack<IPoolable> _pool;
        private readonly Transform _parentTrm;
        private readonly GameObject _prefab;

        public Pool(IPoolable poolable, Transform parentTrm, int count)
        {
            _pool = new Stack<IPoolable>(count);
            _parentTrm = parentTrm;
            _prefab = poolable.GameObject;
            for (int i = 0; i < count; i++)
            {
                GameObject go = Object.Instantiate(_prefab, _parentTrm);
                go.SetActive(false);
                IPoolable item = go.GetComponent<IPoolable>();
                Debug.Assert(item != null, $"Poolable 콤포넌트를 찾을 수 없습니다. {_prefab.name}");
                _pool.Push(item);
            }
        }

        public IPoolable Pop()
        {
            IPoolable item;
            if (_pool.Count > 0)
            {
                item = _pool.Pop();
                item.GameObject.SetActive(true);
            }
            else
            {
                GameObject go = Object.Instantiate(_prefab, _parentTrm);
                item = go.GetComponent<IPoolable>();
            }
            item.ResetItem();
            return item;
        }

        public void Push(IPoolable item)
        {
            item.GameObject.SetActive(false);
            _pool.Push(item);
        }
    }
}