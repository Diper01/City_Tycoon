using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingWindow : EditorWindow {
  public Tilemap footprintMap;
  public TileBase footprintTile;
  public BuildingData buildingData;
  public string assetName = "NewBuildingData";

  public GameObject previewPrefab;
  public BuildCategory category;
  public Vector2Int pivot;

  public BuildingData previewData;
  public GameObject previewInstance;

  private Vector2 scroll;
  private AssetSelectionPanel assetPanel;
  private FootprintEditorPanel footprintPanel;
  private PreviewEditorPanel previewPanel;
  private UpgradesEditorPanel upgradesPanel;

  [MenuItem("Tools/Building Composer")]
  public static void ShowWindow() {
    GetWindow<BuildingWindow>("Building Composer");
  }

  private void OnEnable() {
    assetPanel = new AssetSelectionPanel(this);
    footprintPanel = new FootprintEditorPanel(this);
    previewPanel = new PreviewEditorPanel(this);
    upgradesPanel = new UpgradesEditorPanel(this);

    assetPanel.Refresh();
    titleContent = new GUIContent("Building Composer");
  }

  private void OnGUI() {
    scroll = EditorGUILayout.BeginScrollView(scroll);

    assetPanel.OnGUI();
    footprintPanel.OnGUI();
    previewPanel.OnGUI();

    if (previewPanel.HasPreview)
      upgradesPanel.OnGUI();

    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
    EditorGUILayout.BeginHorizontal();
    EditorGUI.BeginDisabledGroup(buildingData == null);

    if (GUILayout.Button("Update Selected"))
      BuildingUtility.SaveBuildingData(this, overwrite: true);

    EditorGUI.EndDisabledGroup();

    if (GUILayout.Button("Save As New"))
      BuildingUtility.SaveBuildingData(this, overwrite: false);

    EditorGUILayout.EndHorizontal();

    EditorGUILayout.EndScrollView();
  }

  public void LoadIntoEditor() {
    if (buildingData == null)
      return;

    previewPrefab = buildingData.prefab;
    category = buildingData.category;
    pivot = buildingData.pivot;
    assetName = buildingData.name;
    BuildingUtility.SpawnPreview(this);

    footprintMap.ClearAllTiles();

    foreach (var cell in buildingData.footprint) {
      var world = footprintMap.CellToWorld(new Vector3Int(cell.x + pivot.x, cell.y + pivot.y, 0));
      footprintMap.SetTile(footprintMap.WorldToCell(world), footprintTile);
    }

    BuildingUtility.SpawnPreview(this);
  }
}