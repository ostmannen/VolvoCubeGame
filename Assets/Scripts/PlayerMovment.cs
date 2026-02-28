using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private int _jumps = 5;
    private bool _exetingGround = false;
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
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Transform _camera;
    [Header("Visual")]
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [Header("LineRenderer")]
    [SerializeField] private int _lineCount;
    private LineRenderer _LineRenderer;
    private Vector3 _PlayerRotation;
    private Vector3 _cameraRotation;
    private Rigidbody rb;
    private float _CurrentTilt;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _LineRenderer = GetComponent<LineRenderer>();
        _LineRenderer.startColor = Color.red;
        _LineRenderer.endColor = Color.green;

        _LineRenderer.startWidth = 0.2f;
        _LineRenderer.endWidth = 0.2f;

        _LineRenderer.positionCount = _lineCount;

    }
    void Update()
    {
        if (!_exetingGround)
        {
            _Grounded = Physics.CheckSphere(_groundPosition.position, _sphereRadius, _groundMask, QueryTriggerInteraction.Ignore);
        }
        if (!_Grounded)
        {
            rb.AddForce(Vector3.down * _extraGravity, ForceMode.Acceleration);
        }

        transform.Rotate(_PlayerRotation * Time.deltaTime);
        _cameraHolder.Rotate(_cameraRotation * Time.deltaTime);
        if (_skinnedMeshRenderer != null && _Grounded)
        {
            _skinnedMeshRenderer.SetBlendShapeWeight(_blendShapeIndex, _CurrentTilt * 100);
        }
        DrawPath();
    }
    void DrawPath()
    {
        float camY = _camera.forward.y;

        float t = Mathf.InverseLerp(_inversLerpMin, _inversLerpMax, camY);

        t = 1f - t;

        float jumpAngle = Mathf.Lerp(_minAngle, _maxAngle, t);

        Debug.Log("jumpangle: " + jumpAngle);

        Vector3 jumpDir = Quaternion.AngleAxis(jumpAngle, transform.right) * transform.forward;

        Vector3 currentPos = transform.position;
        Vector3 velocity = jumpDir * (_JumpForce * _CurrentTilt + _baseJumpForce) / rb.mass;
        Vector3 hitPosition = Vector3.zero;
        bool weHit = false;

        for (int i = 0; i < _lineCount; i++)
        {
            if (weHit)
            {
                _LineRenderer.SetPosition(i, hitPosition);
                continue;
            }
            Vector3 nextPos = currentPos + velocity * Time.fixedDeltaTime;

            if (Physics.Linecast(currentPos, nextPos, out RaycastHit hit))
            {
                hitPosition = nextPos;
                weHit = true;
                continue;
            }

            currentPos = nextPos;
            velocity.y += Physics.gravity.y * Time.fixedDeltaTime;

            velocity *= (1f - rb.linearDamping * Time.fixedDeltaTime);
            _LineRenderer.SetPosition(i, currentPos);
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
        }
    }
    public void Jump3()
    {
        _skinnedMeshRenderer.SetBlendShapeWeight(_blendShapeIndex, 0);
        _exetingGround = true;
        _Grounded = false;
        Invoke(nameof(ResetJump), _jumpDelay);

        float camY = _camera.forward.y;

        float t = Mathf.InverseLerp(_inversLerpMin, _inversLerpMax, camY);

        t = 1f - t;

        //t = Mathf.Clamp01(t);

        float jumpAngle = Mathf.Lerp(_minAngle, _maxAngle, t);

        Debug.Log("jumpangle: " + jumpAngle);

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
}
