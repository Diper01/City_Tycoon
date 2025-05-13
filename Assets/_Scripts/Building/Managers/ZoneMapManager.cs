using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct ZoneEntry {
  public TileBase zoneTile;
  public List<BuildCategory> allowed;
}

public class ZoneMapManager : MonoBehaviour {
  public static ZoneMapManager Instance { get; private set; }
  public Tilemap zoneTilemap;
  public List<ZoneEntry> zoneEntries;

  Dictionary<TileBase, List<BuildCategory>> _tileToAllowed = new();
  Dictionary<Vector3Int, List<BuildCategory>> _cellAllowed = new();

  void Awake() {
    Instance = this;

    foreach (var e in zoneEntries)
      if (!_tileToAllowed.ContainsKey(e.zoneTile))
        _tileToAllowed[e.zoneTile] = e.allowed;

    var bounds = zoneTilemap.cellBounds;

    foreach (var pos in bounds.allPositionsWithin) {
      var tile = zoneTilemap.GetTile(pos);

      if (tile != null && _tileToAllowed.TryGetValue(tile, out var list))
        _cellAllowed[pos] = list;
    }
  }

  public List<BuildCategory> GetAllowedCategoriesAt (Vector3 worldPos) {
    var cell = zoneTilemap.WorldToCell(worldPos);

    if (_cellAllowed.TryGetValue(cell, out var list))
      return list;

    return new List<BuildCategory>();
  }
}