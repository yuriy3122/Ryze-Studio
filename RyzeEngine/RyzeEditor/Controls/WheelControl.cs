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

        delegate void SetValuesCallback();

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

            ResumeLayout(false);
            PerformLayout();

            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
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
                    //subMeshSelectComboBox.Items.Clear();

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
                        //subMeshSelectComboBox.Items.Add(subMeshId);
                    }

                    selectedMeshesListBox.Items.Clear();

                    foreach (var subMeshId in wheel.SubMeshIds)
                    {
                        selectedMeshesListBox.Items.Add(subMeshId);
                    }

                    _selectedWheel = wheel;
                    _selectedWheel.Name = Guid.NewGuid().ToString();//Notify property changed
                    _selectedWheel.SubMeshIdsChanged += OnSubMeshIdsChanged;
                    UpdateCustomControls();
                }
            }
        }

        private void OnSubMeshIdsChanged(object sender, EventArgs eventArgs)
        {
            Wheel wheel = sender as Wheel;

            if (wheel != null)
            {
                foreach (var subMeshId in wheel.SubMeshIds)
                {
                    if (!selectedMeshesListBox.Items.Contains(subMeshId))
                    {
                        selectedMeshesListBox.Items.Add(subMeshId);
                    }
                }

                subMeshSelectButton.FlatStyle = FlatStyle.Standard;
                subMeshSelectButton.Text = "Select";
            }
        }

        private void UpdateCustomControls()
        {
            if (InvokeRequired)
            {
                SetValuesCallback d = new SetValuesCallback(UpdateCustomControls);
                Invoke(d);
            }
            else
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
                }
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

        private void SubMeshSelectButton_Click(object sender, EventArgs e)
        {
            bool isActive = false;

            if (subMeshSelectButton.FlatStyle == FlatStyle.Flat)
            {
                subMeshSelectButton.FlatStyle = FlatStyle.Standard;
                subMeshSelectButton.Text = "Select";
                isActive = false;
            }
            else if (subMeshSelectButton.FlatStyle == FlatStyle.Standard)
            {
                subMeshSelectButton.FlatStyle = FlatStyle.Flat;
                subMeshSelectButton.Text = "Cancel";
                isActive = true;
            }

            if (_selectedWheel != null)
            {
                _selectedWheel.SubmeshSelectionModeChanged?.Invoke(isActive, new EventArgs());
            }
        }
    }
}
