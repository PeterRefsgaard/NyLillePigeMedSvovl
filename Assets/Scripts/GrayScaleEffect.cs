using UnityEngine;

[ExecuteInEditMode]
public class GrayscaleEffect : MonoBehaviour
{
    public Material grayscaleMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (grayscaleMaterial != null)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(source.width, source.height, 0);
            Graphics.Blit(source, tempRT, grayscaleMaterial);
            GrayscaleController[] controllers = FindObjectsOfType<GrayscaleController>();
            foreach (var obj in controllers)
            {
                if (obj.excludeFromGrayscale)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material originalMaterial = renderer.sharedMaterial;
                        Graphics.DrawMesh(
                            obj.GetComponent<MeshFilter>().sharedMesh,
                            obj.transform.localToWorldMatrix,
                            originalMaterial,
                            0,
                            Camera.current
                        );
                    }
                }
            }
            Graphics.Blit(tempRT, destination);
            RenderTexture.ReleaseTemporary(tempRT);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
