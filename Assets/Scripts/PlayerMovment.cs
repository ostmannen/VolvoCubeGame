using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerMovment : MonoBehaviour
{
    [Header("Jumping")]
    [SerializeField] private float _baseJumpForce = 1f;
    [SerializeField] private float _JumpForce = 1;
    [SerializeField] private float _jumpDelay = 1f;
    [SerializeField] private float _startJumpHeight = 3f;
    [SerializeField] private float _minAngle = -20;
    [SerializeField] private float _maxAngle = 70;
    [SerializeField] private float _inversLerpMin = -1;
    [SerializeField] private float _inversLerpMax = -0.2f;
    [SerializeField] private int _blendShapeIndex = 0;
    [SerializeField] private int _maxJumps = 5;
    [SerializeField] private float _chargeAmountForLanding = 0.3f;
    private int _CurrentJumps;
    private bool _exetingGround = false;
    private bool _InAirBig = false;
    private bool _InAirSmall = false;
    [Header("Gravity")]
    [SerializeField] private float _extraGravity;
    [Header("Grounded")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _sphereRadius;
    [SerializeField] private Transform _groundPosition;
    private bool _Grounded;
    [Header("Camera")]
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _rotateCameraSpeed = 1;
    [SerializeField] public Transform _cameraHolder;
    [SerializeField] private Transform _camera;
    [Header("Visual")]
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private VisualEffect _impact;
    [SerializeField] private float _FovVelocityChangeBack = 10f;
    [SerializeField] private GameObject _impactEffect;
    [SerializeField] private LayerMask _noImpactLayerMask;
    [SerializeField] private Renderer _renderer;
    private bool _ImpactReady = false;
    [Header("LineRenderer")]
    [SerializeField] private int _lineCount;
    [SerializeField] private LayerMask _lineRaymask;
    private LineRenderer _LineRenderer;
    private Vector3 _PlayerRotation;
    private float _XRotation = 0f;
    private Vector3 _cameraRotation;
    private Rigidbody rb;
    private Animator _Animator;
    private float _CurrentTilt;
    Vector3 hitPosition;
    bool _FovChanged = false;
    //awawafw
    void Start()
    {
        GameEventsManager.instance.OnRespawnPlayer += OnRespawn;

        rb = GetComponent<Rigidbody>();
        _Animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _LineRenderer = GetComponent<LineRenderer>();
        _LineRenderer.startColor = Color.red;
        _LineRenderer.endColor = Color.green;

        _LineRenderer.startWidth = 0.2f;
        _LineRenderer.endWidth = 0.2f;

        ResetJumpCount();
    }
    void OnDisable()
    {
        GameEventsManager.instance.OnRespawnPlayer -= OnRespawn;
    }

    [SerializeField] private VisualEffect targetDecal;
    void Update()
    {
        if (!_exetingGround)
        {
            _Grounded = Physics.CheckSphere(_groundPosition.position, _sphereRadius, _groundMask, QueryTriggerInteraction.Ignore);

            if (_Grounded && _FovChanged == true)
            {
                GetComponent<PostProcessingBehaviour>().LensDistortionChanged(false);
                _FovChanged = false;
            }
            else if (_FovChanged == true && rb.linearVelocity.magnitude < _FovVelocityChangeBack)
            {
                GetComponent<PostProcessingBehaviour>().LensDistortionChanged(false);
                _FovChanged = false;
            }

            if (_Grounded && _CurrentJumps == 0)
            {
                ResetJumpCount();
                GameEventsManager.instance.Death(this.gameObject);
            }
            if (_Grounded)
            {
                if (_InAirBig)
                {
                    if (_Animator != null)
                    {
                        _Animator.SetTrigger("Land");
                        AudioManager.Instance.Play("Impact");
                    }
                }
                _InAirBig = false;
            }
        }
        if (!_Grounded)
        {
            rb.AddForce(Vector3.down * _extraGravity, ForceMode.Acceleration);
        }
        if (_InAirSmall && _Grounded)
        {
            _InAirSmall = false;
            float _crackAmount = 0.2f * (_maxJumps - (_CurrentJumps - 1));
            _renderer.material.SetFloat("_Damage", _crackAmount);
        }
        transform.Rotate(_PlayerRotation * Time.deltaTime);

        _XRotation += _cameraRotation.x * Time.deltaTime;
        _XRotation = Mathf.Clamp(_XRotation, 0, 90);
        _cameraHolder.localRotation = Quaternion.Euler(_XRotation, 0, 0);

        //_cameraHolder.Rotate(_cameraRotation * Time.deltaTime);

        if (_skinnedMeshRenderer != null && _Grounded)
        {
            _skinnedMeshRenderer.SetBlendShapeWeight(_blendShapeIndex, _CurrentTilt * 100);
        }
        DrawPath();
        targetDecal.SetVector3("WorldSpacePos", hitPosition);
    }
    void DrawPath()
    {
        if (_CurrentTilt == 0 || _exetingGround || !_Grounded)
        {
            targetDecal.SetBool("Disabled", true);
            _LineRenderer.positionCount = 0;
            return;
        }
        targetDecal.SetBool("Disabled", false);

        float camY = _camera.forward.y;

        float t = Mathf.InverseLerp(_inversLerpMin, _inversLerpMax, camY);

        t = 1f - t;

        float jumpAngle = Mathf.Lerp(_minAngle, _maxAngle, t);

        Vector3 jumpDir = Quaternion.AngleAxis(jumpAngle, transform.right) * transform.forward;

        Vector3 currentPos = transform.position;
        Vector3 velocity = jumpDir * (_JumpForce * _CurrentTilt + _baseJumpForce) / rb.mass;
        hitPosition = Vector3.zero;
        Vector3 lastRayStart = currentPos;
        bool weHit = false;
        _LineRenderer.positionCount = _lineCount / 4;

        for (int i = 0; i < _lineCount; i++)
        {
            if (weHit)
            {
                _LineRenderer.SetPosition(i, hitPosition);
                continue;
            }
            Vector3 nextPos = currentPos + velocity * Time.fixedDeltaTime;

            if (i % 4 == 0)
            {
                if (Physics.Linecast(lastRayStart, nextPos, out RaycastHit hit, _lineRaymask, QueryTriggerInteraction.Ignore))
                {
                    hitPosition = nextPos;
                    weHit = true;
                    _LineRenderer.positionCount = i / 4 + 1;
                    _LineRenderer.SetPosition(i / 4, nextPos);

                    return;
                }
                lastRayStart = nextPos;
            }

            currentPos = nextPos;
            velocity.y += Physics.gravity.y * Time.fixedDeltaTime;

            velocity *= (1f - rb.linearDamping * Time.fixedDeltaTime);
            if (i % 4 == 0)
            {
                _LineRenderer.SetPosition(i / 4, currentPos);
            }
        }
    }
    public void ChargeJump(InputAction.CallbackContext ctx)
    {
        _CurrentTilt = ctx.ReadValue<Vector2>().y * -1;
        _CurrentTilt = Mathf.Clamp(_CurrentTilt, 0, 1);
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _Grounded)
        {
            Jump3();
            //weewoo
            if (_CurrentTilt >= 0.7f)
            {
                _FovChanged = true;
                GetComponent<PostProcessingBehaviour>().LensDistortionChanged(true);

                if (_Animator != null)
                {
                    _Animator.SetTrigger("LaunchHard");
                    AudioManager.Instance.Play("Swoosh");
                }
            }
        }
    }
    public void Jump3()
    {
        _skinnedMeshRenderer.SetBlendShapeWeight(_blendShapeIndex, 0);
        _exetingGround = true;
        _Grounded = false;
        if (_CurrentTilt >= _chargeAmountForLanding)
        {
            _InAirBig = true;
            _ImpactReady = true;
        }
        _InAirSmall = true;
        Invoke(nameof(ResetJump), _jumpDelay);

        float camY = _camera.forward.y;

        float t = Mathf.InverseLerp(_inversLerpMin, _inversLerpMax, camY);

        t = 1f - t;

        _CurrentJumps--;

        //t = Mathf.Clamp01(t);

        float jumpAngle = Mathf.Lerp(_minAngle, _maxAngle, t);

        //Debug.Log("jumpangle: " + jumpAngle);

        Vector3 jumpDir = Quaternion.AngleAxis(jumpAngle, transform.right) * transform.forward;

        rb.AddForce(jumpDir * (_JumpForce * _CurrentTilt + _baseJumpForce), ForceMode.Impulse);
    }
    public void CameraRotation(InputAction.CallbackContext ctx)
    {
        Vector2 lookDir = ctx.ReadValue<Vector2>();

        _PlayerRotation = new Vector3(0, lookDir.x * _rotationSpeed, 0);
        _cameraRotation = new Vector3(lookDir.y * _rotationSpeed, 0, 0);
    }
    private void ResetJump()
    {
        _exetingGround = false;
    }
    public void ResetJumpCount()
    {
        _ImpactReady = false;
        _CurrentJumps = _maxJumps;
        _renderer.material.SetFloat("_Damage", 0);
    }
    public void OnRespawn(GameObject player)
    {
        Debug.Log(player.name + " / " + this.gameObject);
        if (player == this.gameObject)
        {
            ResetJumpCount();
            Debug.Log("fenjfael");
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (_ImpactReady)
        {

            if (Physics.CheckSphere(_groundPosition.position, _sphereRadius, _noImpactLayerMask, QueryTriggerInteraction.Ignore)) return;
            AudioManager.Instance.Play("Impact");

            Quaternion rot = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);

            Instantiate(_impactEffect, collision.contacts[0].point, rot);
            if (Physics.CheckSphere(_groundPosition.position, _sphereRadius, _groundMask, QueryTriggerInteraction.Ignore))
            {
                _ImpactReady = false;
            }
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMeny");
    }
}
