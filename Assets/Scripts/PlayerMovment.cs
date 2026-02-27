using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float _JumpForce = 1;
    [SerializeField] private float _jumpDelay = 1f;
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _rotateCameraSpeed = 1;
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
        _camera.Rotate(_cameraRotation * Time.deltaTime);
    }
    public void ChargeJump(InputAction.CallbackContext ctx)
    {
        _CurrentTilt = ctx.ReadValue<Vector2>().y;
        Debug.Log("charge jump: " + _CurrentTilt);

    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _CurrentTilt = _CurrentTilt * -1;
            _CurrentTilt = (_CurrentTilt > 0) ? _CurrentTilt : 0;
            rb.AddForce(transform.forward * _JumpForce * _CurrentTilt, ForceMode.Impulse);
        }
    }
    public void CameraRotation(InputAction.CallbackContext ctx)
    {
        Vector2 lookDir = ctx.ReadValue<Vector2>();

        _PlayerRotation = new Vector3(0, lookDir.x * _rotationSpeed, 0);
        _cameraRotation = new Vector3(lookDir.y * _rotationSpeed, 0, 0);
    }
}
