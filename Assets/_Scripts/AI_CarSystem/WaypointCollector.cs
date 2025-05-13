using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WaypointCollector : MonoBehaviour {
  public List<Waypoint> selectedWaypoints = new List<Waypoint>();

  public List<Waypoint> CollectRandomWaypoints (int count = 20) {
    var all = FindObjectsOfType<Waypoint>().ToList();

    if (all.Count <= count) {
      selectedWaypoints = all;
    } else {
      selectedWaypoints = new List<Waypoint>();

      while (selectedWaypoints.Count < count) {
        var candidate = all[Random.Range(0, all.Count)];

        if (!selectedWaypoints.Contains(candidate))
          selectedWaypoints.Add(candidate);
      }
    }

    return selectedWaypoints;

  }
}