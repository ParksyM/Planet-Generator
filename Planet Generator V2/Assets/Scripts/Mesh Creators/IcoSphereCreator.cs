//Based on code written by Michael Garforth for the Unity Community Wiki
//Available at: http://wiki.unity3d.com/index.php/CreateIcoSphere
//Additional methods included to fix texture issues
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IcoSphereCreator {

    static int MAX_SUBDIVISIONS = 6;
    static float PHI = (1 + Mathf.Sqrt(5f)) / 2f;

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

	public static Mesh Create(int subdivisions, float radius)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Triangle> faces = new List<Triangle>();
        List<int> indices = new List<int>();
        //List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        //List<Vector4> tangents = new List<Vector4>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        if (subdivisions < 0)
        {
            subdivisions = 0;
            Debug.LogWarning("IcoSphere subdivisions set to 0 as minimum.");
        }
        else if (subdivisions > MAX_SUBDIVISIONS)
        {
            subdivisions = MAX_SUBDIVISIONS;
            Debug.LogWarning("IcoSphere subdivisions set to" + MAX_SUBDIVISIONS + " as maximum.");
        }

        //Create 12 vertices of icosahedron
        vertices.Add(new Vector3(-1f,  PHI, 0f).normalized);
        vertices.Add(new Vector3( 1f,  PHI, 0f).normalized);
        vertices.Add(new Vector3(-1f, -PHI, 0f).normalized);
        vertices.Add(new Vector3( 1f, -PHI, 0f).normalized);

        vertices.Add(new Vector3(0f, -1f,  PHI).normalized);
        vertices.Add(new Vector3(0f,  1f,  PHI).normalized);
        vertices.Add(new Vector3(0f, -1f, -PHI).normalized);
        vertices.Add(new Vector3(0f,  1f, -PHI).normalized);

        vertices.Add(new Vector3( PHI, 0f, -1f).normalized);
        vertices.Add(new Vector3( PHI, 0f,  1f).normalized);
        vertices.Add(new Vector3(-PHI, 0f, -1f).normalized);
        vertices.Add(new Vector3(-PHI, 0f,  1f).normalized);

        //Create 20 triangles of icosahedron
        faces.Add(new Triangle(0, 11,  5));
        faces.Add(new Triangle(0,  5,  1));
        faces.Add(new Triangle(0,  1,  7));
        faces.Add(new Triangle(0,  7, 10));
        faces.Add(new Triangle(0, 10, 11));

        //5 adjacent faces
        faces.Add(new Triangle( 1,  5, 9));
        faces.Add(new Triangle( 5, 11, 4));
        faces.Add(new Triangle(11, 10, 2));
        faces.Add(new Triangle(10,  7, 6));
        faces.Add(new Triangle( 7,  1, 8));

        //5 faces around point 3
        faces.Add(new Triangle(3, 9, 4));
        faces.Add(new Triangle(3, 4, 2));
        faces.Add(new Triangle(3, 2, 6));
        faces.Add(new Triangle(3, 6, 8));
        faces.Add(new Triangle(3, 8, 9));

        //5 adjacent faces
        faces.Add(new Triangle(4, 9,  5));
        faces.Add(new Triangle(2, 4, 11));
        faces.Add(new Triangle(6, 2, 10));
        faces.Add(new Triangle(8, 6,  7));
        faces.Add(new Triangle(9, 8,  1));

        //Refine triangles
        for(int i = 0; i < subdivisions; ++i)
        {
            List<Triangle> newFaces = new List<Triangle>();
            foreach(Triangle tri in faces)
            {
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertices, ref middlePointIndexCache);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertices, ref middlePointIndexCache);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertices, ref middlePointIndexCache);

                newFaces.Add(new Triangle(tri.v1, a, c));
                newFaces.Add(new Triangle(tri.v2, b, a));
                newFaces.Add(new Triangle(tri.v3, c, b));
                newFaces.Add(new Triangle(a, b, c));
            }
            faces = newFaces;
        }

        //Calculate UVs
        foreach (Vector3 vertex in vertices)
        {
            Vector3 unitVector = vertex.normalized;
            //float x = (Mathf.Atan2(unitVector.x, unitVector.z) + Mathf.PI) / Mathf.PI / 2f;
            //float y = (Mathf.Acos(unitVector.y) + Mathf.PI) / Mathf.PI - 1;

            //float x = 0.5f + Mathf.Atan2(unitVector.z, unitVector.x) / (2 * Mathf.PI);
            //float y = 0.5f - Mathf.Asin(unitVector.y) / Mathf.PI;

            float x = Mathf.Atan2(unitVector.x, unitVector.z) / (-2f * Mathf.PI) - 0.5f;
            if (x < 0f)
            {
                x += 1f;
            }
            float y = Mathf.Asin(unitVector.y) / Mathf.PI + 0.5f;
            uv.Add(new Vector2(x, y));
        }

        //Fix wrapping issues
        FixWrappedUV(ref vertices, ref faces, ref uv);
        FixSharedPoleVertices(ref vertices, ref faces, ref uv);

        //Calculate indices
        //int[] indices = new int[faces.Count * 3];
        foreach(Triangle face in faces)
        {
            indices.Add(face.v1);
            indices.Add(face.v2);
            indices.Add(face.v3);
        }

        //Calculate normals
        Vector3[] normals = new Vector3[vertices.Count];
        for(int i = 0; i < normals.Length; i++)
        {
            normals[i] = vertices[i].normalized;
        }
        //foreach (Vector3 vertex in vertices)
        //{
        //    normals.Add(vertex.normalized);
        //}

        //Calculate tangents
        Vector4[] tangents = new Vector4[vertices.Count];
        CalculateTangents(vertices, normals, uv, faces, tangents);

        //Set mesh data
        mesh.vertices = vertices.ToArray();
        //mesh.normals = normals.ToArray();
        mesh.normals = normals;
        mesh.uv = uv.ToArray();
        //mesh.triangles = indices.ToArray();
        mesh.triangles = indices.ToArray();
        //mesh.tangents = tangents.ToArray();
        mesh.tangents = tangents;

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache)
    {
        //Check if middle point exists already
        long smallIndex = Mathf.Min(p1, p2);
        long greatIndex = Mathf.Max(p1, p2);
        long key = (smallIndex << 32) + greatIndex;

        int ret;
        if(cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        //If not, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        //Add unit vertex
        int i = vertices.Count;
        vertices.Add(middle.normalized);

        //Cache and return index
        cache.Add(key, i);

        return i;
    }

    //https://mft-dev.dk/uv-mapping-sphere/
    private static int[] DetectWrappedUVCoordinates(List<Triangle> faces, List<Vector2> uv)
    {
        List<int> indices = new List<int>();
        for(int i = 0; i < faces.Count; i++)
        {
            int a = faces[i].v1;
            int b = faces[i].v2;
            int c = faces[i].v3;

            Vector3 texA = new Vector3(uv[a].x, uv[a].y, 0);
            Vector3 texB = new Vector3(uv[b].x, uv[b].y, 0);
            Vector3 texC = new Vector3(uv[c].x, uv[c].y, 0);
            Vector3 texNormal = -Vector3.Cross(texB - texA, texC - texA);
            if(texNormal.z < 0)
            {
                indices.Add(i);
            }
        }
        //Debug.Log("Wrapped UV coords: " + indices.Count);
        return indices.ToArray();
    }

    //Fixes the vertical texture seam
    //https://mft-dev.dk/uv-mapping-sphere/
    private static void FixWrappedUV(ref List<Vector3> vertices, ref List<Triangle> faces, ref List<Vector2> uv)
    {
        int vertexIndex = vertices.Count - 1;
        Dictionary<int, int> visited = new Dictionary<int, int>();

        int[] wrapped = DetectWrappedUVCoordinates(faces, uv);
        foreach(int i in wrapped)
        {
            int a = faces[i].v1;
            int b = faces[i].v2;
            int c = faces[i].v3;
            Vector3 vertA = vertices[a];
            Vector3 vertB = vertices[b];
            Vector3 vertC = vertices[c];
            Vector2 texA = uv[a];
            Vector2 texB = uv[b];
            Vector2 texC = uv[c];

            if(texA.x < 0.25f)
            {
                int tempA = a;
                if(!visited.TryGetValue(a, out tempA))
                {
                    texA.x += 1f;
                    vertices.Add(vertA);
                    uv.Add(texA);
                    vertexIndex++;
                    visited[a] = vertexIndex;
                    tempA = vertexIndex;
                }
                a = tempA;
            }

            if (texB.x < 0.25f)
            {
                int tempB = b;
                if (!visited.TryGetValue(b, out tempB))
                {
                    texB.x += 1f;
                    vertices.Add(vertB);
                    uv.Add(texB);
                    vertexIndex++;
                    visited[b] = vertexIndex;
                    tempB = vertexIndex;
                }
                b = tempB;
            }

            if (texC.x < 0.25f)
            {
                int tempC = c;
                if (!visited.TryGetValue(c, out tempC))
                {
                    texC.x += 1f;
                    vertices.Add(vertC);
                    uv.Add(texC);
                    vertexIndex++;
                    visited[c] = vertexIndex;
                    tempC = vertexIndex;
                }
                c = tempC;
            }

            faces[i] = new Triangle(a, b, c);
        }
    }

    //Fixes the texture wrapping problem at the poles
    //https://mft-dev.dk/uv-mapping-sphere/
    private static void FixSharedPoleVertices(ref List<Vector3> vertices, ref List<Triangle> faces, ref List<Vector2> uv)
    {
        Vector3 north = vertices.Find((v) => v.y == 1);
        //Debug.Log(north);
        Vector3 south = vertices.Find((v) => v.y == -1);
        //Debug.Log(south);
        int northIndex = vertices.IndexOf(north);
        int southIndex = vertices.IndexOf(south);
        int vertexIndex = vertices.Count - 1;

        for(int i = 0; i < faces.Count; i++)
        {
            Triangle newFace = faces[i];
            FixPoleFace(ref newFace, northIndex, ref vertexIndex, north, ref vertices, ref uv);
            FixPoleFace(ref newFace, southIndex, ref vertexIndex, south, ref vertices, ref uv);
            faces[i] = newFace;

            //if(faces[i].v1 == northIndex)
            //{
            //    //Debug.Log(faces[i].v1);
            //    //Vector3 A = vertices[faces[i].v1];
            //    //Vector2 texA = uv[faces[i].v1];
            //    Vector2 texB = uv[faces[i].v2];
            //    Vector2 texC = uv[faces[i].v3];
            //    Vector3 newNorth = north;
            //    Vector2 newUV = uv[vertices.IndexOf(north)];
            //    //Debug.Log("uv x is " + newUV.x);
            //    newUV = new Vector2((texB.x + texC.x) / 2f, newUV.y);
            //    //Debug.Log("Average of " + texB.x + " and " + texC.x + " is " + newUV.x);
            //    //Debug.Log(newUV);
            //    vertexIndex++;
            //    vertices.Add(newNorth);
            //    uv.Add(newUV);
            //    faces[i] = new Triangle(vertexIndex, faces[i].v2, faces[i].v3);
            //}
            //else if (faces[i].v2 == northIndex)
            //{
            //    //Debug.Log(faces[i].v2);
            //    //Vector3 A = vertices[faces[i].v1];
            //    //Vector2 texA = uv[faces[i].v1];
            //    Vector2 texB = uv[faces[i].v1];
            //    Vector2 texC = uv[faces[i].v3];
            //    Vector3 newNorth = north;
            //    Vector2 newUV = uv[vertices.IndexOf(north)];

            //    //Debug.Log("uv x is " + newUV.x);
            //    newUV = new Vector2((texB.x + texC.x) / 2f, newUV.y);
            //    //Debug.Log("Average of " + texB.x + " and " + texC.x + " is " + newUV.x);
            //    //Debug.Log(newUV);
            //    vertexIndex++;
            //    vertices.Add(newNorth);
            //    uv.Add(newUV);
            //    faces[i] = new Triangle(faces[i].v1, vertexIndex, faces[i].v3);
            //}
            //else if (faces[i].v3 == northIndex)
            //{
            //    //Debug.Log(faces[i].v3);
            //    //Vector3 A = vertices[faces[i].v1];
            //    //Vector2 texA = uv[faces[i].v1];
            //    Vector2 texB = uv[faces[i].v1];
            //    Vector2 texC = uv[faces[i].v2];
            //    Vector3 newNorth = north;
            //    Vector2 newUV = uv[vertices.IndexOf(north)];

            //    newUV = new Vector2((texB.x + texC.x) / 2f, newUV.y);
            //    //Debug.Log("uv x is " + newUV.x);
            //    //Debug.Log("Average of " + texB.x + " and " + texC.x + " is " + newUV.x);
            //    //Debug.Log(newUV);
            //    vertexIndex++;
            //    vertices.Add(newNorth);
            //    uv.Add(newUV);
            //    faces[i] = new Triangle(faces[i].v1, faces[i].v2, vertexIndex);
            //}
            //else if (faces[i].v1 == southIndex)
            //{
            //    //Debug.Log("Yep 2");
            //    //Vector3 A = vertices[faces[i].v1];
            //    //Vector2 texA = uv[faces[i].v1];
            //    Vector2 texB = uv[faces[i].v2];
            //    Vector2 texC = uv[faces[i].v3];
            //    Vector3 newSouth = south;
            //    Vector2 newUV = uv[vertices.IndexOf(south)];

            //    newUV = new Vector2((texB.x + texC.x) / 2f, newUV.y);
            //    vertexIndex++;
            //    vertices.Add(newSouth);
            //    uv.Add(newUV);
            //    faces[i] = new Triangle(vertexIndex, faces[i].v2, faces[i].v3);
            //}
            //else if (faces[i].v2 == southIndex)
            //{
            //    //Debug.Log("Yep 2");
            //    //Vector3 A = vertices[faces[i].v1];
            //    //Vector2 texA = uv[faces[i].v1];
            //    Vector2 texB = uv[faces[i].v1];
            //    Vector2 texC = uv[faces[i].v3];
            //    Vector3 newSouth = south;
            //    Vector2 newUV = uv[vertices.IndexOf(south)];

            //    newUV = new Vector2((texB.x + texC.x) / 2f, newUV.y);
            //    vertexIndex++;
            //    vertices.Add(newSouth);
            //    uv.Add(newUV);
            //    faces[i] = new Triangle(faces[i].v1, vertexIndex, faces[i].v3);
            //}
            //else if (faces[i].v3 == southIndex)
            //{
            //    //Debug.Log("Yep 2");
            //    //Vector3 A = vertices[faces[i].v1];
            //    //Vector2 texA = uv[faces[i].v1];
            //    Vector2 texB = uv[faces[i].v1];
            //    Vector2 texC = uv[faces[i].v2];
            //    Vector3 newSouth = south;
            //    Vector2 newUV = uv[vertices.IndexOf(south)];

            //    newUV = new Vector2((texB.x + texC.x) / 2f, newUV.y);
            //    vertexIndex++;
            //    vertices.Add(newSouth);
            //    uv.Add(newUV);
            //    faces[i] = new Triangle(faces[i].v1, faces[i].v2, vertexIndex);
            //}
        }
    }

    private static void FixPoleFace(ref Triangle face, int poleIndex, ref int vertexIndex, Vector3 poleVertex, ref List<Vector3> vertices, ref List<Vector2> uv)
    {
        if(face.v1 == poleIndex)
        {
            Vector2 tex1 = uv[face.v2];
            Vector2 tex2 = uv[face.v3];
            Vector3 newPole = poleVertex;
            Vector2 newUV = uv[vertices.IndexOf(poleVertex)];

            newUV = new Vector2((tex1.x + tex2.x) / 2f, newUV.y);
            vertexIndex++;
            vertices.Add(newPole);
            uv.Add(newUV);
            face = new Triangle(vertexIndex, face.v2, face.v3);
        }
        else if (face.v2 == poleIndex)
        {
            Vector2 tex1 = uv[face.v1];
            Vector2 tex2 = uv[face.v3];
            Vector3 newPole = poleVertex;
            Vector2 newUV = uv[vertices.IndexOf(poleVertex)];

            newUV = new Vector2((tex1.x + tex2.x) / 2f, newUV.y);
            vertexIndex++;
            vertices.Add(newPole);
            uv.Add(newUV);
            face = new Triangle(face.v1, vertexIndex, face.v3);
        }
        else if (face.v3 == poleIndex)
        {
            Vector2 tex1 = uv[face.v1];
            Vector2 tex2 = uv[face.v2];
            Vector3 newPole = poleVertex;
            Vector2 newUV = uv[vertices.IndexOf(poleVertex)];

            newUV = new Vector2((tex1.x + tex2.x) / 2f, newUV.y);
            vertexIndex++;
            vertices.Add(newPole);
            uv.Add(newUV);
            face = new Triangle(face.v1, face.v2, vertexIndex);
        }
    }

    //http://answers.unity3d.com/comments/190515/view.html
    private static void CalculateTangents(List<Vector3> vertices, Vector3[] normals, List<Vector2> uv, List<Triangle> faces, Vector4[] tangents)
    {
        Vector3[] tan1 = new Vector3[vertices.Count];
        Vector3[] tan2 = new Vector3[vertices.Count];

        for(int i = 0; i < faces.Count; i++)
        {
            int i1 = faces[i].v1;
            int i2 = faces[i].v2;
            int i3 = faces[i].v3;

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 u1 = uv[i1];
            Vector2 u2 = uv[i2];
            Vector2 u3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = u2.x - u1.x;
            float s2 = u3.x - u1.x;
            float t1 = u2.y - u1.y;
            float t2 = u3.y - u1.y;

            float div = s1 * t2 - s2 * t1;
            float r = div == 0f ? 0f : 1f / div;

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, 
                                       (t2 * y1 - t1 * y2) * r, 
                                       (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r,
                                       (s1 * y2 - s2 * y1) * r,
                                       (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for(int i = 0; i < vertices.Count; i++)
        {
            Vector3 n = normals[i];
            Vector3 t = tan1[i];

            Vector3.OrthoNormalize(ref n, ref t);
            Vector4 tangent = new Vector4();

            tangent.x = t.x;
            tangent.y = t.y;
            tangent.z = t.z;
            tangent.w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0f) ? -1f : 1f;
            tangents[i] = tangent;
            //tangents.Add(tangent);
        }
    }
}
