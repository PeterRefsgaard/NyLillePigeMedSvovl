using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Frost")]
public class FrostEffect : MonoBehaviour
{
    public float FrostAmount = 0.5f; //0-1 (0=minimum Frost, 1=maximum frost)
    public float EdgeSharpness = 1; //>=1
    public float minFrost = 0; //0-1
    public float maxFrost = 0; //0-1, initialiseres til 0
    public float seethroughness = 0.2f; //blends between 2 ways of applying the frost effect: 0=normal blend mode, 1="overlay" blend mode
    public float distortion = 0.1f; //how much the original image is distorted through the frost (value depends on normal map)
    public Texture2D Frost; //RGBA
    public Texture2D FrostNormals; //normalmap
    public Shader Shader; //ImageBlendEffect.shader
	
    private Material material;

    private void Awake()
    {
        material = new Material(Shader);
        material.SetTexture("_BlendTex", Frost);
        material.SetTexture("_BumpMap", FrostNormals);
    }

private void Update()
{
    // Synkroniser "max frost" med "freezing sound"-volumen
    if (SoundManager.Instance != null)
    {
        float freezingVolume = SoundManager.Instance.GetFreezingSoundVolume(); // Hent volumen

        // Aktivér frost kun hvis volumen >= 0.65
        if (freezingVolume >= 0.65f)
        {
            maxFrost = Mathf.Clamp01((freezingVolume - 0.65f) / (1.0f - 0.65f)); // Beregn normaliseret frost
        }
        else
        {
            maxFrost = 0; // Ingen frost hvis volumen er under 65%
        }

        // Tjek lose condition
        if (maxFrost >= 1)
        {
            SoundManager.Instance.TriggerLoseCondition(); // Udløs lose condition
        }
    }
}


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Application.isPlaying)
        {
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
            EdgeSharpness = Mathf.Max(1, EdgeSharpness);
        }

        material.SetFloat("_BlendAmount", Mathf.Clamp01(Mathf.Clamp01(FrostAmount) * (maxFrost - minFrost) + minFrost));
        material.SetFloat("_EdgeSharpness", EdgeSharpness);
        material.SetFloat("_SeeThroughness", seethroughness);
        material.SetFloat("_Distortion", distortion);

        Graphics.Blit(source, destination, material);
    }
}