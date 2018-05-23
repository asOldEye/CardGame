using UnityEngine;
using UnityEngine.UI;

public class ConnectScript : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] InputField ip;
    [SerializeField] InputField port;

    void Start()
    {
        toggle.isOn = PlayerPrefs.HasKey("connectToggleIsOn") ? PlayerPrefs.GetInt("connectToggleIsOn") == 1 : false;

        ip.text = PlayerPrefs.HasKey("connectIP") ? PlayerPrefs.GetString("connectIP") : "";
        port.text = PlayerPrefs.HasKey("connectPort") ? PlayerPrefs.GetString("connectPort") : "";
    }

    public void Connect()
    {
        int.TryParse(port.text, out ClientConnectionHolder.ThisConnection.port);
        ClientConnectionHolder.ThisConnection.address = ip.text;
        ClientConnectionHolder.ThisConnection.Connect();
    }

    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("connectToggleIsOn", toggle.isOn ? 1 : 0);
        if (toggle.isOn)
            SerializeLogPass();
        else
            ClearLogPass();
    }
    public void SerializeLogPass()
    {
        PlayerPrefs.SetString("connectIP", ip.text);
        PlayerPrefs.SetString("connectPort", port.text);
    }
    public void ClearLogPass()
    {
        PlayerPrefs.SetString("connectIP", "");
        PlayerPrefs.SetString("connectPort", "");
    }
}