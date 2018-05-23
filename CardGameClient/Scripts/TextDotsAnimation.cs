using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDotsAnimation : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] int MaxDotsCount = 3;
    [SerializeField] [Range(0.1f, 10)] float Delay = 3;

    [SerializeField] public List<Text> Texts;
    int dotsCount = 0;
    void Start() { Anim(); }

    void Anim()
    {
        if (Texts != null && Texts.Count > 0)
            if (dotsCount < MaxDotsCount)
            {
                dotsCount++;
                foreach (var f in Texts) f.text += ".";
            }
            else
            {
                foreach (var f in Texts) f.text = f.text.Split('.')[0];
                dotsCount = 0;
            }
        Invoke("Anim", Delay);
    }
}