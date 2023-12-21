using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControlsPhys : MonoBehaviour
{
    [SerializeField] CameraPosition _cameraPos;
    [SerializeField] CapsuleCollider _capsuleCollider;
    [SerializeField] private InputActionAsset playerInput;
    private InputAction moveInput, jumpInput, lookInput, LMB, RMB,CrouchInput;
    private Rigidbody _rb;
    Vector3 _moveVector;
    private const int groundLayer = 1 << 10;
    private const int collideableLayer = 1 << 11;
    private PlayerManager playerManager;

    private float _verticalMousePos = 0;

    public float MouseSensitivity = 1.0f;
    [Header("Movement")]
    public float PlayerAcceleration;
    public float MaxPlayerSpeed;
    public float PlayerDecceleration;
    public float MaxHookSpeed;
    public float HookedAcceleration;
    public float HookedAirFriction = 1;

    public bool Bouncy = false;

    [Header("Jumps")]
    [SerializeField] private float _lowerGravityTime;
    public float JumpUpSpeed;

    [SerializeField] private UnityEvent<bool> PrimaryPressed;

    static Vector3 gizmoBall;


    [SerializeField]
    float jumpBallSize = 0.415f;

    private float lastTimeJumped;

    [Header("HookStuff")]
    [SerializeField] ConfigurableJoint _hookJoint;
    [SerializeField] Rigidbody _hookPoint;

    private enum Player_state
    {
        none,
        onGround,
        hooked
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
        CrouchInput = playerInput.FindAction("Crouch");

        LMB.performed += LMBPress;
        LMB.canceled += LMBReleased;
        moveInput.performed += Move;
        moveInput.canceled += Move;
        jumpInput.performed += Jump;
        lookInput.performed += Look;
        lookInput.canceled += Look;
    }

    private void OnDestroy()
    {
        LMB.performed -= LMBPress;
        LMB.canceled -= LMBReleased;
        moveInput.performed -= Move;
        moveInput.canceled -= Move;
        jumpInput.performed -= Jump;
        lookInput.performed -= Look;
        lookInput.canceled -= Look;
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
                lastTimeJumped = Time.time;
                if (_rb.velocity.y > JumpUpSpeed && Bouncy)
                    return;

                if(_rb.velocity.y<0)
                    _rb.velocity = new Vector3(_rb.velocity.x,0,_rb.velocity.z); 
                if(Bouncy)
                    _rb.AddRelativeForce((JumpUpSpeed-_rb.velocity.y) / Time.fixedDeltaTime * _rb.mass * Vector3.up);
                else
                    _rb.AddRelativeForce((JumpUpSpeed) / Time.fixedDeltaTime * _rb.mass * Vector3.up);
            }
    }

    private void Update()
    {
        _cameraPos.AddVerticalOffset(_verticalMousePos);
    }
    private void FixedUpdate()
    {
        Player_state state = Player_state.none;
        if (_hookPoint == null)
            state = Physics.CheckSphere(transform.position, jumpBallSize, groundLayer) ? Player_state.onGround : state;
        else
            state = Player_state.hooked;
        Vector3 playerVel = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (state == Player_state.none)
        {

        }
        Vector3 localPlayerMoveVector;
        switch (state)
        {
            case Player_state.none:
                var moveVecLimited = new Vector3(_moveVector.x, 0, MaxPlayerSpeed < playerVel.magnitude && _moveVector.z > 0 ? 0 : _moveVector.z);
                localPlayerMoveVector = transform.TransformVector(moveVecLimited.normalized);
                playerVel = Vector3.ClampMagnitude(playerVel + PlayerAcceleration * 0.33f * localPlayerMoveVector * Time.fixedDeltaTime, Mathf.Max(MaxPlayerSpeed, playerVel.magnitude));
                playerVel.y = _rb.velocity.y;
                break;
            case Player_state.onGround:
                localPlayerMoveVector = transform.TransformVector(_moveVector.normalized);
                playerVel = Vector3.MoveTowards(playerVel, MaxPlayerSpeed * localPlayerMoveVector,
                    (PlayerAcceleration + (Vector3.Angle(playerVel, MaxPlayerSpeed * localPlayerMoveVector) / 360 * PlayerDecceleration)) * Time.fixedDeltaTime);
                playerVel.y = _rb.velocity.y;
                break;
            case Player_state.hooked:
                localPlayerMoveVector = transform.TransformVector((Quaternion.FromToRotation(Vector3.up, _hookPoint.position - _rb.position) * _moveVector));
                playerVel.y = _rb.velocity.y;
                playerVel = Vector3.ClampMagnitude(playerVel + HookedAcceleration * Time.fixedDeltaTime * localPlayerMoveVector, Mathf.Max(MaxHookSpeed, playerVel.magnitude));
                break;
        }




        ////var movmentNextFrame = CalculateNewDirectionWithCollisions(transform.TransformPoint(_collider.center), _collider.height, (playerVel -_rb.velocity) * Time.fixedDeltaTime, _collider.radius, 10, groundLayer | collideableLayer, Bouncy);


        _rb.AddForce((playerVel - _rb.velocity) / Time.fixedDeltaTime * _rb.mass);
    }

    public void HookToRB(Rigidbody rb)
    {
        _hookPoint = rb;
        SoftJointLimit jointLimit = _hookJoint.linearLimit;
        _hookJoint.xMotion = ConfigurableJointMotion.Limited;
        _hookJoint.yMotion = ConfigurableJointMotion.Limited;
        _hookJoint.zMotion = ConfigurableJointMotion.Limited;
        jointLimit.limit = Vector3.Distance(rb.position, _rb.position);
        _hookJoint.linearLimit = jointLimit;
        _hookJoint.connectedBody = rb;
        rb.drag = HookedAirFriction;
    }
    public void DropHook()
    {
        _hookJoint.xMotion = ConfigurableJointMotion.Free;
        _hookJoint.yMotion = ConfigurableJointMotion.Free;
        _hookJoint.zMotion = ConfigurableJointMotion.Free;
        _hookPoint = null;
        _hookJoint.connectedBody = null;
        _rb.drag = 0;
    }

    public void Teleport(Vector3 pos, Quaternion rot)
    {
        _rb.velocity = Vector3.zero;
        transform.SetPositionAndRotation(pos, rot);
        lookInput.Reset();
        _verticalMousePos = 0;
        DropHook();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(gizmoBall, 0.41f * 0.999f);
    }


    private float angleSign(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }
    public Vector3 GetVelocity()
    {
        return _rb.velocity;
    }
}
