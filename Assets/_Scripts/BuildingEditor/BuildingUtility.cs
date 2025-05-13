using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class BuildingUtility {

  private static FootprintManager footprintManager;

  public static void SpawnPreview (BuildingWindow win) {
    if (win.previewInstance)
      Object.DestroyImmediate(win.previewInstance);

    if (win.buildingData == null) {
      win.previewData = ScriptableObject.CreateInstance<BuildingData>();
      win.previewData.footprint = new List<Vector2Int>();
      win.previewData.upgrades = new List<UpgradeStep>();
    } else {
      win.previewData = win.buildingData;
    }

    win.previewData.prefab = win.previewPrefab;
    win.previewData.category = win.category;
    win.previewData.pivot = win.pivot;

    win.previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(win.previewData.prefab);
    win.previewInstance.name = "Preview_" + win.assetName;

    var inst = win.previewInstance.GetComponent<BuildingInstance>() ?? win.previewInstance.AddComponent<BuildingInstance>();
    inst.Init(win.previewData, new List<Vector3Int>());
    inst.data = win.previewData;


    if (win.previewData.upgrades != null) {
      for (int lvl = 0; lvl < win.previewData.upgrades.Count; lvl++) {
        var step = win.previewData.upgrades[lvl];

        if (step.details == null)
          continue;

        for (int j = 0; j < step.details.Count; j++) {
          var d = step.details[j];
          var child = (GameObject)PrefabUtility.InstantiatePrefab(d.prefab, win.previewInstance.transform);
          child.name = $"L{lvl}_D{j}";

          child.transform.localPosition = d.localPosition;
          child.transform.localEulerAngles = d.localEulerAngles;
          child.transform.localScale = d.localScale;

          child.SetActive(!d.isHidden);
        }
      }
    }


    Selection.activeGameObject = win.previewInstance;
    Tools.current = Tool.Move;
  }

  public static void SaveBuildingData (BuildingWindow win, bool overwrite) {
    if (win.footprintMap == null || win.previewData == null || win.previewInstance == null) {
      Debug.LogError("[SaveBuildingData] missing footprintMap, previewData or previewInstance.");
      return;
    }

    const string root = "Assets/Resources/Data";
    const string dir = root + "/Buildings";

    if (!AssetDatabase.IsValidFolder(root))
      AssetDatabase.CreateFolder("Assets", "Data");

    if (!AssetDatabase.IsValidFolder(dir))
      AssetDatabase.CreateFolder(root, "Buildings");

    footprintManager ??= GameObject.FindObjectOfType<FootprintManager>();
    var cells = footprintManager.GetAllFootprintCells();
    win.previewData.footprint = new List<Vector2Int>();

    foreach (var c in cells) {
      var rel = c - new Vector3Int(win.pivot.x, win.pivot.y, 0);
      win.previewData.footprint.Add(new Vector2Int(rel.x, rel.y));
    }

    Debug.Log($"[Save] footprint count={cells.Count}");

    string desiredPath = $"{dir}/{win.assetName}.asset";

    if (!overwrite) {
      var newData = ScriptableObject.CreateInstance<BuildingData>();
      EditorUtility.CopySerialized(win.previewData, newData);

      string uniquePath = AssetDatabase.GenerateUniqueAssetPath(desiredPath);
      AssetDatabase.CreateAsset(newData, uniquePath);
      Debug.Log($"Created NEW BuildingData at {uniquePath}");
      win.buildingData = newData;
    } else {
      if (win.buildingData == null) {
        Debug.LogError("[Save] Cannot overwrite: no BuildingData selected.");
        return;
      }

      EditorUtility.CopySerialized(win.previewData, win.buildingData);
      Debug.Log($"Updated EXISTING BuildingData: {AssetDatabase.GetAssetPath(win.buildingData)}");
    }

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
    Selection.activeObject = win.buildingData;
  }
}