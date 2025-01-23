using UnityEngine;
using System.Collections;

public class MatchLightScript : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject matchPrefab;
    public Transform spawnPoint;

    [Header("Spawn Limits")]
    public int maxSpawns = 3;

    [Header("Despawn Timers")]
    public float mathcPrefabTopDespawnTime = 8.0f;
    public float prefabDespawnTime = 10.0f;

    private int spawnCount = 0;
    private GameObject currentSpawnedMatch = null;

    public void SpawnMatch()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.LoseConditionTriggered())
        {
            return;
        }
        if (currentSpawnedMatch != null) return; 
        spawnCount++;
        currentSpawnedMatch = Instantiate(matchPrefab, spawnPoint.position, Quaternion.identity); 
        SoundManager.Instance.PlayLightMatchSound();
        SoundManager.Instance.StartMatchEffect(prefabDespawnTime);
        StartCoroutine(HandleMatchDespawning(currentSpawnedMatch));
    }
    private IEnumerator HandleMatchDespawning(GameObject spawnedMatch)
    {
        Transform matchHead = spawnedMatch.transform.Find("Match Head");
        Transform matchLight = spawnedMatch.transform.Find("Match Light");
        yield return new WaitForSeconds(mathcPrefabTopDespawnTime);
        Destroy(matchHead.gameObject);
        Destroy(matchLight.gameObject);
        SoundManager.Instance.PlayCloseMatchSound();
        yield return new WaitForSeconds(prefabDespawnTime - mathcPrefabTopDespawnTime);
        if (spawnedMatch != null)
        {
            Destroy(spawnedMatch);
        }
        currentSpawnedMatch = null;
        SoundManager.Instance.StopMatchEffect(); 
    }
}
