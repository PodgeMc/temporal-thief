// ShowForwardGizmo.cs
using UnityEngine;

public class ShowForwardGizmo : MonoBehaviour {
    public Color gizmoColor=Color.green;
    public float lineLength=2f;
    void OnDrawGizmos(){
        Gizmos.color=gizmoColor;
        var s=transform.position;
        var e=s+transform.forward*lineLength;
        Gizmos.DrawLine(s,e);
        Gizmos.DrawSphere(e,0.1f);
    }
}
