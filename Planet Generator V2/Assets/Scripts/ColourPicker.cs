using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class ColourPicker : MonoBehaviour {

    public enum RangeType
    {
        Random,
        RandomOffset,

    }

    public Color mainColour;

    public RangeType rangeType;

    public Color oppositeColourRGB;
    public Color oppositeColourHSL;

    public float offset;
    //public GameObject other;

    public Color[] colourRange;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnValidate()
    {
        GetOppositeColour();

        switch (rangeType)
        {
            case RangeType.Random:
                GetRangeRandom();
                break;
            case RangeType.RandomOffset:
                GetRangeOffset();
                break;
            default:
                break;
        }
    }

    void GetOppositeColour()
    {
        oppositeColourRGB.r = 1 - mainColour.r;
        oppositeColourRGB.g = 1 - mainColour.g;
        oppositeColourRGB.b = 1 - mainColour.b;

        gameObject.GetComponent<Renderer>().material.color = mainColour;
        //other.GetComponent<Renderer>().material.color = oppositeColourRGB;

        HSLColor hsl = HSLColor.FromColor(mainColour);
        hsl.h += 180;
        if(hsl.h >= 360)
        {
            hsl.h -= 360;
        }
        //hsl.s = 1 - hsl.s;
        hsl.l = 1 - hsl.l;

        oppositeColourHSL = HSLColor.ToColor(hsl);
    }

    void GetRangeRandom()
    {
        for(int i = 0; i < colourRange.Length; i++)
        {
            Color newColour = new Color();

            newColour.r = Random.value;
            newColour.g = Random.value;
            newColour.b = Random.value;

            colourRange[i] = newColour;
        }
    }

    void GetRangeOffset()
    {
        for (int i = 0; i < colourRange.Length; i++)
        {
            Color newColour = new Color();

            newColour.r = mainColour.r + Random.Range(-offset, offset);
            newColour.g = mainColour.g + Random.Range(-offset, offset);
            newColour.b = mainColour.b + Random.Range(-offset, offset);

            colourRange[i] = newColour;
        }
    }

    public static Color[] GetRangeOffset(Color main, int size, float offset, int seed)
    {
        System.Random random = new System.Random(seed);

        Color[] range = new Color[size];

        for(int i = 0; i < range.Length; i++) 
        {
            //float newOffset = (float)random.NextDouble() * 2 - 1;
            float r = main.r + (float)random.NextDouble() * 2 * offset - offset;
            float g = main.g + (float)random.NextDouble() * 2 * offset - offset;
            float b = main.b + (float)random.NextDouble() * 2 * offset - offset;

            range[i] = new Color(r, g, b);
        }

        return range;
    }
}
