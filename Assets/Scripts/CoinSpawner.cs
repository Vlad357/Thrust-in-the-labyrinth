using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    public GameObject coinPrefab;
    public Transform[] spawnPoints;

    private void Start()
    {
        if (IsServer)
        {
            print("run");
            InvokeRepeating(nameof(SpawnCoin), 5f, 5f);
        }
    }

    private void SpawnCoin()
    {
        int index = Random.Range(0, spawnPoints.Length);
        GameObject coin = Instantiate(coinPrefab, spawnPoints[index].position, Quaternion.identity);
        coin.GetComponent<NetworkObject>().Spawn();
    }
}
