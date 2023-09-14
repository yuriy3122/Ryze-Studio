using System.Windows.Forms;

namespace RyzeEditor.Controls
{
    public partial class ConsoleOutputControl : UserControl
    {
        public ConsoleOutputControl()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {
            OutputListBox.Items.Add(message);
        }

        public void ClearAll()
        {
            OutputListBox.Items.Clear();
        }
    }
}
