using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private PatchworkView _graphView;
        private EditorWindow _window;

        public void Init(PatchworkView graphView, EditorWindow window)
        {
            _graphView = graphView;
            _window = window;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Add Node"), 0),


            new SearchTreeGroupEntry(new GUIContent("Input"), 1),

            new SearchTreeEntry(new GUIContent("Image Texture"))
            {
                userData = typeof(ImageTextureNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Colour Texture"))
            {
                userData = typeof(ColourTextureNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Circle Texture"))
            {
                userData = typeof(CircleTextureNode), level = 2
            },


            new SearchTreeGroupEntry(new GUIContent("Adjust"), 1),

            new SearchTreeEntry(new GUIContent("HSV Adjust"))
            {
                userData = typeof(HSVAdjustNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Offset"))
            {
                userData = typeof(OffsetNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Resample"))
            {
                userData = typeof(ResampleNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Smart Downscale"))
            {
                userData = typeof(SmartDownscaleNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Erode"))
            {
                userData = typeof(ErodeNode), level = 2
            },


            new SearchTreeGroupEntry(new GUIContent("Combine"), 1),

            new SearchTreeEntry(new GUIContent("Mask"))
            {
                userData = typeof(MaskNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Overlay"))
            {
                userData = typeof(OverlayNode), level = 2
            },

            new SearchTreeGroupEntry(new GUIContent("Maths"), 2),

            new SearchTreeEntry(new GUIContent("Add"))
            {
                userData = typeof(AddNode), level = 3
            },
            new SearchTreeEntry(new GUIContent("Subtract"))
            {
                userData = typeof(SubtractNode), level = 3
            },
            new SearchTreeEntry(new GUIContent("Multiply"))
            {
                userData = typeof(MultiplyNode), level = 3
            },

            new SearchTreeGroupEntry(new GUIContent("Generate"), 1),

            new SearchTreeEntry(new GUIContent("4 Bit Autotile"))
            {
                userData = typeof(Autotile4BitNode), level = 2
            },

            new SearchTreeGroupEntry(new GUIContent("Output"), 1),

            new SearchTreeEntry(new GUIContent("Preview"))
            {
                userData = typeof(PreviewNode), level = 2
            },
            new SearchTreeEntry(new GUIContent("Output"))
            {
                userData = typeof(OutputNode), level = 2
            }
        };

            return searchTree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
            var localWorldPosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

            NodeBase node = Activator.CreateInstance(SearchTreeEntry.userData as Type) as NodeBase;

            if (node == null) return false;

            _graphView.AddElement(node);
            node.SetNodePosition(localWorldPosition);

            return true;
        }
    }
}