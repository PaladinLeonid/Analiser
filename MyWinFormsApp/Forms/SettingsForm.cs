using System;
using System.Drawing;
using System.Windows.Forms;
using LinkManagerApp.Models;
using LinkManagerApp.Utilities;

namespace LinkManagerApp.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly AppSettings _settings;

        public SettingsForm(AppSettings settings)
        {
            _settings = settings;
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "⚙️ Настройки";
            Size = new Size(400, 300);
            UIHelper.SetDefaultFormStyle(this);

            var titleLabel = new Label
            {
                Text = "⚙️ НАСТРОЙКИ ПРИЛОЖЕНИЯ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(360, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            var adminPasswordLabel = new Label
            {
                Text = "Пароль администратора:",
                Location = new Point(20, 70),
                Size = new Size(200, 20)
            };

            var adminPasswordTextBox = new TextBox
            {
                Location = new Point(220, 70),
                Size = new Size(150, 30),
                Text = _settings.AdminPassword,
                PasswordChar = '*'
            };

            var dbPathLabel = new Label
            {
                Text = "Путь к базе ссылок:",
                Location = new Point(20, 110),
                Size = new Size(200, 20)
            };

            var dbPathTextBox = new TextBox
            {
                Location = new Point(220, 110),
                Size = new Size(150, 30),
                Text = _settings.LinksDatabasePath,
                ReadOnly = true
            };

            var credPathLabel = new Label
            {
                Text = "Путь к credentials:",
                Location = new Point(20, 150),
                Size = new Size(200, 20)
            };

            var credPathTextBox = new TextBox
            {
                Location = new Point(220, 150),
                Size = new Size(150, 30),
                Text = _settings.CredentialsFilePath,
                ReadOnly = true
            };

            var saveButton = UIHelper.CreateButton("💾 Сохранить", 120, 40);
            saveButton.Location = new Point(100, 200);
            saveButton.Click += (s, e) =>
            {
                _settings.AdminPassword = adminPasswordTextBox.Text;
                MessageBox.Show("Настройки сохранены!", "Успех", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            };

            var cancelButton = UIHelper.CreateButton("Отмена", 120, 40);
            cancelButton.Location = new Point(240, 200);
            cancelButton.Click += (s, e) => Close();

            Controls.AddRange(new Control[] {
                titleLabel, adminPasswordLabel, adminPasswordTextBox,
                dbPathLabel, dbPathTextBox, credPathLabel, credPathTextBox,
                saveButton, cancelButton
            });
        }
    }
}