using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class ResampleNode : NodeBase
    {
        int xSize = 16;
        Slider xSizeSlider;
        IntegerField xSizeField;

        int ySize = 16;
        Slider ySizeSlider;
        IntegerField ySizeField;

        public override void CreateNode()
        {
            title = "Resample";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            //X Offset
            VisualElement xSizeContainer = NodeFactory.CreateIntegerInput("X Size (px)", 1, 128, SetXSize, out xSizeSlider, out xSizeField);
            inputContainer.Add(xSizeContainer);

            //Y Offset
            VisualElement ySizeContainer = NodeFactory.CreateIntegerInput("Y Size (px)", 1, 128, SetYSize, out ySizeSlider, out ySizeField);
            inputContainer.Add(ySizeContainer);

            UpdateControls(false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void SetXSize(int v)
        {
            xSize = v;

            UpdateControls();
        }

        void SetYSize(int v)
        {
            ySize = v;

            UpdateControls();
        }

        void UpdateControls(bool updateTexture = true)
        {
            xSizeField.value = xSize;
            xSizeSlider.value = xSize;
            ySizeField.value = ySize;
            ySizeSlider.value = ySize;

            if (updateTexture) UpdateTexture();
        }

        public override Texture2D ApplyNode()
        {
            if (baseTexture.width == xSize && baseTexture.height == ySize) return baseTexture;

            Color[] pixels = baseTexture.GetPixels();
            Color[] newPixels = new Color[xSize * ySize];

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    float xfac = (float)x / (xSize-1);
                    float yfac = (float)y / (ySize-1);

                    int x1 = Mathf.Clamp(Mathf.RoundToInt(xfac * baseTexture.width), 0, baseTexture.width - 1);
                    int y1 = Mathf.Clamp(Mathf.RoundToInt(yfac * baseTexture.height), 0, baseTexture.height - 1);

                    int i1 = x1 + y1 * baseTexture.width;
                    int i2 = x + y * xSize;

                    newPixels[i2] = pixels[i1];
                }
            }

            Texture2D newTexture = new Texture2D(xSize, ySize);
            newTexture.SetPixels(newPixels);
            newTexture.Apply();
            return newTexture;
        }

        public override object[] Serialize()
        {
            return new object[] { xSize, ySize};
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) xSize = (int)data[0];
            if (data.Length > 1) ySize = (int)data[1];

            UpdateControls(false);
        }
    }
}
