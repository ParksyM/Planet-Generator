//Currently unused, may be required for colour picking later on
using UnityEngine;
using System.Collections;

[System.Serializable]
public class HSLColor {

    /// <summary>
    /// hue in degrees
    /// </summary>
    public float h;
    /// <summary>
    /// saturation 
    /// </summary>
    public float s;
    /// <summary>
    /// luminance
    /// </summary>
    public float l;

    public float a;

    public HSLColor() : this(0, 0, 0, 1) { }

    public HSLColor(float h, float s, float l) : this(0, 0, 0, 1) { }

    public HSLColor(float h, float s, float l, float a)
    {
        this.h = h;
        this.s = s;
        this.l = l;
        this.a = a;
    }

    public static HSLColor FromRGB(float r, float g, float b)
    {
        return FromRGB(r, g, b, 1);
    }
    public static HSLColor FromRGB(float r, float g, float b, float a)
    {
        float h;
        float s;
        float l;

        float min = Mathf.Min(r, g, b);
        float max = Mathf.Max(r, g, b);

        l = (min + max) / 2;

        if (min == max)
        {
            return new HSLColor(0, 0, l, a);
        }

        if(l < 0.5f)
        {
            s = (max - min) / (max + min);
        }
        else
        {
            s = (max - min) / (2 - max - min); 
        }

        if (max == r)
        {
            h = (g - b) / (max - min);
        }
        else if (max == g)
        {
            h = 2 + (b - r) / (max - min);
        }
        else
        {
            h = 4 + (r - g) / (max - min);
        }

        h *= 60;
        if (h < 0)
        {
            h += 360;
        }

        return new HSLColor(h, s, l, a);
    }

    public static HSLColor FromColor(Color color)
    {
        return FromRGB(color.r, color.g, color.b, color.a);
    }

    public static Color ToColor(HSLColor hsl)
    {
        float r;
        float g;
        float b;

        float temp1;
        float temp2;
        float hue;
        float tempR;
        float tempG;
        float tempB;

        if(hsl.s == 0)
        {
            return new Color(hsl.l, hsl.l, hsl.l, hsl.a);
        }

        if(hsl.l < 0.5f)
        {
            temp1 = hsl.l * (1 + hsl.s);
        }
        else
        {
            temp1 = hsl.l + hsl.s - hsl.l * hsl.s;
        }

        temp2 = 2 * hsl.l - temp1;

        hue = hsl.h / 360;

        tempR = hue + 1 / 3f;
        tempG = hue;
        tempB = hue - 1 / 3f;

        r = HueCheck(tempR, temp1, temp2);
        g = HueCheck(tempG, temp1, temp2);
        b = HueCheck(tempB, temp1, temp2);

        return new Color(r, g, b, hsl.a);
    }

    static float HueCheck(float value, float temp1, float temp2)
    {
        if(value < 0)
        {
            value++;
        }
        if(value > 1)
        {
            value--;
        }

        if(value * 6 < 1)
        {
            return temp2 + (temp1 - temp2) * 6 * value;
        }
        else
        {
            if (value * 2 < 1)
            {
                return temp1;
            }
            else
            {
                if (value * 3 < 2)
                {
                    return temp2 + (temp1 - temp2) * (2 / 3f - value) * 6;
                }
                else
                {
                    return temp2;
                }
            }
        }
    }
}
