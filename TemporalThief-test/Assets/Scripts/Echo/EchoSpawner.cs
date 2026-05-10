using System.Collections.Generic;
using UnityEngine;

public class EchoSpawner : MonoBehaviour
{
    [SerializeField] EchoGhost echoPrefab;
    public int maxEchoes = 4;

    readonly List<EchoGhost> activeGhosts = new List<EchoGhost>();
    readonly Queue<EchoGhost> pool = new Queue<EchoGhost>();

    public EchoGhost SpawnEcho(EchoRecording recording, Vector3 position, Quaternion rotation)
    {
        if (recording == null || recording.frames.Count == 0 || echoPrefab == null)
            return null;

        if (activeGhosts.Count >= maxEchoes)
        {
            var old = activeGhosts[0];
            activeGhosts.RemoveAt(0);
            old.gameObject.SetActive(false);
            pool.Enqueue(old);
        }

        EchoGhost ghost;
        if (pool.Count > 0)
        {
            ghost = pool.Dequeue();
            ghost.transform.SetPositionAndRotation(position, rotation);
            ghost.gameObject.SetActive(true);
        }
        else
        {
            ghost = Instantiate(echoPrefab, position, rotation);
        }

        ghost.SetRecording(recording);
        ghost.onFinished = null;
        ghost.onFinished += returned =>
        {
            activeGhosts.Remove(returned);
            pool.Enqueue(returned);
        };

        activeGhosts.Add(ghost);
        return ghost;
    }

    public void DestroyAllGhosts()
    {
        foreach (var ghost in activeGhosts)
        {
            if (ghost != null)
            {
                ghost.gameObject.SetActive(false);
                pool.Enqueue(ghost);
            }
        }
        activeGhosts.Clear();
    }
}
