using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GGMLib.ObjectPool.Runtime;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class GroundItem : AbstractMonoPoolable
    {
        public ItemSO Item { get; private set; }

        private SpriteRenderer _spriteRenderer;
        private Renderer _renderer;
        private readonly string _outlineProperty = "_OutlineThickness";

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _renderer = GetComponent<Renderer>();
        }

        private void OnEnable()
        {
            SetOutline(0f);
        }

        public void InitItem(ItemSO item)
        {
            Item = item;
            _spriteRenderer.sprite = item.Icon;
        }

        public void FitSpriteToGround(Vector3 groundSize)
        {
            Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;

            transform.localScale = new Vector3(groundSize.x / spriteSize.x,
                                                                    groundSize.z / spriteSize.y, 1f);
        }

        public void SetOutline(float thickness)
        {
            _renderer.material.SetFloat(_outlineProperty, thickness);
        }
    }
}