//Based on code written by Sebastian Lague as part of his tutorial series
//Available at: https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

    public Renderer[] textureRenderers;
    public Vector2 offsetRate;
    private Vector2 offset;
    void Start()
    {
        offset = new Vector2();
    }

    public void DrawTexture(Texture2D texture)
    {
        //Set texture for each supplied renderer
        foreach(Renderer textureRenderer in textureRenderers)
        {
            textureRenderer.material.mainTexture = texture;
            //textureRenderer.sharedMaterial.mainTexture = texture;
        }
    }

    void Update()
    {
        //Increase the textures offset
        offset += offsetRate * Time.deltaTime;
        foreach(Renderer textureRenderer in textureRenderers)
        {
            textureRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
    }
}
