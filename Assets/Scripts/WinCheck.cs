using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCheck : MonoBehaviour
{
    private int _PlayersInMyBox = 0;
    [SerializeField] private string _nextScene;
    [SerializeField] private float _loadSceneDelay = 1f;
    void LoadNextScene()
    {
        if (_PlayersInMyBox == 2)
        {
            SceneManager.LoadScene(_nextScene);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _PlayersInMyBox++;

            if (_PlayersInMyBox == 2)
            {
                Invoke(nameof(LoadNextScene), _loadSceneDelay);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _PlayersInMyBox--;
        }
    }
}
