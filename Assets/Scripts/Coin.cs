using System;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddScore(5);
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}