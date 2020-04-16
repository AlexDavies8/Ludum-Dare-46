using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.Serialization.Json;
using System;

namespace Plugins.Patchwork
{
    public class Patchwork : EditorWindow
    {
        private PatchworkView _graphView;

        [MenuItem("Plugins/Patchwork")]
        public static void OpenNodeSpriterWindow()
        {
            Patchwork window = GetWindow<Patchwork>();
            window.titleContent = new GUIContent("Patchwork");
            window.minSize = new Vector2(800, 500);
            window.Show();
        }

        private void OnEnable()
        {
            CreateGraphView();
            CreateToolbar();
        }

        private void CreateToolbar()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new ToolbarButton(() => SaveGraph(true)) { text = "Save" });
            toolbar.Add(new ToolbarButton(() => SaveGraph(false)) { text = "Save As" });
            toolbar.Add(GetSpacer(70f));
            toolbar.Add(new ToolbarButton(OpenGraph) { text = "Open" });
            toolbar.Add(GetSpacer(70f));
            toolbar.Add(new ToolbarButton(ExportGraph) { text = "Export" });

            rootVisualElement.Add(toolbar);
        }

        ToolbarSpacer GetSpacer(float width)
        {
            ToolbarSpacer spacer = new ToolbarSpacer();
            spacer.style.width = width;
            return spacer;
        }

        private void CreateGraphView()
        {
            _graphView = new PatchworkView(this)
            {
                name = "Patchwork Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private string savePath;

        private void SaveGraph(bool useSavePath)
        {
            if (!useSavePath || savePath == null || savePath.Length == 0 || !File.Exists(savePath))
            {
                string path = EditorUtility.SaveFilePanel("Save", Application.dataPath, "graph", "json");

                if (path == null || path.Length == 0) return;

                savePath = path;
            }

            List<NodeBase> nodes = _graphView.nodes.ToList().Cast<NodeBase>().ToList();

            List<NodeData> allData = new List<NodeData>();
            foreach (var node in nodes)
            {
                NodeData nodeData = node.SerializeBase();
                if (nodeData != null)
                {
                    allData.Add(nodeData);
                }
            }

            File.Delete(savePath);

            var ser = new DataContractJsonSerializer(typeof(List<NodeData>));

            using (FileStream fs = File.OpenWrite(savePath))
            {
                ser.WriteObject(fs, allData);
            }
        }

        private void OpenGraph()
        {
            string path = EditorUtility.OpenFilePanel("Open", Application.dataPath, "json");

            if (path == null || path.Length == 0) return;
            if (!File.Exists(path)) return;

            List<NodeData> allData = new List<NodeData>();

            var ser = new DataContractJsonSerializer(typeof(List<NodeData>));

            using (FileStream fs = File.OpenRead(path))
            {
                allData = (List<NodeData>)ser.ReadObject(fs);
            }

            _graphView.ClearGraph();

            List<(NodeBase, NodeData)> nodes = new List<(NodeBase, NodeData)>();
            foreach (NodeData nodeData in allData)
            {
                var node = Activator.CreateInstance(Type.GetType(nodeData.type)) as NodeBase;
                node.DeserializeBase(nodeData);
                _graphView.AddElement(node);
                nodes.Add((node, nodeData));
            }

            foreach ((NodeBase node, NodeData nodeData) pair in nodes)
            {
                pair.node.DeserializeConnections(pair.nodeData, _graphView);
            }

            foreach (Node n in _graphView.nodes.ToList())
            {
                (n as NodeBase).UpdateTexture();
            }

            savePath = path;
        }

        private void ExportGraph()
        {
            string path = EditorUtility.SaveFilePanel("Export", Application.dataPath, "image", "png");

            if (path == null || path.Length == 0) return;

            OutputNode outputNode = _graphView.nodes.ToList().Where(n => n is OutputNode).FirstOrDefault() as OutputNode;

            Texture2D exportTexture = outputNode.baseTexture;
            exportTexture.filterMode = FilterMode.Point;

            byte[] bytes = exportTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}
