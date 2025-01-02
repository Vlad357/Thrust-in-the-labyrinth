using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Spawner[] spawners;

    private int currentSpawnerIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AssignSpawnerToPlayer;
    }

    private void AssignSpawnerToPlayer(ulong clientId)
    {
        if (!IsServer) return;

        var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObject == null) return;

        var playerController = playerObject.GetComponent<Player>();
        if (playerController != null && currentSpawnerIndex < spawners.Length)
        {
            playerController.AssignSpawnerClientRpc(spawners[currentSpawnerIndex].SpawnPosition);
            currentSpawnerIndex++;
        }
    }

    public void DeclareWinner(ulong winnerId)
    {
        if (!IsServer) return;

        DeclareWinnerClientRpc(winnerId);

        StartCoroutine(RestartGame());
    }


    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(5f); 

        foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            player.ResetScore();
        }

        foreach (var coin in FindObjectsByType<Coin>(FindObjectsSortMode.None))
        {
            Destroy(coin.gameObject);
        }

        RespawnAllPlayers();

        StartNewRoundClientRpc();
    }

    private void RespawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject == null) continue;

            var playerController = playerObject.GetComponent<Player>();
            if (playerController != null)
            {
                playerController.RespawnClientRpc();
            }
        }
    }

    [ClientRpc]
    private void DeclareWinnerClientRpc(ulong winnerId)
    {
        Debug.Log($"Игрок с ID {winnerId} победил!");
    }

    [ClientRpc]
    private void StartNewRoundClientRpc()
    {
        Debug.Log("Новый раунд начался!");
    }
}
