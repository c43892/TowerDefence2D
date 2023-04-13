using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(FrameAni))]
public class FrameAniEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FrameAni ani = (FrameAni)target;

        if (GUILayout.Button("Load Sprites"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Sprites Folder", "Assets", "");

            if (!string.IsNullOrEmpty(path))
            {
                string[] files = System.IO.Directory.GetFiles(path, "*.png");
                Array.Sort(files);

                ani.Sprites = new Sprite[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    string localPath = files[i].Substring(files[i].IndexOf("Assets"));
                    ani.Sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(localPath);
                }

                var sr = ani.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = ani.Sprites[0];

                var img = ani.GetComponent<Image>();
                if (img != null)
                    img.sprite = ani.Sprites[0];
            }
        }
    }
}
