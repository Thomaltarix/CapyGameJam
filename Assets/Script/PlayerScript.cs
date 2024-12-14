using System.IO;
using UnityEngine;
using Utils;

public class PlayerScript : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject player;
    private float _speed = 5.0f;

    private const string SettingsPath = "Assets/configs/keybinds.json";
    private MovementKeybinds _keybinds = new MovementKeybinds();

    private float _dashCooldown = 0.0f;

    private Rigidbody _rigidbody;
    private float _jumpForce = 3.0f;
    private bool _isGrounded = true;



    private struct KeyMapping
    {
        public KeyCode KeyCode;
        public System.Action Action;
    }

    private KeyMapping[] _keyMappings;

    void Start()
    {
        player = this.gameObject;
        playerCamera = Camera.main;

        if (playerCamera != null) {
            playerCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.75f, player.transform.position.z) + player.transform.forward * 0.2f;
            playerCamera.transform.parent = player.transform;
        }

        _rigidbody = player.GetComponent<Rigidbody>();
        if (_rigidbody == null) {
            Debug.LogError("Rigidbody component not found on player object.");
            _rigidbody = player.AddComponent<Rigidbody>();
            _rigidbody.useGravity = true;
        }
        _rigidbody.freezeRotation = true;

        if (File.Exists(SettingsPath)) {
            var json = File.ReadAllText(SettingsPath);
            _keybinds = JsonUtility.FromJson<MovementKeybinds>(json);
        } else {
            Debug.LogWarning($"Settings file not found at {SettingsPath}. Using default keybinds.");
            _keybinds = GetDefaultKeybinds();
        }

        InitializeKeyMappings();
    }

    void Update()
    {
        foreach (var mapping in _keyMappings)
            if (Input.GetKey(mapping.KeyCode))
                mapping.Action?.Invoke();

        playerCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.75f, player.transform.position.z) + player.transform.forward * 0.2f;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        player.transform.Rotate(Vector3.up, mouseX * 2);

        playerCamera.transform.Rotate(Vector3.right, -mouseY * 2);

        player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
        _dashCooldown += Time.deltaTime;

        if (Physics.Raycast(player.transform.position, Vector3.down, 1.1f))
            _isGrounded = true;
    }

    private void InitializeKeyMappings()
    {
        _keyMappings = new KeyMapping[]
        {
            new KeyMapping { KeyCode = _keybinds.forward, Action = MoveForward },
            new KeyMapping { KeyCode = _keybinds.backward, Action = MoveBackward },
            new KeyMapping { KeyCode = _keybinds.left, Action = MoveLeft },
            new KeyMapping { KeyCode = _keybinds.right, Action = MoveRight },
            new KeyMapping { KeyCode = _keybinds.up, Action = Jump },
            new KeyMapping { KeyCode = _keybinds.down, Action = Slide },
            new KeyMapping { KeyCode = _keybinds.dash, Action = Dash }
        };
    }

    private MovementKeybinds GetDefaultKeybinds()
    {
        return new MovementKeybinds
        {
            forward = KeyCode.W,
            backward = KeyCode.S,
            left = KeyCode.A,
            right = KeyCode.D,
            up = KeyCode.Space,
            down = KeyCode.LeftControl,
            dash = KeyCode.LeftShift
        };
    }

    private void MoveForward()
    {
        player.transform.position += player.transform.forward * _speed * Time.deltaTime;
    }

    private void MoveBackward()
    {
        player.transform.position -= player.transform.forward * _speed * Time.deltaTime;
    }

    private void MoveLeft()
    {
        player.transform.position -= player.transform.right * _speed * Time.deltaTime;
    }

    private void MoveRight()
    {
        player.transform.position += player.transform.right * _speed * Time.deltaTime;
    }

    private void Jump()
    {
        if (_isGrounded) {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    private void Slide()
    {
        player.transform.position -= player.transform.up * _speed * Time.deltaTime;
    }

    private void Dash()
    {
        if (_dashCooldown < 1.0f)
            return;

        Vector3 dashDirection = Vector3.zero;

        if (Input.GetKey(_keybinds.backward)) dashDirection -= player.transform.forward;
        else if (Input.GetKey(_keybinds.left)) dashDirection -= player.transform.right;
        else if (Input.GetKey(_keybinds.right)) dashDirection += player.transform.right;
        //else if (Input.GetKey(_keybinds.up)) dashDirection += player.transform.up;
        //else if (Input.GetKey(_keybinds.down)) dashDirection -= player.transform.up;
        else dashDirection += player.transform.forward;

        if (dashDirection != Vector3.zero)
            dashDirection.Normalize();

        player.transform.position += dashDirection * _speed * 3;
        _dashCooldown = 0.0f;
    }
}
