using System;
using System.Windows.Forms;
using SharpDX;
using RyzeEditor.Extentions;
using SharpDX.Direct3D11;

namespace RyzeEditor.Controls
{
	public partial class VectorUpDown : UserControl
	{
		[field: NonSerialized]
		private Vector3 _vector;

		[field: NonSerialized]
		private Vector3 _eplison = new Vector3(0.000000001f, 0.000000001f, 0.000000001f);

		[field: NonSerialized]
		public EventHandler ValueChanged;

        delegate void SetValuesCallback();

        public Vector3 Vector
		{
			get
			{
				return _vector;
			}
			set
			{
				if (Vector3.NearEqual(ref _vector, ref value, ref _eplison))
				{
					return;
				}

				_vector = value;

                SetValues();
            }
		}

        private void SetValues()
        {
            if (InvokeRequired)
            {
                SetValuesCallback d = new SetValuesCallback(SetValues);
                Invoke(d);
            }
            else
            {
                numSpinXAxis.ValueChanged -= NumSpinXAxis_ValueChanged;
                numSpinYAxis.ValueChanged -= NumSpinYAxis_ValueChanged;
                numSpinZAxis.ValueChanged -= NumSpinZAxis_ValueChanged;

                numSpinXAxis.Value = _vector.X.ToDecimal();
                numSpinYAxis.Value = _vector.Y.ToDecimal();
                numSpinZAxis.Value = _vector.Z.ToDecimal();

                numSpinXAxis.ValueChanged += NumSpinXAxis_ValueChanged;
                numSpinYAxis.ValueChanged += NumSpinYAxis_ValueChanged;
                numSpinZAxis.ValueChanged += NumSpinZAxis_ValueChanged;
            }
        }

		public VectorUpDown()
		{
			InitializeComponent();
		}

		private void NumSpinXAxis_ValueChanged(object sender, EventArgs e)
		{
			_vector.X = Convert.ToSingle(numSpinXAxis.Value);

            ValueChanged?.Invoke(this, e);
        }

		private void NumSpinYAxis_ValueChanged(object sender, EventArgs e)
		{
			_vector.Y = Convert.ToSingle(numSpinYAxis.Value);

            ValueChanged?.Invoke(this, e);
        }

		private void NumSpinZAxis_ValueChanged(object sender, EventArgs e)
		{
			_vector.Z = Convert.ToSingle(numSpinZAxis.Value);

            ValueChanged?.Invoke(this, e);
        }
	}
}