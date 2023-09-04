using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using SharpDX;
using RyzeEditor.Tools;
using RyzeEditor.GameWorld;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Controls
{
	public partial class InspectorControl : UserControl
	{
		[field: NonSerialized]
		private Selection _selection;

		[field: NonSerialized]
		private ITool _tool;

		[field: NonSerialized]
		private readonly Dictionary<string, Control> _controls = new Dictionary<string, Control>();

        [field: NonSerialized]
        private int _retval;

        public InspectorControl()
		{
			InitializeComponent();
		}

		public Selection Selection
		{
			set
			{
				_selection = value;

				if (_selection != null)
				{
					_selection.SelectionChanged += OnSelectionChanged;
				}
			}
			get
			{
				return _selection;
			}
		}

		public void UpdateControls(ITool tool)
		{
			_tool = tool;

			UpdateControls();
		}

		private void UpdateControls()
		{
			layoutPanel.Controls.Clear();
            layoutPanel.RowStyles.Clear();
			_controls.Clear();

            switch (_tool)
            {
                case PlacementTool _:
                    UpdatePlacementToolControls();
                    break;
                case CollisionTool _:
                    UpdateCollisionToolControls();
                    break;
                case VehicleTool _:
                    UpdateVehicleToolControls();
                    break;
                default:
                    UpdateCustomToolControls();
                    break;
            }
        }

        private static void LoadCollisionSelectionModeCombo(Control cbo, Type enumType)
        {
            if (!(cbo is ComboBox control)) return;

            control.Items.Clear();

            foreach (var item in Enum.GetValues(enumType))
            {
                control.Items.Add(item);
            }
        }

        private void UpdateCollisionToolControls()
        {
            if (!(_tool is CollisionTool tool) || _selection.Get().Count > 1)
            {
                return;
            }

            SuspendLayout();
            layoutPanel.SuspendLayout();

            var cmbSelectMesh = new ComboBox { AutoSize = true, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSelectMesh.MouseWheel += CmbSelectMesh_MouseWheel;
            var files = ResourceManager.Instance.GetMeshIdList();

            foreach (var file in files)
            {
                cmbSelectMesh.Items.Add(file);
            }

            if (tool.SelectedMeshId != null)
            {
                cmbSelectMesh.SelectedIndex = cmbSelectMesh.Items.IndexOf(tool.SelectedMeshId);
            }

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Select collision mesh:" };
            layoutPanel.Controls.Add(label, 0, _retval);
            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(cmbSelectMesh, 0, _retval);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Selection Mode:" };
            layoutPanel.Controls.Add(label, 0, _retval);

            var cmbSelectMode = new ComboBox { AutoSize = true, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSelectMode.MouseWheel += CmbSelectMesh_MouseWheel;
            LoadCollisionSelectionModeCombo(cmbSelectMode, typeof(CollisionSelectionMode));

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(cmbSelectMode, 0, _retval);

            var cmbSelectSubMesh = new ComboBox { AutoSize = true, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSelectSubMesh.MouseWheel += CmbSelectMesh_MouseWheel;

            tool.SubMeshChanged += (sender, args) =>
            {
                if (args == null || !cmbSelectSubMesh.Items.Contains(args.SubMeshId))
                {
                    return;
                }

                cmbSelectSubMesh.SelectedIndex = cmbSelectSubMesh.Items.IndexOf(args.SubMeshId);
            };

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Select collision submesh:" };
            layoutPanel.Controls.Add(label, 0, _retval);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(cmbSelectSubMesh, 0, _retval);

            cmbSelectMesh.SelectedValueChanged += (sender, args) =>
            {
                var comboBox = sender as ComboBox;

                if (comboBox != null && comboBox.SelectedItem == null) return;

                cmbSelectMode.SelectedItem = -1;
                if (comboBox != null) tool.SelectedMeshId = comboBox.SelectedItem.ToString();
                tool.Options.SubMeshIds = new List<int>();
                InitRigidBodyPropertyControls(tool.SelectedRigidBody, 17);
            };

            cmbSelectMode.SelectedValueChanged += (sender, args) =>
            {
                var comboBox = sender as ComboBox;

                if (comboBox?.SelectedItem == null) return;

                tool.SelectionMode = (CollisionSelectionMode)cmbSelectMode.SelectedItem;

                var rigidBody = tool.SelectedRigidBody;

                tool.RemoveUnselectedRigidBodies();
                cmbSelectSubMesh.Items.Clear();

                if (rigidBody == null || tool.SelectionMode != CollisionSelectionMode.CollisionSubMesh) return;

                foreach (var subMesh in rigidBody.Mesh.SubMeshes)
                {
                    cmbSelectSubMesh.Items.Add((int)subMesh.Id);
                }
            };

            cmbSelectMode.SelectedItem = tool?.SelectedRigidBody?.SubMeshId != null ? CollisionSelectionMode.CollisionSubMesh : CollisionSelectionMode.CollisionMesh;

            cmbSelectSubMesh.SelectedValueChanged += (sender, args) =>
            {
                var comboBox = sender as ComboBox;

                if (comboBox?.SelectedItem == null) return;

                tool.SelectSubMesh((int)comboBox.SelectedItem);
                InitRigidBodyPropertyControls(tool.SelectedRigidBody, 17);
            };

            if (tool?.SelectedRigidBody?.SubMeshId != null)
            {
                int id = (int)tool.SelectedRigidBody.SubMeshId.Value;
                cmbSelectSubMesh.SelectedItem = id;
            }

            InitRigidBodyPropertyControls(tool?.SelectedRigidBody, _retval);

            ResumeLayout(false);
            PerformLayout();

            layoutPanel.ResumeLayout(false);
            layoutPanel.PerformLayout();
        }

        private void CmbSelectMesh_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void UpdateVehicleToolControls()
        {
            if (!(_tool is VehicleTool tool) || _selection.Get().Count > 1)
            {
                return;
            }

            var vehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();

            Button createVehicleButton = null;

            if (vehicle != null && vehicle.ChassisMeshId != null)
            {
                tool.SelectedMeshId = vehicle.ChassisMeshId;
            }
            else
            {
                createVehicleButton = new Button { AutoSize = true, Dock = DockStyle.Fill, Text = "Create Vehicle" };

                createVehicleButton.Click += (sender, args) =>
                {
                    var gameObject = _selection.Get().OfType<GameObject>().FirstOrDefault();

                    if (gameObject != null)
                    {
                        tool.SelectedMeshId = gameObject.GeometryMeshes.FirstOrDefault()?.Id;
                        var control = sender as Button;
                        control.Visible = false;
                    }
                };
            }

            layoutPanel.Controls.Clear();
            layoutPanel.RowStyles.Clear();
            layoutPanel.RowCount = 0;
            _retval = 0;

            SuspendLayout();
            layoutPanel.SuspendLayout();

            if (createVehicleButton != null)
            {
                _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layoutPanel.Controls.Add(createVehicleButton, 0, _retval);
            }

            InitCustomControlProperties();

            ResumeLayout(false);
            PerformLayout();

            layoutPanel.ResumeLayout(false);
            layoutPanel.PerformLayout();
        }

        private void InitRigidBodyPropertyControls(EntityBase rigidBody, int retval)
        {
            if (rigidBody == null)
            {
                return;
            }

            var controlCount = layoutPanel.Controls.Count;

            for (var i = 0; i < controlCount - 6; i++)
            {
                layoutPanel.Controls.RemoveAt(6);
            }

            var properties = rigidBody.GetType().GetProperties();

            foreach (var property in properties.Where(property => property.GetSetMethod() != null && property.GetGetMethod() != null))
            {
                if (property.GetCustomAttribute<InspectorVisible>()?.IsVisible == false) continue;

                retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = $@"{property.Name}:" };
                layoutPanel.Controls.Add(label, 0, retval);

                Control control = null;

                if (property.PropertyType.BaseType == typeof(Enum))
                {
                    control = new ComboBox { AutoSize = true, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
                    control.MouseWheel += CmbSelectMesh_MouseWheel;
                    control.DataBindings.Add("SelectedItem", rigidBody, property.Name);
                    LoadCollisionSelectionModeCombo(control, property.PropertyType);
                }
                else if (property.PropertyType == typeof(float))
                {
                    control = new NumericUpDown { DecimalPlaces = 2, Increment = 0.1M };
                    control.DataBindings.Add("Value", rigidBody, property.Name);
                }
                else if (property.PropertyType == typeof(Vector3))
                {
                    control = new VectorUpDown();
                    control.DataBindings.Add("Vector", rigidBody, property.Name);
                }
                else if (property.PropertyType == typeof(Matrix3x3))
                {
                    control = new Matrix3x3Control();
                    control.DataBindings.Add("Matrix", rigidBody, property.Name);
                }

                if (control == null) continue;

                retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layoutPanel.Controls.Add(control, 0, retval);
            }
        }

        private void UpdatePlacementToolControls()
		{
			SuspendLayout();
			layoutPanel.SuspendLayout();

			var cmbSelectMesh = new ComboBox {AutoSize = true, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSelectMesh.MouseWheel += CmbSelectMesh_MouseWheel;

            var files = ResourceManager.Instance.GetMeshIdList();

			foreach (var file in files)
			{
				cmbSelectMesh.Items.Add(file);
			}

			cmbSelectMesh.SelectedValueChanged += (sender, args) =>
			{
                var comboBox = sender as ComboBox;

                if (_tool is PlacementTool tool && comboBox != null && comboBox.SelectedItem != null)
                {
                    tool.SelectedMeshId = comboBox.SelectedItem.ToString();
                }
            };

			int retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = @"Select mesh:"};
			layoutPanel.Controls.Add(label, 0, retval);

			retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layoutPanel.Controls.Add(cmbSelectMesh, 0, retval);

			ResumeLayout(false);
			PerformLayout();

			layoutPanel.ResumeLayout(false);
			layoutPanel.PerformLayout();			
		}

        private void InitQuaternionControl(IReadOnlyCollection<EntityBase> entities, PropertyInfo property)
        {
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = $@"{property.Name}:" };

            int retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(label, 0, retval);
            var propertyInfo = entities.First().GetType().GetProperty(property.Name);

            var control = new QuaternionUpDown();

            if (entities.Count == 1)
            {
                if (propertyInfo != null)
                {
                    var quaternion = (Quaternion)propertyInfo.GetValue(entities.First());

                    control.Axis = quaternion.Axis;
                    control.Angle = quaternion.Angle;
                }
            }

            var prop = property;

            control.ValueChanged += ControlValueChanged;

            foreach (var entity in entities)
            {
                entity.PropertyChanged += (sender, args) =>
                {
                    if (sender == null || !_controls.ContainsKey(args.PropertyName) || !(_controls[args.PropertyName] is QuaternionUpDown quaternionDown))
                    {
                        return;
                    }

                    if (control.ValueChanged != null)
                    {
                        control.ValueChanged -= ControlValueChanged;
                    }

                    if (_selection.Get().OfType<EntityBase>().Count() == 1)
                    {
                        var param = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
                        var quaternion = (Quaternion)param;

                        quaternionDown.Axis = quaternion.Axis;
                        quaternionDown.Angle = quaternion.Angle;
                    }

                    control.ValueChanged += ControlValueChanged;
                };
            }

            _controls.Add(prop.Name, control);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(control, 0, _retval);
        }

        private void InitVector3Control(IReadOnlyCollection<EntityBase> entities, PropertyInfo property)
        {
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = $@"{property.Name}:" };

            int retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(label, 0, retval);
            var propertyInfo = entities.First().GetType().GetProperty(property.Name);

            var control = new VectorUpDown();

            if (entities.Count == 1)
            {
                if (propertyInfo != null) control.Vector = (Vector3) propertyInfo.GetValue(entities.First());
            }

            var prop = property;

            control.ValueChanged += ControlValueChanged;

            foreach (var entity in entities)
            {
                entity.PropertyChanged += (sender, args) =>
                {
                    if (sender == null || !_controls.ContainsKey(args.PropertyName) || !(_controls[args.PropertyName] is VectorUpDown vecUpDown))
                    {
                        return;
                    }

                    if (control.ValueChanged != null)
                    {
                        control.ValueChanged -= ControlValueChanged;
                    }

                    if (_selection.Get().OfType<EntityBase>().Count() == 1)
                    {
                        var param = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
                        vecUpDown.Vector = (Vector3)param;
                    }

                    control.ValueChanged += ControlValueChanged;
                };
            }

            _controls.Add(prop.Name, control);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(control, 0, _retval);
        }

        private void InitInt32Contol(List<EntityBase> entities, PropertyInfo property)
        {
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            var propertyInfo = entities.First().GetType().GetProperty(property.Name);            
            var control = new NumericUpDown() { DecimalPlaces = 0 };

            if (entities.Count == 1)
            {
                control.Value = Convert.ToDecimal(propertyInfo.GetValue(entities.First()));
                control.ValueChanged += ControlValueChanged;
            }

            foreach (var entity in entities)
            {
                entity.PropertyChanged += (sender, args) =>
                {
                    if (sender == null || !_controls.ContainsKey(args.PropertyName) || !(_controls[args.PropertyName] is NumericUpDown numUpDown))
                    {
                        return;
                    }

                    control.ValueChanged -= ControlValueChanged;

                    if (_selection.Get().OfType<EntityBase>().Count() == 1)
                    {
                        var param = sender.GetType().GetProperty(args.PropertyName)?.GetValue(sender);
                        if (param != null) numUpDown.Value = (int) param;
                    }

                    control.ValueChanged += ControlValueChanged;
                };
            }

            var prop = property;

            var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = string.Format("{0}: ", property.Name) };
            var panel = new TableLayoutPanel { ColumnCount = 2, RowCount = 1 };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(label, 0, 0);
            panel.Controls.Add(control, 1, 0);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(panel, 0, _retval);
        }

        private void InitBoolControl(List<EntityBase> entities, PropertyInfo property)
        {
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            var propertyInfo = entities.First().GetType().GetProperty(property.Name);
            var checkBoxControl = new CheckBox() { Dock = DockStyle.Fill, Anchor = AnchorStyles.Top };

            if (entities.Count == 1)
            {
                checkBoxControl.Checked = Convert.ToBoolean(propertyInfo.GetValue(entities.First()));
                checkBoxControl.CheckedChanged += ControlValueChanged;
            }

            foreach (var entity in entities)
            {
                entity.PropertyChanged += (sender, args) =>
                {
                    if (sender == null || !_controls.ContainsKey(args.PropertyName) || !(_controls[args.PropertyName] is CheckBox checkBox))
                    {
                        return;
                    }

                    checkBox.CheckStateChanged -= ControlValueChanged;

                    if (_selection.Get().OfType<EntityBase>().Count() == 1)
                    {
                        var param = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
                        checkBox.Checked = (bool)param;
                    }

                    checkBoxControl.CheckedChanged += ControlValueChanged;
                };
            }

            _controls.Add(property.Name, checkBoxControl);

            var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = $@"{property.Name}"};
            var panel = new TableLayoutPanel() { Dock = DockStyle.Fill, Height = 30, ColumnCount = 2, RowCount = 1 };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(label, 0, 0);
            panel.Controls.Add(checkBoxControl, 1, 0);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(panel, 0, _retval);
        }

        private void InitFloatControl(List<EntityBase> entities, PropertyInfo property)
        {
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            var propertyInfo = entities.First().GetType().GetProperty(property.Name);
            var numericControl = new NumericUpDown { DecimalPlaces = 2, Maximum = 10000000, Increment = 0.1M, Dock = DockStyle.Fill, Anchor = AnchorStyles.Top };

            if (entities.Count == 1)
            {
                numericControl.Value = Convert.ToDecimal(propertyInfo.GetValue(entities.First()));
                numericControl.ValueChanged += ControlValueChanged;
            }

            foreach (var entity in entities)
            {
                entity.PropertyChanged += (sender, args) =>
                {
                    if (sender == null || !_controls.ContainsKey(args.PropertyName) || !(_controls[args.PropertyName] is NumericUpDown numUpDown))
                    {
                        return;
                    }

                    numUpDown.ValueChanged -= ControlValueChanged;

                    if (_selection.Get().OfType<EntityBase>().Count() == 1)
                    {
                        var param = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);

                        if (param != null)
                        {
                            numUpDown.Value = Convert.ToDecimal(param);
                        }
                    }

                    numUpDown.ValueChanged += ControlValueChanged;
                };
            }

            _controls.Add(property.Name, numericControl);

            var label = new Label { AutoSize = true, Dock = DockStyle.Fill, Text = $@"{property.Name}" };
            var panel = new TableLayoutPanel() { Dock = DockStyle.Fill, Height = 30, ColumnCount = 2, RowCount = 1 };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(label, 0, 0);
            panel.Controls.Add(numericControl, 1, 0);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(panel, 0, _retval);
        }

        private void InitCustomControlProperties()
        {
            var entities = _selection.Get().OfType<EntityBase>().ToList();

            if (entities.Count == 0)
            {
                return;
            }

            var propertyNames = new List<string>(entities.First().GetType().GetProperties().Select(p => p.Name));

            foreach (var entity in entities)
            {
                propertyNames = propertyNames.Intersect(entity.GetType().GetProperties().Select(p => p.Name)).ToList();
            }

            var properties = new List<PropertyInfo>();

            foreach (var propertyName in propertyNames)
            {
                properties.Add(entities.First().GetType().GetProperty(propertyName));
            }

            foreach (var property in properties.Where(property => property.GetSetMethod() != null && property.GetGetMethod() != null))
            {
                if (property.GetCustomAttribute<InspectorVisible>()?.IsVisible == false) continue;

                var propType = property.PropertyType;

                if (propType == typeof(Vector3))
                {
                    InitVector3Control(entities, property);
                }
                if (propType == typeof(Quaternion))
                {
                    InitQuaternionControl(entities, property);
                }
                else if (propType == typeof(int))
                {
                    InitInt32Contol(entities, property);
                }
                else if (propType == typeof(bool))
                {
                    InitBoolControl(entities, property);
                }
                else if (propType == typeof(float))
                {
                    InitFloatControl(entities, property);
                }
                else if (propType == typeof(List<Wheel>))
                {
                    InitWheelControl(entities, property);
                }
            }
        }

        private void InitWheelControl(List<EntityBase> entities, PropertyInfo property)
        {
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }
            var propertyInfo = entities.First().GetType().GetProperty(property.Name);

            var control = new WheelControl();

            if (entities.Count == 1)
            {
                if (propertyInfo != null)
                {
                    control.Wheels = (List<Wheel>)propertyInfo.GetValue(entities.First());
                }
            }

            var prop = property;

            control.WheelsChanged += ControlValueChanged;

            foreach (var entity in entities)
            {
                entity.PropertyChanged += (sender, args) =>
                {
                    if (sender == null || !_controls.ContainsKey(args.PropertyName) || !(_controls[args.PropertyName] is WheelControl wheelControl))
                    {
                        return;
                    }

                    if (control.WheelsChanged != null)
                    {
                        control.WheelsChanged -= ControlValueChanged;
                    }

                    if (_selection.Get().OfType<EntityBase>().Count() == 1)
                    {
                        var param = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
                        wheelControl.Wheels = (List<Wheel>)param;
                    }

                    control.WheelsChanged += ControlValueChanged;
                };
            }

            _controls.Add(prop.Name, control);

            _retval = layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.Controls.Add(control, 0, _retval);
        }

        private void UpdateCustomToolControls()
		{
            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            layoutPanel.Controls.Clear();
            layoutPanel.RowStyles.Clear();
            layoutPanel.RowCount = 0;
            _retval = 0;

            SuspendLayout();
			layoutPanel.SuspendLayout();

            InitCustomControlProperties();

            ResumeLayout(false);
			PerformLayout();

			layoutPanel.ResumeLayout(false);
			layoutPanel.PerformLayout();
		}

        private void ControlValueChanged(object sender, EventArgs eventArgs)
		{
			var propName = string.Empty;
			var ctrl = sender as Control;

			foreach (var c in _controls.Where(c => c.Value == ctrl))
			{
				propName = c.Key;
				break;
			}

			if (string.IsNullOrEmpty(propName))
			{
				return;
			}

            if (_selection == null || !_selection.Get().OfType<EntityBase>().Any())
            {
                return;
            }

            var controlType = ctrl.GetType().ToString();
            var entities = _selection.Get().ToList();
            var propertyInfo = entities.First().GetType().GetProperty(propName);
            var relative = propertyInfo.GetCustomAttribute<RelativeChangeable>() != null;

            if (controlType.Contains("VectorUpDown"))
			{
                foreach (var entity in entities)
                {
                    if (entities.Count > 1 && relative)
                    {
                        var vector = (Vector3)entity.GetType().GetProperty(propName)?.GetValue(entity);

                        vector += ((VectorUpDown)ctrl).Vector;

                        entity.GetType().GetProperty(propName)?.SetValue(entity, vector);
                    }
                    else
                    {
                        entity.GetType().GetProperty(propName)?.SetValue(entity, ((VectorUpDown)ctrl).Vector);
                    }
                }
            }
            else if (controlType.Contains("QuaternionUpDown"))
            {
                foreach (var entity in entities)
                {
                    if (entities.Count > 1 && relative)
                    {
                        var entityProp = (Quaternion)entity.GetType().GetProperty(propName)?.GetValue(entity);
                        var quaternion = Quaternion.RotationAxis(((QuaternionUpDown)ctrl).Axis, ((QuaternionUpDown)ctrl).Angle);
                        entityProp *= quaternion;
                        entity.GetType().GetProperty(propName)?.SetValue(entity, entityProp);
                    }
                    else
                    {
                        var axis = ((QuaternionUpDown)ctrl).Axis;
                        var angle = ((QuaternionUpDown)ctrl).Angle;
                        var quaternion = Quaternion.RotationAxis(axis, angle);

                        entity.GetType().GetProperty(propName)?.SetValue(entity, quaternion);
                    }
                }
            }
            else if (controlType.Contains("WheelControl"))
            {
                var entity = entities.FirstOrDefault();
                entity?.GetType().GetProperty(propName)?.SetValue(entity, ((WheelControl)ctrl).Wheels);
            }
            else if (controlType.Contains("NumericUpDown"))
            {
                foreach (var entity in entities)
                {
                    var numControl = (NumericUpDown)ctrl;

                    if (entity == null) continue;

                    var property = entity.GetType().GetProperty(name: propName);
                        
                    if (property.PropertyType == typeof(int))
                    {
                        var value = decimal.ToInt32(numControl.Value);

                        if (entities.Count > 1 && relative)
                        {
                            value = (int)property.GetValue(entity) + value;
                        }

                        entity.GetType().GetProperty(propName)?.SetValue(entity, value);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        var str = numControl.Value.ToString().Replace(",", ".");
                        var value = float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);

                        if (entities.Count > 1 && relative)
                        {
                            value = (float)property.GetValue(entity) + value;
                        }

                        entity.GetType().GetProperty(propName)?.SetValue(entity, value);
                    }
                }
            }
            else if (controlType.Contains("CheckBox"))
            {
                foreach (var entity in entities)
                {
                    entity.GetType().GetProperty(propName)?.SetValue(entity, ((CheckBox)ctrl).Checked);                    
                }
            }

            WorldMap.CommitChanges();
        }

		private void OnSelectionChanged(object sender, GameWorld.SelectionChangedEventArgs selectionChangedEventArgs)
		{
			UpdateControls();
		}
	}
}
