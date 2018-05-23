using System;

namespace CardEnvironmentShared
{
    /// <summary>
    /// Форма регистрации
    /// </summary>
    [Serializable]
    public class LoginningForm
    {
        public LoginningForm(string login, string password)
        {
            try
            {
                Login = login;
                Password = password;
            }
            catch { throw; }
        }

        string login, password;
        public string Login
        {
            get { return login; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Login));
                if ((login = value) == String.Empty) throw new ArgumentException("Empty login");
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Password));
                if ((password = value) == String.Empty) throw new ArgumentException("Empty password");
            }
        }
    }
}