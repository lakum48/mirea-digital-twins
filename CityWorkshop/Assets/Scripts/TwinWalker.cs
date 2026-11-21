using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TwinWalker : MonoBehaviour
{
    public Camera eyes;
    CharacterController controller;
    Rigidbody held;
    float pitch, fall, portalTime;
    RoomDevice targeted;
    string prompt;

    void Awake() { controller = GetComponent<CharacterController>(); }
    public void Teleport(Vector3 where)
    {
        if (Time.time < portalTime) return;
        Drop();
        controller.enabled = false;
        transform.position = where;
        controller.enabled = true;
        fall = 0;
        portalTime = Time.time + .6f;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;
        if (Cursor.lockState == CursorLockMode.Locked) {
            transform.Rotate(0, Input.GetAxis("Mouse X") * 2, 0);
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 2, -82, 82);
            eyes.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        direction = transform.TransformDirection(Vector3.ClampMagnitude(direction, 1));
        fall = controller.isGrounded && fall < 0 ? -2 : fall + Physics.gravity.y * Time.deltaTime;
        controller.Move((direction * 4.3f + Vector3.up * fall) * Time.deltaTime);
        prompt = "";
        targeted = null;
        if (held) {
            Vector3 next = eyes.transform.position + eyes.transform.forward * 1.45f;
            if (Physics.Raycast(eyes.transform.position, eyes.transform.forward, out RaycastHit wall, 1.7f,
                    ~0, QueryTriggerInteraction.Ignore) && wall.rigidbody != held)
                next = eyes.transform.position + eyes.transform.forward * Mathf.Max(.25f, wall.distance - .22f);
            held.transform.position = next;
            prompt = "E — положить";
            if (Input.GetKeyDown(KeyCode.E)) Drop();
        } else if (Physics.Raycast(eyes.transform.position, eyes.transform.forward, out RaycastHit hit, 3.6f,
                    ~0, QueryTriggerInteraction.Ignore)) {
            targeted = hit.collider.GetComponentInParent<RoomDevice>();
            CarryItem item = hit.collider.GetComponentInParent<CarryItem>();
            if (targeted) {
                prompt = "E — переключить";
                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) targeted.Toggle();
            } else if (item) {
                bool allowed = !item.cabinet || item.cabinet.OpenFraction > .85f;
                prompt = allowed ? "E — взять" : "Сначала откройте шкаф";
                if (allowed && Input.GetKeyDown(KeyCode.E)) {
                    item.cabinet = null;
                    held = item.GetComponent<Rigidbody>();
                    held.isKinematic = true;
                    held.detectCollisions = false;
                }
            }
        }
    }
    void Drop() {
        if (!held) return;
        held.detectCollisions = true;
        held.isKinematic = false;
        held = null;
    }
    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb && !rb.isKinematic && hit.moveDirection.y > -.4f)
            rb.AddForceAtPosition(Vector3.ProjectOnPlane(hit.moveDirection, Vector3.up) * 14, hit.point, ForceMode.Force);
    }
    void OnGUI() {
        GUI.Label(new Rect(Screen.width / 2 - 3, Screen.height / 2 - 10, 18, 25), "+");
        GUI.Box(new Rect(10, Screen.height - 62, 435, 48), "WASD — движение   |   мышь — обзор   |   E — действие\n" + prompt);
    }
}
