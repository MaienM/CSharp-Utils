using System.Collections.Generic;
using System.Windows.Forms;
using CSharpUtils.Utils.StatusLogger;

namespace CSharpUtils.GUI
{
    public partial class StatusDisplay : Form
    {
        private readonly Dictionary<string, TreeNode> _nodes = new Dictionary<string, TreeNode>();

        public StatusDisplay(BaseStatusLogger logger)
        {
            InitializeComponent();

            logger.Changed += OnChanged;
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
            if (tvStatus.InvokeRequired)
            {
                tvStatus.Invoke((MethodInvoker)delegate
                {
                    OnChanged(sender, e);
                });
                return;
            }

            string[] parts = e.Key.Split('.');

            TreeNode rootNode = null;
            Dictionary<string, TreeNode> nodes = _nodes;
            foreach (string part in parts)
            {
                if (nodes != null && !nodes.ContainsKey(part))
                {
                    // Create child node.
                    TreeNode node = new TreeNode
                    {
                        Name = part,
                        Text = part,
                        Tag = new Dictionary<string, TreeNode>()
                    };

                    // Store child node.
                    if (rootNode != null)
                    {
                        rootNode.Nodes.Add(node);
                    }
                    else
                    {
                        tvStatus.Nodes.Add(node);
                    }
                    nodes.Add(part, node);
                }

                // Get new root node.
                rootNode = nodes?[part];
                nodes = rootNode?.Tag as Dictionary<string, TreeNode>;
            }

            // Update and show the final node.
            if (rootNode == null) return;
            rootNode.Text = $"{parts[parts.Length - 1]}: {e.NewValueString}";
            rootNode.EnsureVisible();
            rootNode.Expand();
        }
    }
}
