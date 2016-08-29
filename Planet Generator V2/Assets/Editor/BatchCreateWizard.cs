using UnityEditor;
using UnityEngine;
using System.Collections;

public class BatchCreateWizard : ScriptableWizard {

	public enum MeshType
    {
        IcoSphere,
        OctahedronSphere,
        Ring
    }

    [MenuItem("Assets/Create/Custom Mesh/Batch Create")]
    private static void CreateWizard()
    {
        DisplayWizard<BatchCreateWizard>("Batch Create");
    }

    public MeshType meshType;
    public string folderPath = "Assets/Meshes/Batched";

    [Header("IcoSphere settings")]
    [Range(0, 6)]
    public int icoSphereLevelMin = 0;
    [Range(0, 6)]
    public int icoSphereLevelMax = 6;
    public float icoSphereRadius = 1;
    public bool icoSphereNoSharedVertices = false;

    [Header("Octahedron Sphere settings")]
    [Range(0, 6)]
    public int octahedronSphereLevelMin = 0;
    [Range(0, 6)]
    public int octahedronSphereLevelMax = 6;
    public float octahedronSphereRadius = 1;
    public bool octahedronSphereNoSharedVertices = false;

    [Header("Ring settings")]
    public int ringSidesMin = 3;
    public int ringSidesMax = 10;
    public float ringRadius = 1;
    public bool ringHollow = true;
    [Range(0, 1)]
    public float ringWidth = 0.5f;
    
    private void OnWizardCreate()
    {
        switch (meshType)
        {
            case MeshType.IcoSphere:
                BatchIcoSphere();
                break;
            case MeshType.OctahedronSphere:
                BatchOctahedronSphere();
                break;
            case MeshType.Ring:
                BatchRing();
                break;
            default:
                break;
        }
    }

    void BatchIcoSphere()
    {
        string meshName;
        string newPath;

        if (AssetDatabase.IsValidFolder(folderPath + "/IcoSphere"))
        {
            newPath = folderPath + "/IcoSphere";
        }
        else
        {
            string guid = AssetDatabase.CreateFolder(folderPath, "IcoSphere");
            newPath = AssetDatabase.GUIDToAssetPath(guid);
        }

        if(newPath.Length > 0)
        {
            for(int i = icoSphereLevelMin; i <= icoSphereLevelMax; i++)
            {
                Mesh mesh;
                meshName = "IcoSphere Level " + i;
                if (icoSphereNoSharedVertices)
                {
                    meshName += " (nsv)";
                }

                string meshPath = newPath + "/" + meshName + ".asset";

                mesh = IcoSphereCreator.Create(i, icoSphereRadius);
                if (icoSphereNoSharedVertices)
                {
                    NoSharedVertices.NoShared(mesh);
                }

                MeshUtility.Optimize(mesh);
                AssetDatabase.CreateAsset(mesh, meshPath);
            }

            Debug.Log(icoSphereLevelMax - icoSphereLevelMin + 1 + " assets written to " + newPath);
        }
    }

    void BatchOctahedronSphere()
    {
        string meshName;
        string newPath;

        if (AssetDatabase.IsValidFolder(folderPath + "/OctahedronSphere"))
        {
            newPath = folderPath + "/OctahedronSphere";
        }
        else
        {
            string guid = AssetDatabase.CreateFolder(folderPath, "OctahedronSphere");
            newPath = AssetDatabase.GUIDToAssetPath(guid);
        }

        if (newPath.Length > 0)
        {
            for (int i = octahedronSphereLevelMin; i <= octahedronSphereLevelMax; i++)
            {
                Mesh mesh;
                meshName = "octahedronSphere Level " + i;
                if (octahedronSphereNoSharedVertices)
                {
                    meshName += " (nsv)";
                }

                string meshPath = newPath + "/" + meshName + ".asset";

                mesh = OctahedronSphereCreator.Create(i, octahedronSphereRadius);
                if (octahedronSphereNoSharedVertices)
                {
                    NoSharedVertices.NoShared(mesh);
                }

                MeshUtility.Optimize(mesh);
                AssetDatabase.CreateAsset(mesh, meshPath);
            }

            Debug.Log(octahedronSphereLevelMax - octahedronSphereLevelMin + 1 + " assets written to " + newPath);
        }
    }

    void BatchRing()
    {
        string meshName;
        string newPath;
        
        if (AssetDatabase.IsValidFolder(folderPath + "/Ring"))
        {
            newPath = folderPath + "/Ring";
        }
        else
        {
            string guid = AssetDatabase.CreateFolder(folderPath, "Ring");
            newPath = AssetDatabase.GUIDToAssetPath(guid);
        }
        

        if (newPath.Length > 0)
        {
            for (int i = ringSidesMin; i <= ringSidesMax; i++)
            {
                Mesh mesh;
                meshName = "Ring (" + i + " sides)";

                if (ringHollow)
                {
                    meshName = "Hollow " + meshName;
                    mesh = RingCreator.CreateHollow(i, ringRadius, ringWidth);
                }
                else
                {
                    mesh = RingCreator.Create(i, ringRadius);
                }

                string meshPath = newPath + "/" + meshName + ".asset";

                MeshUtility.Optimize(mesh);
                AssetDatabase.CreateAsset(mesh, meshPath);
            }

            Debug.Log(ringSidesMax - ringSidesMin + 1 + " assets written to " + newPath);
        }
    }

    void OnValidate()
    {
        ringSidesMin = Mathf.Max(ringSidesMin, 3);
        ringSidesMax = Mathf.Max(ringSidesMax, 3);
        ringRadius = Mathf.Max(ringRadius, 0);
        ringHollow = ringWidth != 1;

        icoSphereRadius = Mathf.Max(icoSphereRadius, 0);
        octahedronSphereRadius = Mathf.Max(octahedronSphereRadius, 0);

        if (icoSphereNoSharedVertices && icoSphereLevelMax > 5)
        {
            icoSphereLevelMax = 5;
        }

        if (octahedronSphereNoSharedVertices && octahedronSphereLevelMax > 5)
        {
            octahedronSphereLevelMax = 5;
        }

        ringSidesMax = Mathf.Max(ringSidesMax, ringSidesMin);
        icoSphereLevelMin = Mathf.Min(icoSphereLevelMin, icoSphereLevelMax);
        octahedronSphereLevelMin = Mathf.Min(octahedronSphereLevelMin, octahedronSphereLevelMax);
    }
}
