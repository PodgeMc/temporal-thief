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
    public int tutorialStep; // If needed, but tutorial is session-based
}

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;
    public static SaveLoadManager Instance => instance;

    [SerializeField] private Transform playerTransform;

    private string saveFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

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
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = playerTransform.position,
            playerRotation = playerTransform.rotation,
            inventoryItems = new List<string>(PlayerInventory.Instance.Items),
            tutorialStep = TutorialManager.sessionStep // If you want to save tutorial progress
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to " + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        // Load scene
        SceneManager.LoadScene(data.sceneName);

        // Wait for scene to load, then restore state
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadData = data;
    }

    private GameSaveData loadData;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Restore player position
        if (playerTransform != null)
        {
            playerTransform.position = loadData.playerPosition;
            playerTransform.rotation = loadData.playerRotation;
        }

        // Restore inventory
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.ClearInventory();
            foreach (string item in loadData.inventoryItems)
            {
                PlayerInventory.Instance.AddItem(item);
            }
        }

        // Restore tutorial if needed
        if (TutorialManager.Instance != null)
        {
            TutorialManager.sessionStep = loadData.tutorialStep;
        }

        Debug.Log("Game loaded from save.");
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }
}