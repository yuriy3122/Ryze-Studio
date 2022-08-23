using System;
using System.Windows.Forms;
using SharpDX;
using RyzeEditor.Extentions;

namespace RyzeEditor.Controls
{
    public partial class Matrix3x3Control : UserControl
    {
        [field: NonSerialized]
        private Matrix3x3 _matrix;

        [field: NonSerialized]
        public EventHandler ValueChanged;

        public Matrix3x3Control()
        {
            InitializeComponent();
        }

        public Matrix3x3 Matrix
        {
            get
            {
                return _matrix;
            }
            set
            {
                _matrix = value;

                numSpinM11.ValueChanged -= numSpinM11_ValueChanged;
                numSpinM12.ValueChanged -= numSpinM12_ValueChanged;
                numSpinM13.ValueChanged -= numSpinM13_ValueChanged;
                numSpinM21.ValueChanged -= numSpinM21_ValueChanged;
                numSpinM22.ValueChanged -= numSpinM22_ValueChanged;
                numSpinM23.ValueChanged -= numSpinM23_ValueChanged;
                numSpinM31.ValueChanged -= numSpinM31_ValueChanged;
                numSpinM32.ValueChanged -= numSpinM32_ValueChanged;
                numSpinM33.ValueChanged -= numSpinM33_ValueChanged;

                numSpinM11.Value = _matrix.M11.ToDecimal();
                numSpinM12.Value = _matrix.M12.ToDecimal();
                numSpinM13.Value = _matrix.M13.ToDecimal();
                numSpinM21.Value = _matrix.M21.ToDecimal();
                numSpinM22.Value = _matrix.M22.ToDecimal();
                numSpinM23.Value = _matrix.M23.ToDecimal();
                numSpinM31.Value = _matrix.M31.ToDecimal();
                numSpinM32.Value = _matrix.M32.ToDecimal();
                numSpinM33.Value = _matrix.M33.ToDecimal();

                numSpinM11.ValueChanged += numSpinM11_ValueChanged;
                numSpinM12.ValueChanged += numSpinM12_ValueChanged;
                numSpinM13.ValueChanged += numSpinM13_ValueChanged;
                numSpinM21.ValueChanged += numSpinM21_ValueChanged;
                numSpinM22.ValueChanged += numSpinM22_ValueChanged;
                numSpinM23.ValueChanged += numSpinM23_ValueChanged;
                numSpinM31.ValueChanged += numSpinM31_ValueChanged;
                numSpinM32.ValueChanged += numSpinM32_ValueChanged;
                numSpinM33.ValueChanged += numSpinM33_ValueChanged;
            }
        }

        private void numSpinM33_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M33 = Convert.ToSingle(numSpinM33.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM32_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M32 = Convert.ToSingle(numSpinM32.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM31_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M31 = Convert.ToSingle(numSpinM31.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM23_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M23 = Convert.ToSingle(numSpinM23.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM22_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M22 = Convert.ToSingle(numSpinM22.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM21_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M21 = Convert.ToSingle(numSpinM21.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM13_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M13 = Convert.ToSingle(numSpinM13.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM11_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M11 = Convert.ToSingle(numSpinM11.Value);
            ValueChanged?.Invoke(this, e);
        }

        private void numSpinM12_ValueChanged(object sender, EventArgs e)
        {
            _matrix.M11 = Convert.ToSingle(numSpinM12.Value);
            ValueChanged?.Invoke(this, e);
        }
    }
}