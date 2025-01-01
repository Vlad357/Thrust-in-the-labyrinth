using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private InputSystem_Actions _inputActions;

    private float _speed = 5f;

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

    private void Update()
    {
        if(!IsOwner) return;
        CheckMoveInput();
    }

    private void CheckMoveInput()
    {
        var input = _inputActions.Player.Move.ReadValue<Vector2>();
        var moveDirectoin = new Vector3(input.x, 0, input.y);

        MoveToDirection(moveDirectoin);
    }

    private void MoveToDirection(Vector3 moveDirectoin)
    {
        transform.Translate(moveDirectoin * Time.deltaTime * _speed);
    }
}
