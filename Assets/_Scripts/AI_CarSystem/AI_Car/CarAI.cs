using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WaypointNavigator))]
[RequireComponent(typeof(ParkingBehavior))]
public class CarAI : MonoBehaviour
{
  [Header("Waypoint Graph")]
  [SerializeField] private Waypoint startWaypoint;

  private NavMeshAgent agent;
  private WaypointNavigator navigator;
  private ParkingBehavior parking;
  private bool isInitialized;

  private void Awake()
  {
    agent = GetComponent<NavMeshAgent>();
    agent.avoidancePriority = Random.Range(0, 100);

    navigator = GetComponent<WaypointNavigator>();
    parking   = GetComponent<ParkingBehavior>();
  }

  public void Initialize(Waypoint first)
  {
    navigator.Initialize(first);
    parking.Initialize(navigator);
    isInitialized = true;
  }

  private void Update()
  {
    if (!isInitialized || agent.pathPending) 
      return;

    if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
    {
      if (parking.TryParking()) 
        return;

      navigator.GoToNext();
    }
  }
}