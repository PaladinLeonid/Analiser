using System;
using System.Drawing;
using System.Windows.Forms;

namespace LinkManagerApp.Controls
{
    public partial class ProgressPanel : UserControl
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        private Label percentageLabel;

        public ProgressPanel()
        {
            InitializePanel();
        }

        private void InitializePanel()
        {
            Size = new Size(400, 100);
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;

            statusLabel = new Label
            {
                Text = "Загрузка...",
                Location = new Point(10, 10),
                Size = new Size(380, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            progressBar = new ProgressBar
            {
                Location = new Point(10, 40),
                Size = new Size(380, 20),
                Style = ProgressBarStyle.Continuous
            };

            percentageLabel = new Label
            {
                Text = "0%",
                Location = new Point(10, 70),
                Size = new Size(380, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(statusLabel);
            Controls.Add(progressBar);
            Controls.Add(percentageLabel);
        }

        public void UpdateProgress(double percentage, string status = null)
        {
            if (status != null)
                statusLabel.Text = status;

            progressBar.Value = (int)percentage;
            percentageLabel.Text = $"{percentage:F1}%";
        }
    }
}