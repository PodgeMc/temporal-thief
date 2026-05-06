using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject menuRoot; // pause menu UI root
    [SerializeField] string mainMenuScene = "MainMenu"; // scene name for main menu

    bool paused; // true when game is paused

    void Start()
    {
        menuRoot.SetActive(false); // hide pause menu at start
        Time.timeScale = 1f; // make sure game starts unpaused

        Cursor.lockState = CursorLockMode.Locked; // lock mouse cursor
        Cursor.visible = false; // hide mouse cursor
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SetPaused(!paused); // toggle pause menu with Escape
    }

    public void Resume()
    {
        SetPaused(false); // resume game from UI button
    }

    public void SaveGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveGame();
            Debug.Log("Game saved from pause menu.");
        }
    }

    public void LoadGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.LoadGame();
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f; // unpause time before leaving
        Cursor.lockState = CursorLockMode.None; // unlock cursor
        Cursor.visible = true; // show cursor
        SceneManager.LoadScene(mainMenuScene); // load main menu scene
    }

    public void SetVolume(float v)
    {
        AudioListener.volume = v; // change volume
    }

    public void SetPaused(bool value)
    {
        paused = value; // save pause state

        Time.timeScale = paused ? 0f : 1f; // pause/unpause the game
        menuRoot.SetActive(paused); // show/hide pause menu UI

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked; // unlock cursor when paused
        Cursor.visible = paused; // show cursor when paused
    }

    public void SetPausedNoMenu(bool value)
    {
        paused = value; // save pause state

        Time.timeScale = paused ? 0f : 1f; // pause/unpause the game
        menuRoot.SetActive(false); // NEVER show pause menu UI

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked; // unlock cursor when paused
        Cursor.visible = paused; // show cursor when paused
    }
}
