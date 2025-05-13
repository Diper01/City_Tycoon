using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ShopItem3D : MonoBehaviour, IPointerDownHandler, IDragHandler {
  [Header("UI References")]
  [SerializeField]
  private RawImage previewImage;
  [SerializeField]
  private TMP_Text nameText;
  [SerializeField]
  private TMP_Text priceText;
  [SerializeField]
  private Button buyButton;
  [SerializeField]
  private TMP_Text countText;
  [SerializeField]
  private TMP_Text incomeText;

  [Header("Preview Settings")]
  [SerializeField]
  private int textureSize = 256;
  [SerializeField]
  private LayerMask previewLayer;
  [SerializeField]
  private float rotationSpeed = 0.2f;

  private RenderTexture _rt;
  private Camera _cam;
  private GameObject _previewInstance;
  private BuildingData _data;
  private Action<BuildingData> _onBuy;
  private Vector2 _lastPointer;

  public void OnPointerDown (PointerEventData e) {
    _lastPointer = e.position;
  }

  public void OnDrag (PointerEventData e) {
    if (_previewInstance == null)
      return;

    Vector2 delta = e.position - _lastPointer;
    _previewInstance.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
    _lastPointer = e.position;

    if (_cam != null)
      _cam.Render();
  }

  public void Init (BuildingData data, float price, Action<BuildingData> onBuy) {
    _data = data;
    _onBuy = onBuy;

    nameText.text = data.name;
    priceText.text = price.ToString("#,0");
    incomeText.text = $"{data.incomePerSecond:F1}/s";

    _rt = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.ARGB32);
    _rt.Create();
    previewImage.texture = _rt;

    var camGO = new GameObject($"ShopCam_{data.name}", typeof(Camera));
    camGO.transform.SetParent(transform, false);
    _cam = camGO.GetComponent<Camera>();
    _cam.enabled = false;
    _cam.clearFlags = CameraClearFlags.Color;
    _cam.backgroundColor = Color.clear;
    _cam.cullingMask = previewLayer;
    _cam.targetTexture = _rt;
    _cam.transform.rotation = Quaternion.identity;
    _previewInstance = Instantiate(data.prefab, _cam.transform, false);
    SetLayerRecursively(_previewInstance, LayerMaskToLayer(previewLayer));
    _previewInstance.transform.localPosition = new Vector3(0, -11, 29);
    _previewInstance.transform.localRotation = Quaternion.Euler(0, 180, 0);

    AdjustCameraToBounds();

    buyButton.onClick.AddListener(OnBuy);
  }

  private void LateUpdate() {
    if (_cam != null)
      _cam.Render();
  }

  private void OnDestroy() {
    if (_rt != null)
      _rt.Release();

    if (_cam != null)
      Destroy(_cam.gameObject);

    if (_previewInstance != null)
      Destroy(_previewInstance);
  }

  private void OnBuy() {
    if (MoneyManager.Instance.TrySpend(_data.purchaseCost)) {
      _onBuy?.Invoke(_data);

    }

  }

  public void UpdateCount (int newCount) {
    countText.text = $"{newCount}/{_data.maxCount}";
    bool canBuy = MoneyManager.Instance.Balance >= _data.purchaseCost && newCount < _data.maxCount;
    buyButton.interactable = canBuy;
  }

  private void AdjustCameraToBounds() {
    var renders = _previewInstance.GetComponentsInChildren<Renderer>();

    if (renders.Length == 0)
      return;

    var b = renders[0].bounds;

    foreach (var r in renders)
      b.Encapsulate(r.bounds);

    float maxSize = Mathf.Max(b.size.x, b.size.y, b.size.z);
    float halfFov = _cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
    float dist = maxSize / (2f * Mathf.Tan(halfFov));

    _cam.transform.localPosition = b.center + new Vector3(0, dist, dist);
    // _cam.transform.LookAt(b.center);
    _cam.transform.rotation = Quaternion.identity;
  }

  private int LayerMaskToLayer (LayerMask mask) {
    int v = mask.value;

    for (int i = 0; i < 32; i++)
      if ((v & (1 << i)) != 0)
        return i;

    return 0;
  }

  private void SetLayerRecursively (GameObject go, int layer) {
    go.layer = layer;

    foreach (Transform ch in go.transform)
      SetLayerRecursively(ch.gameObject, layer);
  }
}