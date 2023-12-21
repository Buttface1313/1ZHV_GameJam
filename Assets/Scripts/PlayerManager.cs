using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    //playerSingleton
    public static PlayerManager LocalPlayer;
    [SerializeField] private InputActionAsset playerInput;

    Dictionary<BoosterTypes, int> _boosterCounters = new();
    Dictionary<BoosterTypes, Action> _boosterAppliers = new ();
    Dictionary<BoosterTypes, Action> _boosterRemovers = new ();
    private int LightCounter = 0;

    private PlayerControlsPhys _playerControls;

    [SerializeField] private float _boostedAccel = 2; 
    [SerializeField] private float _boostedMaxSpeed = 20;
    [SerializeField] private float _boostedJump = 4;

    private float _originalAccel;
    private float _originalMaxSpeed;
    private float _originalJump;
    private float _originalDecel;

    InputAction _respawnInput;
    InputAction _moveInput;
    InputAction _jumpInput;
    private void Awake()
    {
        LocalPlayer = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _playerControls = GetComponent<PlayerControlsPhys>();
        _originalAccel = _playerControls.PlayerAcceleration;
        _originalMaxSpeed = _playerControls.MaxPlayerSpeed;
        _originalJump = _playerControls.JumpUpSpeed;
        _originalDecel = _playerControls.PlayerDecceleration;

        _boosterAppliers.Add(BoosterTypes.SpeedBoost, ApplySpeedBoost);
        _boosterRemovers.Add(BoosterTypes.SpeedBoost, RemoveSpeedBoost);

        _boosterAppliers.Add(BoosterTypes.JumpBoost, ApplyJumpBoost);
        _boosterRemovers.Add(BoosterTypes.JumpBoost, RemoveJumpBoost);

        playerInput.Enable();
        _respawnInput = playerInput.FindAction("RestartCharacter");
        _moveInput = playerInput.FindAction("Move");
        _jumpInput = playerInput.FindAction("Jump");
        _moveInput.performed += StartTimer;
        _jumpInput.performed += StartTimer;
        _respawnInput.performed += Respawn;

        _playerControls.MouseSensitivity = SettingsManager.Instance.MouseSensitivity;
        SettingsManager.Instance.OnMouseSensitivityChanged.AddListener((v)=> { _playerControls.MouseSensitivity = v; });
    }
    private void OnDestroy()
    {
        SettingsManager.Instance.OnMouseSensitivityChanged.RemoveListener((v) => { _playerControls.MouseSensitivity = v; });
        _respawnInput.performed -= Respawn;
        _moveInput.performed -= StartTimer;
        _jumpInput.performed -= StartTimer;
    }
    private void StartTimer(InputAction.CallbackContext context)
    {
        GameManager.Instance.StartTimer();
        _moveInput.performed -= StartTimer;
        _jumpInput.performed -= StartTimer;
    }

    public void AddBooster(BoosterTypes c)
    {
        if(_boosterCounters.ContainsKey(c))
        {
            _boosterCounters[c]++;
        }
        else
        {
            _boosterCounters.Add(c, 1);
        }
        if( _boosterCounters[c] == 1 && _boosterAppliers.ContainsKey(c)) {
            _boosterAppliers[c].Invoke();
        }
    }

    public void RemoveCounter(BoosterTypes c)
    {
        if (_boosterCounters.ContainsKey(c))
        {
            _boosterCounters[c]--;
        }
        else
        {
            _boosterCounters.Add(c, 0);
        }
        if (_boosterCounters[c] == 0 && _boosterRemovers.ContainsKey(c))
        {
            _boosterRemovers[c].Invoke();
        }
    }


    public enum BoosterTypes
    {
        Any,
        SpeedBoost,
        JumpBoost
    }

    private void ApplySpeedBoost()
    {
        _playerControls.PlayerAcceleration = _boostedAccel;
        _playerControls.PlayerDecceleration = _boostedAccel * 2;
        _playerControls.MaxPlayerSpeed = _boostedMaxSpeed;
    }
    private void RemoveSpeedBoost()
    {
        _playerControls.PlayerAcceleration = _originalAccel;
        _playerControls.PlayerDecceleration = _originalDecel;
        _playerControls.MaxPlayerSpeed = _originalMaxSpeed;
    }

    private void ApplyJumpBoost()
    {
        _playerControls.Bouncy = true;
        _playerControls.JumpUpSpeed = _boostedJump;
    }
    private void RemoveJumpBoost()
    {
        _playerControls.Bouncy = false;
        _playerControls.JumpUpSpeed = _originalJump;
    }

    public void Respawn(InputAction.CallbackContext context)
    {
        Transform target = GameManager.Instance.CurrentRespawnPoint;
        _playerControls.Teleport(target.position, target.rotation);
        GameManager.Instance.StartTimer();
        GameManager.Instance.PlayerRespawned?.Invoke();
    }
}