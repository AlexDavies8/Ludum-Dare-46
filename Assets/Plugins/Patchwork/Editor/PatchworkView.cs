using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

namespace Plugins.Patchwork
{
    public class PatchworkView : GraphView
    {
        private NodeSearchWindow _searchWindow;
        private EditorWindow _parentWindow;

        public PatchworkView(EditorWindow parentWindow)
        {
            _parentWindow = parentWindow;
            styleSheets.Add(Resources.Load<StyleSheet>("Patchwork"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale * 2f, 0.1f, 1f);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            var colourNode = new ColourTextureNode();
            colourNode.SetNodePosition(new Vector2(400, 400));
            AddElement(colourNode);

            var outputNode = new OutputNode();
            outputNode.SetNodePosition(new Vector2(800, 300));
            AddElement(outputNode);

            Edge edge = (colourNode.outputContainer[0] as Port).ConnectTo(outputNode.inputContainer[0] as Port);
            Add(edge);

            colourNode.UpdateTexture();

            AddSearchWindow();
        }

        private void AddSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(this, _parentWindow);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node && startPort.portType == port.portType)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void ClearGraph()
        {
            List<Node> nodeList = nodes.ToList();
            List<Edge> edgeList = edges.ToList();
            foreach (var node in nodeList)
            {
                edgeList.Where(x => x.input.node == node).ToList().ForEach(edge => RemoveElement(edge));

                RemoveElement(node);
            }
        }
    }
}