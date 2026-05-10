using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public List<string> inventoryItems;
    public int tutorialStep;
}

public class SaveLoadManager : MonoBehaviour
{
    static SaveLoadManager instance;
    public static SaveLoadManager Instance => instance;

    [SerializeField] Transform playerTransform;
    GameSaveData loadData;

    string SaveFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void SaveGame()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("SaveLoadManager: playerTransform is not assigned.");
            return;
        }

        var data = new GameSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = playerTransform.position,
            playerRotation = playerTransform.rotation,
            inventoryItems = new List<string>(PlayerInventory.Instance?.Items ?? new List<string>()),
            tutorialStep = TutorialManager.Instance?.CurrentStep ?? 0
        };

        File.WriteAllText(SaveFilePath, JsonUtility.ToJson(data, true));
        Debug.Log("Game saved to " + SaveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("SaveLoadManager: no save file found.");
            return;
        }

        var json = File.ReadAllText(SaveFilePath);
        loadData = JsonUtility.FromJson<GameSaveData>(json);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(loadData.sceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
        {
            playerTransform.position = loadData.playerPosition;
            playerTransform.rotation = loadData.playerRotation;
        }

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.ClearInventory();
            foreach (var item in loadData.inventoryItems)
                PlayerInventory.Instance.AddItem(item);
        }

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.SetStep(loadData.tutorialStep);

        Debug.Log("Game loaded from save.");
    }

    public bool HasSaveFile()
    {
        return File.Exists(SaveFilePath);
    }
}
