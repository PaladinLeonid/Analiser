using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LinkManagerApp.Controls;
using LinkManagerApp.Models;
using LinkManagerApp.Services;
using LinkManagerApp.Utilities;

namespace LinkManagerApp.Forms
{
    public partial class DownloadForm : Form
    {
        private readonly DownloadService _downloadService;
        private readonly LinkService _linkService;
        private readonly string _userName;
        private ProgressPanel _progressPanel;

        public DownloadForm(DownloadService downloadService, LinkService linkService, string userName)
        {
            _downloadService = downloadService;
            _linkService = linkService;
            _userName = userName;
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "📥 Скачивание файла";
            Size = new Size(600, 400);
            UIHelper.SetDefaultFormStyle(this);

            var titleLabel = new Label
            {
                Text = "📥 СКАЧИВАНИЕ ФАЙЛА",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(560, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            var urlLabel = new Label
            {
                Text = "URL файла:",
                Location = new Point(20, 70),
                Size = new Size(100, 20)
            };

            var urlTextBox = UIHelper.CreateTextBox("https://example.com/file.zip");
            urlTextBox.Location = new Point(130, 70);
            urlTextBox.Size = new Size(430, 30);

            var fileNameLabel = new Label
            {
                Text = "Имя файла:",
                Location = new Point(20, 110),
                Size = new Size(100, 20)
            };

            var fileNameTextBox = UIHelper.CreateTextBox("downloaded_file");
            fileNameTextBox.Location = new Point(130, 110);
            fileNameTextBox.Size = new Size(430, 30);

            // Панель прогресса
            _progressPanel = new ProgressPanel
            {
                Location = new Point(20, 160),
                Visible = false
            };

            var downloadButton = UIHelper.CreateButton("📥 Скачать", 150, 40);
            downloadButton.Location = new Point(150, 280);
            downloadButton.Click += async (s, e) =>
            {
                await StartDownload(urlTextBox.Text, fileNameTextBox.Text);
            };

            var cancelButton = UIHelper.CreateButton("Отмена", 150, 40);
            cancelButton.Location = new Point(320, 280);
            cancelButton.Click += (s, e) => Close();

            // Предложить сохраненные ссылки
            var savedLinks = _linkService.LoadUserLinks(_userName)
                .Where(link => !string.IsNullOrEmpty(link.Url))
                .Take(5)
                .ToList();

            if (savedLinks.Count > 0)
            {
                var linksLabel = new Label
                {
                    Text = "Сохраненные ссылки:",
                    Location = new Point(20, 330),
                    Size = new Size(200, 20),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                var linksComboBox = new ComboBox
                {
                    Location = new Point(180, 330),
                    Size = new Size(380, 30),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                linksComboBox.Items.AddRange(savedLinks.ToArray());
                linksComboBox.DisplayMember = "Title";
                linksComboBox.SelectedIndexChanged += (s, e) =>
                {
                    if (linksComboBox.SelectedItem is UserLink selectedLink)
                    {
                        urlTextBox.Text = selectedLink.Url;
                        fileNameTextBox.Text = selectedLink.Title;
                    }
                };

                Controls.Add(linksLabel);
                Controls.Add(linksComboBox);
            }

            Controls.AddRange(new Control[] { titleLabel, urlLabel, urlTextBox, 
                fileNameLabel, fileNameTextBox, _progressPanel, downloadButton, cancelButton });
        }

        private async Task StartDownload(string url, string fileName)
        {
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("URL не может быть пустым!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"downloaded_file_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            _progressPanel.Visible = true;
            _progressPanel.UpdateProgress(0, "Начинаем скачивание...");

            try
            {
                var progress = new Progress<(double percentage, long current, long total)>(value =>
                {
                    _progressPanel.UpdateProgress(value.percentage, 
                        $"Скачивание: {FileHelper.FormatFileSize(value.current)} / {FileHelper.FormatFileSize(value.total)}");
                });

                var filePath = await _downloadService.DownloadFileAsync(url, fileName, progress);
                
                _progressPanel.UpdateProgress(100, "Скачивание завершено!");

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    MessageBox.Show($"✅ Файл успешно скачан!\n\n📍 Путь: {filePath}\n📊 Размер: {FileHelper.FormatFileSize(fileInfo.Length)}", 
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Предложить сохранить ссылку
                    if (MessageBox.Show("Хотите сохранить эту ссылку?", "Сохранение ссылки", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var newLink = new UserLink
                        {
                            UserName = _userName,
                            Url = url,
                            Title = fileName,
                            Category = "Загрузки",
                            AddedDate = DateTime.Now
                        };

                        _linkService.SaveUserLink(newLink);
                        MessageBox.Show("✅ Ссылка сохранена!", "Успех", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                _progressPanel.Visible = false;
                MessageBox.Show($"❌ Ошибка скачивания: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Создать информационный файл
                CreateAlternativeFile(fileName, url);
            }
        }

        private void CreateAlternativeFile(string fileName, string originalUrl)
        {
            try
            {
                string downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                    "Downloads");
                
                string filePath = Path.Combine(downloadsPath, $"{fileName}_info.txt");

                string content = $"Файл: {fileName}\n";
                content += $"Оригинальный URL: {originalUrl}\n";
                content += $"Дата попытки скачивания: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
                content += new string('═', 60) + "\n";
                content += "⚠️ Файл не был скачан автоматически. Возможные причины:\n\n";
                content += "🔒 Сайт блокирует скачивание программами\n";
                content += "💡 Решение: Откройте ссылку в браузере и скачайте вручную\n";

                File.WriteAllText(filePath, content);
                
                MessageBox.Show($"Создан информационный файл: {filePath}", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                // Игнорируем ошибки создания файла
            }
        }
    }
}