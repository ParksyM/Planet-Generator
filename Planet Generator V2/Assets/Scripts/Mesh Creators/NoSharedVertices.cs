/// <summary>
/// Based on code provided by robertbu:
/// http://answers.unity3d.com/questions/798510/flat-shading.html
/// </summary>

using UnityEngine;
using System.Collections;

public class NoSharedVertices {

    public static void NoShared(Mesh mesh)
    {
        Vector3[] oldVerts = mesh.vertices;
        //Vector3[] oldNormals = mesh.normals;
        Vector2[] oldUVs = mesh.uv;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = new Vector3[triangles.Length];
        Vector2[] uvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            vertices[i] = oldVerts[triangles[i]];
            uvs[i] = oldUVs[triangles[i]];
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
