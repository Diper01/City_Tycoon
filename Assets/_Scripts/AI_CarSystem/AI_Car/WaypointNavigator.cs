using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class WaypointNavigator : MonoBehaviour {
  public Waypoint Current { get; private set; }

  private Waypoint last;
  private NavMeshAgent agent;

  public void Initialize (Waypoint first) {
    agent = GetComponent<NavMeshAgent>();
    Current = first;
    last = null;
    agent.SetDestination(Current.transform.position);
  }

  public void GoToNext() {
    var options = new List<Waypoint>();

    foreach (var wp in Current.neighbors) {
      if (wp == null || wp == last)
        continue;

      options.Add(wp);
    }

    if (options.Count == 0 && last != null)
      options.Add(last);

    var next = options[Random.Range(0, options.Count)];
    last = Current;
    Current = next;
    agent.SetDestination(Current.transform.position);
  }

  public void SwitchToParking (Waypoint park) {
    last = Current;
    Current = park;
    agent.SetDestination(Current.transform.position);
  }
}