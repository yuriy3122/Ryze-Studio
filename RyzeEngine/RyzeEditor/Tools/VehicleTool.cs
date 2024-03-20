using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;
using RyzeEditor.Helpers;
using RyzeEditor.ResourceManagment;
using RyzeEditor.Packer;

namespace RyzeEditor.Tools
{
    public class VehicleTool : ToolBase, IVisualElement
    {
        [field: NonSerialized]
        private Wheel _selectedWheel;

        [field: NonSerialized]
        private readonly Dictionary<Guid, Vector3?> _wheelPositions;

        [field: NonSerialized]
        private readonly GameObject _arrowWeelAxleGameObject;

        [field: NonSerialized]
        private readonly GameObject _arrowWeelDirectionCSGameObject;

        [field: NonSerialized]
        private List<Vector3> _chassisPoints;

        [field: NonSerialized]
        private int? _subMeshIndex;

        [field: NonSerialized]
        private bool _wheelSelectionIsActive;

        public VehicleTool(WorldMap world, Selection selection) : base(world, selection)
        {
            Options = new RenderOptions();
            _wheelPositions = new Dictionary<Guid, Vector3?>();

            var arrowMesh = new List<string> { GeometryShape.Cone };

            const float scale = 0.0125f;

            _arrowWeelAxleGameObject = new GameObject(arrowMesh, arrowMesh)
            {
                Scale = new Vector3(scale, scale, scale),
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Rotation = Quaternion.Identity
            };

            _arrowWeelDirectionCSGameObject = new GameObject(arrowMesh, arrowMesh)
            {
                Scale = new Vector3(scale, scale, scale),
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Rotation = Quaternion.Identity
            };

            _wheelSelectionIsActive = false;
        }

        public string SelectedMeshId
        {
            get
            {
                var selectedObject = _selection.Get().OfType<Vehicle>().FirstOrDefault();

                return selectedObject?.ChassisMeshId;
            }
            set
            {
                var selectedVehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();

                if (selectedVehicle == null)
                {
                    var selectedObject = _selection.Get().OfType<GameObject>().FirstOrDefault();

                    if (selectedObject != null)
                    {
                        selectedVehicle = new Vehicle(new List<string>() { value }, null) { ChassisMeshId = value };

                        var type = typeof(GameObject);
                        var objectFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                        foreach (FieldInfo fi in objectFields)
                        {
                            fi.SetValue(selectedVehicle, fi.GetValue(selectedObject));
                        }

                        _selection.RemoveEntity(selectedObject.Id);
                        _world.RemoveEntity(selectedObject);

                        _world.AddEntity(selectedVehicle);
                        _selection.AddEntity(selectedVehicle);
                    }
                }
                else
                {
                    if (selectedVehicle.ChassisMeshId != value)
                    {
                        foreach (var wheel in selectedVehicle.Wheels)
                        {
                            wheel.MeshId = value;
                            wheel.SubMeshIds = new List<string>();
                        }
                    }

                    selectedVehicle.ChassisMeshId = value;
                }

                if (selectedVehicle == null)
                {
                    return;
                }

                foreach (var wheel in selectedVehicle.Wheels)
                {
                    wheel.PropertyChanged += WheelPropertyChanged;
                }

                selectedVehicle.PropertyChanged += SelectedVehiclePropertyChanged;
            }
        }

        private void SelectedVehiclePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_selectedWheel == null)
            {
                return;
            }

