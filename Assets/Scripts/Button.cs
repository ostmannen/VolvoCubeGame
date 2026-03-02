using UnityEngine;

public class Button : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private int id = 0;
    private int _PlayersOnButton = 0;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_PlayersOnButton == 0)
            {
                animator.SetBool("Pressed", true);
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
                animator.SetBool("Pressed", false);
                GameEventsManager.instance.ButtonPress(id, false);
            }
        }
    }
}
