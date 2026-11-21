using UnityEngine;

public class ShapeKeys : MonoBehaviour
{
    public bool move, resize, rotate;

    protected virtual void Update()
    {
        if (move) ApplyMove(MoveAxes(), Time.deltaTime);
        if (resize) ApplyScale(ScaleAxes(), Time.deltaTime);
        if (rotate) ApplyTurn(TurnAxes(), Time.deltaTime);
    }

    protected static Vector3 MoveAxes() => new Vector3(
        Axis(KeyCode.RightArrow, KeyCode.LeftArrow), Axis(KeyCode.PageUp, KeyCode.PageDown),
        Axis(KeyCode.UpArrow, KeyCode.DownArrow));
    protected static Vector3 ScaleAxes() => new Vector3(
        Axis(KeyCode.U, KeyCode.J), Axis(KeyCode.I, KeyCode.K), Axis(KeyCode.O, KeyCode.L));
    protected static Vector3 TurnAxes() => new Vector3(
        Axis(KeyCode.R, KeyCode.F), Axis(KeyCode.T, KeyCode.G), Axis(KeyCode.Y, KeyCode.H));

    public void ApplyMove(Vector3 axes, float seconds) =>
        transform.Translate(axes * (2f * seconds), Space.World);
    public void ApplyScale(Vector3 axes, float seconds) =>
        transform.localScale = Vector3.Max(Vector3.one * .15f, transform.localScale + axes * seconds);
    public void ApplyTurn(Vector3 axes, float seconds) =>
        transform.Rotate(axes * (50f * seconds), Space.World);

    static int Axis(KeyCode plus, KeyCode minus) =>
        (Input.GetKey(plus) ? 1 : 0) - (Input.GetKey(minus) ? 1 : 0);
}
