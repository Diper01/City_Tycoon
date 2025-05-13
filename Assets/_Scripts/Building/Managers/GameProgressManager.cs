using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameProgressManager : MonoBehaviour {
  private BuildingData [] allBuildingData;
  public static GameProgressManager Instance;
  private string filePath
    => Path.Combine(Application.persistentDataPath, "progress.json");

  [Serializable]
  private class SaveContainer {
    public float balance;
    public List<BuildingSaveEntry> buildings;
  }

  private void Awake() {
    Instance = this;
    allBuildingData = Resources.LoadAll<BuildingData>("Data/Buildings");

  }

  private void Start() {
    LoadProgress();
  }

  private void OnApplicationQuit() {
    SaveProgress();
  }

  public void SaveProgress() {
    var save = new SaveContainer {
      balance = MoneyManager.Instance.Balance,
      buildings = new List<BuildingSaveEntry>()
    };

    foreach (var inst in FindObjectsOfType<BuildingInstance>()) {
      save.buildings.Add(new BuildingSaveEntry {
        dataName = inst.data.name,
        position = inst.transform.position,
        eulerRotation = inst.transform.eulerAngles,
        level = inst.GetLevel()
      });
    }

    string json = JsonUtility.ToJson(save, true);
    File.WriteAllText(filePath, json);
    Debug.Log($"[GameProgress] Saved → {filePath}");
  }

  public void LoadProgress() {

    if (File.Exists(filePath)) {
      string json = File.ReadAllText(filePath);
      var save = JsonUtility.FromJson<SaveContainer>(json);



      foreach (var old in FindObjectsOfType<BuildingInstance>())
        Destroy(old.gameObject);

      foreach (var entry in save.buildings) {
        var data = Array.Find(allBuildingData, d => d.name == entry.dataName);

        if (data == null) {
          Debug.LogWarning($"[GameProgress] SO '{entry.dataName}' не знайдено");
          continue;
        }

        BuildManager.Instance.BuildLoaded(data, entry.position, Quaternion.Euler(entry.eulerRotation), entry.level);
      }

      Debug.Log("[GameProgress] Progress loaded");
    } else {
      Debug.Log("[GameProgress] Save file was not found, start pure");
    }
  }
}