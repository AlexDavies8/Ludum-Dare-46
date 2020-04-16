using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class ColourTextureNode : NodeBase
    {
        ColorField colourField;

        public override void CreateNode()
        {
            title = "Colour Texture";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            VisualElement colourContainer = new VisualElement();
            colourContainer.style.flexDirection = FlexDirection.Row;
            colourContainer.style.marginLeft = colourContainer.style.marginRight = 5;

            var colourLabel = new Label("Colour");
            colourLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            colourContainer.Add(colourLabel);

            colourField = new ColorField();
            colourField.value = Color.white;
            colourField.style.width = 100f;
            colourContainer.Add(colourField);

            colourField.RegisterCallback<ChangeEvent<Color>>(e => UpdateTexture());

            inputContainer.Add(colourContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public override Texture2D ApplyNode()
        {
            Texture2D texture = new Texture2D(4, 4);
            Color[] pixels = new Color[texture.width * texture.height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = colourField.value;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        public override object[] Serialize()
        {
            return new object[] { colourField.value.r, colourField.value.g, colourField.value.b, colourField.value.a };
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 3)
            {
                Color col = new Color(Convert.ToSingle(data[0]), Convert.ToSingle(data[1]), Convert.ToSingle(data[2]), Convert.ToSingle(data[3]));
                colourField.value = col;
            }
        }
    }
}
