using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    private static Color unactiveColor = new Color(130f / 255f, 130f / 255f, 130f / 255f, 200f / 255f);
    private static Color activeColor = new Color(255, 255, 255, 200);

    public bool freeze;
    public bool activeGrapple;

    private Vector3 _velocityToSet;
    private bool _enableMovementOnNextTouch;

    [Header("References")]
    public Camera playerCamera;
    public Transform playerObj;
    public GameObject canvaLeftArm;
    public GameObject canvaRightArm;
    public GameObject canvaLeftLeg;
    public GameObject canvaRightLeg;
    public GameObject spawnPoint;
    private Rigidbody _rb;

    [Header("Movement")]
    public float speed = 5.0f;
    public float airMultiplier = 0.4f;
    public float groundDrag = 6.0f;

    [Header("Jumping")]
    public float jumpForce = 3.0f;
    private bool _isGrounded = true;

    [Header("Dashing")]
    public float dashCooldown = 1.0f;
    public float dashIntensity = 10.0f;
    private float _dashCooldown;

    [Header("Sliding")]
    public float slideForce = 5.0f;
    public float maxSlideTime = 0.5f;
    private bool _isSliding;
    private float _slideTime;
    private float _startYScale;

    [Header("Wallrunning")]
    public float wallRunForce;
    public float wallClimbSpeed;
    private float _wallRunTimer;
    private bool _wallRunning;
    private bool _upwardsRunning;
    private bool _downwardsRunning;

    [Header("Detection")]
    private RaycastHit _leftWallhit;
    private RaycastHit _rightWallhit;
    private bool _wallLeft;
    private bool _wallRight;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dash = KeyCode.LeftShift;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode upwardsRunKey = KeyCode.Q;
    public KeyCode downwardsRunKey = KeyCode.E;

    private float _horizontal;
    private float _vertical;

    private bool _readyToJump;

    private Vector3 _moveDirection;

    private bool _isLeftArm = true;
    private bool _isRightArm = true;
    private bool _isLeftLeg = true;
    private bool _isRightLeg = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        playerCamera = Camera.main;

        _startYScale = playerObj.localScale.y;
        _readyToJump = true;

        if (playerCamera != null) {
            playerCamera.transform.position = new Vector3(playerObj.position.x, playerObj.position.y + 1.75f, playerObj.position.z) + playerObj.forward * 0.2f;
            playerCamera.transform.parent = playerObj;
        }
    }

    void Update()
    {
        _isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down , 1.1f);

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down , 1.1f) && Physics.Raycast(transform.position + Vector3.up, Vector3.down , out RaycastHit hit) && hit.collider.CompareTag("killzone"))
            transform.position = spawnPoint.transform.position;

        playerCamera.transform.position = new Vector3(playerObj.position.x, playerObj.position.y + 1.75f, playerObj.position.z) + playerObj.forward * 0.2f;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        playerObj.Rotate(Vector3.up, mouseX * 2);


        float xRot = playerCamera.transform.eulerAngles.x - mouseY * 2;
        if (xRot > 180)
            xRot -= 360;
        playerCamera.transform.localEulerAngles = new Vector3(Mathf.Clamp(xRot, -90, 90), playerCamera.transform.localEulerAngles.y, 0);

        playerObj.rotation = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0);
        _dashCooldown += Time.deltaTime;

        CheckForWall();
        MyInput();
        SpeedControl();

        if (freeze)
            _rb.linearVelocity = Vector3.zero;

        if (_isGrounded)
            _rb.linearDamping = groundDrag;
        else
            _rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if (_isSliding)
            SlidingMovement();
        if (_wallRunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        _wallRight = Physics.Raycast(transform.position, transform.right, 1f) && Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight) && hitRight.collider.CompareTag("Wall");
        _wallLeft = Physics.Raycast(transform.position, -transform.right, 1f) && Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft) && hitLeft.collider.CompareTag("Wall");
    }

    private void MyInput()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && _isGrounded)
            Jump();

        if (Input.GetKeyDown(dash))
            Dash();

        if (Input.GetKeyDown(slideKey) && (!_isSliding) && _isGrounded)
            StartSlide();

        if (Input.GetKeyUp(slideKey))
            EndSlide();

        if(Input.GetKey(jumpKey) && _readyToJump && (_isGrounded || _wallRunning)) {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), 0.2f);
        }

        _upwardsRunning = Input.GetKey(upwardsRunKey);
        _downwardsRunning = Input.GetKey(downwardsRunKey);

        if((_wallLeft || _wallRight) && _vertical > 0 && !_isGrounded) {
            if (!_wallRunning)
                StartWallRun();
        } else {
            if (_wallRunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        _wallRunning = true;
    }


    private void WallRunningMovement()
    {
        _rb.useGravity = false;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        Vector3 wallNormal = _wallRight ? _rightWallhit.normal : _leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((playerObj.forward - wallForward).magnitude > (playerObj.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        _rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (_upwardsRunning)
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, wallClimbSpeed, _rb.linearVelocity.z);
        if (_downwardsRunning)
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, -wallClimbSpeed, _rb.linearVelocity.z);

        if (!(_wallLeft && _horizontal > 0) && !(_wallRight && _horizontal < 0))
            _rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    private void StopWallRun()
    {
        _wallRunning = false;
        _rb.useGravity = true;
    }

    private void MovePlayer()
    {
        _moveDirection = playerObj.forward * _vertical + playerObj.right * _horizontal;
        if(_isGrounded)
            _rb.AddForce(_moveDirection.normalized * speed, ForceMode.Force);
        else
            _rb.AddForce(_moveDirection.normalized * speed * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        if(flatVel.magnitude > speed) {
            Vector3 limitedVel = flatVel.normalized * speed;
            _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        Debug.Log("Jumping");
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void StartSlide()
    {
        _isSliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale / 2, playerObj.localScale.z);
        playerCamera.transform.position = new Vector3(playerObj.position.x, playerObj.position.y + 0.5f, playerObj.position.z) + playerObj.forward * 0.2f;

        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        _slideTime = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = playerObj.forward * _vertical + playerObj.right * _horizontal;

        _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Impulse);

        _slideTime = Mathf.Max(_slideTime - Time.deltaTime, 0);
        if (_slideTime <= 0)
            EndSlide();
    }

    private void EndSlide()
    {
        _isSliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);
        playerCamera.transform.position = new Vector3(playerObj.position.x, playerObj.position.y + 1.75f, playerObj.position.z) + playerObj.forward * 0.2f;
    }


    private void Dash()
    {
        if (_dashCooldown < dashCooldown)
            return;

        Vector3 dashDirection = Vector3.zero;

        if (_vertical < 0) dashDirection -= playerObj.transform.forward;
        else if (_horizontal < 0) dashDirection -= playerObj.transform.right;
        else if (_horizontal > 0) dashDirection += playerObj.transform.right;
        else dashDirection += playerObj.transform.forward;

        if (dashDirection != Vector3.zero)
            dashDirection.Normalize();

        if (_isGrounded)
            _rb.AddForce(dashDirection * (speed / 2) * dashIntensity, ForceMode.Impulse);
        else
            _rb.AddForce(dashDirection * speed * (dashIntensity * 1.2f) * airMultiplier, ForceMode.Impulse);
        _dashCooldown = 0.0f;
    }

    public void JumpToPosition(Vector3 position, float trajectoryHeight)
    {
        activeGrapple = true;
        _velocityToSet = CalculateJumpVelocity(transform.position, position, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
        Invoke(nameof(ResetRestrictions), 3f);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endpoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endpoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endpoint.x - startPoint.x, 0, endpoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void SetVelocity()
    {
        _rb.linearVelocity = _velocityToSet;
        _enableMovementOnNextTouch = true;
    }

    private void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_enableMovementOnNextTouch)
        {
            _enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

}
