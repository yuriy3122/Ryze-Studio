using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX.Windows;
using RyzeEditor.Tools;

namespace RyzeEditor
{
	public partial class MainForm : RenderForm
	{
		private readonly List<ToolStripButton> _toolButtons = new List<ToolStripButton>();

		public MainForm()
		{
			InitializeComponent();

			tbPlacement.Tag = Tool.Placement;
			_toolButtons.Add(tbPlacement);

			tbSelect.Tag = Tool.Select;
			_toolButtons.Add(tbSelect);

			tbCustomSelect.Tag = Tool.CustomSelect;
			_toolButtons.Add(tbCustomSelect);

			tbTranslate.Tag = Tool.Translation;
			_toolButtons.Add(tbTranslate);

            tbRotate.Tag = Tool.Rotation;
            _toolButtons.Add(tbRotate);

            tbCollision.Tag = Tool.Collision;
            _toolButtons.Add(tbCollision);

            tbPointLight.Tag = Tool.PointLight;
            _toolButtons.Add(tbPointLight);

            tbVehicle.Tag = Tool.Vehicle;
            _toolButtons.Add(tbVehicle);
        }

		public event EventHandler<ToolChangedEventArgs> ToolChanged;

		public event EventHandler<FileOpenEventArgs> FileOpened;

		public event EventHandler<FileSaveEventArgs> FileSaved;

        public event EventHandler<EventArgs> UndoClicked;

        public event EventHandler<EventArgs> RedoClicked;

        public event EventHandler<EventArgs> PackClicked;

        public Control Panel
		{
			get { return inspectorPanel; }
		}

		private void tbPlacement_Click(object sender, EventArgs e)
		{
			ActivateToolButton(sender);
		}

		private void tbTranslate_Click(object sender, EventArgs e)
		{
			ActivateToolButton(sender);
		}

		private void tbRotate_Click(object sender, EventArgs e)
		{
			ActivateToolButton(sender);
		}

        private void tbPointLight_Click(object sender, EventArgs e)
        {
            ActivateToolButton(sender);
        }

        private void tbSelect_Click(object sender, EventArgs e)
		{
			ActivateToolButton(sender);
		}

        private void tbCustomSelect_Click(object sender, EventArgs e)
        {
            ActivateToolButton(sender);
        }

        private void tbCollision_Click(object sender, EventArgs e)
        {
            ActivateToolButton(sender);
        }

        private void tbVehicle_Click(object sender, EventArgs e)
        {
            ActivateToolButton(sender);
        }

        private void ActivateToolButton(object sender)
		{
			var tb = sender as ToolStripButton;

			foreach (var toolButton in _toolButtons)
			{
				toolButton.CheckState = toolButton == tb ? CheckState.Checked : CheckState.Unchecked;
			}

			var handler = ToolChanged;

			if (handler == null) return;

			if (tb != null) handler(this, new ToolChangedEventArgs((Tool)tb.Tag));
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();

			if (dialog.ShowDialog() != DialogResult.OK) return;

			EventHandler<FileOpenEventArgs> fileOpenEvent = FileOpened;

			if (fileOpenEvent == null) return;

			try
			{
				Cursor.Current = Cursors.WaitCursor;
				fileOpenEvent(this, new FileOpenEventArgs(dialog.FileName));
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

        private void compileStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandler<EventArgs> packEvent = PackClicked;

            if (packEvent == null)
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                packEvent(this, new EventArgs());
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialog = new SaveFileDialog();

			if (dialog.ShowDialog() != DialogResult.OK) return;

			EventHandler<FileSaveEventArgs> fileSaveEvent = FileSaved;

			if (fileSaveEvent == null) return;

			try
			{
				Cursor.Current = Cursors.WaitCursor;
				fileSaveEvent(this, new FileSaveEventArgs(dialog.FileName));
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

        private void tbUndo_Click(object sender, EventArgs e)
        {
            UndoClicked?.Invoke(this, new EventArgs());
        }

        private void tbRedo_Click(object sender, EventArgs e)
        {
            RedoClicked?.Invoke(this, new EventArgs());
        }
    }

    public class ToolChangedEventArgs : EventArgs
	{
		public ToolChangedEventArgs(Tool tool)
		{
			Tool = tool;
		}

		public Tool Tool { get; set; }
	}

	public class FileOpenEventArgs : EventArgs
	{
		public string FileName { get; private set; }

		public FileOpenEventArgs(String fileName)
		{
			FileName = fileName;
		}
	}

	public class FileSaveEventArgs : EventArgs
	{
		public string FileName { get; private set; }

		public FileSaveEventArgs(String fileName)
		{
			FileName = fileName;
		}
	}

    public class SelectionChangedEventArgs : EventArgs
	{
	}
}