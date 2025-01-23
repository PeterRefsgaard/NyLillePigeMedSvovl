using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamFeed : MonoBehaviour
{
    public RawImage rawImage; // The UI element displaying the feed
    private WebCamTexture webCamTexture;

    public WebCamTexture GetWebCamTexture()
    {
        return webCamTexture;
    }

    void Start()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            webCamTexture = new WebCamTexture();
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;
            webCamTexture.Play();
        }
        else
        {
            Debug.LogWarning("No webcam detected.");
        }
    }

    void OnDisable()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}


