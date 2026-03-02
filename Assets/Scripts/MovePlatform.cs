using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] private Transform _startTransform;
    [SerializeField] private Transform _endTransform;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _id;
    private int _IsMoving = 0;
    void Start()
    {
        GameEventsManager.instance.OnButtonPress += ButtonPressed;
    }
    void Update()
    {
        if (_IsMoving > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, _endTransform.position, _speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _startTransform.position, _speed * Time.deltaTime);
        }
    }
    private void ButtonPressed(int id, bool pressedDown)
    {
        if (_id == id)
        {
            _IsMoving += pressedDown ? 1 : -1;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = this.transform;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
