using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "BuildingComposerScripts/BuildingData")]
public class BuildingData : ScriptableObject {
  public GameObject prefab;
  public BuildCategory category;
  public Vector2Int pivot;
  public List<Vector2Int> footprint;
  public List<UpgradeStep> upgrades;

  [Header("Economy base")]
  public float incomePerSecond = 1f;
  public float purchaseCost = 10;

  [Header("Income")]
  public int incomeAmount = 1;
  public float incomeInterval = 3;

  [Header("Limits")]
  public int maxCount = 5;

  [Header("Upgrade")]
  public int baseUpgradeCost = 100;
  public float upgradeMultiplier = 1.5f;
}

[Serializable]
public class UpgradeStep {
  public Material material;
  public Vector3 scale;
  [Header("Upgrades details")]
  public List<DetailData> details;
  [Tooltip("Economy")]
  public float incomeMultiplier = 1f;

}

[Serializable]
public class DetailData {
  public GameObject prefab;
  public Vector3 localPosition;
  public Vector3 localEulerAngles;
  public Vector3 localScale;
  public bool isHidden;

}