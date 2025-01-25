using System.Collections;
using UnityEngine;

public class MatchLightScript : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject matchPrefab;
    public Transform spawnPoint;

    [Header("Spawn Limits")]
    public int maxSpawns = 3;

    [Header("Despawn Timers")]
    public float matchPrefabTopDespawnTime = 8.0f;
    public float prefabDespawnTime = 10.0f;

    private int spawnCount = 0;
    private GameObject currentSpawnedMatch = null;

    public Animator animator;
    public Transform avatarTransform;
    public float rotationDuration = 2.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && CameraZoom.cameraAtEnd && IsCurrentAnimation("Sitting Idle"))
        {
            StartCoroutine(PlaySequence());
        }
    }

    private IEnumerator PlaySequence()
    {
        // Step 1: Rejs dig op fra Sitting til Standing Idle
        animator.SetTrigger("StandUp");
        yield return new WaitForSeconds(1.5f); // Vent til standing animation er færdig

        // Step 2: Roter 180 grader
        yield return StartCoroutine(RotateAvatar(180f));

        // Step 3: Afspil Light Match animationen
        animator.SetTrigger("LightMatch");
        yield return new WaitForSeconds(GetAnimationLength("Light Match"));

        // Step 4: Spawn tændstik efter "Light Match" animation er færdig
        SpawnMatch();

        // Step 5: Vent til tændstikken er despawnet
        yield return new WaitUntil(() => currentSpawnedMatch == null);

        // Step 6: Stop "Light Match" og gå til "Standing Idle"
        if (IsCurrentAnimation("Light Match"))
        {
            animator.SetTrigger("ToStandingIdle");
            yield return new WaitForSeconds(1.0f); // Giv tid til animationen at skifte
        }

        // Step 7: Roter 180 grader tilbage først efter animationen er skiftet til Standing Idle
        yield return StartCoroutine(RotateAvatar(-180f));

        // Step 8: Vent et sekund efter rotationen
        yield return new WaitForSeconds(1.0f);

        // Step 9: Sæt sig ned igen
        animator.SetTrigger("SitDown");
    }
    private bool IsCurrentAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }


    private IEnumerator RotateAvatar(float angle)
    {
        Quaternion startRotation = avatarTransform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            avatarTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        avatarTransform.rotation = endRotation;
    }

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

        yield return new WaitForSeconds(matchPrefabTopDespawnTime);
        if (matchHead != null) Destroy(matchHead.gameObject);
        if (matchLight != null) Destroy(matchLight.gameObject);

        SoundManager.Instance.PlayCloseMatchSound();
        yield return new WaitForSeconds(prefabDespawnTime - matchPrefabTopDespawnTime);

        if (spawnedMatch != null)
        {
            Destroy(spawnedMatch);
            currentSpawnedMatch = null;  // Tillad ny tændstik at blive tændt
        }

        SoundManager.Instance.StopMatchEffect();
    }
    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }
}
