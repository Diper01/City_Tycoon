using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSelector : MonoBehaviour {
  public LayerMask buildingLayer;
  public UpgradeMenuUI upgradeMenu;

  private void Update() {
    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && BuildController.Instance.SelectedBuilding == null) {
      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out var hit, 100f, buildingLayer)) {
        var inst = hit.collider.GetComponent<BuildingInstance>();

        if (inst != null) {
          upgradeMenu.Open(inst);
          return;
        }
      }

      upgradeMenu.Close();
    }
  }
}