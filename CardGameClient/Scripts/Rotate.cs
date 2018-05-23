using UnityEngine;

public class Rotate : MonoBehaviour
{
[SerializeField] Vector3 rotationAxis = new Vector3(0, 0, 0);
    [SerializeField] float rotationSpeed = 1f;

    private void Awake()
    { RotationAxis = rotationAxis; }

    public Vector3 RotationAxis
    {
        get { return rotationAxis; }
        set { rotationAxis = value.normalized; }
    }
    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = value; }
    }

    protected void RotateMe()
    {
        if (RotationSpeed != 0)
            transform.Rotate(rotationAxis * Time.deltaTime * RotationSpeed);
    }

    protected virtual void Update()
    { RotateMe(); }
}