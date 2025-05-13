using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenuUI : MonoBehaviour {
  [Header("UI Elements")]
  public TMP_Text levelText;
  public TMP_Text incomeText;
  public TMP_Text upgradeCostText;
  public TMP_Text sellCostText;
  public Button btnUpgrade;
  public Button btnSell;
  public Button btnClose;

  private BuildingInstance target;
  private const float sellRatio = 0.5f;

  private void Awake() {
    btnUpgrade.onClick.AddListener(OnUpgradeClicked);
    btnSell.onClick.AddListener(OnSellClicked);
    btnClose.onClick.AddListener(Close);
  }

  private void OnEnable() {
    if (MoneyManager.Exists)
      MoneyManager.Instance.OnBalanceChanged += OnBalanceChanged;
  }

  private void OnDisable() {
    if (MoneyManager.Exists)
      MoneyManager.Instance.OnBalanceChanged -= OnBalanceChanged;
  }

  public void Open (BuildingInstance inst) {
    target = inst;
    UIStateManager.Instance.OpenUpgrade();
    Refresh();
  }

  public void Close() {
    target = null;
    UIStateManager.Instance.Resume();
  }

  private void OnBalanceChanged (float _) {
    Refresh();
  }

  private void Refresh() {
    if (target == null)
      return;

    int lvl = target.GetLevel();
    int lvlmax = target.GetMaxUpgrades();
    levelText.text = $"Lvl: {lvl}/{lvlmax}";
    incomeText.text = $"Income{target.CurrentIncomePerSecond:F1}/s";

    float upCost = target.GetNextUpgradeCost();
    upgradeCostText.text = upCost > 0 ? upCost.ToString("F1") : "-";
    btnUpgrade.interactable = upCost > 0 && MoneyManager.Instance.Balance >= upCost;

    float refund = target.data.purchaseCost * sellRatio;
    sellCostText.text = refund.ToString("F1");
  }

  private void OnUpgradeClicked() {
    if (target != null && target.TryUpgrade()) {
      GameProgressManager.Instance.SaveProgress();
      Refresh();
    }

  }

  private void OnSellClicked() {
    if (target == null)
      return;

    float refund = target.data.purchaseCost * sellRatio;
    MoneyManager.Instance.AddMoney(refund);
    Destroy(target.gameObject);
    GameProgressManager.Instance.SaveProgress();
    Close();
  }
}