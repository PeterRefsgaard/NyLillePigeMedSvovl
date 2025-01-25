using System.Collections;
using UnityEngine;
using TMPro;

public class CameraZoom : MonoBehaviour
{
    public Transform startPoint;
    public Transform middlePoint;
    public Transform endPoint;
    public float moveDuration = 5f;
    public static bool cameraAtEnd = false;
    public TextMeshProUGUI introText;

    void Start()
    {
        cameraAtEnd = false;
        StartCoroutine(MoveCameraSequence());
    }

    private IEnumerator MoveCameraSequence()
    {
        introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 1f);
        yield return StartCoroutine(MoveToPosition(startPoint.position, middlePoint.position, moveDuration, true));
        yield return StartCoroutine(MoveToPosition(middlePoint.position, endPoint.position, moveDuration, false));
        cameraAtEnd = true;
    }

    private IEnumerator MoveToPosition(Vector3 from, Vector3 to, float duration, bool fadeText)
    {
        float elapsedTime = 0f;
        Color textColor = introText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);

            if (fadeText)
            {
                float fadeAmount = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                introText.color = new Color(textColor.r, textColor.g, textColor.b, fadeAmount);
            }

            yield return null;
        }

        transform.position = to;

        if (fadeText)
        {
            introText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        }
    }
}
