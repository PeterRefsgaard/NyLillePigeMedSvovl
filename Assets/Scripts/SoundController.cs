using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioSourceGroup
    {
        public AudioSource SnowSound;
        public AudioSource LightMatchSound;
        public AudioSource EndMatchSound;
        public AudioSource FreezingSound;
        public AudioSource HappyFamilySound;
    }

    [System.Serializable]
    public class SnowstormSettings
    {
        public float windPanSpeed = 1f;
        public float minPan = -1f;
        public float maxPan = 1f;
        public float maxVolume = 0.5f;
        public float volumeChangeSpeed = 0.5f; 
        public float restoreSpeed = 0.1f; 
    }
    [System.Serializable]
    public class FreezingSoundSettings
    {
        public float volumeDecreaseMultiplier = 2.5f; 
        public float volumeIncreaseMultiplier = 0.03f;
    }

    public static SoundManager Instance;
    [Header("Audio Sources")]
    public AudioSourceGroup audioSources;
    [Header("Dynamic Snowstorm Settings")]
    public SnowstormSettings snowstormSettings;
    [Header("Internal Snowstorm Dynamics")]

    [Header("Freezing Sound Settings")]
    public FreezingSoundSettings freezingSoundSettings = new FreezingSoundSettings();
    private float currentPanDirection;
    private float windDirectionChange = 1f;
    private bool isRestoringVolume = false; 
    private bool isMatchActive = false; 

    private float currentFreezingSoundVolume;
    private bool isFreezingSoundMuted = false;

    
    #region Singleton
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Lifecycle Methods
    private void Awake()
    {
        InitializeSingleton();
    }
    public void Start()
    {
        PlayDynamicSnowSound();
        PlayDynamicFreezingSound();
        PlayHappyFamilySound();
    }
    public void Update()
    {
        ChangeSnowSoundStereoPanDir();
        UpdateSnowSoundVolume();
        UpdateFreezingSoundVolume();
    }
    #endregion

    #region Dynamic Snow Sound Methods
    private void PlayDynamicSnowSound()
    {
        if (audioSources.SnowSound != null)
        {
            currentPanDirection = 0f;
            audioSources.SnowSound.panStereo = currentPanDirection;
            audioSources.SnowSound.volume = snowstormSettings.maxVolume;
            audioSources.SnowSound.Play();
        }
    }

    private void ChangeSnowSoundStereoPanDir()
    {
        if (audioSources.SnowSound != null)
        {
            currentPanDirection += windDirectionChange * snowstormSettings.windPanSpeed * Time.deltaTime;
            if (currentPanDirection > snowstormSettings.maxPan || currentPanDirection < snowstormSettings.minPan)
            {
                windDirectionChange *= -1f;
                currentPanDirection = Mathf.Clamp(currentPanDirection, snowstormSettings.minPan, snowstormSettings.maxPan);
            }

            audioSources.SnowSound.panStereo = currentPanDirection;
        }
    }

    private void UpdateSnowSoundVolume()
    {
        if (audioSources.SnowSound != null && !isRestoringVolume)
        {
            float volumeFluctuation = Mathf.Sin(Time.time * snowstormSettings.volumeChangeSpeed) * 0.01f;
            audioSources.SnowSound.volume = Mathf.Clamp(snowstormSettings.maxVolume + volumeFluctuation, 0f, snowstormSettings.maxVolume);
        }
    }

    public void MuteSnowVolumeMatchLit()
    {
        if (audioSources.SnowSound != null)
        {
            audioSources.SnowSound.volume *= 0.20f;
            isRestoringVolume = true; 
        }
    }
    public void RestoreSnowSoundVolume()
    {
        if (audioSources.SnowSound != null)
        {
            StartCoroutine(RestoreVolumeToMax());
        }
    }
    private IEnumerator RestoreVolumeToMax()
    {
        float initialVolume = audioSources.SnowSound.volume;
        float targetVolume = snowstormSettings.maxVolume;
        float elapsedTime = 0f;
        float duration = 3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSources.SnowSound.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        audioSources.SnowSound.volume = targetVolume;
        isRestoringVolume = false; 
    }
    #endregion

    #region Match Lightning Sound Methods
    public void PlayLightMatchSound()
    {
        if (audioSources.LightMatchSound != null)
        {
            audioSources.LightMatchSound.Play();
        }
    }
    public void PlayCloseMatchSound()
    {
        if (audioSources.EndMatchSound != null)
        {
            audioSources.EndMatchSound.Play();
        }
    }
    #endregion

    #region Freezing Sound Methods
    private bool isIncreasingFreezingVolume = false; 
    private float freezingVolumeIncreaseRate = 0.03f; 
    private float freezingTargetVolume = 1.0f;
    public float GetFreezingSoundVolume()
    {
        if (audioSources.FreezingSound != null)
        {
            return audioSources.FreezingSound.volume;
        }
        return 0f; 
    }
    public void PlayDynamicFreezingSound()
    {
        if (audioSources.FreezingSound != null)
        {
            audioSources.FreezingSound.volume = 0.15f;
            audioSources.FreezingSound.loop = true;
            audioSources.FreezingSound.Play();
            isIncreasingFreezingVolume = true;         }
    }
    public void GraduallyReduceFreezingSoundVolume(float duration)
    {
        if (audioSources.FreezingSound != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(GraduallyReduceVolumeCoroutine(duration));
        }
    }
    private IEnumerator GraduallyReduceVolumeCoroutine(float duration)
    {
        float initialVolume = audioSources.FreezingSound.volume; 
        float targetVolume = initialVolume * 0.5f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSources.FreezingSound.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / duration);
            yield return null; 
        }

        audioSources.FreezingSound.volume = targetVolume; 
    }

    public void RestoreFreezingSoundVolume()
    {
        if (audioSources.FreezingSound != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(GraduallyRestoreFreezingSoundVolume(5.0f)); 
        }
    }

    private IEnumerator GraduallyRestoreFreezingSoundVolume(float duration)
    {
        float initialVolume = audioSources.FreezingSound.volume;
        float targetVolume = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSources.FreezingSound.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / duration);
            yield return null; 
        }

        audioSources.FreezingSound.volume = targetVolume; 
    }

    public void NoticeAndHalfFreezingSoundVolumeBeforeLitMatch()
{
    if (audioSources.FreezingSound != null && !isFreezingSoundMuted)
    {
        currentFreezingSoundVolume = audioSources.FreezingSound.volume; 
        audioSources.FreezingSound.Stop(); 
        isFreezingSoundMuted = true; 
        isIncreasingFreezingVolume = false; 
    }
}
    public void ResumeFreezingSoundAfterMatchClose()
    {
        if (audioSources.FreezingSound != null && isFreezingSoundMuted)
        {
            isFreezingSoundMuted = false;
            float startVolume = audioSources.FreezingSound.volume * 0.5f;
            audioSources.FreezingSound.volume = startVolume;
            audioSources.FreezingSound.Play();
            isIncreasingFreezingVolume = true;
        }
    }

