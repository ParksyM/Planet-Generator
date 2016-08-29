using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class Planet : MonoBehaviour {

    public Mesh mesh;
    public Planet[] moons;
    public Ring[] rings;
    public float[] ringRotationSpeeds;

    public GameObject ring;

    public Vector3 axis;

	// Use this for initialization
	void Start () {

        //Create a new ring object
        GameObject o = new GameObject("Ring 0");
        MeshFilter ringMesh = o.AddComponent<MeshFilter>();
        o.AddComponent<MeshRenderer>();
        ringMesh.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Meshes/Batched/Ring/Hollow Ring (8 sides).asset", typeof(Mesh));
        o.AddComponent<Ring>();
        o.GetComponent<MeshRenderer>().material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Ring.mat", typeof(Material));
        //Center the ring around the planet
        o.transform.position = transform.position;
        o.transform.rotation = transform.rotation;
        //Attach the ring
        o.transform.parent = transform;
        o.transform.localScale = new Vector3(3, 1, 3);

        o.GetComponent<Ring>().rotationSpeed = 1;


        //GameObject newRing = Instantiate(ring);
        //newRing.transform.position = transform.position;
        //newRing.transform.rotation = transform.rotation;
        //newRing.transform.parent = transform;
        //newRing.transform.localScale = new Vector3(3, 1, 3);
        //newRing.GetComponent<Ring>().rotationSpeed = 1;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
