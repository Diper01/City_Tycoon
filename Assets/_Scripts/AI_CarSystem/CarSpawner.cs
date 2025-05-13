using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class CarSpawner : MonoBehaviour {

  [SerializeField]
  private GameObject carPrefab;
  public List<GameObject> carPrefabs = new List<GameObject>();

  [Header("Start waypoints for each car")]
  public List<Waypoint> startWaypoints = new List<Waypoint>();

  private List<GameObject> spawnedCars = new List<GameObject>();
  [SerializeField] private WaypointCollector _waypointCollector;

  private void Start() {
    startWaypoints = _waypointCollector.CollectRandomWaypoints(20);
    SpawnAllCars();
  }

  public void SpawnAllCars() {
    foreach (var c in spawnedCars)
      if (c != null)
        Destroy(c);

    spawnedCars.Clear();

    
    
    foreach (var wp in startWaypoints) {
      if (wp == null)
        continue;

      var iaCar = Instantiate(carPrefab, wp.transform.position, wp.transform.rotation, transform);
      var carInside = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], wp.transform.position, wp.transform.rotation, transform);
      carInside.transform.SetParent(iaCar.transform);
      iaCar.name = "Car AI";
      spawnedCars.Add(iaCar);

      var ai = iaCar.GetComponent<CarAI>();

      if (ai != null)
        ai.Initialize(wp);
    }
  }

  public void RefreshSpawn() {
    SpawnAllCars();
  }
}