public IEnumerator GraduallyReduceFreezingSoundOverLifetime(float lifetime)
{
    float initialVolume = audioSources.FreezingSound.volume;
    float targetVolume = initialVolume * 0.5f; 
    float elapsedTime = 0f;

    while (elapsedTime < lifetime)
    {
        elapsedTime += Time.deltaTime;
        audioSources.FreezingSound.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / lifetime);
        yield return null; 
    }
    audioSources.FreezingSound.volume = targetVolume; 
    isMatchActive = false; 
}

public void StartMatchEffect(float matchLifetime)
{
    if (!isMatchActive && audioSources.FreezingSound != null)
    {
        isMatchActive = true;
        StopAllCoroutines();
        StartCoroutine(GraduallyReduceFreezingSoundOverLifetime(matchLifetime));
        IncreaseHappyFamilySoundVolumeQuickly();
        MuteSnowVolumeMatchLit();
    }
}

public void StopMatchEffect()
{
    isMatchActive = false; 
    DecreaseHappyFamilySoundVolumeGradually();
    RestoreSnowSoundVolume();
}


private void UpdateFreezingSoundVolume()
{
    if (audioSources.FreezingSound != null)
    {
        float adjustmentRate = isMatchActive 
            ? -freezingSoundSettings.volumeDecreaseMultiplier 
            : freezingSoundSettings.volumeIncreaseMultiplier;

        audioSources.FreezingSound.volume = Mathf.Clamp(
            audioSources.FreezingSound.volume + adjustmentRate * Time.deltaTime,
            0f,
            freezingTargetVolume
        );
    }
}
    #endregion


    #region Happy Family Sound Methods

    private float happyFamilyBaseVolume = 0.02f; 
    private float happyFamilyMaxVolume = 0.7f;  
    private float happyFamilyVolumeIncreaseRate = 1f; 
    private float happyFamilyVolumeDecreaseRate = 0.04f;

    public void PlayHappyFamilySound()
    {
        if (audioSources.HappyFamilySound != null)
        {
            audioSources.HappyFamilySound.volume = happyFamilyBaseVolume; 
            audioSources.HappyFamilySound.loop = true; 
            audioSources.HappyFamilySound.Play();
        }
    }

