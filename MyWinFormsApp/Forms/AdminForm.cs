using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LinkManagerApp.Models;
using LinkManagerApp.Services;
using LinkManagerApp.Utilities;

namespace LinkManagerApp.Forms
{
    public partial class AdminForm : Form
    {
        private readonly AuthService _authService;
        private readonly AppSettings _settings;

        public AdminForm(AuthService authService, AppSettings settings)
        {
            _authService = authService;
            _settings = settings;
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "🛠️ Панель администратора";
            Size = new Size(500, 400);
            UIHelper.SetDefaultFormStyle(this);

            var titleLabel = new Label
            {
                Text = "🛠️ ПАНЕЛЬ АДМИНИСТРАТОРА",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(460, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            var buttons = new[]
            {
                CreateAdminButton("👥 Показать credentials", ShowAllCredentials),
                CreateAdminButton("🗑️ Очистить credentials", ClearCredentials),
                CreateAdminButton("📊 Информация о системе", ShowSystemInfo),
                CreateAdminButton("↩️ Назад", CloseForm)
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(150, 80 + i * 70);
            }

            Controls.Add(titleLabel);
            foreach (var button in buttons)
            {
                Controls.Add(button);
            }
        }

        private Button CreateAdminButton(string text, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += clickHandler;
            return button;
        }

        private void ShowAllCredentials(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_settings.CredentialsFilePath))
                {
                    string content = File.ReadAllText(_settings.CredentialsFilePath);
                    
                    var textBox = new TextBox
                    {
                        Text = content,
                        Multiline = true,
                        ScrollBars = ScrollBars.Vertical,
                        ReadOnly = true,
                        Size = new Size(450, 300),
                        Font = new Font("Consolas", 9)
                    };

                    var form = new Form
                    {
                        Text = "👥 Все сохраненные credentials",
                        Size = new Size(500, 400),
                        StartPosition = FormStartPosition.CenterParent
                    };
                    form.Controls.Add(textBox);
                    
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Файл с credentials не найден!", "Информация", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearCredentials(object sender, EventArgs e)
        {
            if (File.Exists(_settings.CredentialsFilePath))
            {
                if (MessageBox.Show("❓ Вы уверены, что хотите очистить файл credentials?", "Подтверждение", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(_settings.CredentialsFilePath);
                        MessageBox.Show("✅ Файл credentials успешно очищен!", "Успех", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Ошибка при удалении файла: {ex.Message}", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Файл не найден!", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowSystemInfo(object sender, EventArgs e)
        {
            var info = $"=== 📊 ИНФОРМАЦИЯ О СИСТЕМЕ ===\n\n" +
                      $"ОС: {Environment.OSVersion}\n" +
                      $"Версия .NET: {Environment.Version}\n" +
                      $"Имя компьютера: {Environment.MachineName}\n" +
                      $"Имя пользователя: {Environment.UserName}\n" +
                      $"Папка программы: {Environment.CurrentDirectory}\n" +
                      $"Память: {FileHelper.FormatFileSize(GC.GetTotalMemory(false))}";

            MessageBox.Show(info, "Системная информация", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }
    }
}