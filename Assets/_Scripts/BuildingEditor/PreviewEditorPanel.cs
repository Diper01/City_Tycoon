using UnityEditor;
using UnityEngine;

public class PreviewEditorPanel {
  private readonly BuildingWindow win;

  public PreviewEditorPanel (BuildingWindow w)
    => win = w;

  public bool HasPreview
    => win.previewData != null && win.previewInstance != null;

  public void OnGUI() {
    EditorGUILayout.LabelField("3) Preview & Parameters", EditorStyles.boldLabel);

    win.previewPrefab = (GameObject)EditorGUILayout.ObjectField("Preview Prefab", win.previewPrefab, typeof(GameObject), false);
    win.category = (BuildCategory)EditorGUILayout.EnumPopup("Category", win.category);
    win.pivot = EditorGUILayout.Vector2IntField("Pivot (cell)", win.pivot);

    EditorGUILayout.Space();
  }
}