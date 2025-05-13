using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class BuildingInstance : MonoBehaviour
{
    public BuildingData data;
    private int currentUpgradeIndex = -1;
    private float totalIncomeMultiplier = 1f;

    public float CurrentIncomePerSecond { get; private set; }

    private readonly List<GameObject> instantiatedDetails = new List<GameObject>();
    private List<Vector3Int> _myCells;


    public void Init(BuildingData buildingData, List<Vector3Int> placedCells)
    {
        data = buildingData;
        _myCells = placedCells;

        CurrentIncomePerSecond = data.incomeAmount / data.incomeInterval;
        totalIncomeMultiplier = 1f;
        currentUpgradeIndex = -1;

        if (MoneyManager.Exists)
            MoneyManager.Instance.RegisterBuilding(this);
    }

    private void OnDestroy()
    {
        if (MoneyManager.Exists)
            MoneyManager.Instance.UnregisterBuilding(this);

        if (BuildManager.Instance != null && _myCells != null)
        {
            foreach (var cell in _myCells)
                BuildManager.Instance.FreeCell(cell);
        }
    }

    public bool TryUpgrade()
    {
        int nextIndex = currentUpgradeIndex + 1;
        if (data.upgrades == null || nextIndex >= data.upgrades.Count)
        {
            Debug.Log("[BuildingInstance] No more upgrades");
            return false;
        }

        float cost = data.baseUpgradeCost * Mathf.Pow(data.upgradeMultiplier, nextIndex);
        if (!MoneyManager.Instance.TrySpend(cost))
            return false;

        ApplyUpgradeStep(nextIndex);
        currentUpgradeIndex = nextIndex;
        totalIncomeMultiplier *= data.upgrades[nextIndex].incomeMultiplier;
        CurrentIncomePerSecond = (data.incomeAmount / data.incomeInterval) * totalIncomeMultiplier;
        return true;
    }

    public void LoadUpgrades(int targetLevel)
    {
        foreach (var go in instantiatedDetails)
            Destroy(go);
        instantiatedDetails.Clear();

        currentUpgradeIndex = -1;
        totalIncomeMultiplier = 1f;

        for (int i = 0; i < targetLevel && data.upgrades != null && i < data.upgrades.Count; i++)
        {
            ApplyUpgradeStep(i);
            totalIncomeMultiplier *= data.upgrades[i].incomeMultiplier;
            currentUpgradeIndex = i;
        }

        CurrentIncomePerSecond = (data.incomeAmount / Mathf.Max(0.0001f, data.incomeInterval)) * totalIncomeMultiplier;
    }

    public int GetLevel() => currentUpgradeIndex + 1;
    public int GetMaxUpgrades() => data.upgrades?.Count ?? 0;

    public float GetNextUpgradeCost()
    {
        int nextIndex = currentUpgradeIndex + 1;
        if (data.upgrades == null || nextIndex >= data.upgrades.Count) 
            return 0f;
        return data.baseUpgradeCost * Mathf.Pow(data.upgradeMultiplier, nextIndex);
    }

    private void ApplyUpgradeStep(int index)
    {
        var step = data.upgrades[index];
        if (step.material != null)
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.sharedMaterial = step.material;

        if (step.scale != Vector3.one)
            transform.localScale = step.scale;

        if (step.details != null)
        {
            foreach (var d in step.details)
            {
                var go = Instantiate(d.prefab, transform);
                go.transform.localPosition    = d.localPosition;
                go.transform.localEulerAngles = d.localEulerAngles;
                go.transform.localScale       = d.localScale;
                go.SetActive(!d.isHidden);
                instantiatedDetails.Add(go);
            }
        }
    }
}
