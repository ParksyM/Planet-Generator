using UnityEditor;
using UnityEngine;
using System.Collections;

public class IcoSphereWizard : ScriptableWizard {

    [MenuItem("Assets/Create/Custom Mesh/IcoSphere")]
    private static void CreateWizard()
    {
        DisplayWizard<IcoSphereWizard>("Create IcoSphere");
    }

    [Range(0, 6)]
    public int level = 1;
    public float radius = 1f;
    public bool noSharedVertices = false;

    private void OnWizardCreate()
    {
        string meshName = "IcoSphere Level " + level;
        if (noSharedVertices)
        {
            meshName += " (nsv)";
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Save IcoSphere",
            meshName,
            "asset",
            "Specify where to save the mesh.");

        if(path.Length > 0)
        {
            Mesh mesh = IcoSphereCreator.Create(level, radius);
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
