#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour {

  [Tooltip("neighbors points")]
  public List<Waypoint> neighbors = new List<Waypoint>();

  public bool HasParkingLink() {
    foreach (var wp in neighbors)
      if (wp != null && wp.CompareTag("Parking"))
        return true;

    return false;
  }

  public Waypoint GetParkingNeighbor() {
    return neighbors.Find(wp => wp != null && wp.CompareTag("Parking"));
  }

  private void OnDrawGizmos() {
#if UNITY_EDITOR
    Handles.color = Color.cyan;
    Handles.DrawSolidDisc(transform.position, Vector3.up, 0.5f);

    Handles.color = Color.green;

    foreach (var n in neighbors)
      if (n != null)
        Handles.DrawAAPolyLine(8f, transform.position, n.transform.position);

    Handles.color = Color.red;

    foreach (var p in neighbors)
      if (p != null && p.CompareTag("Parking"))
        Handles.DrawAAPolyLine(8f, transform.position, p.transform.position);
#else
        /*Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);*/
#endif
  }
}