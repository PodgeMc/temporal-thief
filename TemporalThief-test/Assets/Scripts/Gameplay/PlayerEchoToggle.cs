using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerEchoToggle : MonoBehaviour
{
    public enum Role { Player, Echo }
    public Role currentRole = Role.Echo;

    void Awake()
    {
        tag = currentRole == Role.Player ? "Player" : "Echo";

        foreach (var c in GetComponentsInChildren<IPlayerOnly>(true))
            ((MonoBehaviour)c).enabled = currentRole == Role.Player;

        foreach (var c in GetComponentsInChildren<IEchoOnly>(true))
            ((MonoBehaviour)c).enabled = currentRole == Role.Echo;
    }
}