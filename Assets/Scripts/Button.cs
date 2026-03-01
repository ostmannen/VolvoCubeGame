using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private int id = 0;
    private int _PlayersOnButton = 0;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_PlayersOnButton == 0)
            {
                GameEventsManager.instance.ButtonPress(id, true);
            }
            _PlayersOnButton++;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _PlayersOnButton--;
            if (_PlayersOnButton == 0)
            {
                GameEventsManager.instance.ButtonPress(id, false);
            }
        }
    }
}
