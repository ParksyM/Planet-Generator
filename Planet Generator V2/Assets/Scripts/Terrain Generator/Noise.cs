//Based on code written by Sebastian Lague as part of his tutorial series
//Available at: https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GeneratePerlinNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance , float lacunarity, Vector2 offset)
    {
        float[,] map = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                map[x, y] = noiseHeight;
            }
        }

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
            }
        }

        return map;
    }

    public static float[,] GenerateSimplex2DNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, SimplexNoise simplexNoise)
    {
        //SimplexNoise sNoise = new SimplexNoise(seed);
        float[,] map = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = simplexNoise.Evaluate(sampleX, sampleY);
                    //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                map[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
            }
        }

        return map;
    }

    public static float[,] GenerateSimplex3DNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, SimplexNoise simplexNoise)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //SimplexNoise sNoise = new SimplexNoise(seed);
        float[,] map = new float[width, height];
        
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float heightValue = 0f;
                float amplitude = 1;
                float frequency = 1;

                for (int i = 0; i < octaves; i++)
                {
                    float x1 = 0, x2 = 1;
                    float y1 = 0, y2 = 1;
                    float dx = x2 - x1;
                    float dy = y2 - y1;

                    float s = x / (float)width;
                    float t = y / (float)height;

                    s /= scale * frequency + octaveOffsets[i].x;
                    t /= scale * frequency + octaveOffsets[i].y;

                    float nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI); // / scale * frequency + octaveOffsets[i].x;
                    float ny = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);// / scale * frequency;// + octaveOffsets[i].x;
                    float nz = t;// / scale * frequency + octaveOffsets[i].y;

                    float floatValue = simplexNoise.Evaluate(nx, ny, nz);
                    heightValue += floatValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;

                }
                if (heightValue > maxNoiseHeight)
                {
                    maxNoiseHeight = heightValue;
                }
                if (heightValue < minNoiseHeight)
                {
                    minNoiseHeight = heightValue;
                }

                map[x, y] = heightValue;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
            }
        }

        return map;
    }

    public static float[,] GenerateSimplex4DNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, SimplexNoise simplexNoise)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //SimplexNoise sNoise = new SimplexNoise(seed);
        float[,] map = new float[width, height];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = 0f;
                float amplitude = 1;
                float frequency = 1;

                for (int i = 0; i < octaves; i++)
                {
                    float x1 = 0, x2 = 2;
                    float y1 = 0, y2 = 2;
                    float dx = x2 - x1;
                    float dy = y2 - y1;

                    float s = x / (float)width + octaveOffsets[i].x;
                    float t = y / (float)height + octaveOffsets[i].y;

                    float nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI) * scale * frequency + octaveOffsets[i].x;
                    float ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI) * scale * frequency + octaveOffsets[i].y;
                    float nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI) * scale * frequency + octaveOffsets[i].y;
                    float nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI) * scale * frequency + octaveOffsets[i].x;

                    float floatValue = simplexNoise.Evaluate(nx, ny, nz, nw);
                    heightValue += floatValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;

                }
                if (heightValue > maxNoiseHeight)
                {
                    maxNoiseHeight = heightValue;
                }
                if (heightValue < minNoiseHeight)
                {
                    minNoiseHeight = heightValue;
                }

                map[x, y] = heightValue;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
            }
        }

        return map;
    }
}
