using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SharpDX;
using SharpDX.RawInput;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Tools
{
    public class CollisionTool : ToolBase, IVisualElement
    {
        private CollisionSelectionMode _selectionMode;
        public RigidBody SelectedRigidBody { get; set; }

        public event EventHandler<SubMeshChangedEventArgs> SubMeshChanged;

        public CollisionTool(WorldMap world, Selection selection) : base(world, selection)
        {
            Options = new RenderOptions();
        }

        public string SelectedMeshId
        {
            get
            {
                var selectedObject = _selection.Get().OfType<GameObject>().FirstOrDefault();
                SelectedRigidBody = _world.Entities.OfType<RigidBody>().Where(x => x.GameObject == selectedObject).FirstOrDefault();

                return SelectedRigidBody?.Mesh.Id;
            }
            set
            {
                var selectedObject = _selection.Get().OfType<GameObject>().FirstOrDefault();
                SelectedRigidBody = null;

                if (selectedObject != null && value != null)
                {
                    var attachedRigidBodies = _world.Entities.OfType<RigidBody>().Where(x => x.GameObject == selectedObject).ToList();

                    foreach (var attachedRigidBody in attachedRigidBodies)
                    {
                        _world.RemoveEntity(attachedRigidBody);
                    }

                    var rigidBody = new RigidBody
                    {
                        GameObject = selectedObject,
                        Mesh = ResourceManager.Instance.GetMesh(value)
                    };
                    _world.AddEntity(rigidBody);

                    SelectedRigidBody = rigidBody;
                }

                Options.ShapeType = !string.IsNullOrEmpty(value) ? ShapeType.CollisionMesh: ShapeType.GeometryMesh;
                Options.GameObjectId = _selection.Get().FirstOrDefault()?.Id;
                Options.SubMeshIds = new List<int>();
            }
        }

        public void RemoveUnselectedRigidBodies()
        {
            var selectedObject = _selection.Get().OfType<GameObject>().FirstOrDefault();

            if (selectedObject == null)
            {
                return;
            }

            var attachedRigidBodies = _world.Entities.OfType<RigidBody>().Where(x => x.GameObject == selectedObject).ToList();

            if (SelectedRigidBody?.GameObject != selectedObject)
            {
                return;
            }

            attachedRigidBodies.Remove(SelectedRigidBody);

            foreach (var attachedRigidBody in attachedRigidBodies)
            {
                _world.RemoveEntity(attachedRigidBody);
            }
        }

        public Vector3 Position
        {
            get { return Vector3.Zero; }
        }

        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(); }
        }

        public CollisionSelectionMode SelectionMode
        {
            get
            {
                return _selectionMode; 
            }
            set
            {
                if (value == CollisionSelectionMode.CollisionMesh)
                {
                    Options.SubMeshIds = new List<int>();
                }

                _selectionMode = value;
            }
        }

        public RenderOptions Options { get; set; }

        public override bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                return false;
            }

            var result = false;

            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                return result;
            }

            if (_world == null)
            {
                return result;
            }

            if (SelectionMode == CollisionSelectionMode.CollisionSubMesh)
            {
                var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);

                RigidBody nearestToCameraObject = null;
                float distance = float.MaxValue;
                int subMeshId = 0;

                var intersects = GetCollisionIntersections(ray);

                foreach (var intersect in intersects)
                {
                    var dist = (intersect.Point - _world.Camera.Position).Length();
                    if (dist < distance)
                    {
                        distance = dist;
                        subMeshId = intersect.SubMeshIndex;
                        nearestToCameraObject = _world.Entities.OfType<RigidBody>().Where(x => x.Id == intersect.GameObjectId).FirstOrDefault();
                    }
                }

                if (nearestToCameraObject != null)
                {
                    if (_selection.Get().OfType<GameObject>().Contains(nearestToCameraObject.GameObject))
                    {
                        Options.SubMeshIds = new List<int>() { subMeshId };
                        Options.Color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

                        SubMeshChanged?.Invoke(this, new SubMeshChangedEventArgs(subMeshId));

                        result = true;
                    }
                }
            }
            else if (SelectionMode == CollisionSelectionMode.CollisionMesh)
            {
                Options.SubMeshIds = new List<int>();
            }

            return result;
        }

        public override bool OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            return false;
        }

        public override bool OnKeyboardInput(object sender, KeyboardInputEventArgs arg)
        {
            return false;
        }

        public void Render3d(IRenderer renderer, RenderMode mode)
        {
            if (SelectedRigidBody == null)
            {
                return;
            }

            if (SelectionMode == CollisionSelectionMode.CollisionMesh)
            {
                var min = SelectedRigidBody.BoundingBox.Minimum;
                var max = SelectedRigidBody.BoundingBox.Maximum;
                var color = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

                RenderHelper.RenderBox(renderer, min, max, color, SelectedRigidBody.GameObject.WorldMatrix);
            }
            else if (SelectionMode == CollisionSelectionMode.CollisionSubMesh)
            {
                //Render SubMesh bound box
            }
        }

        private List<RayPickData> GetCollisionIntersections(Ray ray)
        {
            var intersects = new List<RayPickData>();

            if (SelectedRigidBody == null || SelectedRigidBody.Mesh == null) return intersects;

            if (SelectedRigidBody.Intersects(ray, out RayPickData data))
            {
                intersects.Add(data);
            }

            return intersects;
        }

        public void SelectSubMesh(int selectedItem)
        {
            Options.SubMeshIds = new List<int> { selectedItem };
            Options.Color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

            var rigidBody = _world.Entities.OfType<RigidBody>().Where(x => x.Mesh.Id == SelectedMeshId && x.SubMeshId == selectedItem).FirstOrDefault();

            if (rigidBody == null)
            {
                rigidBody = new RigidBody
                {
                    GameObject = SelectedRigidBody.GameObject,
                    Mesh = ResourceManager.Instance.GetMesh(SelectedRigidBody.Mesh.Id),
                    SubMeshId = (uint)selectedItem
                };
                _world.AddEntity(rigidBody);
            }

            SelectedRigidBody = rigidBody;

            var rigidBodyToRemove = _world.Entities.OfType<RigidBody>().Where(x => x.Mesh.Id == SelectedMeshId && x.SubMeshId == null).FirstOrDefault();

            if (rigidBodyToRemove != null)
            {
                _world.RemoveEntity(rigidBodyToRemove);
            }
        }
    }

    public class SubMeshChangedEventArgs
    {
        public SubMeshChangedEventArgs(int subMeshId)
        {
            SubMeshId = subMeshId;
        }

        public int SubMeshId { get; set; }
    }

    public enum CollisionSelectionMode
    {
        [Description("Mesh")]
        CollisionMesh = 0,

        [Description("SubMesh")]
        CollisionSubMesh = 1
    }
}