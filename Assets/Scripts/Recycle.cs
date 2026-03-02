using System.Collections;
using UnityEngine;

public class Recycle : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _recyleDuration;
    [SerializeField] private Transform _spitOutPlayerPosition;
    [SerializeField] private Transform _grindPosition;
    [SerializeField] private Animator _animator;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovment playerMovment = other.transform.parent.GetComponent<PlayerMovment>();
            Transform cam = playerMovment._cameraHolder;
            GameObject player = playerMovment.gameObject;

            _animator.SetBool("Crushing", true);
            AudioManager.Instance.Play("Crusher");
            cam.parent = _cameraTransform;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.Euler(0, 0, 0);
            player.transform.position = _grindPosition.position;
            player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            playerMovment.enabled = false;

            StartCoroutine(ResetPlayerStuff(playerMovment));
        }
    }
    IEnumerator ResetPlayerStuff(PlayerMovment playerMovment)
    {
        yield return new WaitForSeconds(_recyleDuration);
        playerMovment.ResetJumpCount();
        playerMovment.gameObject.transform.position = _spitOutPlayerPosition.position;
        playerMovment.enabled = true;
        Transform cam = playerMovment._cameraHolder;
        cam.parent = playerMovment.gameObject.transform;
        cam.transform.localPosition = Vector3.zero;
        _animator.SetBool("Crushing", false);
    }
}
