using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private float _winSceneLoadDelay = 2f;
    [SerializeField] private float _loseSceneLoadDelay = 2f;
    public void LoadSceneByNameIdk(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void LoadSceneWithDelay(string name, float delay)
    {
        StartCoroutine(LoadDelay(name, delay));
    }
    public IEnumerator LoadDelay(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadSceneByNameIdk(name);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
