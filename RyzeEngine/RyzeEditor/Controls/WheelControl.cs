using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using RyzeEditor.GameWorld;

namespace RyzeEditor.Controls
{
    public partial class WheelControl : UserControl
    {
        [field: NonSerialized]
        private List<Wheel> _wheels;

        [field: NonSerialized]
        private string _chassisMeshId;

        [field: NonSerialized]
        public EventHandler WheelsChanged;

        [field: NonSerialized]
        private Wheel _selectedWheel;

        [field: NonSerialized]
        private VectorUpDown _wheelAxleVectorUpDown;

        [field: NonSerialized]
        private VectorUpDown _wheelDirectionCSVectorUpDown;

        [field: NonSerialized]
        private VectorUpDown _chassisConnectionPointCSVectorUpDown;

        [field: NonSerialized]
        private NumericUpDown _suspensionRestLengthNumericUpDown;

        [field: NonSerialized]
        private NumericUpDown _suspensionStiffnessNumericUpDown;

        [field: NonSerialized]
        private NumericUpDown _suspensionCompressionNumericUpDown;

        [field: NonSerialized]
        private NumericUpDown _suspensionDampingNumericUpDown;

        public string ChassisMeshId
        {
            get
            {
                return _chassisMeshId;
            }
            set
            {
                _chassisMeshId = value;
            }
        }

        public List<Wheel> Wheels
        {
            get
            {
                return _wheels;
            }
            set
            {
                _wheels = value;
            }
        }

        public WheelControl()
        {
            InitializeComponent();

            SuspendLayout();
            tableLayoutPanel.SuspendLayout();

            int retval = 7;

            var axleLabel = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Direction of the wheel's axle:" };
            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(axleLabel, 0, retval);

            _wheelAxleVectorUpDown = new VectorUpDown { Dock = DockStyle.Fill, AutoSize = true };
            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(_wheelAxleVectorUpDown, 0, retval);

            var wheelDirectionLabel = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Direction of ray cast:" };
            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(wheelDirectionLabel, 0, retval);

            _wheelDirectionCSVectorUpDown = new VectorUpDown { Dock = DockStyle.Fill, AutoSize = true };
            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(_wheelDirectionCSVectorUpDown, 0, retval);

            var chassisConnectionPointCSLabel = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Starting point of the ray:" };
            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(chassisConnectionPointCSLabel, 0, retval);

            _chassisConnectionPointCSVectorUpDown = new VectorUpDown { Dock = DockStyle.Fill, AutoSize = true };
            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(_chassisConnectionPointCSVectorUpDown, 0, retval);

            var suspMaxLenLabel = new Label
            {
                AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.None, Text = @"Max susp. length:"
            };

            _suspensionRestLengthNumericUpDown = new NumericUpDown
            {
                DecimalPlaces = 2, Maximum = 1000, Increment = 0.1M, Dock = DockStyle.Fill, Anchor = AnchorStyles.Top
            };

            var suspensionRestLengthPanel = new TableLayoutPanel() { Dock = DockStyle.Fill, Height = 30, ColumnCount = 2, RowCount = 1 };
            suspensionRestLengthPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            suspensionRestLengthPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            suspensionRestLengthPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            suspensionRestLengthPanel.Controls.Add(suspMaxLenLabel, 0, 0);
            suspensionRestLengthPanel.Controls.Add(_suspensionRestLengthNumericUpDown, 1, 0);

            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(suspensionRestLengthPanel, 0, retval);

            var suspensionStiffness = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Text = @"Stiffness constant :"
            };

            _suspensionStiffnessNumericUpDown = new NumericUpDown
            {
                DecimalPlaces = 2,
                Maximum = 1000,
                Increment = 0.1M,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top
            };

            var suspensionStiffnessPanel = new TableLayoutPanel() { Dock = DockStyle.Fill, Height = 30, ColumnCount = 2, RowCount = 1 };
            suspensionStiffnessPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            suspensionStiffnessPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            suspensionStiffnessPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            suspensionStiffnessPanel.Controls.Add(suspensionStiffness, 0, 0);
            suspensionStiffnessPanel.Controls.Add(_suspensionStiffnessNumericUpDown, 1, 0);

            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(suspensionStiffnessPanel, 0, retval);

            var suspensionCompression = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Text = @"Susp. compr.:"
            };

