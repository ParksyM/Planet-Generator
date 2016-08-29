//Based on code written by Sebastian Lague as part of his tutorial series
//Available at: https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode
    {
        NoiseMap,
        ColourMap,
        FalloffMap
    }

    public enum NoiseType
    {
        Perlin,
        Simplex2D,
        Simplex3D,
        Simplex4D
    }

    public NoiseType noiseType;

    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public bool useFalloff;
    [Range(1, 10)]
    public float falloffA = 3f;
    [Range(1, 10)]
    public float falloffB = 2.2f;

    public float regionOffset;
    public Color colourMain;
    public Color[] regionColours;
    public TerrainType[] regions;

    private float[,] falloffMap;

    private float[,] map;
    private Color[] colourMap;

    private SimplexNoise simplexNoise;

    void Start()
    {
        simplexNoise = new SimplexNoise(seed);
        GenerateMap();
    }

    public void GenerateMap()
    {
        if(simplexNoise == null)
        {
            Debug.Log("SimplexNoise not initiated.");
            simplexNoise = new SimplexNoise(seed);
        }
        //Generate noise map
        switch (noiseType)
        {
            case NoiseType.Perlin:
                map = Noise.GeneratePerlinNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
                break;
            case NoiseType.Simplex2D:
                map = Noise.GenerateSimplex2DNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset, simplexNoise);
                break;
            case NoiseType.Simplex3D:
                map = Noise.GenerateSimplex3DNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset, simplexNoise);
                break;
            case NoiseType.Simplex4D:
                map = Noise.GenerateSimplex4DNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset, simplexNoise);
                break;
            default:
                break;
        }
        //Should the falloff map be generated
        if (useFalloff || drawMode == DrawMode.FalloffMap)
        {
            falloffMap = FalloffGenerator.GenerateFalloffMap1D(mapWidth, falloffA, falloffB);
        }
        
        colourMap = new Color[mapWidth * mapHeight];

        //Set region colours
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                if (useFalloff)
                {
                    map[x, y] = Mathf.Clamp01(map[x, y] - falloffMap[x, y]);
                }
                float currentHeight = map[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        //Draw the map texture to set objects
        MapDisplay display = FindObjectOfType<MapDisplay>();
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(map));
                break;
            case DrawMode.ColourMap:
                display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
                break;
            case DrawMode.FalloffMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(falloffMap));
                break;
            default:
                break;
        }
    }

    void GenerateRegions()
    {
        //Get and set offset colours for region map
        regionColours = ColourPicker.GetRangeOffset(colourMain, regions.Length, regionOffset, seed);
        float regionSize = 1 / (float)regions.Length;
        for(int i = 0; i < regions.Length; i++)
        {
            regions[i].colour = regionColours[i];
            regions[i].height = regionSize * (i + 1);
            regions[i].name = "Region " + i;
        }
    }

    void OnValidate()
    {
        //Ensure variables remain above respective values
        mapWidth = Mathf.Max(mapWidth, 1);
        mapHeight = Mathf.Max(mapHeight, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        octaves = Mathf.Max(octaves, 0);
        noiseScale = Mathf.Max(noiseScale, 0.0001f);

        GenerateRegions();
    }

    public Texture2D GetCurrentMap()
    {
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                return TextureGenerator.TextureFromHeightMap(map);
            case DrawMode.ColourMap:
                return TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight);
            case DrawMode.FalloffMap:
                return TextureGenerator.TextureFromHeightMap(falloffMap);
            default:
                return null;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
