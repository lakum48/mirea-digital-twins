using UnityEngine;

public sealed class ShapeMove : ShapeKeys
{
    protected override void Update() => ApplyMove(MoveAxes(), Time.deltaTime);
}
