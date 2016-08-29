//Based on code written by Sebastian Lague as part of his tutorial series
//Available at: https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

[CanEditMultipleObjects, CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
        if (GUILayout.Button("Save"))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Texture",
                "texture",
                "png",
                "Specify where to save the texture");
            if(path.Length > 0)
            {
                Texture2D texture = mapGen.GetCurrentMap();
                byte[] texByte = texture.EncodeToPNG();
                File.WriteAllBytes(path, texByte);
                Selection.activeObject = texture;
            }
        }
    }
}
