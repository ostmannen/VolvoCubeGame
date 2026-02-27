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
    [SerializeField] private float _jumpMultiplyer = 1;
    [Header("Camera")]
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _rotateCameraSpeed = 1;
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Transform _camera;

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
        transform.Rotate(_PlayerRotation * Time.deltaTime);
        _cameraHolder.Rotate(_cameraRotation * Time.deltaTime);
    }
    public void ChargeJump(InputAction.CallbackContext ctx)
    {
        _CurrentTilt = ctx.ReadValue<Vector2>().y * -1;
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _CurrentTilt = Mathf.Clamp(_CurrentTilt, 0, Mathf.Infinity);

            float cameraHeight = transform.position.y - _camera.position.y;
            cameraHeight = Mathf.Clamp(cameraHeight + _startJumpHeight, 0, Mathf.Infinity);

            Vector3 jumpDir = (transform.forward + new Vector3(0, cameraHeight * _jumpMultiplyer, 0)).normalized;
            Debug.Log(jumpDir + " JUmp dir");

            rb.AddForce(jumpDir * (_JumpForce * _CurrentTilt + _baseJumpForce), ForceMode.Impulse);
        }
    }
    public void CameraRotation(InputAction.CallbackContext ctx)
    {
        Vector2 lookDir = ctx.ReadValue<Vector2>();

        _PlayerRotation = new Vector3(0, lookDir.x * _rotationSpeed, 0);
        _cameraRotation = new Vector3(lookDir.y * _rotationSpeed, 0, 0);
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
