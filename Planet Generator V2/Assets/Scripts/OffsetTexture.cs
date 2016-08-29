using UnityEngine;
using System.Collections;

public class OffsetTexture : MonoBehaviour {

    public Vector2 offsetRate;
    public Renderer textureRenderer;
    private Material material;
    private Vector2 offset;

	// Use this for initialization
	void Start () {
        textureRenderer = GetComponent<Renderer>();
        material = textureRenderer.sharedMaterial;
        offset = new Vector2();
	}
	
	// Update is called once per frame
	void Update () {
        offset += offsetRate * Time.deltaTime;
        material.SetTextureOffset("_MainTex", offset);
	}
}
