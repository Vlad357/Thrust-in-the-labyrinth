using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    public GameObject coinPrefab;

    private Transform lastSpawnPoint; 
    private float exclusionRadius = 5f;

    [SerializeField] private List<Transform> spawnPoints;


    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnCoinRoutine());
        }
    }

    private IEnumerator SpawnCoinRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        if (!IsServer) return;

        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (lastSpawnPoint == null || Vector3.Distance(point.position, lastSpawnPoint.position) > exclusionRadius)
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
        {
            availablePoints = new List<Transform>(spawnPoints);
        }

        Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
        lastSpawnPoint = spawnPoint;

        GameObject coin = Instantiate(coinPrefab, spawnPoint.position, coinPrefab.transform.rotation);
        coin.GetComponent<NetworkObject>().Spawn();
    }
}
