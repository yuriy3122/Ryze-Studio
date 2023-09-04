using System;
using System.Windows.Forms;
using SharpDX;
using RyzeEditor.Extentions;
using RyzeEditor.GameWorld;

namespace RyzeEditor.Controls
{
	public partial class QuaternionUpDown : UserControl
	{
		[field: NonSerialized]
		private Vector3 _axis;
        private float _angle;

		[field: NonSerialized]
		public EventHandler ValueChanged;

		public Vector3 Axis
        {
			get
			{
				return _axis;
			}
			set
			{
                _axis = value;

				numSpinXAxis.ValueChanged -= NumSpinXAxis_ValueChanged;
				numSpinYAxis.ValueChanged -= NumSpinYAxis_ValueChanged;
				numSpinZAxis.ValueChanged -= NumSpinZAxis_ValueChanged;

                numSpinXAxis.Value = _axis.X.ToDecimal();
				numSpinYAxis.Value = _axis.Y.ToDecimal();
                numSpinZAxis.Value = _axis.Z.ToDecimal();

                numSpinXAxis.ValueChanged += NumSpinXAxis_ValueChanged;
				numSpinYAxis.ValueChanged += NumSpinYAxis_ValueChanged;
				numSpinZAxis.ValueChanged += NumSpinZAxis_ValueChanged;
			}
		}

        public float Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;

                numSpinWAxis.ValueChanged -= NumSpinXAxis_ValueChanged;

                numSpinWAxis.Value = _angle.ToDecimal();

                numSpinWAxis.ValueChanged += NumSpinZAxis_ValueChanged;
            }
        }

        public QuaternionUpDown()
		{
			InitializeComponent();
		}

		private void NumSpinXAxis_ValueChanged(object sender, EventArgs e)
		{
            _axis.X = Convert.ToSingle(numSpinXAxis.Value);

            ValueChanged?.Invoke(this, e);
        }

		private void NumSpinYAxis_ValueChanged(object sender, EventArgs e)
		{
            _axis.Y = Convert.ToSingle(numSpinYAxis.Value);

            ValueChanged?.Invoke(this, e);
        }

		private void NumSpinZAxis_ValueChanged(object sender, EventArgs e)
		{
            _axis.Z = Convert.ToSingle(numSpinZAxis.Value);

            ValueChanged?.Invoke(this, e);
        }

        private void NumSpinWAxis_ValueChanged(object sender, EventArgs e)
        {
            _angle = Convert.ToSingle(numSpinWAxis.Value);

            ValueChanged?.Invoke(this, e);
        }
    }
}