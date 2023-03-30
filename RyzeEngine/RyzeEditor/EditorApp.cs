using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.RawInput;
using SharpDX.Windows;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;
using RyzeEditor.Serialization;
using RyzeEditor.Tools;
using RyzeEditor.Packer;
using RyzeEditor.Controls;

namespace RyzeEditor
{
    [Serializable]
    public class EditorApp
    {
        [field: NonSerialized]
        private WorldMap _worldMap;

        [field: NonSerialized]
        private Selection _selection = new Selection();

        [field: NonSerialized]
        private ToolManager _toolManager;

        [field: NonSerialized]
        private RendererD3d _renderer;

        [field: NonSerialized]
        private  bool _userResized;

        [field: NonSerialized]
        private Size _clientSize;

        [field: NonSerialized]
        private ObjectHierarchyControl _objectHierarchyControl;

        [field: NonSerialized]
        private InspectorControl _inspectorControl;

        public void Run()
        {
            var form = new MainForm();

            _userResized = true;
            bool userMinimized = false;

            var camera = CreateDefaultCamera(form);
            _worldMap = new WorldMap(camera);

            form.UserResized += (sender, args) =>
            {
                _worldMap.Camera.AspectRatio = (float)form.ClientSize.Width / form.ClientSize.Height;
                _worldMap.Camera.ClientWndSize = form.ClientSize;
                _clientSize = form.ClientSize;
                _userResized = true;
                userMinimized = form.WindowState == FormWindowState.Minimized;
            };

            _selection = new Selection();

            _worldMap.EntityDeleted += WorldMapEntityDeleted;

            form.Inspector.Selection = _selection;

            _objectHierarchyControl = form.ObjectHierarchyControl;

            _worldMap.EntityAdded += WorldMapEntityAdded;

            _objectHierarchyControl.SelectionChanged += ObjectHierarchyControlSelectionChanged;

            _inspectorControl = form.Inspector;

            InitToolManager(form);

            InitFormEventHandlers(form);

            _renderer = new RendererD3d();
            _renderer.Initialize(form.Handle, _worldMap.Camera);
            var context = new RenderContext(_renderer, _toolManager);

            RenderLoop.Run(form, () =>
            {
                if (userMinimized)
                {
                    return;
                }

                if (_userResized)
                {
                    context.ResizeWindow(new Size(form.ClientSize.Width, form.ClientSize.Height));
                    _userResized = false;
                }
                context.RenderWorld(_worldMap);
            });

            context.Dispose();
        }

        private void ObjectHierarchyControlSelectionChanged(object sender, EntityEventArgs e)
        {
            var entity = _worldMap.Entities.FirstOrDefault(x => x.Id == e.EntityId);

            if (entity != null)
            {
                _selection.Clear();
                _selection.AddEntity(entity);

                _toolManager.SetActiveTool(Tool.Select);

                _inspectorControl.UpdateControls(_toolManager.GetFirstActiveTool());
            }
        }

        private void WorldMapEntityAdded(object sender, EntityEventArgs e)
        {
            var entities = _worldMap.Entities.ToList();

            _objectHierarchyControl.UpdateHierarchy(entities);
        }

        private void WorldMapEntityDeleted(object sender, EntityEventArgs e)
        {
            if (_selection != null)
            {
                _selection.RemoveEntity(e.EntityId);
            }

            var entities = _worldMap.Entities.ToList();

            _objectHierarchyControl.UpdateHierarchy(entities);
        }

        private void SaveWorldMap(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                WorldMapSerializer.Serialize(_worldMap, fileStream);
            }
        }

        private Camera CreateDefaultCamera(MainForm form)
        {
            return new Camera
            {
                IsActive = true,
                AspectRatio = (float)form.ClientSize.Width / form.ClientSize.Height,
                ClientWndSize = form.ClientSize,
                LookAtDir = new Vector3(0.0f, 0.0f, 0.0f),
                Position = new Vector3(0.0f, 5.0f, -25.0f),
                UpDir = Vector3.UnitY,
                FOV = (float)Math.PI / 4.0f,
                ZNear = 0.1f,
                ZFar = 1000000.0f
            };
        }

        private void InitToolManager(MainForm form)
        {
            _toolManager = new ToolManager(_worldMap, _selection);

            form.ToolChanged += (sender, args) =>
            {
                _toolManager.SetActiveTool(args.Tool);
                form.Inspector.UpdateControls(_toolManager.GetFirstActiveTool());
            };

            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);

            Device.KeyboardInput += (sender, args) =>
            {
                _toolManager.OnKeyboardInput(sender, args);
            };
        }

        private void InitFormEventHandlers(MainForm form)
        {
            _clientSize = form.ClientSize;

            form.MouseDown += (s, e) => { _toolManager.OnMouseDown(s, e); };
            form.MouseMove += (s, e) => { _toolManager.OnMouseMove(s, e); };
            form.MouseUp += (s, e) => { _toolManager.OnMouseUp(s, e); };
            form.MouseWheel += (s, e) => { _toolManager.OnMouseWheel(s, e); };

            form.FileOpened += FormFileOpened;

            form.FileSaved += (sender, args) =>
            {
                SaveWorldMap(args.FileName);
            };

            form.UndoClicked += (sender, args) =>
            {
                WorldMap.UndoChanges();
                form.Inspector.UpdateControls(_toolManager.GetFirstActiveTool());
            };

            form.RedoClicked += (sender, args) =>
            {
                WorldMap.RedoChanges();
                form.Inspector.UpdateControls(_toolManager.GetFirstActiveTool());
            };

            form.PackClicked += (sender, args) =>
            {
                var packer = new WorldMapPacker(_worldMap, new PackerOptions());

                packer.Execute();
            };
        }

        private void FormFileOpened(object sender, FileOpenEventArgs args)
        {
            using (var fileStream = new FileStream(args.FileName, FileMode.Open))
            {
                _selection.Clear();
                _worldMap = WorldMapSerializer.Deserialize(fileStream);

                if (_worldMap == null)
                {
                    return;
                }

                _worldMap.Camera.AspectRatio = (float)_clientSize.Width / _clientSize.Height;
                _worldMap.Camera.ClientWndSize = _clientSize;
                _worldMap.EntityDeleted += (s, e) => { _selection?.RemoveEntity(e.EntityId); };
                _toolManager.WorldMap = _worldMap;
                _renderer.Camera = _worldMap.Camera;

                if (_objectHierarchyControl != null)
                {
                    _objectHierarchyControl.UpdateHierarchy(_worldMap.Entities.ToList());
                }

                _userResized = true;
            }
        }
    }
}
