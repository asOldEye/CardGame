using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoginWindowScript : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] InputField login;
    [SerializeField] InputField password;
    [SerializeField] Text wrongAnswerText;
    [SerializeField] Text wrongAnswerHead;
    [SerializeField] Text[] Name;

    public UnityEvent NotConnectedLoginning;
    public UnityEvent EmptyLoginningData;
    public UnityEvent Loginned;
    public UnityEvent NotLoginned;
    public UnityEvent Unloginned;

    void Start()
    {
        toggle.isOn = PlayerPrefs.HasKey("loginToggleIsOn") ? PlayerPrefs.GetInt("loginToggleIsOn") == 1 : false;

        login.text = PlayerPrefs.HasKey("loginLogin") ? PlayerPrefs.GetString("loginLogin") : "";
        password.text = PlayerPrefs.HasKey("loginPassword") ? PlayerPrefs.GetString("loginPassword") : "";
    }

    public void Login()
    {
        Send(false);
    }
    public void Registration()
    {
        Send(true);
    }
    void Send(bool reg)
    {
        if (ClientConnectionHolder.ThisConnection.Connected)
        {
            if (login.text == string.Empty || password.text == string.Empty)
            {
                if (EmptyLoginningData != null) EmptyLoginningData.Invoke();
                return;
            }
            if (reg) Game.ThisGame.Registration(login.text, password.text);
            else Game.ThisGame.Login(login.text, password.text);
        }
        else if (NotConnectedLoginning != null) NotConnectedLoginning.Invoke();
    }

    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("loginToggleIsOn", toggle.isOn ? 1 : 0);
        if (toggle.isOn)
            SerializeLogPass();
        else
            ClearLogPass();
    }
    public void SerializeLogPass()
    {
        PlayerPrefs.SetString("loginLogin", login.text);
        PlayerPrefs.SetString("loginPassword", password.text);
    }
    public void ClearLogPass()
    {
        PlayerPrefs.SetString("loginLogin", "");
        PlayerPrefs.SetString("loginPassword", "");
    }

    public void WrongAnswer(string head, string message)
    {
        wrongAnswerHead.text = head;
        wrongAnswerText.text = message;
        NotLoginned.Invoke();
    }
    public void CorrectMessage()
    {
        foreach (var f in Name) f.text = Game.ThisGame.Me.Name;
        Loginned.Invoke();
    }

    public void Unlogin()
    {
        foreach (var f in Name) f.text = "Unsigned";
        if (Unloginned != null) Unloginned.Invoke();
    }
}