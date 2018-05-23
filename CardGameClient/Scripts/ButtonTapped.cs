using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonTapped : AnyButtonTrigger
{
    public UnityEvent OnEscapeTapped;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape) && OnEscapeTapped != null)
            OnEscapeTapped.Invoke();
    }
}
