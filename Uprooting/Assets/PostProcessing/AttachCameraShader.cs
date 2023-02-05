using UnityEngine;

[ExecuteInEditMode]
public class AttachCameraShader : MonoBehaviour
{
    [SerializeField]
    private Material material;

    public void Start()
    {
        if (!Application.isPlaying)
        {
            material.SetFloat("_BlackRadius", 1f);
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
