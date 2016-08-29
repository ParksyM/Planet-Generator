//Based on code written by Sebastian Lague as part of his tutorial series
//Available at: https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

	public static float[,] GenerateFalloffMap(int size, float a, float b)
    {
        float[,] map = new float[size, size];

        for(int i= 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value, a, b);
            }
        }

        return map;
    }

    public static float[,] GenerateFalloffMap1D(int size, float a, float b)
    {
        float[,] map = new float[size, size];

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                //Use j to generate falloff along the top and bottom edges
                //Use i to generate falloff along the left and right edges
                float value = j / (float)size * 2 - 1;
                map[i, j] = Evaluate(Mathf.Abs(value), a, b);
            }
        }

        return map;
    }

    private static float Evaluate(float value, float a, float b)
    {
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
