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
    private Vector3 _PlayerRotation;
    private Vector3 _cameraRotation;

    private Rigidbody rb;
    private float _CurrentTilt;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        float t = Mathf.InverseLerp(-1f, -0.2f, camY);

        t = 1f - t;

        //t = Mathf.Clamp01(t);

        float jumpAngle = Mathf.Lerp(_minAngle, _maxAngle, t);

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
    private void OnDrawGizmos()
    {
        float temp = _CurrentTilt;
        temp = Mathf.Clamp(temp, 0, Mathf.Infinity);

        float cameraHeight = transform.position.y - _camera.position.y;
        cameraHeight = Mathf.Clamp(cameraHeight + _startJumpHeight, 0, Mathf.Infinity);

        Vector3 jumpDir = (transform.forward + new Vector3(0, cameraHeight, 0)).normalized;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, jumpDir * (_JumpForce * _CurrentTilt + _baseJumpForce));
    }
}
