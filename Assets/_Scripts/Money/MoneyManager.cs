using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoneyManager : MonoBehaviour {
  public static MoneyManager Instance { get; private set; }
  public static bool Exists
    => Instance != null;

  [Header("UI or Debug")]
  public float Balance;

  [Header("Saving")]
  public string saveFileName = "money_save.json";
  public float autoSaveInterval = 30f;

  private readonly List<BuildingInstance> _buildings = new();
  private float _saveTimer;
  private float _incomeTimer;

  public event Action<float> OnBalanceChanged;

  private void Awake() {
    if (Instance != null) {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
  }

  private void Start() {
    Load();
  }

  private void Update() {
    _incomeTimer += Time.deltaTime;

    if (_incomeTimer >= 1f) {
      CollectIncome(_incomeTimer);
      _incomeTimer %= 1f;
    }

    _saveTimer += Time.deltaTime;

    if (_saveTimer >= autoSaveInterval) {
      Save();
      _saveTimer = 0f;
    }
  }

  public void RegisterBuilding (BuildingInstance bi) {
    if (!_buildings.Contains(bi) && bi != null)
      _buildings.Add(bi);
  }

  public void UnregisterBuilding (BuildingInstance bi) {
    _buildings.Remove(bi);
  }

  private void CollectIncome (float seconds) {
    float totalIncome = 0f;

    foreach (var b in _buildings) {
      if (b == null)
        continue;

      float inc = b.CurrentIncomePerSecond * seconds;

      if (float.IsNaN(inc) || float.IsInfinity(inc)) {
        Debug.LogError($"[MoneyManager] Incorrect income {inc} from {b.name}. CPS={b.CurrentIncomePerSecond}, sec={seconds}");
        continue;
      }

      totalIncome += inc;
    }

    if (totalIncome != 0f)
      AddMoney_Internal(totalIncome);
  }

  public bool TrySpend (float amount) {
    if (float.IsNaN(amount) || float.IsInfinity(amount)) {
      Debug.LogError($"[MoneyManager] Attempted to spend an incorrect amount: {amount}");
      return false;
    }

    if (Balance < amount) {
      Debug.Log("[MoneyManager] Not enough money");
      return false;
    }

    Balance -= amount;
    SanitizeBalance();
    OnBalanceChanged?.Invoke(Balance);
    Debug.Log($"[MoneyManager] -{amount:F2} → {Balance:F2}");
    return true;
  }

  public void AddMoney (float amount) {
    if (float.IsNaN(amount) || float.IsInfinity(amount)) {
      Debug.LogError($"[MoneyManager] Attempted to add incorrect amount: {amount}");
      return;
    }

    AddMoney_Internal(amount);
  }

  private void AddMoney_Internal (float amount) {
    Balance += amount;
    SanitizeBalance();
    OnBalanceChanged?.Invoke(Balance);
    //        Debug.Log($"[MoneyManager] +{amount:F2} → {Balance:F2}");
  }

  private void SanitizeBalance() {
    if (float.IsNaN(Balance) || float.IsInfinity(Balance)) {
      Debug.LogError($"[MoneyManager] Balance incorrect ({Balance}), overthrow в 0");
      Balance = 0f;
    }
  }

  [Serializable]
  private class SaveData {
    public float balance;
  }

  public void Save() {
    try {
      var data = new SaveData {
        balance = Balance
      };

      string json = JsonUtility.ToJson(data, true);
      string path = Path.Combine(Application.persistentDataPath, saveFileName);
      File.WriteAllText(path, json);
      Debug.Log($"[MoneyManager] Saved to {path}");
    } catch (Exception e) {
      Debug.LogError("[MoneyManager] Save failed: " + e);
    }
  }

  public void Load() {
    try {
      string path = Path.Combine(Application.persistentDataPath, saveFileName);

      if (!File.Exists(path)) {
        Debug.Log("[MoneyManager] No save found, starting fresh.");
        return;
      }

      string json = File.ReadAllText(path);
      var data = JsonUtility.FromJson<SaveData>(json);
      Balance = data.balance;
      SanitizeBalance();
      OnBalanceChanged?.Invoke(Balance);
      Debug.Log($"[MoneyManager] Loaded balance: {Balance:F2}");
    } catch (Exception e) {
      Debug.LogError("[MoneyManager] Load failed: " + e);
    }
  }

  private void OnApplicationQuit() {
    Save();
  }
}