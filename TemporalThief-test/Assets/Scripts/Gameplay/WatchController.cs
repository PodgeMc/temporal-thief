using UnityEngine;

public class WatchController : MonoBehaviour
{
    public EchoLoopManager loopManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            loopManager?.EndRunAndStartNext();
    }
}
