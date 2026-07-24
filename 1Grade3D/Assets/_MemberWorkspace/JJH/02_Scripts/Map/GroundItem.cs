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

        [Header("Scan Materials")]
        [SerializeField] private Material spriteScanPixelateMaterial;
        [SerializeField] private Material movingStripeMaterial;

        public ItemSO Item { get; private set; }
        public bool IsDug => _isPopUped;

        private BoxCollider _boxCollider;
        private SpriteRenderer _spriteRenderer;
        private Renderer _renderer;
        private readonly string _outlineProperty = "_OutlineThickness";
        private MaterialPropertyBlock _propertyBlock;

        private Coroutine _popRoutine;

        private readonly Plane _dragPlane = new Plane(Vector3.up, Vector3.up * 0.5f);

        private bool _isDragging;
        private bool _isPopUped = false;
        private bool _isScanning;
        private bool _isInDigRange;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            playerChannel.AddListener<ScannerEvent>(HandleScannerEvent);
            _isScanning = PlayerEvents.ScannerEvent.IsStart;
            _isInDigRange = false;
            ApplyCurrentMaterial();
            SetOutline(0f);
            playerInput.OnMousePerformed += HandleMousePerformed;
            playerInput.OnMouseCanceled += HandleMouseCanceled;
        }

        private void OnDisable()
        {
            playerChannel.RemoveListener<ScannerEvent>(HandleScannerEvent);
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

        public void InitItem(ItemSO item)
        {
            Item = item;
            _spriteRenderer.sprite = item.Icon;

            transform.localScale = Vector3.one * item.Scale;

            Bounds spriteBounds = _spriteRenderer.sprite.bounds;
            _boxCollider.size = new Vector3(spriteBounds.size.x, spriteBounds.size.y, _boxCollider.size.z);
            _boxCollider.center = new Vector3(spriteBounds.center.x, spriteBounds.center.y, _boxCollider.center.z);
        }

        public void SetOutline(float thickness)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(_outlineProperty, thickness);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void HandleScannerEvent(ScannerEvent @event)
        {
            _isScanning = @event.IsStart;
            ApplyCurrentMaterial();
        }

        public void SetDigRangeHighlighted(bool isHighlighted)
        {
            if (_isPopUped)
            {
                isHighlighted = false;
            }

            if (_isInDigRange == isHighlighted)
            {
                return;
            }

            _isInDigRange = isHighlighted;
            ApplyCurrentMaterial();
        }

        private void ApplyCurrentMaterial()
        {
            Material targetMaterial =
                !_isScanning && _isInDigRange
                    ? movingStripeMaterial
                    : spriteScanPixelateMaterial;

            if (targetMaterial == null)
            {
                Debug.LogError(
                    "GroundItem: 적용할 Material이 지정되지 않았습니다.",
                    this);
                return;
            }

            _spriteRenderer.sharedMaterial = targetMaterial;
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

        #region CutScene

        public void PlayDropAnimation(float duration = 0.5f)
        {
            if (_popRoutine != null)
                StopCoroutine(_popRoutine);

            StartCoroutine(DropRoutine(duration));
        }

        private IEnumerator DropRoutine(float duration)
        {
            Vector3 end = transform.position;
            Vector3 start = end + Vector3.up * 15f;

            transform.position = start;

            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;

                float p = t / duration;
                p = 1f - Mathf.Pow(1f - p, 3f);

                transform.position = Vector3.Lerp(start, end, p);
                transform.Rotate(720f * Time.deltaTime, 0f, 0f);

                yield return null;
            }

            transform.position = end;
        }

        #endregion
    }
}
