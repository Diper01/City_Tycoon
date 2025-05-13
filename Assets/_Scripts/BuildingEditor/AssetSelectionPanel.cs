using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class AssetSelectionPanel {
  private BuildingWindow win;
  private string [] guids;
  private string [] names;
  private int selectedIndex = -1;

  public AssetSelectionPanel (BuildingWindow w)
    => win = w;

  public void Refresh() {
    guids = AssetDatabase.FindAssets("t:BuildingData", new [] {
      "Assets/Resources/Data/Buildings"
    });

    names = guids.Select(g => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(g))).ToArray();

    if (names.Length > 0)
      selectedIndex = Mathf.Clamp(selectedIndex, 0, names.Length - 1);
    else
      selectedIndex = -1;
  }

  public void OnGUI() {
    EditorGUILayout.LabelField("1) Load / Create BuildingData", EditorStyles.boldLabel);
    EditorGUILayout.BeginHorizontal();

    if (names != null && names.Length > 0) {
      selectedIndex = EditorGUILayout.Popup(selectedIndex, names);

      if (GUILayout.Button("Load", GUILayout.Width(50))) {
        var path = AssetDatabase.GUIDToAssetPath(guids[selectedIndex]);
        win.buildingData = AssetDatabase.LoadAssetAtPath<BuildingData>(path);
        win.LoadIntoEditor();
        Debug.Log($"Loaded {win.buildingData.name}");
      }
    } else {
      EditorGUILayout.HelpBox("No BuildingData found", MessageType.Info);
    }

    EditorGUILayout.EndHorizontal();

    win.assetName = EditorGUILayout.TextField("New Asset Name", win.assetName);

    if (GUILayout.Button("Create New", GUILayout.Width(100))) {
      win.buildingData = null;
      win.previewData = null;
      win.LoadIntoEditor();
    }

    if (GUILayout.Button("Refresh Asset List", GUILayout.Width(150)))
      Refresh();

    EditorGUILayout.Space();
  }
}