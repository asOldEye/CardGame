using UnityEngine;
using UnityEngine.Events;

public class AnyButtonTrigger : MonoBehaviour
{
    [SerializeField]
    public UnityEvent OnAnyButtonPressed;

    protected virtual void Update()
    {
        if (Input.anyKey)
        {
            if (OnAnyButtonPressed != null) OnAnyButtonPressed.Invoke();
        }
    }
}