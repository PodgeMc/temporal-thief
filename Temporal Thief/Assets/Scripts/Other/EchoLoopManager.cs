// EchoLoopManager v12
// Very simple rules:
//
// - spawnPoints[0] = where run 1 (echo 1) lives
// - spawnPoints[1] = where run 2 (echo 2) lives
// - spawnPoints[2] = where run 3 lives
// - etc...
//
// Player rule:
//
// - While you are doing a run, you stand on one spawn point.
// - When you press E:
//      1) We save that run for this spawn.
//      2) We spawn echoes: run i -> spawnPoints[i].
//      3) We move the player to spawnPoints[lastEchoIndex + 1].
//         (So the player is ALWAYS one spawn behind the newest echo.)
//
// Example with 5 spawn points and maxRuns = 4:
//
// Start:
//   currentRunIndex = 0
//   Player at spawn[0]  (front)
//
// Press E 1st time:
//   Save run 0  (echo 0 -> spawn[0])
//   Player -> spawn[1]
//   Next run will be "run 1" at spawn[1]
//
// Press E 2nd time:
//   Save run 1  (echo 1 -> spawn[1])
//   Player -> spawn[2]
//
// ... and so on.

using UnityEngine;
using System.Collections.Generic;

public class EchoLoopManager : MonoBehaviour
{
    [Header("Needed scripts")]

    public EchoRecorder recorder;   // On the Player
    public EchoSpawner spawner;     // On EchoSystem
    public Transform player;        // Player transform
    public CollectibleResetManager collectibleManager; // To reset collectibles on new runs
    public PlayerInventory inventory; // To clear inventory on new runs

    [Header("Spawn points (front to back)")]
    // Fill this list in the Inspector with Spawn, Spawn (1), Spawn (2)...
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Runs")]
    public int maxRuns = 4; // How many echoes/runs we allow

    // All finished runs
    List<EchoRecording> runs = new List<EchoRecording>();

    // Which spawn index the CURRENT live run is using
    // (this is where the player is standing while recording)
    int currentRunIndex = 0;

    // Are we currently recording?
    bool isRecording = false;

    void Start()
    {
        Debug.Log("[EchoLoopManager] Start() called.");
        Setup();
    }

    /// Put everything in a clean state and start the first run.
    
    public void Setup()
    {
        Debug.Log("[EchoLoopManager] Setup() starting...");

        if (recorder == null || spawner == null || player == null)
        {
            Debug.LogWarning("[EchoLoopManager] Missing references (recorder / spawner / player).");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("[EchoLoopManager] No spawn points set.");
            return;
        }

        if (spawnPoints.Count < 2)
        {
            Debug.LogWarning("[EchoLoopManager] Need at least 2 spawn points.");
            return;
        }

        // We always need at least one extra spawn for the player behind the last echo.
        int maxPossibleRuns = spawnPoints.Count - 1;
        if (maxRuns > maxPossibleRuns)
        {
            Debug.Log("[EchoLoopManager] maxRuns (" + maxRuns + ") too big, clamping to " + maxPossibleRuns);
            maxRuns = maxPossibleRuns;
        }

        // Clear old data
        runs.Clear();
        spawner.DestroyAllGhosts();

        // First live run will be at spawn index 0
        currentRunIndex = 0;

        // Teleport player to spawn[0]
        TeleportPlayerToSpawn(currentRunIndex);
        Debug.Log("[EchoLoopManager] Setup OK. Player at spawn index 0, pos " + player.position);

        // Start recording the first run
        StartNewRun();
    }

    void StartNewRun()
    {
        if (runs.Count >= maxRuns)
        {
            isRecording = false;
            Debug.Log("[EchoLoopManager] StartNewRun: maxRuns reached, no new runs.");
            return;
        }

        recorder.StartRecording();
        isRecording = true;
        Debug.Log("[EchoLoopManager] Started recording run " + runs.Count + " at spawn index " + currentRunIndex + ".");
    }

