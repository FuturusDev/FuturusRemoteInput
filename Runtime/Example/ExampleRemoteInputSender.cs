using UnityEngine;
using UnityEngine.EventSystems;

namespace Futurus.RemoteInput
{
    public class ExampleRemoteInputSender : MonoBehaviour, IRemoteInputProvider
    {
        const float MinLineWidth = 0.0001f;

        #region Inspector
        [SerializeField] protected float _maxLength = 1000f;
        [SerializeField] protected bool _occlude;
        [SerializeField] protected LayerMask _occludeMask;

        [Header("Presentation")]
        [SerializeField] protected LineRenderer _lineRenderer = null;
        [SerializeField, Min(MinLineWidth)] protected float _lineWidth = 0.002f;
        [SerializeField] protected AnimationCurve _widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [SerializeField] protected Gradient _gradient = new Gradient();
        [SerializeField] protected float _cursorScale = 0.1f;
        [SerializeField] protected Sprite _cursorSprite = null;
        #endregion

        #region Runtime
        Mesh _cursorMesh = null;
        Material _cursorMat = null;
        bool _selectDown = false;
        bool _hasHit = false;
        Vector3 _endpoint = Vector3.zero;
        Vector3 _endpointNormal = Vector3.zero;
        Vector3[] _points = new Vector3[2];
        RemoteInputModule _cachedRemoteInputModule;
        RemoteInputEventData _cachedEventData;
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

        /// <summary>
        /// Toggles whether this RemoteInputProvider is selecting 
        /// </summary>
        /// <returns>The new state of RemoteInputSender.SelectDown</returns>
        /// <remarks>ButtonDeltaState is managed internally, pressed and released will remain active for 1 frame</remarks>
        public bool ToggleSelect() => SelectDown = !SelectDown;
        #endregion

        #region Unity
        void OnEnable()
        {
            ValidateProvider();
            ValidatePresentation();
        }
        void OnDisable()
        {
            _cachedRemoteInputModule?.Deregister(this);
        }
        void Update()
        {
            if (!ValidateProvider())
                return;
            _cachedEventData = _cachedRemoteInputModule.GetRemoteInputEventData(this);
            UpdateLine(_cachedEventData.LastRaycastResult);
            _cachedEventData.UpdateFromRemote();
            SelectDelta = ButtonDeltaState.NoChange;
            if (ValidatePresentation())
                UpdatePresentation();
        }
        void OnDrawGizmos()
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
        void OnValidate()
        {
            if (_lineRenderer == null || Application.isPlaying)
                return;
            _lineRenderer.positionCount = 0;
        }
        #endregion

        #region Events
        #endregion

        #region Internal
        bool ValidateProvider()
        {
            _cachedRemoteInputModule = (_cachedRemoteInputModule != null) ? _cachedRemoteInputModule : EventSystem.current.currentInputModule as RemoteInputModule;
            _cachedRemoteInputModule?.SetRegistration(this, true);
            return _cachedRemoteInputModule != null;
        }
        bool ValidatePresentation()
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
        
        void UpdateLine(RaycastResult result)
        {
            _hasHit = result.isValid;
            if (result.isValid)
            {
                _endpoint = result.worldPosition;
                // result.worldNormal doesn't work properly, seems to always have the normal face directly up
                // instead, we calculate the normal via the inverse of the forward vector on what we hit. Unity UI elements
                // by default face away from the user, so we use that assumption to find the true "normal"
                // If you use a curved UI canvas this likely will not work 
                _endpointNormal = result.gameObject.transform.forward * -1;
            }
            else
            {
                _endpoint = transform.position + transform.forward * _maxLength;
                _endpointNormal = (transform.position - _endpoint).normalized;
            }
            _points[0] = transform.position;
            _points[_points.Length - 1] = _endpoint;
        }
        void UpdatePresentation()
        {
            _lineRenderer.enabled = ValidRaycastHit;
            if (!ValidRaycastHit)
                return;

            _lineRenderer.SetPositions(_points);
            if (_cursorMesh != null && _cursorMat != null)
            {
                _cursorMat.color = _gradient.Evaluate(1);
                var matrix = Matrix4x4.TRS(_points[1], Quaternion.Euler(_endpointNormal), Vector3.one * _cursorScale);
                Graphics.DrawMesh(_cursorMesh, matrix, _cursorMat, 0);
            }
        }
        #endregion
    }
}
