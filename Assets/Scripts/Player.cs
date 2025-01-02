using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : NetworkBehaviour
{
    private InputSystem_Actions _inputActions;

    public NetworkVariable<int> PlayerScore = new NetworkVariable<int>();
    public TextMeshProUGUI textScore;

    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    public float maxSpeedMultiplier = 2f;
    public float accelerationTime = 3.5f;
    public float decelerationRate = 5f;

    private Rigidbody _rigidbody;
    private float currentSpeed;
    private float speedMultiplier = 1f;
    private float accelerationTimer = 0f;

    private const int WinningScore = 100;

    [SerializeField]private Vector3 assignedSpawner;

    private Camera mainCamera;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Start()
    {
        if (!IsOwner) return;

        _rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        

        if (IsClient)
        {
            textScore = GameObject.Find("ScorePlayerUI").GetComponent<TextMeshProUGUI>();

            PlayerScore.OnValueChanged += (oldValue, newValue) =>
            {
                textScore.text = newValue.ToString();
            };

            if (mainCamera != null)
            {
                CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow == null)
                {
                    cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
                }
                cameraFollow.target = transform;
            }
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
    }

    public void AddScore(int points)
    {
        if (IsServer)
        {
            PlayerScore.Value += points;

            if (PlayerScore.Value >= WinningScore)
            {
                GameManager.Instance.DeclareWinner(this.OwnerClientId);
            }
        }
    }

    public void ResetScore()
    {
        if (IsServer)
        {
            PlayerScore.Value = 0;
        }
    }

    [ClientRpc]
    public void AssignSpawnerClientRpc(Vector3 spawnPoint)
    {
        if (IsClient)
        {
            assignedSpawner = spawnPoint;
            RespawnClientRpc();
        }
    }

    [ClientRpc]
    public void RespawnClientRpc()
    {
        if (IsClient)
        {
            transform.position = assignedSpawner;
        }
    }

    private void HandleMovement()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        var input = _inputActions.Player.Move.ReadValue<Vector2>();

        float horizontal = input.x;
        float vertical = input.y;

        Vector3 direction = forward * vertical + right * horizontal;
        direction.Normalize();

        if (vertical > 0)
        {
            accelerationTimer += Time.deltaTime;
            speedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, accelerationTimer / accelerationTime);
        }
        else
        {
            accelerationTimer = Mathf.Max(0, accelerationTimer - Time.deltaTime * decelerationRate);
            speedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, accelerationTimer / accelerationTime);
        }

        currentSpeed = baseSpeed * speedMultiplier;

        Vector3 velocity = direction * currentSpeed;
        _rigidbody.linearVelocity = new Vector3(velocity.x, _rigidbody.linearVelocity.y, velocity.z);

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        UpdatePositionServerRpc(transform.position);
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
        UpdatePositionClientRpc(newPosition);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            transform.position = newPosition;
        }
    }
}
