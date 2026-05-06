using UnityEngine;
using System.Collections.Generic;

public class EchoSpawner : MonoBehaviour
{
    [Header("Prefab must already be cleaned of player-only scripts")]
    [SerializeField] private EchoGhost echoPrefab;

    public int maxEchoes = 10;
    private readonly List<EchoGhost> ghosts = new();
    private readonly Queue<EchoGhost> _pool = new();

    public EchoGhost SpawnEchoAt(EchoRecording r, Vector3 p, Quaternion q)
    {
        if (r == null || r.frames == null || r.frames.Count == 0)
            return null;

        if (!echoPrefab)
        {
            Debug.LogError("EchoSpawner: Echo prefab not assigned in Inspector.", this);
            return null;
        }

        // limit count
        if (ghosts.Count >= maxEchoes)
        {
            var old = ghosts[0];
            ghosts.RemoveAt(0);
            old.gameObject.SetActive(false);
            _pool.Enqueue(old);
        }

        // spawn echo
        EchoGhost g;
        if (_pool.Count > 0)
        {
            g = _pool.Dequeue();
            g.transform.position = p;
            g.transform.rotation = q;
            g.gameObject.SetActive(true);
        }
        else
        {
            g = Instantiate(echoPrefab, p, q);
        }

        g.SetRecording(r);
        g.onFinished += x => 
        {
            ghosts.Remove(x);
            x.gameObject.SetActive(false);
            _pool.Enqueue(x);
        };

        ghosts.Add(g);
        return g;
    }

    public void DestroyAllGhosts()
    {
        foreach (var g in ghosts)
        {
            if (g) 
            {
                g.gameObject.SetActive(false);
                _pool.Enqueue(g);
            }
        }

        ghosts.Clear();
    }
}
