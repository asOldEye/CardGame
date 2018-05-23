using UnityEngine;

public class ShineOnMouse : MonoBehaviour
{
    [SerializeField] int materialIndex;
    [SerializeField] Material shineMaterial;
    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().materials[materialIndex];
        //material = new Material(material);
        //GetComponent<MeshRenderer>().materials[materialIndex] = material;
    }

    private void OnMouseEnter() { Revert(); }
    private void OnMouseExit() { Revert(); }
    void Revert()
    {
        var f = new Material(material);
        material.CopyPropertiesFromMaterial(shineMaterial);
        shineMaterial.CopyPropertiesFromMaterial(f);
    }

    private void OnMouseDown()
    {

    }
}