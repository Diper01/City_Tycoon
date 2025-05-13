using UnityEngine;

public class UIState : IUIState {
  private readonly UIStateManager manager;
  private readonly GameObject panel;
  private readonly float timeScale;

  public UIState (UIStateManager manager, GameObject panel, float timeScale) {
    this.manager = manager;
    this.panel = panel;
    this.timeScale = timeScale;
  }

  public void Enter() {
    panel.SetActive(true);
    Time.timeScale = timeScale;
  }

  public void Exit() {
    panel.SetActive(false);
  }
}