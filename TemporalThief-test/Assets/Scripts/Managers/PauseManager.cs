using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject menuRoot;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] string mainMenuScene = "MainMenu";
    bool paused;

    void Start()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!paused);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void SaveGame()
    {
        SaveLoadManager.Instance?.SaveGame();
    }

    public void LoadGame()
    {
        SaveLoadManager.Instance?.LoadGame();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void SetPaused(bool value)
    {
        paused = value;
        Time.timeScale = paused ? 0f : 1f;

        if (menuRoot != null)
            menuRoot.SetActive(paused);

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ToggleSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
