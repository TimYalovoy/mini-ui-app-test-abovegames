using MainApp.Configs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ServerConfig))]
public class ServerConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(20);

        ServerConfig config = (ServerConfig)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate JSON", GUILayout.Height(30)))
        {
            GenerateJsonFile(config);
        }

        if (GUILayout.Button("Load from JSON", GUILayout.Height(30)))
        {
            LoadFromJsonFile(config);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "Generate JSON: Creates a JSON file from this config\n" +
            "Load from JSON: Updates this config from a JSON file",
            MessageType.Info
        );
    }

    private void GenerateJsonFile(ServerConfig config)
    {
        var data = new
        {
            config.DomenUrl,
            config.FolderUrl,
            config.RequestDelay,
            config.IsCaching,
            config.IsClearCachingOnStart
        };

        string json = JsonUtility.ToJson(data, true);

        string path = EditorUtility.SaveFilePanel(
            "Save Server Config as JSON",
            Application.streamingAssetsPath,
            $"{config.name}_server_config.json",
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            Debug.Log($"Config saved to: {path}");

            if (path.StartsWith(Application.streamingAssetsPath))
            {
                string relativePath = "StreamingAssets" + path.Substring(Application.streamingAssetsPath.Length);
                AssetDatabase.ImportAsset(relativePath);
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath));
            }
        }
    }

    private void LoadFromJsonFile(ServerConfig config)
    {
        string path = EditorUtility.OpenFilePanel(
            "Load Server Config from JSON",
            Application.streamingAssetsPath,
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            string json = File.ReadAllText(path);

            JsonUtility.FromJsonOverwrite(json, config);

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            Debug.Log($"Config loaded from: {path}");
        }
    }
}
