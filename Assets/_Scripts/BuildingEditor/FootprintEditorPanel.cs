using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FootprintEditorPanel {
  private readonly BuildingWindow win;

  public FootprintEditorPanel (BuildingWindow w)
    => win = w;

  public void OnGUI() {
    EditorGUILayout.LabelField("2) Footprint Settings", EditorStyles.boldLabel);
    win.footprintMap = (Tilemap)EditorGUILayout.ObjectField("Footprint Tilemap", win.footprintMap, typeof(Tilemap), true);
    win.footprintTile = (TileBase)EditorGUILayout.ObjectField("Footprint Tile", win.footprintTile, typeof(TileBase), false);

    EditorGUILayout.BeginHorizontal();

    if (GUILayout.Button("Clear Footprint"))
      win.footprintMap?.ClearAllTiles();

    if (GUILayout.Button("Spawn Preview"))
      BuildingUtility.SpawnPreview(win);

    EditorGUILayout.EndHorizontal();

    EditorGUILayout.Space();
  }
}