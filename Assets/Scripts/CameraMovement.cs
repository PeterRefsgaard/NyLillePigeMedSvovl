using System.Collections;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform startPoint;  // Kameraets startpunkt
    public Transform middlePoint; // Kameraets midtpunkt
    public Transform endPoint;    // Kameraets slutpunkt

    public float moveDuration = 5f;  // Tid til at bevæge sig mellem punkter

    void Start()
    {
        // Start automatisk bevægelsen igennem sekvensen
        StartCoroutine(MoveCameraSequence());
    }

    private IEnumerator MoveCameraSequence()
    {
        // Flyt kameraet fra startPoint til middlePoint
        yield return StartCoroutine(MoveToPosition(startPoint.position, middlePoint.position, moveDuration));

        // Flyt kameraet fra middlePoint til endPoint
        yield return StartCoroutine(MoveToPosition(middlePoint.position, endPoint.position, moveDuration));

        // Hvis du vil flytte kameraet tilbage, fjern kommentaren nedenfor
        // yield return StartCoroutine(MoveToPosition(endPoint.position, middlePoint.position, moveDuration));
    }

    private IEnumerator MoveToPosition(Vector3 from, Vector3 to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            yield return null; // Vent til næste frame
        }

        transform.position = to;  // Sikrer præcis slutposition
    }
}
