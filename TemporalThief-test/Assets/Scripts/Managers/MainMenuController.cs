using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Button backButton;

    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(Continue);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(Quit);
        backButton.onClick.AddListener(CloseSettings);

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(v => AudioListener.volume = v);

        sensitivitySlider.minValue = 0.5f;
        sensitivitySlider.maxValue = 10f;
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        sensitivitySlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("MouseSensitivity", v));

        continueButton.interactable = SaveLoadManager.Instance != null
            && SaveLoadManager.Instance.HasSaveFile();

        settingsPanel.SetActive(false);
    }

    public void NewGame() => SceneManager.LoadScene(1);

    public void Continue()
    {
        SaveLoadManager.Instance?.LoadGame();
    }

    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("Quit pressed — stopping play mode (Application.Quit suppressed in editor).");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
