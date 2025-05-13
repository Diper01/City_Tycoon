using UnityEngine;
using System.Collections;

public class ParkingBehavior : MonoBehaviour {
  [Header("Parking Settings")]
  [SerializeField, Range(0f, 1f)]
  private float parkChance = 0.2f;
  [SerializeField]
  private float minWaitTime = 2f;
  [SerializeField]
  private float maxWaitTime = 5f;

  private WaypointNavigator navigator;
  private bool isParking;

  public void Initialize (WaypointNavigator navigator) {
    this.navigator = navigator;
    isParking = false;
  }

  public bool TryParking() {
    var current = navigator.Current;

    if (!isParking && current.HasParkingLink() && Random.value < parkChance) {
      isParking = true;
      var park = current.GetParkingNeighbor();
      navigator.SwitchToParking(park);
      StartCoroutine(ParkAndDrive());
      return true;
    }

    return false;
  }

  private IEnumerator ParkAndDrive() {
    yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
    isParking = false;
    navigator.GoToNext();
  }
}