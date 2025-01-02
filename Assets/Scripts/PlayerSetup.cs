using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Renderer playerRenderer;
    private Material playerMaterial;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        var randomColor = Random.ColorHSV();

        if (playerRenderer != null)
        {
            if (playerMaterial == null)
            {
                playerMaterial = new Material(playerRenderer.material);
                playerRenderer.material = playerMaterial;
            }
            playerMaterial.color = randomColor;
        }
    }
}
