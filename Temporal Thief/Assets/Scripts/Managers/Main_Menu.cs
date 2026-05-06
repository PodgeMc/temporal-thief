using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string gameScene = "Level_Training";
    [SerializeField] string scoreboardScene = "Scoreboard";

    [Header("UI Panels")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject optionsPanel;

    [Header("Options")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Initialize panels
        if (mainPanel) mainPanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);

        // Load saved settings
        LoadSettings();
    }

    public void Continue() { SceneManager.LoadScene(gameScene); }
    public void Load() 
    { 
        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSaveFile())
        {
            SaveLoadManager.Instance.LoadGame();
        }
        else
        {
            SceneManager.LoadScene(gameScene); // Fallback to new game
        }
    }
    public void NewGame() { SceneManager.LoadScene(gameScene); }
    public void Scoreboard() { SceneManager.LoadScene(scoreboardScene); }

    public void ShowOptions()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void HideOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        if (mainPanel) mainPanel.SetActive(true);
        SaveSettings();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetMusicVolume(float value)
    {
        // Assuming you have a music audio source, adjust here
        // e.g., musicSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        // Assuming you have SFX sources, adjust here
        // e.g., sfxSource.volume = value;
    }

    void LoadSettings()
    {
        if (masterVolumeSlider) masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (musicVolumeSlider) musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (sfxVolumeSlider) sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume(masterVolumeSlider ? masterVolumeSlider.value : 1f);
        SetMusicVolume(musicVolumeSlider ? musicVolumeSlider.value : 1f);
        SetSFXVolume(sfxVolumeSlider ? sfxVolumeSlider.value : 1f);
    }

    void SaveSettings()
    {
        if (masterVolumeSlider) PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        if (musicVolumeSlider) PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        if (sfxVolumeSlider) PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.Save();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
