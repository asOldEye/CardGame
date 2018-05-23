using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditorCanvas : MonoBehaviour
{
    [SerializeField] ListBoxControl have;
    [SerializeField] ListBoxControl use;
    [SerializeField] Dropdown category;
    [SerializeField] GameObject button;
    [SerializeField] Text description;
    [SerializeField] GameObject wiew1;
    [SerializeField] GameObject wiew2;
    [SerializeField] GameObject wiew3;
    [SerializeField] GameObject wiew4;
    [SerializeField] Button chest;
    [SerializeField] Button take;
    GameObject active;
    List<string> spells = new List<string>();
    List<string> soliders = new List<string>();

    void Move(string where)
    {
        if (where == "chest")
        {
            for (var f = 0; f < use.transform.childCount; f++)
                if (use.transform.GetChild(f) == active.transform)
                {
                    active.transform.SetParent(chest.transform);

                    //active.transform.localPosition = new Vector3(0, )
                }
        }
    }

    void Change(GameObject obj)
    {
        if (active != null) active.GetComponent<Button>().interactable = true;
        active = obj;
        obj.GetComponent<Button>().interactable = false;
        take.interactable = true;
        chest.interactable = true;
    }

    void ReDraw()
    {
        //foreach(var f in )
        //have.AddItem(0, )
        
    }
}