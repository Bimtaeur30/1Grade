using _MemberWorkspace.JJW.Asset._02_Script.Item;
using GameLib.EventChannelSystem;
using GGMLib.ObjectPool.Runtime;
using System.Collections;
using UnityEngine;

namespace _MemberWorkspace.JJH._02_Scripts.Map
{
    public class GroundItem : AbstractMonoPoolable
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private EventChannelSO playerChannel;
        [SerializeField] private PoolManagerSO poolManager;

        public ItemSO Item { get; private set; }

        private BoxCollider _boxCollider;
        private SpriteRenderer _spriteRenderer;
        private Renderer _renderer;
        private readonly string _outlineProperty = "_OutlineThickness";

        private Coroutine _popRoutine;

        private readonly Plane _dragPlane = new Plane(Vector3.up, Vector3.up * 0.5f);

        private bool _isDragging;
        private bool _isPopUped = false;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _renderer = GetComponent<Renderer>();
        }

        private void OnEnable()
        {
            SetOutline(0f);
            playerInput.OnMousePerformed += HandleMousePerformed;
            playerInput.OnMouseCanceled += HandleMouseCanceled;
        }

        private void OnDisable()
        {
            playerInput.OnMousePerformed -= HandleMousePerformed;
            playerInput.OnMouseCanceled -= HandleMouseCanceled;
        }

        private void Update()
        {
            if (!_isDragging)
                return;

            Drag();
        }

        private void HandleMousePerformed()
        {
            if (!_isPopUped || _isDragging)
                return;

            if (playerInput.GetGroundItem() == this)
                _isDragging = true;
        }

        private void HandleMouseCanceled()
        {
            if (!_isDragging)
                return;

            Drop();
        }

        public override void ResetItem()
        {
            base.ResetItem();

            _isPopUped = false;
        }

        public void InitItem(ItemSO item, Vector3 groundSize)
        {
            Item = item;
            _spriteRenderer.sprite = item.Icon;

            Bounds spriteBounds = _spriteRenderer.sprite.bounds;
            Vector2 spriteSize = spriteBounds.size;
            transform.localScale = new Vector3(groundSize.x / spriteSize.x,
                                                                    groundSize.z / spriteSize.y, 1f);
            _boxCollider.size = new Vector3(spriteBounds.size.x, spriteBounds.size.y, _boxCollider.size.z);
            _boxCollider.center = new Vector3(spriteBounds.center.x, spriteBounds.center.y, _boxCollider.center.z);
        }

        public void SetOutline(float thickness)
        {
            _renderer.material.SetFloat(_outlineProperty, thickness);
        }

        public void PopUp(float jumpHeight = 0.8f, float duration = 0.35f)
        {
            if (_popRoutine != null)
                StopCoroutine(_popRoutine);

            _isPopUped = true;

            _popRoutine = StartCoroutine(PopUpRoutine(jumpHeight, duration));
        }

        private IEnumerator PopUpRoutine(float jumpHeight, float duration)
        {
            Vector3 basePos = transform.position + new Vector3(0, 0.5f, 0);
            Vector3 startPos = basePos - Vector3.up * 1.5f;

            float scatterRadius = 0.5f;
            Vector2 randomOffset = Random.insideUnitCircle * scatterRadius;
            Vector3 targetPos = basePos + new Vector3(randomOffset.x, 0f, randomOffset.y);

            Quaternion startRot = transform.rotation;
            Quaternion endRot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float arc = 4f * jumpHeight * t * (1f - t);

                float horizontalT = 1f - Mathf.Pow(1f - t, 2f);
                transform.position = Vector3.Lerp(startPos, targetPos, horizontalT) + Vector3.up * arc;
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);

                yield return null;
            }
            transform.position = basePos;
            transform.rotation = endRot;
        }

        #region Drag

        private void Drag()
        {
            Ray ray = playerInput.MainCamera.ScreenPointToRay(playerInput.MousePosition);

            if (_dragPlane.Raycast(ray, out float enter))
                transform.position = ray.GetPoint(enter);
        }

        private void Drop()
        {
            _isDragging = false;

            ScalePlate plate = playerInput.GetScalePlate();

            if (plate == null)
                return;

            playerChannel.RaiseEvent(PlayerEvents.ItemEquipEvent.Init(Item, plate.ScalePlateEnum));

            poolManager.Push(this);
        }

        #endregion
    }
}