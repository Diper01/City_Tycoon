using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class FootprintManager : MonoBehaviour {

  public Tilemap footprintMap;
  public List<Vector3Int> GetAllFootprintCells() {
    Debug.Log("sdfsd");
    var bounds = footprintMap.cellBounds;
    var cells = new List<Vector3Int>();
    Debug.Log($"cellBounds origin={bounds.min}, size={bounds.size}");

    foreach (var pos in bounds.allPositionsWithin) {

      if (footprintMap.HasTile(pos))
        cells.Add(pos);
    }

    foreach (var pos in bounds.allPositionsWithin) {
      if (footprintMap.HasTile(pos))
        cells.Add(pos);

    }

    return cells;
  }
}

#region Legacy
/*public List<Vector3Int> GetConnectedFootprint (Vector3 worldPos) {
    var start = footprintMap.WorldToCell(worldPos);

    if (!footprintMap.HasTile(start)) {
      Debug.Log("  if (!footprintMap.HasTile(start)) {");

    return new List<Vector3Int>();
    }


    var visited = new HashSet<Vector3Int> {
      start
    };

    var queue = new Queue<Vector3Int>();
    queue.Enqueue(start);

    var dirs = new [] {
      Vector3Int.up,
      Vector3Int.right,
      Vector3Int.down,
      Vector3Int.left
    };

    while (queue.Count > 0) {
      var cell = queue.Dequeue();

      foreach (var d in dirs) {
        var n = cell + d;

        if (!visited.Contains(n) && footprintMap.HasTile(n)) {
          visited.Add(n);
          queue.Enqueue(n);
        }
      }
    }
    Debug.Log(start);
    return new List<Vector3Int>(visited);
  }*/
#endregion