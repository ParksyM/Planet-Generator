/// <summary>
/// Written using tutorial available at:
/// https://www.binpress.com/tutorial/creating-an-octahedron-sphere/162
/// </summary>

using UnityEditor;
using UnityEngine;
using System.Collections;

public class OctahedronSphereWizard : ScriptableWizard {

    [MenuItem("Assets/Create/Custom Mesh/Octahedron Sphere")]
    private static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<OctahedronSphereWizard>("Create Octahedron Sphere");
    }

    [Range(0, 6)]
    public int level = 6;
    public float radius = 1f;
    public bool noSharedVertices = false;

    private void OnWizardCreate()
    {
        string meshName = "Octahedron Sphere Level " + level;
        if (noSharedVertices)
        {
            meshName += " (no shared vertices)";
        }
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Octahedron Sphere",
            meshName,
            "asset",
            "Specify where to save the mesh.");
        if (path.Length > 0)
        {
            Mesh mesh = OctahedronSphereCreator.Create(level, radius);
            if (noSharedVertices)
            {
                NoSharedVertices.NoShared(mesh);
            }
            MeshUtility.Optimize(mesh);
            AssetDatabase.CreateAsset(mesh, path);
            Selection.activeObject = mesh;
        }
    }
}
