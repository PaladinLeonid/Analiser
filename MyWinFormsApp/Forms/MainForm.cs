using System;
using System.Drawing;
using System.Windows.Forms;
using LinkManagerApp.Forms;
using LinkManagerApp.Models;
using LinkManagerApp.Services;
using LinkManagerApp.Utilities;

namespace LinkManagerApp
{
    public partial class MainForm : Form
    {
        private readonly AppSettings _settings;
        private readonly AuthService _authService;
        private readonly LinkService _linkService;
        private readonly DownloadService _downloadService;
        private readonly FileService _fileService;

        public MainForm()
        {
            InitializeComponent();
            
            // Инициализация настроек
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _settings = new AppSettings
            {
                CredentialsFilePath = Path.Combine(desktopPath, "user_credentials.txt"),
                LinksDatabasePath = Path.Combine(desktopPath, "user_links_db.json")
            };

            // Инициализация сервисов
            _authService = new AuthService(_settings);
            _linkService = new LinkService(_settings.LinksDatabasePath);
            _downloadService = new DownloadService();
            _fileService = new FileService();

            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "🌟 Менеджер ссылок";
            Size = new Size(800, 600);
            UIHelper.SetDefaultFormStyle(this);

            ShowLoginForm();
        }

        private void ShowLoginForm()
        {
            var loginForm = new LoginForm(_authService, _settings);
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                _settings.CurrentUser = loginForm.Username;
                _authService.SaveCredentials(loginForm.Username, loginForm.Password);
                ShowMainMenu();
            }
            else
            {
                Close();
            }
        }

        private void ShowMainMenu()
        {
            Controls.Clear();

            var titleLabel = new Label
            {
                Text = $"🎯 ГЛАВНОЕ МЕНЮ - Пользователь: {_settings.CurrentUser}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(750, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var buttons = new[]
            {
                CreateMenuButton("📁 Просмотреть мои данные", 300, 60, ShowMyCredentials),
                CreateMenuButton("🌐 Управление ссылками", 300, 60, ShowLinkManager),
                CreateMenuButton("📥 Скачать файл", 300, 60, ShowDownloadForm),
                CreateMenuButton("📝 Создать текстовый файл", 300, 60, ShowCreateFileForm),
                CreateMenuButton("🔍 Поиск файлов", 300, 60, ShowSearchFiles),
                CreateMenuButton("🛠️ Панель администратора", 300, 60, ShowAdminPanel),
                CreateMenuButton("🚪 Выход", 300, 60, ExitApplication)
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(250, 80 + i * 70);
            }

            Controls.Add(titleLabel);
            foreach (var button in buttons)
            {
                Controls.Add(button);
            }
        }

        private Button CreateMenuButton(string text, int width, int height, EventHandler clickHandler)
        {
            var button = UIHelper.CreateButton(text, width, height);
            button.Click += clickHandler;
            return button;
        }

        private void ShowMyCredentials(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_settings.CredentialsFilePath))
                {
                    string content = File.ReadAllText(_settings.CredentialsFilePath);
                    MessageBox.Show(content, "📋 Ваши сохраненные данные", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Файл с данными не найден!", "Информация", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowLinkManager(object sender, EventArgs e)
        {
            var linkManagerForm = new LinkManagerForm(_linkService, _settings.CurrentUser);
            linkManagerForm.ShowDialog();
        }

        private void ShowDownloadForm(object sender, EventArgs e)
        {
            var downloadForm = new DownloadForm(_downloadService, _linkService, _settings.CurrentUser);
            downloadForm.ShowDialog();
        }

        private void ShowCreateFileForm(object sender, EventArgs e)
        {
            // Реализация формы создания файла
        }

        private void ShowSearchFiles(object sender, EventArgs e)
        {
            // Реализация поиска файлов
        }

        private void ShowAdminPanel(object sender, EventArgs e)
        {
            var adminForm = new AdminForm(_authService, _settings);
            adminForm.ShowDialog();
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            Close();
        }
    }
}