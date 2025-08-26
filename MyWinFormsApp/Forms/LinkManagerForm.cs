using System;
using System.Collections.Generic;
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
    public partial class LinkManagerForm : Form
    {
        private readonly LinkService _linkService;
        private readonly string _userName;
        private List<UserLink> _userLinks;
        private FlowLayoutPanel _linksPanel;

        public LinkManagerForm(LinkService linkService, string userName)
        {
            _linkService = linkService;
            _userName = userName;
            InitializeComponent();
            InitializeForm();
            LoadLinks();
        }

        private void InitializeForm()
        {
            Text = $"🌐 Управление ссылками - {_userName}";
            Size = new Size(900, 600);
            UIHelper.SetDefaultFormStyle(this);
            BackColor = Color.FromArgb(245, 245, 245);

            // Заголовок
            var titleLabel = new Label
            {
                Text = $"🌐 УПРАВЛЕНИЕ ССЫЛКАМИ - {_userName}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(850, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            // Панель кнопок
            var buttonsPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(850, 50),
                BackColor = Color.Transparent
            };

            var buttons = new[]
            {
                CreateActionButton("📋 Показать все", ShowAllLinks),
                CreateActionButton("➕ Добавить", AddNewLink),
                CreateActionButton("🗑️ Удалить", DeleteLink),
                CreateActionButton("🔍 Поиск", SearchLinks),
                CreateActionButton("📂 Экспорт", ExportLinks),
                CreateActionButton("↩️ Назад", CloseForm)
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(i * 140, 0);
                buttonsPanel.Controls.Add(buttons[i]);
            }

            // Панель для ссылок с прокруткой
            var scrollPanel = new Panel
            {
                Location = new Point(20, 120),
                Size = new Size(850, 420),
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            _linksPanel = new FlowLayoutPanel
            {
                Size = new Size(830, 400),
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            scrollPanel.Controls.Add(_linksPanel);

            Controls.Add(titleLabel);
            Controls.Add(buttonsPanel);
            Controls.Add(scrollPanel);
        }

        private Button CreateActionButton(string text, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += clickHandler;
            return button;
        }

        private void LoadLinks()
        {
            _userLinks = _linkService.LoadUserLinks(_userName);
            DisplayLinks(_userLinks);
        }

        private void DisplayLinks(List<UserLink> links)
        {
            _linksPanel.Controls.Clear();

            if (links.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "📋 У вас пока нет сохраненных ссылок.",
                    Font = new Font("Segoe UI", 12),
                    Size = new Size(800, 50),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Gray
                };
                _linksPanel.Controls.Add(emptyLabel);
                return;
            }

            foreach (var link in links)
            {
                var linkCard = new LinkCard(link);
                linkCard.DeleteClicked += (s, e) => DeleteLinkCard(link);
                linkCard.OpenClicked += (s, e) => OpenLink(link.Url);
                _linksPanel.Controls.Add(linkCard);
            }
        }

        private void ShowAllLinks(object sender, EventArgs e)
        {
            LoadLinks();
        }

        private void AddNewLink(object sender, EventArgs e)
        {
            using (var addForm = new AddLinkForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    var newLink = new UserLink
                    {
                        UserName = _userName,
                        Url = addForm.Url,
                        Title = addForm.Title,
                        Category = addForm.Category,
                        AddedDate = DateTime.Now
                    };

                    _linkService.SaveUserLink(newLink);
                    LoadLinks();
                    MessageBox.Show("✅ Ссылка успешно добавлена!", "Успех", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DeleteLink(object sender, EventArgs e)
        {
            if (_userLinks.Count == 0)
            {
                MessageBox.Show("❌ У вас нет ссылок для удаления.", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var deleteForm = new DeleteLinkForm(_userLinks))
            {
                if (deleteForm.ShowDialog() == DialogResult.OK && deleteForm.SelectedLink != null)
                {
                    _linkService.RemoveUserLink(deleteForm.SelectedLink);
                    LoadLinks();
                    MessageBox.Show("✅ Ссылка удалена!", "Успех", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DeleteLinkCard(UserLink link)
        {
            if (MessageBox.Show($"Удалить ссылку '{link.Title}'?", "Подтверждение", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _linkService.RemoveUserLink(link);
                LoadLinks();
            }
        }

        private void SearchLinks(object sender, EventArgs e)
        {
            using (var searchForm = new SearchForm())
            {
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    var results = _linkService.SearchLinks(_userName, searchForm.SearchText);
                    DisplayLinks(results);

                    if (results.Count == 0)
                    {
                        MessageBox.Show("🔍 По вашему запросу ничего не найдено.", "Результаты поиска", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void ExportLinks(object sender, EventArgs e)
        {
            if (_userLinks.Count == 0)
            {
                MessageBox.Show("❌ Нет ссылок для экспорта.", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string exportPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"{_userName}_links_export_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using (var writer = new StreamWriter(exportPath))
                {
                    writer.WriteLine($"Экспорт ссылок пользователя: {_userName}");
                    writer.WriteLine($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    writer.WriteLine(new string('=', 60));

                    foreach (var link in _userLinks)
                    {
                        writer.WriteLine($"🔗 Ссылка: {link.Url}");
                        writer.WriteLine($"📝 Название: {link.Title}");
                        writer.WriteLine($"📂 Категория: {link.Category}");
                        writer.WriteLine($"📅 Добавлено: {link.AddedDate:dd.MM.yyyy HH:mm}");
                        writer.WriteLine(new string('─', 40));
                    }
                }

                MessageBox.Show($"✅ Ссылки экспортированы в файл:\n{exportPath}", "Успех", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка при экспорте: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenLink(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка при открытии ссылки: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }
    }

    // Вспомогательные формы
    public class AddLinkForm : Form
    {
        public string Url { get; private set; } = "";
        public string Title { get; private set; } = "";
        public string Category { get; private set; } = "Общее";

        public AddLinkForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "➕ Добавление новой ссылки";
            Size = new Size(400, 250);
            UIHelper.SetDefaultFormStyle(this);

            var urlLabel = new Label { Text = "URL ссылки:", Location = new Point(20, 20), Size = new Size(100, 20) };
            var urlTextBox = UIHelper.CreateTextBox("https://example.com");
            urlTextBox.Location = new Point(130, 20);

            var titleLabel = new Label { Text = "Название:", Location = new Point(20, 60), Size = new Size(100, 20) };
            var titleTextBox = UIHelper.CreateTextBox("Моя ссылка");
            titleTextBox.Location = new Point(130, 60);

            var categoryLabel = new Label { Text = "Категория:", Location = new Point(20, 100), Size = new Size(100, 20) };
            var categoryTextBox = UIHelper.CreateTextBox("Общее");
            categoryTextBox.Location = new Point(130, 100);

            var okButton = UIHelper.CreateButton("Добавить", 100, 40);
            okButton.Location = new Point(100, 150);
            okButton.Click += (s, e) =>
            {
                Url = urlTextBox.Text;
                Title = titleTextBox.Text;
                Category = categoryTextBox.Text;

                if (string.IsNullOrEmpty(Url))
                {
                    MessageBox.Show("URL не может быть пустым!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Url.StartsWith("http://") && !Url.StartsWith("https://"))
                {
                    Url = "https://" + Url;
                }

                DialogResult = DialogResult.OK;
                Close();
            };

            var cancelButton = UIHelper.CreateButton("Отмена", 100, 40);
            cancelButton.Location = new Point(220, 150);
            cancelButton.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { urlLabel, urlTextBox, titleLabel, titleTextBox, 
                categoryLabel, categoryTextBox, okButton, cancelButton });
        }
    }

    public class DeleteLinkForm : Form
    {
        public UserLink SelectedLink { get; private set; }

        public DeleteLinkForm(List<UserLink> links)
        {
            InitializeForm(links);
        }

        private void InitializeForm(List<UserLink> links)
        {
            Text = "🗑️ Удаление ссылки";
            Size = new Size(500, 300);
            UIHelper.SetDefaultFormStyle(this);

            var label = new Label
            {
                Text = "Выберите ссылку для удаления:",
                Location = new Point(20, 20),
                Size = new Size(450, 20)
            };

            var listBox = new ListBox
            {
                Location = new Point(20, 50),
                Size = new Size(450, 150),
                DisplayMember = "Title"
            };

            foreach (var link in links)
            {
                listBox.Items.Add(link);
            }

            var deleteButton = UIHelper.CreateButton("Удалить", 100, 40);
            deleteButton.Location = new Point(150, 210);
            deleteButton.Click += (s, e) =>
            {
                if (listBox.SelectedItem is UserLink selectedLink)
                {
                    SelectedLink = selectedLink;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Выберите ссылку для удаления!", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var cancelButton = UIHelper.CreateButton("Отмена", 100, 40);
            cancelButton.Location = new Point(270, 210);
            cancelButton.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { label, listBox, deleteButton, cancelButton });
        }
    }

    public class SearchForm : Form
    {
        public string SearchText { get; private set; } = "";

        public SearchForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "🔍 Поиск по ссылкам";
            Size = new Size(400, 150);
            UIHelper.SetDefaultFormStyle(this);

            var label = new Label
            {
                Text = "Введите текст для поиска:",
                Location = new Point(20, 20),
                Size = new Size(350, 20)
            };

            var searchTextBox = UIHelper.CreateTextBox();
            searchTextBox.Location = new Point(20, 50);
            searchTextBox.Size = new Size(350, 30);

            var searchButton = UIHelper.CreateButton("Искать", 100, 40);
            searchButton.Location = new Point(120, 90);
            searchButton.Click += (s, e) =>
            {
                SearchText = searchTextBox.Text;
                if (string.IsNullOrEmpty(SearchText))
                {
                    MessageBox.Show("Текст поиска не может быть пустым!", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.AddRange(new Control[] { label, searchTextBox, searchButton });
        }
    }
}