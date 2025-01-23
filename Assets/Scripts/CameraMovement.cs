using System.Collections;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform startPoint;  // Kameraets startpunkt
    public Transform middlePoint; // Kameraets midtpunkt
    public Transform endPoint;    // Kameraets slutpunkt
    private bool hasMovedToMiddle = false; // Flag for at sikre, at kameraet altid bevæger sig fra start til middle først

    public float duration = 5f;   // Tid til at bevæge sig mellem punkter

    void Start()
    {
        // Flyt kameraet automatisk fra startPoint til middlePoint
        StartCoroutine(MoveToMiddlePointFromStart());
    }

    private IEnumerator MoveToMiddlePointFromStart()
    {
        float elapsedTime = 0f;

        // Flyt kameraet fra startPoint til middlePoint
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint.position, middlePoint.position, elapsedTime / duration);
            yield return null; // Vent til næste frame
        }

        transform.position = middlePoint.position; // Sikrer præcis slutposition
        hasMovedToMiddle = true; // Markér, at kameraet er nået til middlePoint
    }

    public IEnumerator MoveToEndPointSynced(float duration)
    {
        // Vent, indtil kameraet har flyttet sig fra startPoint til middlePoint
        while (!hasMovedToMiddle)
        {
            yield return null;
        }

        float elapsedTime = 0f;

        // Flyt kameraet fra middlePoint til endPoint
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(middlePoint.position, endPoint.position, elapsedTime / duration);
            yield return null; // Vent til næste frame
        }

        transform.position = endPoint.position; // Sikrer præcis slutposition
    }

    public IEnumerator MoveToMiddlePointSynced(float duration)
    {
        float elapsedTime = 0f;

        // Flyt kameraet fra endPoint tilbage til middlePoint
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(endPoint.position, middlePoint.position, elapsedTime / duration);
            yield return null; // Vent til næste frame
        }

        transform.position = middlePoint.position; // Sikrer præcis slutposition
    }
}
