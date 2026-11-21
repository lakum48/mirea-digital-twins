using UnityEngine;

public sealed class ShapeTurn : ShapeKeys
{
    protected override void Update() => ApplyTurn(TurnAxes(), Time.deltaTime);
}
