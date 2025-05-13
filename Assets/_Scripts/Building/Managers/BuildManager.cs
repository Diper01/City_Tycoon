using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour {
  [Header("Tilemaps")]
  public Tilemap zoneTilemap;

  private readonly HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();
  public static BuildManager Instance;

  private void Awake() {
    Instance = this;
  }

  public bool CanBuildHere (BuildingData data, Vector3 worldPos, Quaternion rotation) {
    int rotIndex = Mathf.RoundToInt(rotation.eulerAngles.y / 90f) % 4;
    Vector3Int clickedCell = zoneTilemap.WorldToCell(worldPos);
    Vector3Int baseCell = clickedCell - new Vector3Int(data.pivot.x, data.pivot.y, 0);

    foreach (var offset in data.footprint) {
      Vector2Int ro = RotateOffset(offset, rotIndex);
      var cell = baseCell + new Vector3Int(ro.x, ro.y, 0);

      if (!zoneTilemap.HasTile(cell))
        return false;

      Vector3 cellWorld = zoneTilemap.GetCellCenterWorld(cell);
      var allowedCategories = ZoneMapManager.Instance.GetAllowedCategoriesAt(cellWorld);

      if (!allowedCategories.Contains(data.category))
        return false;

      if (occupied.Contains(cell))
        return false;
    }

    return true;
  }

  public void Build (BuildingData data, Vector3 worldPos, Quaternion rotation) {
    if (!CanBuildHere(data, worldPos, rotation))
      return;

    int rotIndex = Mathf.RoundToInt(rotation.eulerAngles.y / 90f) % 4;
    Vector3Int clickedCell = zoneTilemap.WorldToCell(worldPos);
    Vector3Int baseCell = clickedCell - new Vector3Int(data.pivot.x, data.pivot.y, 0);

    var placedCells = new List<Vector3Int>();

    foreach (var offset in data.footprint) {
      Vector2Int ro = RotateOffset(offset, rotIndex);
      var cell = baseCell + new Vector3Int(ro.x, ro.y, 0);
      occupied.Add(cell);
      placedCells.Add(cell);
    }

    Vector3Int spawnCell = baseCell + new Vector3Int(data.pivot.x, data.pivot.y, 0);
    Vector3 spawnPos = zoneTilemap.GetCellCenterWorld(spawnCell);

    GameObject go = Instantiate(data.prefab, spawnPos, rotation);

    if (go.GetComponent<Collider>() == null)
      go.AddComponent<BoxCollider>();

    var inst = go.GetComponent<BuildingInstance>() ?? go.AddComponent<BuildingInstance>();
    inst.Init(data, placedCells);
    GameProgressManager.Instance.SaveProgress();
  }

  public void BuildLoaded (BuildingData data, Vector3 worldPos, Quaternion rotation, int level) {
    int rot = Mathf.RoundToInt(rotation.eulerAngles.y / 90f) % 4;
    Vector3Int cell0 = zoneTilemap.WorldToCell(worldPos) - new Vector3Int(data.pivot.x, data.pivot.y, 0);

    var placedCells = new List<Vector3Int>();

    foreach (var off in data.footprint) {
      Vector2Int ro = RotateOffset(off, rot);
      var cell = cell0 + new Vector3Int(ro.x, ro.y, 0);
      occupied.Add(cell);
      placedCells.Add(cell);
    }

    Vector3Int spawn = cell0 + new Vector3Int(data.pivot.x, data.pivot.y, 0);
    Vector3 pos = zoneTilemap.GetCellCenterWorld(spawn);

    var go = Instantiate(data.prefab, pos, rotation);

    if (go.GetComponent<Collider>() == null)
      go.AddComponent<BoxCollider>();

    var inst = go.GetComponent<BuildingInstance>() ?? go.AddComponent<BuildingInstance>();
    inst.Init(data, placedCells);
    inst.LoadUpgrades(level);
  }

  private Vector2Int RotateOffset (Vector2Int off, int rotIndex) {
    switch (rotIndex) {
      case 1:
        return new Vector2Int(off.y, -off.x);
      case 2:
        return new Vector2Int(-off.x, -off.y);
      case 3:
        return new Vector2Int(-off.y, off.x);
      default:
        return off;
    }
  }

  public void FreeCell (Vector3Int cell) {
    occupied.Remove(cell);
  }
}