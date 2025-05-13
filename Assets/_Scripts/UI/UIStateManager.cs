using UnityEngine;
public class UIStateManager : MonoBehaviour {
  [Header("UI Panels")]
  [SerializeField]
  private GameObject _startPanel;
  [SerializeField]
  private GameObject _gamePanel;
  [SerializeField]
  private GameObject _pausePanel;
  [SerializeField]
  private GameObject _shopPanel;
  [SerializeField]
  private GameObject _upgradePanel;

  private IUIState _currentState;

  private IUIState _startState;
  private IUIState _gameState;
  private IUIState _pauseState;
  private IUIState _shopState;
  private IUIState _upgradeState;

  public static UIStateManager Instance { get; private set; }

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);

      _gameState = new UIState(this, _gamePanel, 1f);
      _pauseState = new UIState(this, _pausePanel, 0f);
      _shopState = new UIState(this, _shopPanel, 1f);
      _upgradeState = new UIState(this, _upgradePanel, 1f);
      _startState = new UIState(this, _startPanel, 1f);
    } else {
      Destroy(gameObject);
    }
  }

  private void Start() {
    StarGame();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape))
      Pause();
  }

  public void Resume()
    => SwitchState(_gameState);

  public void OpenShop()
    => SwitchState(_shopState);

  public void OpenUpgrade()
    => SwitchState(_upgradeState);

  public void StarGame()
    => SwitchState(_startState);

  public void Pause()
    => SwitchState(_currentState == _pauseState ? _gameState : _pauseState);

  public void Quit() {
    GameProgressManager.Instance.SaveProgress();
    MoneyManager.Instance.Save();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }


  public void SwitchState (IUIState newState) {
    _currentState?.Exit();
    _currentState = newState;
    _currentState.Enter();
  }
}