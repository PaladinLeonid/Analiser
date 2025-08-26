using System;
using System.Drawing;
using System.Windows.Forms;
using LinkManagerApp.Models;

namespace LinkManagerApp.Controls
{
    public partial class LinkCard : UserControl
    {
        public UserLink Link { get; private set; }
        public event EventHandler DeleteClicked;
        public event EventHandler OpenClicked;

        public LinkCard(UserLink link)
        {
            InitializeComponent();
            Link = link;
            InitializeCard();
        }

        private void InitializeCard()
        {
            Size = new Size(300, 150);
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = Link.Title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(280, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var urlLabel = new Label
            {
                Text = Link.Url,
                Font = new Font("Segoe UI", 8),
                Location = new Point(10, 35),
                Size = new Size(280, 15),
                ForeColor = Color.Blue,
                Cursor = Cursors.Hand
            };
            urlLabel.Click += (s, e) => OpenClicked?.Invoke(this, EventArgs.Empty);

            var categoryLabel = new Label
            {
                Text = $"Категория: {Link.Category}",
                Font = new Font("Segoe UI", 8),
                Location = new Point(10, 55),
                Size = new Size(280, 15)
            };

            var dateLabel = new Label
            {
                Text = $"Добавлено: {Link.AddedDate:dd.MM.yyyy HH:mm}",
                Font = new Font("Segoe UI", 8),
                Location = new Point(10, 75),
                Size = new Size(280, 15)
            };

            var deleteButton = new Button
            {
                Text = "🗑️",
                Size = new Size(30, 30),
                Location = new Point(250, 100),
                FlatStyle = FlatStyle.Flat
            };
            deleteButton.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

            var openButton = new Button
            {
                Text = "🌐",
                Size = new Size(30, 30),
                Location = new Point(210, 100),
                FlatStyle = FlatStyle.Flat
            };
            openButton.Click += (s, e) => OpenClicked?.Invoke(this, EventArgs.Empty);

            Controls.Add(titleLabel);
            Controls.Add(urlLabel);
            Controls.Add(categoryLabel);
            Controls.Add(dateLabel);
            Controls.Add(deleteButton);
            Controls.Add(openButton);
        }
    }
}