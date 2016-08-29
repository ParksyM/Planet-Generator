using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public Vector3 rotateVector = new Vector3(0f, 30f, 0f);

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateVector * Time.deltaTime);
	}
}
