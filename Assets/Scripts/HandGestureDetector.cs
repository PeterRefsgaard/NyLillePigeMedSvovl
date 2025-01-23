using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI components like RawImage
using TMPro; // If using TextMeshPro for debug text

public class HandGestureDetector : MonoBehaviour
{
    public WebcamFeed webcamFeed; // Link the WebcamFeed script in the Inspector
    private WebCamTexture webCamTexture;

    public RawImage rawImage; // The RawImage displaying the webcam feed
    public GameObject handMarker; // UI marker to show when a hand is detected
    public TextMeshProUGUI debugText; // Debug TextMeshPro UI for feedback (optional)

    public Color32 targetColor = new Color32(255, 224, 189, 255); // Approximate skin tone
    public float colorThreshold = 50f; // Tolerance for skin color matching
    public int detectionThreshold = 1000; // Number of "skin pixels" needed to detect a hand

    private bool isHandRaised = false; // Tracks the current state

    void Start()
    {
        // Get the webcam texture from the WebcamFeed script
        if (webcamFeed != null)
        {
            webCamTexture = webcamFeed.GetWebCamTexture();
            Debug.Log("WebCamTexture started");
        }

        if (webCamTexture == null)
        {
            Debug.LogWarning("No webcam texture found for gesture detection.");
        }

        // Initialize UI elements
        if (debugText != null)
        {
            debugText.text = "Waiting for hand gesture...";
        }

        if (handMarker != null)
        {
            handMarker.SetActive(false); // Hide the marker initially
        }
    }

    void Update()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            DetectRaisedHand();
        }
    }

    void DetectRaisedHand()
    {
        // Get the pixels from the webcam texture
        Color32[] pixels = webCamTexture.GetPixels32();
        int width = webCamTexture.width;
        int height = webCamTexture.height;

        int skinPixelCount = 0;

        // Check a portion of the screen (e.g., the upper half for hand detection)
        for (int y = height / 2; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (IsSkinColor(pixels[index]))
                {
                    skinPixelCount++;
                }
            }
        }

        // Log skin pixel count for debugging
        Debug.Log($"Skin Pixel Count: {skinPixelCount}");

        // If enough skin pixels are detected, consider it a hand raise
        if (skinPixelCount > detectionThreshold)
        {
            if (!isHandRaised)
            {
                isHandRaised = true;
                OnHandRaise();
            }
        }
        else
        {
            if (isHandRaised)
            {
                isHandRaised = false;
                OnHandLower();
            }
        }
    }

    bool IsSkinColor(Color32 color)
    {
        // Compare the pixel color to the target skin color
        float diff = Mathf.Abs(color.r - targetColor.r) +
                     Mathf.Abs(color.g - targetColor.g) +
                     Mathf.Abs(color.b - targetColor.b);
        return diff < colorThreshold;
    }

    void OnHandRaise()
    {
        Debug.Log($"Hand Raised Detected at {Time.time} seconds!");

        // Update the debug text
        if (debugText != null)
        {
            debugText.text = "Hand Raised Detected!";
            Debug.Log("DebugText updated: Hand Raised Detected!");
        }

        // Change the RawImage color to green
        if (rawImage != null)
        {
            rawImage.color = Color.green;
            Debug.Log("RawImage color set to green.");
        }

        // Show the hand marker
        if (handMarker != null)
        {
            handMarker.SetActive(true);
            Debug.Log("HandMarker is now active.");
        }
    }

    void OnHandLower()
    {
        Debug.Log("Hand Lowered.");

        // Update the debug text
        if (debugText != null)
        {
            debugText.text = "Hand Lowered.";
            Debug.Log("DebugText updated: Hand Lowered.");
        }

        // Reset the RawImage color to white
        if (rawImage != null)
        {
            rawImage.color = Color.white;
            Debug.Log("RawImage color reset to white.");
        }

        // Hide the hand marker
        if (handMarker != null)
        {
            handMarker.SetActive(false);
            Debug.Log("HandMarker is now inactive.");
        }
    }
}


