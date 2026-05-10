using System.Collections.Generic;
using UnityEngine;

public class EchoLoopManager : MonoBehaviour
{
    public EchoRecorder recorder;
    public EchoSpawner spawner;
    public Transform player;
    public CollectibleResetManager collectibleResetManager;
    public PlayerInventory playerInventory;
    public Transform startTransform;
    public int maxRuns = 4;

    readonly List<EchoRecording> runs = new List<EchoRecording>();
    readonly List<PoseFrame> generatedSpawnPoints = new List<PoseFrame>();
    int currentRunIndex;
    bool isRecording;
    CharacterController controller;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (recorder == null || spawner == null || player == null || startTransform == null)
        {
            Debug.LogWarning("EchoLoopManager is missing required references.");
            return;
        }

        controller = player.GetComponent<CharacterController>();
        runs.Clear();
        generatedSpawnPoints.Clear();
        spawner.DestroyAllGhosts();
        currentRunIndex = 0;
        TeleportPlayerTo(startTransform.position, startTransform.rotation);
        StartNewRun();
    }

    void StartNewRun()
    {
        if (runs.Count >= maxRuns)
        {
            isRecording = false;
            return;
        }

        recorder.StartRecording();
        isRecording = true;
    }

    EchoRecording StopCurrentRun()
    {
        if (!isRecording)
            return null;

        var recording = recorder.StopAndGetRecording();
        isRecording = false;
        if (recording == null)
            return new EchoRecording();

        if (recording.frames.Count == 0 && player != null)
        {
            recording.frames.Add(new PoseFrame
            {
                time = 0f,
                position = player.position,
                rotation = player.rotation
            });
            recording.duration = 0f;
        }

        return recording;
    }

    void TeleportPlayerTo(Vector3 position, Quaternion rotation)
    {
        if (controller != null)
            controller.enabled = false;

        player.SetPositionAndRotation(position, rotation);

        if (controller != null)
            controller.enabled = true;
    }

    PoseFrame GetSpawnPose(int runIndex)
    {
        if (runIndex == 0)
            return new PoseFrame { position = startTransform.position, rotation = startTransform.rotation };

        int spawnIndex = runIndex - 1;
        if (spawnIndex < generatedSpawnPoints.Count)
            return generatedSpawnPoints[spawnIndex];

        var fallback = generatedSpawnPoints.Count > 0
            ? generatedSpawnPoints[0]
            : new PoseFrame { position = startTransform.position, rotation = startTransform.rotation };
        return fallback;
    }

    void GenerateSpawnPoints(EchoRecording run0)
    {
        generatedSpawnPoints.Clear();

        if (run0 == null || run0.frames.Count == 0)
        {
            Debug.LogWarning("EchoLoopManager: Run 0 recording too short — using start pose for all runs.");
            return;
        }

        for (int i = 0; i < maxRuns; i++)
        {
            float t = run0.duration * (i + 1f) / (maxRuns + 1f);
            generatedSpawnPoints.Add(SampleRecording(run0, t));
        }
    }

    PoseFrame SampleRecording(EchoRecording recording, float t)
    {
        var frames = recording.frames;
        if (frames.Count == 0)
            return new PoseFrame { position = startTransform.position, rotation = startTransform.rotation };

        if (frames.Count == 1 || t <= frames[0].time)
            return new PoseFrame { position = frames[0].position, rotation = frames[0].rotation };

        for (int i = 0; i < frames.Count - 1; i++)
        {
            if (frames[i + 1].time >= t)
            {
                float segment = frames[i + 1].time - frames[i].time;
                float frac = segment > 0f ? (t - frames[i].time) / segment : 0f;
                return new PoseFrame
                {
                    position = Vector3.Lerp(frames[i].position, frames[i + 1].position, frac),
                    rotation = Quaternion.Slerp(frames[i].rotation, frames[i + 1].rotation, frac)
                };
            }
        }

        var last = frames[frames.Count - 1];
        return new PoseFrame { position = last.position, rotation = last.rotation };
    }

    public void EndRunAndStartNext()
    {
        if (playerInventory != null)
            playerInventory.ClearInventory();

        collectibleResetManager?.ResetAllCollectibles();

        if (isRecording && runs.Count < maxRuns)
        {
            var finished = StopCurrentRun();

            if (runs.Count == 0)
                GenerateSpawnPoints(finished);

            runs.Add(finished);
        }
        else if (isRecording)
        {
            StopCurrentRun();
        }

        spawner.DestroyAllGhosts();

        for (int i = 0; i < runs.Count; i++)
        {
            var pose = GetSpawnPose(i);
            spawner.SpawnEcho(runs[i], pose.position, pose.rotation);
        }

        currentRunIndex = runs.Count;
        var nextPose = GetSpawnPose(currentRunIndex);
        TeleportPlayerTo(nextPose.position, nextPose.rotation);

        if (runs.Count < maxRuns)
            StartNewRun();
    }
}
