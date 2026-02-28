using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosition1;
    [SerializeField] private Transform _spawnPosition2;
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [SerializeField] private float _deathHeight = -10;
    void Update()
    {
        if (_player1.position.y < _deathHeight)
        {
            _player1.position = _spawnPosition1.position;
        }
        if (_player2.position .y < _deathHeight)
        {
            _player2.position = _spawnPosition2.position;
        }
    }
}
