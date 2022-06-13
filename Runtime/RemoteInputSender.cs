using UnityEngine;
using UnityEngine.EventSystems;

namespace Futurus.RemoteInput
{
    public class RemoteInputSender : MonoBehaviour, IRemoteInputProvider
    {
        const float MinLineWidth = 0.0001f;

        #region Inspector
        [Header("Raycaster")]
        [SerializeField] protected float _maxLength = 100f;
        [SerializeField] protected bool _occlude;
        [SerializeField] protected LayerMask _occludeMask;

        [Header("Presentation - Line")]
        [SerializeField] protected bool _lineRendererAlwaysOn = false;
        [SerializeField] protected LineRenderer _lineRenderer = null;
        [SerializeField, Min(MinLineWidth)] protected float _lineWidth = 0.002f;
        [SerializeField] protected AnimationCurve _widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [SerializeField] protected Gradient _gradient = new Gradient();
        [Header("Presentation - Cursor")]
        [SerializeField] protected float _cursorScale = 0.1f;
        [SerializeField] protected Sprite _cursorSprite = null;
        #endregion

        #region Runtime
        protected Mesh _cursorMesh = null;
        protected Material _cursorMat = null;
        protected bool _selectDown = false;
        protected bool _hasHit = false;
        protected Vector3 _endpoint = Vector3.zero;
        protected Vector3 _endpointNormal = Vector3.zero;
        protected Vector3[] _points = new Vector3[2];
        protected RemoteInputModule _cachedRemoteInputModule;
        protected RemoteInputEventData _cachedEventData;
        #endregion

        #region Public
        public bool CheckOcclusion => _occlude;
        public LayerMask BlockingOcclusionMask => _occludeMask;
        public float MaxDistance => _maxLength;
        public Vector3[] Points => _points;
        public bool ValidRaycastHit => _hasHit && gameObject.activeSelf && enabled;
        public ButtonDeltaState SelectDelta { get; set; }
        public bool SelectDown
        {
            get => _selectDown;
            set
            {
                if (_selectDown != value)
                {
                    _selectDown = value;
                    SelectDelta |= value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released;
                }
            }
        }
        public bool Validated { get; protected set; }

        /// <summary>
        /// Toggles whether this RemoteInputProvider is selecting 
        /// </summary>
        /// <returns>The new state of RemoteInputSender.SelectDown</returns>
        /// <remarks>ButtonDeltaState is managed internally, pressed and released will remain active for 1 frame</remarks>
        public bool ToggleSelect() => SelectDown = !SelectDown;
        #endregion

        #region Unity
        protected virtual void OnEnable()
        {
            ValidateProvider();
            ValidatePresentation();
            _cachedRemoteInputModule?.Register(this);
        }
        protected virtual void OnDisable()
        {
            DrawLine(false);
            _cachedRemoteInputModule?.Deregister(this);
        }
        // Using Late Update because if another component modifies SelectDown in Update
        // Order of Operations can create bugs. Update order is inconsistent
        protected virtual void LateUpdate()
        {
            if (!ValidateProvider())
                return;
            _cachedEventData = _cachedRemoteInputModule.GetRemoteInputEventData(this);
            UpdateLine(_cachedEventData.LastRaycastResult);
            _cachedEventData.UpdateFromRemote();
            DrawCursor();
        }
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.015f);
            if (!Application.isPlaying || !enabled || !gameObject.activeSelf)
            {
                Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 0.05f));
                return;
            }
            Gizmos.color = _hasHit ? Color.red : Color.blue;
            Gizmos.DrawLine(transform.position, _endpoint);
            Gizmos.DrawSphere(_endpoint, 0.01f);
        }
        protected virtual void OnValidate()
        {
            if (_lineRenderer == null || Application.isPlaying)
                return;
            _lineRenderer.positionCount = 0;
        }
        #endregion

        #region Events
        #endregion

        #region Internal
        protected bool ValidateProvider()
        {
            if (Validated) return true;
            _cachedRemoteInputModule = (_cachedRemoteInputModule != null) ? _cachedRemoteInputModule : EventSystem.current.currentInputModule as RemoteInputModule;
            _cachedRemoteInputModule?.SetRegistration(this, true);
            Validated = _cachedRemoteInputModule != null;
            return Validated;
        }
        protected bool ValidatePresentation()
        {
            _lineRenderer = (_lineRenderer != null) ? _lineRenderer : GetComponent<LineRenderer>();
            if (_lineRenderer == null)
                return false;

            _lineRenderer.widthMultiplier = _lineWidth;
            _lineRenderer.widthCurve = _widthCurve;
            _lineRenderer.colorGradient = _gradient;
            _lineRenderer.positionCount = _points.Length;
            if (_cursorSprite != null && _cursorMat == null)
            {
                _cursorMat = new Material(Shader.Find("Sprites/Default"));
                _cursorMat.SetTexture("_MainTex", _cursorSprite.texture);
                _cursorMat.renderQueue = 4000; // Set renderqueue so it renders above existing UI
                // There's a known issue here where this cursor does NOT render above dropdown components. 
                // it's due to something in how dropdowns create a new canvas and manipulate its sorting order,
                // and since we draw our cursor directly to the Graphics API we can't use CPU-based sorting layers
                // if you have this issue, I recommend drawing the cursor as an unlit mesh instead
                if (_cursorMesh == null)
                    _cursorMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            }

            return true;
        }
        
        protected void UpdateLine(RaycastResult result)
        {
            _hasHit = result.isValid;
            if (result.isValid)
            {
                _endpoint = result.worldPosition;
                _endpointNormal = result.worldNormal;
            }
            else /// not raycast hit
            {
                if (!_lineRendererAlwaysOn)
                {
                    DrawLine(false);
                    return;
                }
                _endpoint = transform.position + transform.forward * _maxLength;
                // _endpointNormal = (transform.position - _endpoint).normalized; // no need to calculate as its not drawn
            }
            DrawLine(true);
            _points[0] = transform.position;
            _points[_points.Length - 1] = _endpoint;
            _lineRenderer.SetPositions(_points);
        }
        protected void DrawCursor()
        {
            if (!ValidRaycastHit)
                return;

            if (_cursorMesh != null && _cursorMat != null)
            {
                _cursorMat.color = _gradient.Evaluate(1);
                var matrix = Matrix4x4.TRS(_points[1], Quaternion.LookRotation(_endpointNormal), Vector3.one * _cursorScale);
                Graphics.DrawMesh(_cursorMesh, matrix, _cursorMat, 0);
            }
        }
        protected void DrawLine(bool toEnable)
        {
            if (_lineRenderer.enabled != toEnable)
                _lineRenderer.enabled = toEnable;
        }
        #endregion
    }
}
