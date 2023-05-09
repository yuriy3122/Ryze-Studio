using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RyzeEditor.GameWorld;

namespace RyzeEditor.Controls
{
    public partial class ObjectHierarchyControl : UserControl
    {
        public event EventHandler<EntityEventArgs> SelectionChanged;

        public ObjectHierarchyControl()
        {
            InitializeComponent();
            HierarchyTreeView.NodeMouseClick += HierarchyTreeViewNodeMouseClick;
        }

        protected virtual void OnSelectionChanged(EntityEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        private void HierarchyTreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OnSelectionChanged(new EntityEventArgs(Guid.Parse((e.Node.Name))));
        }

        public void UpdateHierarchy(List<EntityBase> entities)
        {
            HierarchyTreeView.Nodes.Clear();

            var rootNode = new TreeNode
            {
                Name = Guid.NewGuid().ToString(),
                Text = "MainScene",
                ImageIndex = 0
            };
            rootNode.Expand();

            foreach (var entity in entities.Where(x => x.ParentId == Guid.Empty))
            {
                int imageIndex = imageIndex = GetObjectImageIndex(entity);

                var parentNode = new TreeNode
                {
                    Name = entity.Id.ToString(),
                    Text = entity.GetType().Name,
                    ImageIndex = imageIndex,
                    SelectedImageIndex = imageIndex
                };

                rootNode.Nodes.Add(parentNode);

                var childEntities = entities.Where(x => x.ParentId == entity.Id).ToList();

                do
                {
                    var newChildEntities = new List<EntityBase>();

                    foreach (var childEntity in childEntities)
                    {
                        imageIndex = GetObjectImageIndex(childEntity);

                        var childNode = new TreeNode
                        {
                            Name = childEntity.Id.ToString(),
                            Text = childEntity.GetType().Name,
                            ImageIndex = imageIndex,
                            SelectedImageIndex = imageIndex
                        };

                        parentNode = rootNode.Nodes.Find(childEntity.ParentId.ToString(), true).FirstOrDefault();

                        parentNode.Nodes.Add(childNode);

                        newChildEntities.AddRange(entities.Where(x => x.ParentId == childEntity.Id));
                    }

                    childEntities = newChildEntities;
                }
                while (childEntities.Any());
            }

            HierarchyTreeView.Nodes.Add(rootNode);
        }

        private int GetObjectImageIndex(EntityBase gameObject)
        {
            int imageIndex;

            switch (gameObject)
            {
                case Vehicle _:
                    imageIndex = 2;
                    break;

                default:
                    imageIndex = 1;
                    break;
            }

            return imageIndex;
        }
    }
}
