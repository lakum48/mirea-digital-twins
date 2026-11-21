using UnityEngine;

public class PortalGate : MonoBehaviour
{
    public Vector3 destination;
    void OnTriggerEnter(Collider hit)
    {
        TwinWalker walker = hit.GetComponentInParent<TwinWalker>();
        if (walker) walker.Teleport(destination);
    }
}
