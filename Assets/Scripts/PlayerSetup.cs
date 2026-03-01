using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetup : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    void Awake()
    {
        var gamepads = Gamepad.all;
        if (gamepads.Count >= 2)
        {
            // Tilldela första gamepaden till player1
            player1.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
            player1.ActivateInput();

            // Tilldela andra gamepaden till player2
            player2.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
            player2.ActivateInput();
        }
        else if (gamepads.Count >= 1)
        {
            player1.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
            player1.ActivateInput();

            player2.ActivateInput();

        }
        else
        {
            Debug.LogWarning("Inte tillräckligt med gamepads anslutna.");
        }
    }
}
