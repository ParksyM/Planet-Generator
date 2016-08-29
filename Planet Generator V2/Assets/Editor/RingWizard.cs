using UnityEditor;
using UnityEngine;
using System.Collections;

public class RingWizard : ScriptableWizard {

	[MenuItem("Assets/Create/Custom Mesh/Ring")]
    private static void CreateWizard()
    {
        DisplayWizard<RingWizard>("Create Ring");
    }

    //Sides of the ring, minimum 3
    public int sides = 3;
    //Ring radius. Currently scaled in scene
    public float radius = 1;
    //Width of ring if hollow
    [Range(0, 1)]
    public float width = 0.5f;
    public bool hollow = true;

    private void OnWizardCreate()
    {
        string meshName = "Ring (" + sides + " sides)";
        if (hollow)
        {
            meshName = "Hollow " + meshName;
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Ring",
            meshName,
            "asset",
            "Specify where to save the mesh.");

        if(path.Length > 0)
        {
            Mesh mesh;
            if (hollow)
            {
                mesh = RingCreator.CreateHollow(sides, radius, width);
            }
            else
            {
                mesh = RingCreator.Create(sides, radius);
            }

            MeshUtility.Optimize(mesh);
            AssetDatabase.CreateAsset(mesh, path);
            Selection.activeObject = mesh;
        }
    }

    void OnValidate()
    {
        sides = Mathf.Max(sides, 3);
        radius = Mathf.Max(radius, 0);
    }
}
