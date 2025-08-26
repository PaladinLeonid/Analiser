using System;
using System.Drawing;
using System.Windows.Forms;
using LinkManagerApp.Models;
using LinkManagerApp.Services;
using LinkManagerApp.Utilities;

namespace LinkManagerApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        private readonly AppSettings _settings;

        public string Username { get; private set; }
        public string Password { get; private set; }

        public LoginForm(AuthService authService, AppSettings settings)
        {
            _authService = authService;
            _settings = settings;
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "🌟 Система авторизации";
            Size = new Size(400, 300);
            UIHelper.SetDefaultFormStyle(this);

            var titleLabel = new Label
            {
                Text = "🌟 СИСТЕМА АВТОРИЗАЦИИ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var loginLabel = new Label
            {
                Text = "Логин:",
                Location = new Point(50, 80),
                Size = new Size(100, 20)
            };

            var loginTextBox = UIHelper.CreateTextBox("Введите логин");
            loginTextBox.Location = new Point(150, 80);

            var passwordLabel = new Label
            {
                Text = "Пароль:",
                Location = new Point(50, 120),
                Size = new Size(100, 20)
            };

            var passwordTextBox = new TextBox
            {
                Location = new Point(150, 120),
                Size = new Size(200, 30),
                PasswordChar = '*'
            };

            var loginButton = UIHelper.CreateButton("Войти", 100, 40);
            loginButton.Location = new Point(150, 170);
            loginButton.Click += (s, e) =>
            {
                if (_authService.ValidateCredentials(loginTextBox.Text, passwordTextBox.Text))
                {
                    Username = loginTextBox.Text;
                    Password = passwordTextBox.Text;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Логин и пароль не могут быть пустыми!", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Controls.Add(titleLabel);
            Controls.Add(loginLabel);
            Controls.Add(loginTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(passwordTextBox);
            Controls.Add(loginButton);
        }
    }
}