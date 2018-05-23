using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AuxiliaryLibrary;
using CardEnvironmentShared;

public class Game : MonoBehaviour
{
    public PlayerInfo Me { get; private set; } = null;
    [SerializeField] LoginWindowScript login;

    static public Game ThisGame { get; private set; }

    void Awake()
    {
        ThisGame = this;
        ClientConnectionHolder.ThisConnection.OnObjectReceived += OnObjectReceived;
    }

    void Update()
    {

    }

    public void Login(string login, string password)
    {
        ClientConnectionHolder.ThisConnection.Send(new APICommand("Login", new object[] { new LoginningForm(login, password) }));
    }
    public void Registration(string login, string password)
    {
        ClientConnectionHolder.ThisConnection.Send(new APICommand("Registration", new object[] { new LoginningForm(login, password) }));
    }
    public void Unlogin()
    {
        ClientConnectionHolder.ThisConnection.Send(new APICommand("Unlogin", new object[] { }));
        login.Unlogin();
        Me = null;
    }

    public void OnObjectReceived(object obj)
    {
        if (obj is APIAnswer)
        {
            var f = (obj as APIAnswer);
            switch (f.Command.Command)
            {
                case "Login":
                    if (f.Exception != null)
                        login.WrongAnswer("Login error", f.Exception.Message);
                    else if (f.Answer is PlayerInfo)
                    {
                        Me = f.Answer as PlayerInfo;
                        login.CorrectMessage();
                    }
                    break;
                case "Registration":
                    if (f.Exception != null)
                        login.WrongAnswer("Registration error", f.Exception.Message);
                    else if (f.Answer is PlayerInfo)
                    {
                        Me = f.Answer as PlayerInfo;
                        login.CorrectMessage();
                    }
                    break;
            }
        }
    }
}