using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class BuildingShopManager : MonoBehaviour {
  [Header("UI Containers")]
  public Transform tabContainer;
  public GameObject tabButtonPrefab;
  public Transform contentContainer;
  public GameObject itemButtonPrefab;

  private List<BuildingData> _allBuildings;

  private void Start() {
    _allBuildings = Resources.LoadAll<BuildingData>("Data/Buildings").ToList();
    Debug.Log(_allBuildings.Count);

    foreach (BuildCategory cat in System.Enum.GetValues(typeof(BuildCategory))) {
      var tabGO = Instantiate(tabButtonPrefab, tabContainer);
      var tabText = tabGO.GetComponentInChildren<TextMeshProUGUI>();
      tabText.text = cat.ToString();

      var btn = tabGO.GetComponent<Button>();
      btn.onClick.AddListener(() => ShowCategory(cat));
    }

    if (_allBuildings.Count > 0)
      ShowCategory(_allBuildings[0].category);
  }

  void ShowCategory (BuildCategory category) {
    foreach (Transform child in contentContainer)
      Destroy(child.gameObject);

    var list = _allBuildings.Where(b => b.category == category);

    foreach (var data in list) {
      var itemGO = Instantiate(itemButtonPrefab, contentContainer);
      var shopItem = itemGO.GetComponent<ShopItem3D>();

      shopItem.Init(data, data.purchaseCost, (d) => BuildController.Instance.SelectBuilding(d));
    }
  }
}