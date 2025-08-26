using System.Drawing;
using System.Windows.Forms;

namespace LinkManagerApp.Utilities
{
    public static class UIHelper
    {
        public static void SetDefaultFormStyle(Form form)
        {
            form.BackColor = Color.FromArgb(240, 240, 240);
            form.Font = new Font("Segoe UI", 9);
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        public static Button CreateButton(string text, int width = 120, int height = 40)
        {
            return new Button
            {
                Text = text,
                Size = new Size(width, height),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        public static TextBox CreateTextBox(string placeholder = "", int width = 250)
        {
            return new TextBox
            {
                Size = new Size(width, 30),
                PlaceholderText = placeholder
            };
        }
    }
}