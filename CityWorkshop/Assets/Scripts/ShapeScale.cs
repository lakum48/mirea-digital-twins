using UnityEngine;

public sealed class ShapeScale : ShapeKeys
{
    protected override void Update() => ApplyScale(ScaleAxes(), Time.deltaTime);
}
