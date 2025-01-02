using Unity.Netcode;
using UnityEngine;

public class RespawnTrigger : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;

        var playerController = other.GetComponent<Player>();
        if (playerController != null)
        {
            playerController.RespawnClientRpc();
        }
    }
}
