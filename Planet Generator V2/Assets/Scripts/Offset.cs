using UnityEngine;
using System.Collections;

public class Offset : MonoBehaviour {

    public Vector2 offsetRate;
    public MapGenerator mapGenerator;

    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    void Update()
    {
        //Scroll the map offset
        Vector2 offset = offsetRate * Time.deltaTime;
        mapGenerator.offset.x += offset.x;
        mapGenerator.offset.y += offset.y;
        mapGenerator.GenerateMap();
    }
}
