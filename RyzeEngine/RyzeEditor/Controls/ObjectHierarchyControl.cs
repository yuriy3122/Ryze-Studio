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
                Text = "MainScene"
            };
            rootNode.Expand();

            foreach (var entity in entities.Where(x => x.ParentId == Guid.Empty))
            {
                var node = new TreeNode
                {
                    Name = entity.Id.ToString(),
                    Text = entity.GetType().Name
                };

                rootNode.Nodes.Add(node);
            }

            HierarchyTreeView.Nodes.Add(rootNode);
        }
    }
}
