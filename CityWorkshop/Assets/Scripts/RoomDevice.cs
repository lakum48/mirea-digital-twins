using UnityEngine;

public class RoomDevice : MonoBehaviour
{
    public enum Kind { Door, Blinds, Fan, Lamp }
    public Kind kind;
    public Light lamp;
    public Transform[] movingParts;
    public bool active;
    public float angle = -95;
    public Vector3 windOrigin;
    public Vector3 windDirection = Vector3.right;
    public float OpenFraction { get; private set; }

    void Start() { if (lamp) lamp.enabled = active; }
    public void Toggle() {
        active = !active;
        if (lamp) lamp.enabled = active;
    }
    void Update()
    {
        OpenFraction = Mathf.MoveTowards(OpenFraction, active ? 1 : 0, 1.6f * Time.deltaTime);
        foreach (Transform part in movingParts)
        {
            if (kind == Kind.Door) part.localRotation = Quaternion.Euler(0, angle * OpenFraction, 0);
            if (kind == Kind.Blinds) part.localRotation = Quaternion.Euler(80 * OpenFraction, 0, 0);
            if (kind == Kind.Fan) part.Rotate(Vector3.forward, 900 * OpenFraction * Time.deltaTime, Space.Self);
        }
    }
    void FixedUpdate()
    {
        if (kind != Kind.Fan || !active) return;
        foreach (Collider c in Physics.OverlapSphere(windOrigin, 4.6f)) {
            Rigidbody rb = c.attachedRigidbody;
            if (!rb || rb.isKinematic || rb.mass > .45f) continue;
            Vector3 delta = rb.worldCenterOfMass - windOrigin;
            float downwind = Vector3.Dot(delta, windDirection.normalized);
            if (downwind < 0 || (delta - downwind * windDirection.normalized).sqrMagnitude > 1.1f) continue;
            Vector3 flow = windDirection.normalized + Vector3.up * .16f;
            rb.AddForce(flow * (30f / (1 + downwind * .28f)), ForceMode.Acceleration);
        }
    }
}