            if (_wheelPositions.ContainsKey(_selectedWheel.Id))
            {
                _wheelPositions[_selectedWheel.Id] = null;
            }
        }

        private void WheelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var wheel = sender as Wheel;

            if (wheel != null)
            {
                _selectedWheel = wheel;

                HighlightSelectedWheel(wheel);

                if (_wheelPositions.ContainsKey(_selectedWheel.Id))
                {
                    _wheelPositions[_selectedWheel.Id] = null;
                }

                _selectedWheel.SubmeshSelectionModeChanged += OnSubmeshSelectionModeChanged;

                _chassisPoints = null;
            }
        }

        private void HighlightSelectedWheel(Wheel wheel)
        {
            Options.ShapeType = ShapeType.WheelMesh;
            Options.GameObjectId = wheel.ParentId;
            Options.Color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            Options.SubMeshIds = wheel.SubMeshIds.ConvertAll(int.Parse);
        }

        private void OnSubmeshSelectionModeChanged(object sender, EventArgs e)
        {
            _wheelSelectionIsActive = (bool)sender;

            if (!_wheelSelectionIsActive && _selectedWheel != null)
            {
                HighlightSelectedWheel(_selectedWheel);
            }
        }

        public RenderOptions Options { get; set; }

        public Vector3 Position
        {
            get { return Vector3.Zero; }
        }

        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(); }
        }

        public override bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            var result = false;

            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                return result;
            }

            if (_subMeshIndex.HasValue)
            {
                if (_selectedWheel.SubMeshIds == null)
                {
                    _selectedWheel.SubMeshIds = new List<string>();
                }

                var selectedSubMeshId = _subMeshIndex.Value;

                _selectedWheel.SubMeshIds.Clear();
                _selectedWheel.SubMeshIds.Add(_subMeshIndex.Value.ToString());

                HighlightSelectedWheel(_selectedWheel);

                _wheelSelectionIsActive = false;
                _selectedWheel.SubMeshIdsChanged.Invoke(_selectedWheel, new EventArgs());
            }

            result = true;

            return result;
        }

        public override bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right || !_wheelSelectionIsActive)
            {
                return false;
            }

            var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);

            float distance = 100000.0f;
            _subMeshIndex = null;

            var intersects = GetGeometryIntersections(ray);

            Options.SubMeshIds.Clear();

            foreach (var intersect in intersects)
            {
                var dist = (intersect.Point - _world.Camera.Position).Length();

                if (dist < distance)
                {
                    distance = dist;

                    var selectedVehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();

                    if (selectedVehicle.Id == intersect.GameObjectId)
                    {
                        _subMeshIndex = intersect.SubMeshIndex;

                        Options.ShapeType = ShapeType.WheelMesh;
                        Options.GameObjectId = _selectedWheel.ParentId;
                        Options.Color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
                        Options.SubMeshIds = new List<int>() { _subMeshIndex.Value };
                    }
                }
            }

            return true;
        }

        public void Render3d(IRenderer renderer, RenderMode mode)
        {
            var selectedVehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();

            if (selectedVehicle == null)
            {
                return;
            }

            if (selectedVehicle.DrawChassisPoints)
            {
                if (_chassisPoints == null)
                {
                    _chassisPoints = new List<Vector3>();

                    var convexHullMeshId = selectedVehicle.GetChassisConvexHullMesh();
                    var mesh = ResourceManager.Instance.GetMesh(convexHullMeshId);
                    var vertices = new List<BulletSharp.Vector3>();

                    foreach (var submesh in mesh.SubMeshes)
                    {
                        var matrix = submesh.GetMatrix(mesh) * Matrix.Scaling(selectedVehicle.Scale);

                        foreach (var vertex in submesh.VertexData)
                        {
                            Vector3 vertPosition = vertex.Pos;
                            Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);
                            vertices.Add(new BulletSharp.Vector3(position.X, position.Y, position.Z));
                        }
                    }

                    var convexHullShape = new ConvexHullShapeEx(vertices);

                    foreach (var point in convexHullShape.UnscaledPoints)
                    {
                        _chassisPoints.Add(new Vector3(point.X, point.Y, point.Z));
                    }
                }

                var color = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);
                foreach (var point in _chassisPoints)
                {
                    var min = point - 0.005f * Vector3.One;
                    var max = point + 0.005f * Vector3.One;                  
                    RenderHelper.RenderBox(renderer, min, max, color, selectedVehicle.WorldMatrix);
                }
            }
            else
            {
                RenderVehicleBoundBox(renderer, mode);
                CalculateCenterForSelectedWheel();

                RenderWheelAxle(renderer, mode);
                RenderChassisConnectionPointCS(renderer, mode);
                RenderWheelDirectionCS(renderer, mode);
            }
        }

        private void CalculateCenterForSelectedWheel()
        {
            if (_selectedWheel == null ||
                _selectedWheel.SubMeshIds.Count == 0 ||
                _selectedWheel.AxleCS.Length() == 0.0f ||
                string.IsNullOrEmpty(_selectedWheel.MeshId))
            {
                return;
            }

            Vector3 wheelPosition = Vector3.Zero;
            var vehicleMatrix = _selection.Get().OfType<Vehicle>().FirstOrDefault().WorldMatrix;

            if (_wheelPositions.ContainsKey(_selectedWheel.Id) && _wheelPositions[_selectedWheel.Id].HasValue)
            {
                wheelPosition = _wheelPositions[_selectedWheel.Id].Value;
            }
            else
            {
                var ids = _selectedWheel.SubMeshIds.ConvertAll(uint.Parse);

                var mesh = ResourceManager.Instance.GetMesh(_selectedWheel.MeshId);

                var positions = new List<Vector3>();

                foreach (var subMesh in mesh.SubMeshes)
                {
                    if (!ids.Contains(subMesh.Id))
                    {
                        continue;
                    }

                    var matrix = subMesh.GetMatrix(mesh) * vehicleMatrix;

                    foreach (var vertex in subMesh.VertexData)
                    {
                        var pos = vertex.Pos;
                        Vector3.TransformCoordinate(ref pos, ref matrix, out Vector3 position);
                        positions.Add(position);
                    }
                }

                var boundBox = BoundingBox.FromPoints(positions.ToArray());

                wheelPosition = (boundBox.Maximum + boundBox.Minimum) * 0.5f;

                _wheelPositions[_selectedWheel.Id] = wheelPosition;
            }
        }

        private void RenderChassisConnectionPointCS(IRenderer renderer, RenderMode mode)
        {
            if (_selectedWheel == null)
            {
                return;
            }

            var vehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();

            var min = _selectedWheel.ChassisConnectionPointCS - 0.05f * Vector3.One;
            var max = _selectedWheel.ChassisConnectionPointCS + 0.05f * Vector3.One;
            var color = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

            RenderHelper.RenderBox(renderer, min, max, color, vehicle.WorldMatrix);
        }

        private void RenderWheelDirectionCS(IRenderer renderer, RenderMode mode)
        {
            if (_selectedWheel == null || _selectedWheel.WheelDirectionCS.Length() == 0.0f)
            {
                return;
            }

            var vehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();
            var rotationMatrix = Matrix.RotationQuaternion(vehicle.Rotation);
            var directionVector = _selectedWheel.WheelDirectionCS;
            directionVector.Normalize();

            Vector3.TransformNormal(ref directionVector, ref rotationMatrix, out Vector3 directionT);
            const float distance = 1.0f;
            directionT *= distance;

            var colorAxis = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
            var position = _selectedWheel.ChassisConnectionPointCS;
            var posMatrix = vehicle.WorldMatrix;

            Vector3.TransformCoordinate(ref position, ref posMatrix, out Vector3 positionT);

            var axisLinePoints = new Point3[2]
            {
                new Point3(new Vector4(positionT.X, positionT.Y, positionT.Z, 0), colorAxis),
                new Point3(new Vector4(positionT.X + directionT.X, positionT.Y + directionT.Y, positionT.Z + directionT.Z, 0), colorAxis),
            };

            renderer.DrawLineStrip(axisLinePoints, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });

            var vx = Vector3.UnitX;
            var directionN = directionT;
            directionN.Normalize();

            if ((vx + directionN).Length() == 0)
            {
                vx.Y = 0.1f;
            }

            var axis = Vector3.Cross(vx, directionN);
            axis.Normalize();
            var angle = (float)Math.Acos(Vector3.Dot(vx, directionN));

            _arrowWeelAxleGameObject.Position = positionT + directionT;
            _arrowWeelAxleGameObject.Rotation = Quaternion.RotationAxis(axis, angle);
            _arrowWeelAxleGameObject.GeometryMeshes[0].SetDiffuseColor(new Color3(0.0f, 0.0f, 1.0f));
            _arrowWeelAxleGameObject.Render3d(renderer, mode);
        }

        private void RenderWheelAxle(IRenderer renderer, RenderMode mode)
        {
            if (_selectedWheel == null)
            {
                return;
            }

            if (!_wheelPositions.ContainsKey(_selectedWheel.Id) ||
                !_wheelPositions[_selectedWheel.Id].HasValue ||
                _selectedWheel.AxleCS.Length() == 0.0f)
            {
                return;
            }

            var vehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();
            var rotationMatrix = Matrix.RotationQuaternion(vehicle.Rotation);
            var axleVector = _selectedWheel.AxleCS;
            axleVector.Normalize();

            Vector3.TransformNormal(ref axleVector, ref rotationMatrix, out Vector3 axleT);
            const float distance = 1.0f;
            axleT *= distance;

            var colorAxis = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            var wheelPosition = _wheelPositions[_selectedWheel.Id].Value;

            var axisLinePoints = new Point3[2]
            {
                new Point3(new Vector4(wheelPosition.X, wheelPosition.Y, wheelPosition.Z, 0), colorAxis),
                new Point3(new Vector4(wheelPosition.X + axleT.X, wheelPosition.Y + axleT.Y, wheelPosition.Z + axleT.Z, 0), colorAxis),
            };

            renderer.DrawLineStrip(axisLinePoints, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });

            var vx = Vector3.UnitX;
            var axleN = axleT;
            axleN.Normalize();

            if ((vx + axleN).Length() == 0)
            {
                vx.Y = 0.1f;
            }

            var axis = Vector3.Cross(vx, axleN);
            axis.Normalize();

            var angle = (float)Math.Acos(Vector3.Dot(vx, axleN));

            _arrowWeelAxleGameObject.Position = wheelPosition + axleT;
            _arrowWeelAxleGameObject.Rotation = Quaternion.RotationAxis(axis, angle);
            _arrowWeelAxleGameObject.GeometryMeshes[0].SetDiffuseColor(new Color3(0.0f, 1.0f, 0.0f));
            _arrowWeelAxleGameObject.Render3d(renderer, mode);
        }

        private void RenderVehicleBoundBox(IRenderer renderer, RenderMode mode)
        {
            var selectedVehicle = _selection.Get().OfType<Vehicle>().FirstOrDefault();

            var min = selectedVehicle.BoundingBox.Minimum;
            var max = selectedVehicle.BoundingBox.Maximum;
            var color = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

            RenderHelper.RenderBox(renderer, min, max, color, selectedVehicle.WorldMatrix);
        }
    }
}
