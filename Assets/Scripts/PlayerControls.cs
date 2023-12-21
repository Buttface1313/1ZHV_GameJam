using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CameraPosition _cameraPos;
    [SerializeField] private InputActionAsset playerInput;
    private InputAction moveInput, jumpInput, lookInput, LMB, RMB;
    private CharacterController _charController;
    private bool _lowerGrav;
    Vector3 _moveVector;
    private const int groundLayer = 1 << 10;
    private const int collideableLayer = 1 << 11;
    private PlayerManager playerManager;
    private Vector3 _velocity;

    private float _verticalMousePos = 0;

    public float MouseSensitivity = 1.0f;
    [Header("Movement")]
    public float PlayerAcceleration;
    public float MaxPlayerSpeed;
    public float PlayerDecceleration;

    public bool Bouncy = false;

    [Header("Jumps")]
    [SerializeField] private float _lowerGravityTime;
    public float JumpUpSpeed;

    [SerializeField] private UnityEvent<bool> PrimaryPressed;

    static Vector3 gizmoBall;


    [SerializeField]
    float jumpBallSize = 0.415f;

    private float lastTimeJumped;

    private enum Player_state
    {
        none,
        onGround,
        onWall
    }
    private void Awake()
    {
        _charController = GetComponent<CharacterController>();
        playerManager = GetComponent<PlayerManager>();
        //rotateInput = playerInput.actions["Look"];
        InputEvents();
        Application.targetFrameRate = -1;
    }
    private void InputEvents()
    {
        playerInput.Enable();
        moveInput = playerInput.FindAction("Move");
        jumpInput = playerInput.FindAction("Jump");
        lookInput = playerInput.FindAction("Look");
        LMB = playerInput.FindAction("LMB");
        RMB = playerInput.FindAction("RMB");

        LMB.performed += LMBPress;
        LMB.canceled += LMBReleased;
        //RMB.performed += RMBPress;
        moveInput.performed += Move;
        moveInput.canceled += Move;
        jumpInput.performed += Jump;
        jumpInput.canceled += JumpCancelled;
        lookInput.performed += Look;
        lookInput.canceled += Look;
    }


    private void LMBPress(InputAction.CallbackContext callback)
    {
        PrimaryPressed.Invoke(true);
    }
    private void LMBReleased(InputAction.CallbackContext callback)
    {
        PrimaryPressed.Invoke(false);
    }


    private void Move(InputAction.CallbackContext context)
    {
        _moveVector = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }
    private void Look(InputAction.CallbackContext context)
    {
        transform.Rotate(Vector3.up, MouseSensitivity * context.ReadValue<Vector2>().x);
        _verticalMousePos = Mathf.Clamp(_verticalMousePos + -MouseSensitivity * context.ReadValue<Vector2>().y, -90, 90);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (Time.time - lastTimeJumped > 0.1f)
            if (Physics.CheckSphere(transform.position, jumpBallSize * 1.1f, groundLayer))
            {
                _velocity = new Vector3(_velocity.x, _velocity.y + JumpUpSpeed, _velocity.z);
                lastTimeJumped = Time.time;
                _lowerGrav = true;
                _charController.Move(_velocity * Time.fixedDeltaTime);
            }
    }

    private void Update()
    {
        _cameraPos.AddVerticalOffset(_verticalMousePos);
    }

    private void JumpCancelled(InputAction.CallbackContext context)
    {
        _lowerGrav = false;
    }
    private void FixedUpdate()
    {
        Player_state state = Player_state.none;
        state = Physics.CheckSphere(transform.position, jumpBallSize, groundLayer) ? Player_state.onGround : state;
        Vector3 playerVel = new Vector3(_velocity.x, 0, _velocity.z);
        if(MaxPlayerSpeed*1.1f < playerVel.magnitude)
        {
            _moveVector.z = _moveVector.z > 0 ? 0 : _moveVector.z;
        }
        Vector3 localPlayerMoveVector = transform.TransformVector(_moveVector.normalized);


        if (_lowerGrav)
        {
            _lowerGrav = Time.time - lastTimeJumped < _lowerGravityTime;
        }

        if (state == Player_state.none && Physics.CheckSphere(transform.position, jumpBallSize, groundLayer))
        {
        }

        switch (state)
        {
            case Player_state.none:
                playerVel = Vector3.ClampMagnitude(playerVel + PlayerAcceleration * 0.2f * localPlayerMoveVector, Mathf.Max(MaxPlayerSpeed, playerVel.magnitude));

                if (_lowerGrav)
                {
                    _velocity.y -= 4.8f * Time.fixedDeltaTime;
                }
                else
                    _velocity.y -= 9.8f * Time.fixedDeltaTime;
                playerVel.y = _velocity.y;
                break;
            case Player_state.onGround:
                playerVel.y = _velocity.y;
                playerVel = Vector3.MoveTowards(playerVel, MaxPlayerSpeed * localPlayerMoveVector, PlayerAcceleration + (Vector3.Angle(playerVel, MaxPlayerSpeed * localPlayerMoveVector) / 360 * PlayerDecceleration));
                break;
            case Player_state.onWall:

                break;
        }


        _velocity = playerVel;



        _velocity = CalculateNewDirectionWithCollisions(transform.TransformPoint(_charController.center), _charController.height, _velocity * Time.fixedDeltaTime, _charController.radius, 10, groundLayer | collideableLayer, Bouncy) / Time.fixedDeltaTime;

        _charController.Move(_velocity * Time.fixedDeltaTime);
        Debug.Log(_velocity.magnitude);
    }

    public void Teleport(Vector3 pos, Quaternion rot)
    {
        _charController.enabled = false;
        transform.position = pos;
        transform.rotation = rot;
        _velocity = Vector3.zero;
        _charController.enabled = true;
    }

    private static Vector3 CalculateNewDirectionWithCollisions(Vector3 start, float height, Vector3 dir, float radius, int maxCollsionCount, LayerMask layerMask, bool bouncy = false)
    {
        float distance = dir.magnitude;
        radius *= .999f;
        if (maxCollsionCount <= 0)
            throw new ArgumentException("max collsionCount should be more than 0");
        Vector3 hitpos = start;
        Vector3 point1 = start - ((height / 2) - radius) * Vector3.up;
        gizmoBall = point1;
        Vector3 point2 = start + ((height / 2) - radius) * Vector3.up;
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < maxCollsionCount; i++)
        {
            if (Physics.CapsuleCast(point1, point2, radius, dir, out hit, distance, layerMask))
            {
                hitpos += dir.normalized * hit.distance;
                point1 = hitpos - (height / 2 - radius) * Vector3.up;
                point2 = hitpos + (height / 2 - radius) * Vector3.up;
                if (bouncy)
                {
                    dir -= 2 * (Vector3.Dot(dir, hit.normal) * hit.normal);
                }
                else
                {
                    distance -= Vector3.Angle(hit.normal, dir) > 60 ? hit.distance : dir.magnitude;
                    dir = Vector3.ProjectOnPlane(dir, hit.normal);
                }


                //gizmoBall = point1;
            }
            else
                return (hitpos + dir.normalized * (distance)) - start;
        }
        return (hitpos + dir.normalized * hit.distance) - start;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(gizmoBall, 0.41f*0.999f);
    }


    private float angleSign(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }
}
