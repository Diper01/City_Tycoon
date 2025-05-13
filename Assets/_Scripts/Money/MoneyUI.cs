using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour {
  [SerializeField]
  private TextMeshProUGUI balanceText;

  private void OnEnable() {
    if (MoneyManager.Exists) {
      MoneyManager.Instance.OnBalanceChanged += UpdateBalance;
      UpdateBalance(MoneyManager.Instance.Balance);
    }
  }

  private void OnDisable() {
    if (MoneyManager.Exists)
      MoneyManager.Instance.OnBalanceChanged -= UpdateBalance;
  }

  private void UpdateBalance (float newBalance) {
    balanceText.text = newBalance.ToString("#,0.00");
  }
}