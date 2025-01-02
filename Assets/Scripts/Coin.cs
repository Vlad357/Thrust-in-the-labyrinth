using System;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    public int points = 5; 

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        
        var player = other.GetComponent<Player>();
        if (player != null)
        {

            player.AddScore(points);

            
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}