private Coroutine happyFamilyCoroutine;

public void IncreaseHappyFamilySoundVolumeQuickly()
{
    if (audioSources.HappyFamilySound != null)
    {
        if (happyFamilyCoroutine != null)
        {
            StopCoroutine(happyFamilyCoroutine); 
        }
        happyFamilyCoroutine = StartCoroutine(AdjustHappyFamilySoundVolume(happyFamilyMaxVolume, happyFamilyVolumeIncreaseRate));
    }
}

public void DecreaseHappyFamilySoundVolumeGradually()
{
    if (audioSources.HappyFamilySound != null)
    {
        if (happyFamilyCoroutine != null)
        {
            StopCoroutine(happyFamilyCoroutine); 
        }
        happyFamilyCoroutine = StartCoroutine(AdjustHappyFamilySoundVolume(happyFamilyBaseVolume, happyFamilyVolumeDecreaseRate));
    }
}
    private IEnumerator AdjustHappyFamilySoundVolume(float targetVolume, float rate)
    {
        while (audioSources.HappyFamilySound != null &&
            Mathf.Abs(audioSources.HappyFamilySound.volume - targetVolume) > 0.01f)
        {
            audioSources.HappyFamilySound.volume = Mathf.MoveTowards(
                audioSources.HappyFamilySound.volume,
                targetVolume,
                rate * Time.deltaTime
            );
            yield return null;
        }

        audioSources.HappyFamilySound.volume = targetVolume; 
    }

    #endregion

    #region Loose condition
private bool loseConditionTriggered = false; 
public bool LoseConditionTriggered()
{
    return loseConditionTriggered; 
}
public void TriggerLoseCondition()
{
    if (loseConditionTriggered) return;
    loseConditionTriggered = true; 

    StartCoroutine(HandleLoseCondition());
}

private void ShowBlackScreen()
{
    GameObject blackScreen = new GameObject("BlackScreen");
    Canvas canvas = blackScreen.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

    CanvasScaler canvasScaler = blackScreen.AddComponent<CanvasScaler>();
    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

    blackScreen.AddComponent<GraphicRaycaster>();

    GameObject imageObj = new GameObject("BlackImage");
    imageObj.transform.SetParent(blackScreen.transform);
    RectTransform rect = imageObj.AddComponent<RectTransform>();
    rect.anchorMin = Vector2.zero;
    rect.anchorMax = Vector2.one;
    rect.offsetMin = Vector2.zero;
    rect.offsetMax = Vector2.zero;
    UnityEngine.UI.Image image = imageObj.AddComponent<UnityEngine.UI.Image>();
    image.color = Color.black;
    StartCoroutine(ExitGameAfterDelay(3f));
}

private IEnumerator HandleLoseCondition()
{
    if (audioSources.LightMatchSound != null) audioSources.LightMatchSound.Stop();
    if (audioSources.EndMatchSound != null) audioSources.EndMatchSound.Stop();
    if (audioSources.FreezingSound != null) audioSources.FreezingSound.Stop();
    if (audioSources.HappyFamilySound != null) audioSources.HappyFamilySound.Stop();

    float initialVolume = audioSources.SnowSound.volume;
    float targetVolume = 0f; 
    float fadeDuration = 5f; 
    float elapsedTime = 0f;

    while (elapsedTime < fadeDuration)
    {
        elapsedTime += Time.deltaTime;
        audioSources.SnowSound.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / fadeDuration);
        yield return null;
    }

    if (audioSources.SnowSound != null)
    {
        audioSources.SnowSound.Stop();
    }
    ShowBlackScreen();
}


private IEnumerator ExitGameAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    Application.Quit(); 
}
    #endregion
}