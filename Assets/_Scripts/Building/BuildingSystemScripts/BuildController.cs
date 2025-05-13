using UnityEngine;
using System;

public class BuildController : MonoBehaviour {
  [Header("References")]
  public LayerMask groundMask;
  public BuildManager buildManager;
  public Material ghostMaterial;

  [Header("Selection Settings")]
  [Tooltip("Размер ячейки сетки в метрах")]
  public float cellSize = 1f;

  private BuildingData _selectedBuilding;
  private Action _onBuiltCallback;
  private GameObject _ghostInstance;
  private Renderer [] _ghostRenderers;
  private string _lastBuildingName;
  private float _currentRotation = 0f;

  public BuildingData SelectedBuilding {
    get
      => _selectedBuilding;
  }
  public static BuildController Instance { get; private set; }

  private void Awake() {
    if (Instance != null && Instance != this) {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
  }

  public void SelectBuilding (BuildingData data, Action onBuilt = null) {
    UIStateManager stateManager = FindObjectOfType<UIStateManager>();
    _selectedBuilding = data;
    _onBuiltCallback = onBuilt;
    stateManager.Resume();
    CreateOrUpdateGhost();
  }

  private void Update() {

    if (_selectedBuilding != null && _selectedBuilding.name != _lastBuildingName)
      CreateOrUpdateGhost();

    if (Input.GetKeyDown(KeyCode.R) && _ghostInstance != null) {
      _currentRotation = (_currentRotation + 90f) % 360f;
      _ghostInstance.transform.rotation = Quaternion.Euler(0, _currentRotation, 0);
    }

    UpdateGhostPosition();

    if (Input.GetMouseButtonDown(0))
      TryPlace();

    if (Input.GetKeyDown(KeyCode.X) && _ghostInstance != null) {
      CancelPlacement();
    }
  }

  private void CreateOrUpdateGhost() {
    if (_ghostInstance != null)
      Destroy(_ghostInstance);

    _lastBuildingName = _selectedBuilding != null ? _selectedBuilding.name : null;

    if (_selectedBuilding == null || _selectedBuilding.prefab == null)
      return;

    _ghostInstance = Instantiate(_selectedBuilding.prefab);
    _ghostInstance.name = "Ghost_" + _selectedBuilding.name;
    _ghostRenderers = _ghostInstance.GetComponentsInChildren<Renderer>();

    foreach (var r in _ghostRenderers) {
      var mats = r.sharedMaterials;

      for (int i = 0; i < mats.Length; i++)
        mats[i] = ghostMaterial;

      r.sharedMaterials = mats;
    }

    _currentRotation = 0f;
    _ghostInstance.transform.rotation = Quaternion.identity;
  }

  private void UpdateGhostPosition() {
    if (_ghostInstance == null)
      return;

    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    if (!Physics.Raycast(ray, out var hit, 100f, groundMask))
      return;

    Vector3 pos = hit.point;
    pos.x = Mathf.Round(pos.x / cellSize) * cellSize;
    pos.z = Mathf.Round(pos.z / cellSize) * cellSize;
    _ghostInstance.transform.position = pos;
    _ghostInstance.transform.rotation = Quaternion.Euler(0, _currentRotation, 0);


    bool canBuild = buildManager.CanBuildHere(_selectedBuilding, pos, Quaternion.Euler(0, _currentRotation, 0));
    SetGhostColor(canBuild ? Color.green : Color.red);
  }

  private void TryPlace() {
    if (_ghostInstance == null || _selectedBuilding == null)
      return;

    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    if (!Physics.Raycast(ray, out var hit, 100f, groundMask))
      return;

    Vector3 pos = hit.point;
    pos.x = Mathf.Round(pos.x / cellSize) * cellSize;
    pos.z = Mathf.Round(pos.z / cellSize) * cellSize;

    var rotation = Quaternion.Euler(0, _currentRotation, 0);

    if (buildManager.CanBuildHere(_selectedBuilding, pos, rotation)) {
      buildManager.Build(_selectedBuilding, pos, rotation);
      Destroy(_ghostInstance);
      _selectedBuilding = null;
      _onBuiltCallback?.Invoke();
      _onBuiltCallback = null;
    } else {
      Debug.Log("Cannot build here");
    }
  }

  private void CancelPlacement() {
    Destroy(_ghostInstance);
    _selectedBuilding = null;
    _onBuiltCallback = null;
    var shop = FindObjectOfType<UIStateManager>();

    if (shop != null)
      shop.OpenShop();
  }

  private void SetGhostColor (Color c) {
    foreach (var r in _ghostRenderers) {
      var mats = r.sharedMaterials;

      for (int i = 0; i < mats.Length; i++) {
        if (mats[i].HasProperty("_BaseColor"))
          mats[i].SetColor("_BaseColor", c);
        else if (mats[i].HasProperty("_Color"))
          mats[i].SetColor("_Color", c);
      }

      r.sharedMaterials = mats;
    }
  }
}