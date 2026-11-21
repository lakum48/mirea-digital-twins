using UnityEngine;

public class RouteMover : MonoBehaviour
{
    public Vector3 start, end;
    public float seconds = 12, pause = 1.5f, offset;
    public Transform leftLeg, rightLeg;
    public bool roadLoop;
    float clock;
    Rigidbody body;

    void Awake() { body = GetComponent<Rigidbody>(); }
    void FixedUpdate()
    {
        clock += Time.fixedDeltaTime;
        if (roadLoop) {
            const float radius = 1.35f, laneLength = 34f, speed = 4.4f;
            float arc = Mathf.PI * radius;
            float s = Mathf.Repeat((clock + offset) * speed, 2 * (laneLength + arc));
            Vector3 p;
            Vector3 tangent;
            if (s < laneLength) {
                p = new Vector3(radius, 0, -17 + s); tangent = Vector3.forward;
            } else if (s < laneLength + arc) {
                float a = (s - laneLength) / radius;
                p = new Vector3(radius * Mathf.Cos(a), 0, 17 + radius * Mathf.Sin(a));
                tangent = new Vector3(-Mathf.Sin(a), 0, Mathf.Cos(a));
            } else if (s < 2 * laneLength + arc) {
                p = new Vector3(-radius, 0, 17 - (s - laneLength - arc)); tangent = Vector3.back;
            } else {
                float a = (s - 2 * laneLength - arc) / radius;
                p = new Vector3(-radius * Mathf.Cos(a), 0, -17 - radius * Mathf.Sin(a));
                tangent = new Vector3(Mathf.Sin(a), 0, -Mathf.Cos(a));
            }
            body.MovePosition(p);
            body.MoveRotation(Quaternion.LookRotation(tangent));
            return;
        }
        float t = Mathf.Repeat(clock + offset, (seconds + pause) * 2);
        bool returning = t >= seconds + pause;
        if (returning) t -= seconds + pause;
        float u = Mathf.Clamp01(t / seconds);
        float progress = Mathf.SmoothStep(0, 1, u);
        Vector3 pos = Vector3.Lerp(returning ? end : start, returning ? start : end, progress);
        Vector3 forward = returning ? start - end : end - start;
        // Рельсовый состав сохраняет ориентацию при обратном движении.
        Quaternion orientation = transform.name.StartsWith("Поезд") ? Quaternion.identity : Quaternion.LookRotation(forward);
        body.MovePosition(pos);
        body.MoveRotation(orientation);
        if (leftLeg && rightLeg) {
            float phase = Mathf.Sin((clock + offset) * 8) * 23 * Mathf.Sin(u * Mathf.PI);
            leftLeg.localRotation = Quaternion.Euler(phase, 0, 0);
            rightLeg.localRotation = Quaternion.Euler(-phase, 0, 0);
        }
    }
}
