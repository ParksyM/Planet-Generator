using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingCreator {

    private struct Triangle
    {
        public int v1 { get; set; }
        public int v2 { get; set; }
        public int v3 { get; set; }

        public Triangle(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    public static Mesh CreateHollow(int sides, float radius, float width)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Triangle> faces = new List<Triangle>();
        List<int> triangles = new List<int>();

        float angle = 360 / (float)sides;
        float innerWidth = 1 - width;

        for(int i = 0; i < sides; i++)
        {
            int f = i * 4;

            Vector3 v1 = Quaternion.AngleAxis(angle * i, Vector3.up) * Vector3.forward * radius;
            Vector3 v2 = Quaternion.AngleAxis(angle * (i + 1), Vector3.up) * Vector3.forward * radius;
            Vector3 v0 = v1 * innerWidth;
            Vector3 v3 = v2 * innerWidth;

            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);

            normals.Add(v0.normalized);
            normals.Add(v1.normalized);
            normals.Add(v2.normalized);
            normals.Add(v3.normalized);

            uv.Add(new Vector2(1, 1));
            uv.Add(new Vector2(1, 0));
            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, 1));

            faces.Add(new Triangle(f, f + 1, f + 2));
            faces.Add(new Triangle(f, f + 2, f + 3));
        }

        foreach(Triangle tri in faces)
        {
            triangles.Add(tri.v1);
            triangles.Add(tri.v2);
            triangles.Add(tri.v3);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh = DoubleSided(mesh);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        return mesh;
    }

    public static Mesh Create(int sides, float radius)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        
        List<Vector2> uv = new List<Vector2>();
        Triangle[] faces = new Triangle[sides];
        List<int> indices = new List<int>();

        //Calculate inside angle of each edge
        float angle = 360 / (float)sides;

        //Add vertex and ring centre
        vertices.Add(new Vector3());

        //Calculate uv coordinate for centre vertex
        float theta = (180 - angle) / 2 * Mathf.Deg2Rad;
        float v = radius * Mathf.Cos(theta);

        uv.Add(new Vector2(0.5f, v));

        for (int i = 0; i < sides; i++)
        {
            Vector3 vector = Quaternion.AngleAxis(angle * i, Vector3.up) * Vector3.forward * radius;
            vertices.Add(vector);
            uv.Add(new Vector2());
            vertices.Add(vector);
            uv.Add(new Vector2(1, 0));
        }

        for(int i = 1; i < sides; i++)
        {
            faces[i - 1] = new Triangle(0, i * 2, i * 2 + 1);
        }
        faces[sides - 1] = new Triangle(0, vertices.Count - 1, 1);

        foreach (Triangle tri in faces)
        {
            indices.Add(tri.v1);
            indices.Add(tri.v2);
            indices.Add(tri.v3);
        }

        //Calculate normals
        Vector3[] normals = new Vector3[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            normals[i] = vertices[i].normalized;
        }

        //Set mesh data
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals;
        mesh.triangles = indices.ToArray();
        mesh.uv = uv.ToArray();

        //Double side the mesh to ensure visibility from both sides
        mesh = DoubleSided(mesh);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        return mesh;
    }

    public static Mesh DoubleSided(Mesh mesh)
    {
        //Double up the vertices and flip the faces, so that the ring is visible from both sides
        Vector3[] vertices = new Vector3[mesh.vertexCount * 2];
        Vector3[] normals = new Vector3[vertices.Length];
        int[] indices = new int[mesh.triangles.Length * 2];
        Vector2[] uv = new Vector2[mesh.uv.Length * 2];

        //Vertices
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            vertices[i] = vertices[i + mesh.vertexCount] = mesh.vertices[i];
        }

        //Normals
        for (int i = 0; i < mesh.normals.Length; i++)
        {
            normals[i] = mesh.normals[i];
            normals[i + mesh.normals.Length] = -normals[i];
        }

        //Faces
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            indices[i] = mesh.triangles[i];
            //indices[i + mesh.triangles.Length] = mesh.triangles[i] + mesh.vertexCount;
            indices[indices.Length - 1 - i] = mesh.triangles[i] + mesh.vertexCount;
        }

        //UVs
        for(int i = 0; i < mesh.uv.Length; i++)
        {
            uv[i] = mesh.uv[i];
            uv[i + mesh.uv.Length] = mesh.uv[i];
        }

        //Set mesh data and return
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = indices;
        mesh.uv = uv;

        return mesh;
    }
}