    EchoRecording StopCurrentRun()
    {
        if (!isRecording)
        {
            Debug.Log("[EchoLoopManager] StopCurrentRun called, but not recording.");
            return null;
        }

        EchoRecording rec = recorder.StopAndGetRecording();
        isRecording = false;

        if (rec == null)
        {
            rec = new EchoRecording();
            Debug.Log("[EchoLoopManager] Recorder returned null, created empty recording.");
        }

        if (rec.frames.Count == 0)
        {
            PoseFrame frame = new PoseFrame
            {
                time = 0f,
                position = player.position,
                rotation = player.rotation
            };
            rec.frames.Add(frame);
            rec.duration = 0f;

            Debug.Log("[EchoLoopManager] Recording had 0 frames, added 1 at player position.");
        }

        return rec;
    }

    /// <summary>
    /// Safe teleport that disables the CharacterController for one frame.
    /// </summary>
    void TeleportPlayerToSpawn(int spawnIndex)
    {
        if (spawnIndex < 0 || spawnIndex >= spawnPoints.Count)
        {
            Debug.LogWarning("[EchoLoopManager] TeleportPlayerToSpawn: index out of range: " + spawnIndex);
            return;
        }

        Transform target = spawnPoints[spawnIndex];

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.SetPositionAndRotation(target.position, target.rotation);

        if (cc != null) cc.enabled = true;

        Debug.Log("[EchoLoopManager] Player TELEPORTED to spawn index " + spawnIndex +
                  " at position " + player.position);
    }

    /// <summary>
    /// Called by WatchController when you press E.
    /// </summary>
    public void EndRunAndStartNext()
    {
        Debug.Log("[EchoLoopManager] === E pressed ===");

        if (inventory != null)
            inventory.ClearInventory();

        if (collectibleManager != null)
            collectibleManager.ResetAllCollectibles();

        if (recorder == null || spawner == null || player == null || spawnPoints == null)
        {
            Debug.LogWarning("[EchoLoopManager] EndRunAndStartNext: missing references.");
            return;
        }

        // 1) If we currently still have room
        if (isRecording && runs.Count < maxRuns)
        {
            EchoRecording finished = StopCurrentRun();
            runs.Add(finished);

            Debug.Log("[EchoLoopManager] Saved run " + (runs.Count - 1) +
                      " for spawn index " + currentRunIndex +
                      ". Total runs = " + runs.Count + ".");
        }
        else if (isRecording && runs.Count >= maxRuns)
        {
            // If somehow recording when already at maxRuns, stop but do not save.
            StopCurrentRun();
            Debug.Log("[EchoLoopManager] Run stopped but NOT saved (already at maxRuns).");
        }
        else
        {
            Debug.Log("[EchoLoopManager] E pressed but not recording (playback-only mode).");
        }

        // 2) Destroy old ghosts
        spawner.DestroyAllGhosts();

        // 3) Spawn echoes: run i -> spawnPoints[i]
        for (int i = 0; i < runs.Count; i++)
        {
            if (i >= spawnPoints.Count)
                break;

            EchoRecording rec = runs[i];
            Transform echoSpawn = spawnPoints[i];

            spawner.SpawnEchoAt(rec, echoSpawn.position, echoSpawn.rotation);
            Debug.Log("[EchoLoopManager] Spawned echo for run " + i + " at spawn index " + i);
        }

        // 4) Decide where the player should stand next:
        //
        //    If we have N saved runs:
        //      - newest echo index = N - 1
        //      - player index      = N (one ahead)
        //
        int nextPlayerIndex = runs.Count; // one ahead of the last echo
        if (nextPlayerIndex >= spawnPoints.Count)
        {
            nextPlayerIndex = spawnPoints.Count - 1; // clamp to last spawn
        }

        currentRunIndex = nextPlayerIndex;
        TeleportPlayerToSpawn(currentRunIndex);

        // 5) If we still have room, start the NEXT run from this new spawn.
        if (runs.Count < maxRuns)
        {
            StartNewRun();
        }
        else
        {
            Debug.Log("[EchoLoopManager] MaxRuns reached. Playback-only from now on.");
        }
    }
}
