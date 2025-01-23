using System.Collections;
using UnityEngine;

public class MoveGirlScript : MonoBehaviour
{
    public Transform endPoint; 
    private Vector3 startPoint;
    public float moveDuration = 5f;
    private bool isMoving = false;
    public MatchLightScript matchLightScript; 
    public CameraZoom cameraZoom; 

    void Start()
    {
        startPoint = transform.position;
    }

    void Update()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.LoseConditionTriggered())
        {
            return; 
        }
        if (Input.GetKeyDown(KeyCode.L) && !isMoving)
        {
            StartCoroutine(ProcessMovementAndMatch());
        }
    }

    private IEnumerator ProcessMovementAndMatch()
    {
        isMoving = true;
        StartCoroutine(cameraZoom.MoveToEndPointSynced(moveDuration)); 
        yield return StartCoroutine(MoveToPoint(startPoint, endPoint.position, moveDuration));

        if (matchLightScript != null)
        {
            matchLightScript.SpawnMatch();
        }

        if (matchLightScript != null)
        {
            yield return new WaitForSeconds(matchLightScript.prefabDespawnTime);
        }

        StartCoroutine(cameraZoom.MoveToMiddlePointSynced(moveDuration)); 
        yield return StartCoroutine(MoveToPoint(endPoint.position, startPoint, moveDuration)); 

        isMoving = false;
    }

    private IEnumerator MoveToPoint(Vector3 from, Vector3 to, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            yield return null;
        }

        transform.position = to; 
    }
}
