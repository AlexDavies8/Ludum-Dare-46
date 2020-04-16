using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Plugins.Patchwork
{
    public class NodeBase : Node
    {
        static readonly Vector2 defaultNodeSize = new Vector2(200, 100);

        public string GUID;
        public Texture2D baseTexture;

        protected Port outputPort;

        public NodeBase() : base()
        {
            GUID = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            CreateNode();

            if (outputPort != null) AddPortListener(outputPort);

            UpdateTexture();
        }

        public virtual Texture2D ApplyNode()
        {
            return baseTexture;
        }

        public virtual void CreateNode()
        {
            return;
        }

        public virtual void SetNodePosition(Vector2 position)
        {
            SetPosition(new Rect(position, defaultNodeSize));
        }

        public virtual void UpdateTexture()
        {
            if (baseTexture == null) baseTexture = Texture2D.whiteTexture;

            Texture2D outputTex = ApplyNode();

            if (outputPort == null) return;

            foreach (var connection in outputPort.connections)
            {
                NodeBase node = connection.input.node as NodeBase;
                if (node.inputContainer.Contains(connection.input))
                    node.UpdateTexture(outputTex, node.inputContainer.IndexOf(connection.input));
                else
                    node.UpdateTexture(outputTex, 1);
            }
        }

        public virtual void UpdateTexture(Texture2D texture, int portID)
        {
            baseTexture = texture;

            Texture2D outputTex = ApplyNode();

            if (outputPort == null) return;

            foreach (var connection in outputPort.connections)
            {
                NodeBase node = connection.input.node as NodeBase;
                if (node.inputContainer.Contains(connection.input))
                    node.UpdateTexture(outputTex, node.inputContainer.IndexOf(connection.input));
                else
                    node.UpdateTexture(outputTex, 1);
            }
        }

        protected void AddPortListener(Port port)
        {
            port.AddManipulator(new EdgeConnector<Edge>(new EdgeConnectorListener(() => UpdateTexture())));
        }

        public NodeData SerializeBase()
        {
            NodeData nodeData = new NodeData(this.GetType(), GUID, GetPosition().position);

            if (outputPort != null)
            {
                foreach (var connection in outputPort.connections)
                {
                    int portID = connection.input.node.inputContainer.IndexOf(connection.input);
                    NodeBase node = connection.input.node as NodeBase;
                    nodeData.AddConnection(node.GUID, portID);
                }
            }

            nodeData.data = Serialize();

            return nodeData;
        }

        public void DeserializeBase(NodeData nodeData)
        {
            GUID = nodeData.guid;
            SetNodePosition(nodeData.position);

            Deserialize(nodeData.data);
        }

        public void DeserializeConnections(NodeData nodeData, PatchworkView _graphView)
        {
            if (outputPort != null)
            {
                List<NodeBase> nodes = _graphView.nodes.ToList().Cast<NodeBase>().ToList();
                foreach ((string guid, int portID) in nodeData.connections)
                {
                    NodeBase node = nodes.Where(n => n.GUID == guid).FirstOrDefault();
                    if (node != null)
                    {
                        var edge = new Edge()
                        {
                            output = outputPort,
                            input = node.inputContainer[portID] as Port
                        };

                        edge.input.Connect(edge);
                        edge.output.Connect(edge);
                        _graphView.Add(edge);
                    }
                }
            }
        }

        public virtual object[] Serialize()
        {
            return new object[0];
        }

        public virtual void Deserialize(object[] data)
        {
            return;
        }

        class EdgeConnectorListener : IEdgeConnectorListener
        {
            public Action callback;

            public EdgeConnectorListener(Action callback)
            {
                this.callback = callback;
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                callback.Invoke();
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                callback.Invoke();
            }
        }
    }

    [Serializable]
    public class NodeData
    {
        public string type;
        public string guid;
        public Vector2 position;
        public List<(string guid, int portID)> connections = new List<(string, int)>();
        public object[] data;

        public NodeData(Type type, string guid, Vector2 position, params object[] data)
        {
            this.type = type.FullName;
            this.guid = guid;
            this.position = position;
            this.data = data;
        }

        public void AddConnection(string guid, int portID)
        {
            connections.Add((guid, portID));
        }
    }
}