            _suspensionCompressionNumericUpDown = new NumericUpDown
            {
                DecimalPlaces = 2,
                Maximum = 1000,
                Increment = 0.1M,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top
            };

            var suspensionCompressionPanel = new TableLayoutPanel() { Dock = DockStyle.Fill, Height = 30, ColumnCount = 2, RowCount = 1 };
            suspensionCompressionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            suspensionCompressionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            suspensionCompressionPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            suspensionCompressionPanel.Controls.Add(suspensionCompression, 0, 0);
            suspensionCompressionPanel.Controls.Add(_suspensionCompressionNumericUpDown, 1, 0);

            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(suspensionCompressionPanel, 0, retval);

            var suspensionDampingLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Text = @"Susp. damping:"
            };

            _suspensionDampingNumericUpDown = new NumericUpDown
            {
                DecimalPlaces = 2,
                Maximum = 1000,
                Increment = 0.1M,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top
            };

            var suspensionDampingPanel = new TableLayoutPanel() { Dock = DockStyle.Fill, Height = 30, ColumnCount = 2, RowCount = 1 };
            suspensionDampingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            suspensionDampingPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            suspensionDampingPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            suspensionDampingPanel.Controls.Add(suspensionDampingLabel, 0, 0);
            suspensionDampingPanel.Controls.Add(_suspensionDampingNumericUpDown, 1, 0);

            retval = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Controls.Add(suspensionDampingPanel, 0, retval);

            ResumeLayout(false);
            PerformLayout();

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
        }

        private void AddSubMeshButton_Click(object sender, EventArgs e)
        {
            if (subMeshSelectComboBox.SelectedItem == null)
            {
                return;
            }

            var selectedSubMeshId = subMeshSelectComboBox.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(selectedSubMeshId))
            {
                if (_selectedWheel != null)
                {
                    if (!_selectedWheel.SubMeshIds.Contains(selectedSubMeshId))
                    {
                        _selectedWheel.SubMeshIds.Add(selectedSubMeshId);
                        _selectedWheel.Name = Guid.NewGuid().ToString();//notify property changed
                        selectedMeshesListBox.Items.Add(selectedSubMeshId);
                        OnSelectedWheelChanged();
                    }
                }
            }
        }

        private void DeleteSelectedMeshButton_Click(object sender, EventArgs e)
        {
            if (selectedMeshesListBox.SelectedItem == null)
            {
                return;
            }

            var selectedSubMeshId = selectedMeshesListBox.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(selectedSubMeshId))
            {
                if (_selectedWheel != null)
                {
                    _selectedWheel.SubMeshIds.Remove(selectedSubMeshId);
                    _selectedWheel.Name = Guid.NewGuid().ToString();//notify property changed
                    selectedMeshesListBox.Items.Remove(selectedMeshesListBox.SelectedItem);
                    OnSelectedWheelChanged();
                }
            }
        }

        private void WheelSelectComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            OnSelectedWheelChanged();
        }

        private void OnSelectedWheelChanged()
        {
            var selectedWheel = wheelSelectComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedWheel))
            {
                Wheel wheel = GetSelectedWheel();

                if (wheel != null)
                {
                    subMeshSelectComboBox.Items.Clear();

                    var mesh = ResourceManagment.ResourceManager.Instance.GetMesh(wheel.MeshId);

                    var subMeshIds = new List<uint>();

                    foreach (var subMesh in mesh.SubMeshes)
                    {
                        if (_wheels.Any(x => x.SubMeshIds.Contains(subMesh.Id.ToString())))
                        {
                            continue;
                        }

                        subMeshIds.Add(subMesh.Id);
                    }
                    subMeshIds.Sort();

                    foreach (var subMeshId in subMeshIds)
                    {
                        subMeshSelectComboBox.Items.Add(subMeshId);
                    }

                    selectedMeshesListBox.Items.Clear();

                    foreach (var subMeshId in wheel.SubMeshIds)
                    {
                        selectedMeshesListBox.Items.Add(subMeshId);
                    }

                    _selectedWheel = wheel;
                    _selectedWheel.Name = Guid.NewGuid().ToString();//Notify property changed
                    UpdateCustomControls();
                }
            }
        }

        private void UpdateCustomControls()
        {
            if (_selectedWheel != null)
            {
                _wheelAxleVectorUpDown.ValueChanged -= WheelAxleValueChanged;
                _wheelAxleVectorUpDown.Vector = _selectedWheel.AxleCS;
                _wheelAxleVectorUpDown.ValueChanged += WheelAxleValueChanged;

                _wheelDirectionCSVectorUpDown.ValueChanged -= WheelDirectionCSVectorValueChanged;
                _wheelDirectionCSVectorUpDown.Vector = _selectedWheel.WheelDirectionCS;
                _wheelDirectionCSVectorUpDown.ValueChanged += WheelDirectionCSVectorValueChanged;

                _chassisConnectionPointCSVectorUpDown.ValueChanged -= ChassisConnectionPointCSValueChanged;
                _chassisConnectionPointCSVectorUpDown.Vector = _selectedWheel.ChassisConnectionPointCS;
                _chassisConnectionPointCSVectorUpDown.ValueChanged += ChassisConnectionPointCSValueChanged;

                _suspensionRestLengthNumericUpDown.ValueChanged -= SuspensionRestLengthValueChanged;
                _suspensionRestLengthNumericUpDown.Value = Convert.ToDecimal(_selectedWheel.SuspensionRestLength);
                _suspensionRestLengthNumericUpDown.ValueChanged += SuspensionRestLengthValueChanged;

                _suspensionStiffnessNumericUpDown.ValueChanged -= SuspensionStiffnessValueChanged;
                _suspensionStiffnessNumericUpDown.Value = Convert.ToDecimal(_selectedWheel.SuspensionStiffness);
                _suspensionStiffnessNumericUpDown.ValueChanged += SuspensionStiffnessValueChanged;

                _suspensionCompressionNumericUpDown.ValueChanged -= SuspensionCompressionValueChanged;
                _suspensionCompressionNumericUpDown.Value = Convert.ToDecimal(_selectedWheel.SuspensionCompression);
                _suspensionCompressionNumericUpDown.ValueChanged += SuspensionCompressionValueChanged;

                _suspensionDampingNumericUpDown.ValueChanged -= SuspensionDampingValueChanged;
                _suspensionDampingNumericUpDown.Value = Convert.ToDecimal(_selectedWheel.SuspensionDamping);
                _suspensionDampingNumericUpDown.ValueChanged += SuspensionDampingValueChanged;
            }
        }

        private void SuspensionDampingValueChanged(object sender, EventArgs e)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.SuspensionDamping = (float)_suspensionDampingNumericUpDown.Value;
            }
        }

        private void SuspensionCompressionValueChanged(object sender, EventArgs e)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.SuspensionCompression = (float)_suspensionCompressionNumericUpDown.Value;
            }
        }

        private void SuspensionStiffnessValueChanged(object sender, EventArgs e)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.SuspensionStiffness = (float)_suspensionStiffnessNumericUpDown.Value;
            }
        }

        private void ChassisConnectionPointCSValueChanged(object sender, EventArgs e)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.ChassisConnectionPointCS = _chassisConnectionPointCSVectorUpDown.Vector;
            }
        }

        private void WheelDirectionCSVectorValueChanged(object sender, EventArgs e)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.WheelDirectionCS = _wheelDirectionCSVectorUpDown.Vector;
            }
        }

        private void SuspensionRestLengthValueChanged(object sender, EventArgs e)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.SuspensionRestLength = (float)_suspensionRestLengthNumericUpDown.Value;
            }
        }

        private void WheelAxleValueChanged(object sender, EventArgs eventArgs)
        {
            if (_selectedWheel != null)
            {
                _selectedWheel.AxleCS = _wheelAxleVectorUpDown.Vector;
            }
        }

        private Wheel GetSelectedWheel()
        {
            Wheel result = null;

            var selectedWheel = wheelSelectComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedWheel))
            {
                switch (selectedWheel)
                {
                    case "Front Left":
                        result = _wheels.FirstOrDefault(x => x.IsFrontWheel && x.IsLeftSideWheel);
                        break;

                    case "Front Right":
                        result = _wheels.FirstOrDefault(x => x.IsFrontWheel && !x.IsLeftSideWheel);
                        break;

                    case "Back Left":
                        result = _wheels.FirstOrDefault(x => !x.IsFrontWheel && x.IsLeftSideWheel);
                        break;

                    case "Back Right":
                        result = _wheels.FirstOrDefault(x => !x.IsFrontWheel && !x.IsLeftSideWheel);
                        break;
                }
            }

            return result;
        }
    }
}
