using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EntityDustParticle : MonoBehaviour
{
    public ParticleSystem dustParticle;
    [SerializeField] private PlayerMovement playerMovement;
    public Color dustColor;
    public Camera cam;
    private RenderTexture renderTexture;
    private Texture2D texture2D;

    private void Start() {
        dustParticle = GetComponent<ParticleSystem>();

        var main = dustParticle.main;
        main.startColor = dustColor;

        renderTexture = new RenderTexture(1, 1, 24);
        texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(gameObject.transform.position);

            RenderTexture.active = renderTexture;

             // Read the pixel from the source Rect (just one pixel at 0, 0 of our 1x1 texture)
            texture2D.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
            texture2D.Apply(); // Apply the changes to the Texture2D
            
            // Get the pixel color
            dustColor = texture2D.GetPixel(0, 0);
            
            // Clean up
            RenderTexture.active = null; 
        }
        else
        {
            Debug.LogWarning("Camera reference is missing for EntityDustParticle.");
        }
    }

}
