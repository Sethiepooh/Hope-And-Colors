using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControl : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject[] otherUIElements;
    bool paused;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Toggle pause state
            if (!paused)
            {
                Time.timeScale = 0f; // Pause the game
                pauseMenu.SetActive(true); // Show pause menu UI
                foreach (var uiElement in otherUIElements)
                {
                    uiElement.SetActive(false); // Hide other UI elements
                }
                paused = true;
            }
            else
            {
                Time.timeScale = 1f; // Resume the game
                pauseMenu.SetActive(false); // Hide pause menu UI
                foreach (var uiElement in otherUIElements)
                {
                    uiElement.SetActive(true); // Show other UI elements
                }
                paused = false;
            }
        }
    }

    public void ButtonPauseToggle()
    {
        // Toggle pause state
        if (!paused)
        {
            Time.timeScale = 0f; // Pause the game
            pauseMenu.SetActive(true); // Show pause menu UI
            foreach (var uiElement in otherUIElements)
            {
                uiElement.SetActive(false); // Hide other UI elements
            }
            paused = true;
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
            pauseMenu.SetActive(false); // Hide pause menu UI
            foreach (var uiElement in otherUIElements)
            {
                uiElement.SetActive(true); // Show other UI elements
            }
            paused = false;
        }
    }
}
