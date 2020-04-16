using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class OffsetNode : NodeBase
    {
        int xOffset = 0;
        Slider xOffsetSlider;
        IntegerField xOffsetField;

        int yOffset = 0;
        Slider yOffsetSlider;
        IntegerField yOffsetField;

        public override void CreateNode()
        {
            title = "Offset";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            //X Offset
            VisualElement xOffsetContainer = NodeFactory.CreateIntegerInput("X Offset (%)", 0, 100, SetXOffset, out xOffsetSlider, out xOffsetField);
            inputContainer.Add(xOffsetContainer);

            //Y Offset
            VisualElement yOffsetContainer = NodeFactory.CreateIntegerInput("Y Offset (%)", 0, 100, SetYOffset, out yOffsetSlider, out yOffsetField);
            inputContainer.Add(yOffsetContainer);

            UpdateControls(false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void SetXOffset(int v)
        {
            xOffset = v;

            UpdateControls();
        }

        void SetYOffset(int v)
        {
            yOffset = v;

            UpdateControls();
        }

        void UpdateControls(bool updateTexture = true)
        {
            xOffsetField.value = xOffset;
            xOffsetSlider.value = xOffset;
            yOffsetField.value = yOffset;
            yOffsetSlider.value = yOffset;

            if (updateTexture) UpdateTexture();
        }

        public override Texture2D ApplyNode()
        {
            Color[] pixels = baseTexture.GetPixels();
            Color[] newPixels = new Color[pixels.Length];

            int shiftx = (int)(baseTexture.width * xOffset * 0.01f) + baseTexture.width;
            int shifty = (int)(baseTexture.height * yOffset * 0.01f) + baseTexture.height;

            for (int x = 0; x < baseTexture.width; x++)
            {
                for (int y = 0; y < baseTexture.height; y++)
                {
                    int i = x + y * baseTexture.width;

                    int sx = (x + shiftx) % baseTexture.width;
                    int sy = (y + shifty) % baseTexture.height;

                    int shifti = sx + sy * baseTexture.width;

                    newPixels[i] = pixels[shifti];
                }
            }

            Texture2D newTexture = new Texture2D(baseTexture.width, baseTexture.height);
            newTexture.SetPixels(newPixels);
            newTexture.Apply();
            return newTexture;
        }

        public override object[] Serialize()
        {
            return new object[] { xOffset, yOffset};
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) xOffset = (int)data[0];
            if (data.Length > 1) yOffset = (int)data[1];

            UpdateControls(false);
        }
    }
}
