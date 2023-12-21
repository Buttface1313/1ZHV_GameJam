using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInput;
    public InputAction Menu;
    public InputAction MouseMovement;

    bool _menuOpened = false;
    
    private void Awake()
    {
        Menu = playerInput.FindAction("Menu");
        Menu.performed += PauseGame;
        MouseMovement = playerInput.FindAction("Look");
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Menu.performed -= PauseGame;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (!_menuOpened)
        {
            PauseGame();
        }
        else
        {
            UnPauseGame();
        }
    }
    public void PauseGame()
    {
        MouseMovement.Disable();
        Time.timeScale = 0;
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _menuOpened = true;
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _menuOpened = false;
        MouseMovement.Enable();
    }
}
