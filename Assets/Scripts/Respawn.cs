using UnityEngine;
using UnityEngine.InputSystem;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosition1;
    [SerializeField] private Transform _spawnPosition2;
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [SerializeField] private PlayerMovment _input1;
    [SerializeField] private PlayerMovment _input2;
    private bool _Respawning1 = false;
    private bool _Respawning2 = false;
    [SerializeField] private float _deathHeight = -10;
    [SerializeField] private float _respawnDelay = 1f;
    [SerializeField] private GameObject _lavaSplash;
    void Start()
    {
        _player1.position = _spawnPosition1.position;
        _player2.position = _spawnPosition2.position;
        GameEventsManager.instance.OnDeath += OnPayerDeath;
    }
    void OnDisable()
    {
        GameEventsManager.instance.OnDeath -= OnPayerDeath;
    }
    void Update()
    {
        if (_player1.position.y < _deathHeight && !_Respawning1)
        {
            if (_lavaSplash != null)
            {
                Instantiate(_lavaSplash, _player1.transform.position, Quaternion.identity);
                        AudioManager.Instance.Play("Lava");

            }
            Invoke(nameof(RespawnPlayer1), _respawnDelay);
            _input1.enabled = false;
            _Respawning1 = true;
        }
        if (_player2.position.y < _deathHeight && !_Respawning2)
        {
            if (_lavaSplash != null)
            {
                Instantiate(_lavaSplash, _player2.transform.position, Quaternion.identity);
                        AudioManager.Instance.Play("Lava");
            }
            Invoke(nameof(RespawnPlayer2), _respawnDelay);
            _input2.enabled = false;
            _Respawning2 = true;
        }
    }

    void RespawnPlayer1()
    {
        _player1.GetComponent<PlayerMovment>().ResetJumpCount();
        _player1.position = _spawnPosition1.position;
        _input1.enabled = true;
        _Respawning1 = false;
    }
    void RespawnPlayer2()
    {
        _player2.GetComponent<PlayerMovment>().ResetJumpCount();
        _player2.position = _spawnPosition2.position;
        _input2.enabled = true;
        _Respawning2 = false;
    }
    private void OnPayerDeath(GameObject player)
    {
        if (player == _player1.gameObject)
        {
            RespawnPlayer1();
        }
        if (player == _player2.gameObject)
        {
            RespawnPlayer2();
        }
    }

    //this is so dumb, but like brrrrrrrr... wrooomm
}
