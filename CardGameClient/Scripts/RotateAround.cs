using UnityEngine;

public class RotateAround : Rotate
{
    [SerializeField] Transform pivotPoint;

    public Transform PivotPoint
    {
        get { return pivotPoint; }
        set { pivotPoint = value; }
    }
    protected void RotateMeAround()
    {
        if (pivotPoint != null)
            if (RotationAxis != Vector3.zero && RotationSpeed != 0)
                transform.RotateAround(pivotPoint.position, RotationAxis, Time.deltaTime * RotationSpeed);
    }
    protected override void Update()
    { RotateMeAround(); }
}