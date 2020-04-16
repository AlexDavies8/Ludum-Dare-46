using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Plugins.Patchwork
{
    public class MaskNode : NodeBase
    {
        Texture2D maskTexture;

        int threshold = 50;
        Slider thresholdSlider;
        IntegerField thresholdField;

        public override void CreateNode()
        {
            title = "Mask";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            var maskPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            maskPort.portName = "Mask";
            inputContainer.Add(maskPort);

            //Threshold
            VisualElement thresholdContainer = NodeFactory.CreateIntegerInput("Threshold (%)", 0, 100, newValue => UpdateThreshold(newValue), out thresholdSlider, out thresholdField);
            inputContainer.Add(thresholdContainer);

            UpdateThreshold(threshold, false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void UpdateThreshold(int v, bool updateTexture = true)
        {
            threshold = v;
            thresholdSlider.value = threshold;
            thresholdField.value = threshold;

            if (updateTexture) UpdateTexture();
        }

        public override Texture2D ApplyNode()
        {
            Color[] pixels = baseTexture.GetPixels();
            Color[] maskPixels = maskTexture.GetPixels();

            int width = Mathf.Max(baseTexture.width, maskTexture.width);
            int height = Mathf.Max(baseTexture.height, maskTexture.height);

            Color[] newPixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float xfac = (float)x / width;
                    float yfac = (float)y / height;

                    int x1 = (int)(xfac * baseTexture.width);
                    int y1 = (int)(yfac * baseTexture.height);
                    int x2 = (int)(xfac * maskTexture.width);
                    int y2 = (int)(yfac * maskTexture.height);

                    int i1 = x1 + y1 * baseTexture.width;
                    int i2 = x2 + y2 * maskTexture.width;

                    float v;
                    Color.RGBToHSV(maskPixels[i2], out _, out _, out v);

                    int i = x + y * width;
                    newPixels[i] = pixels[i1];

                    if (v > threshold * 0.01f)
                        newPixels[i].a = pixels[i1].a;
                    else
                        newPixels[i].a = 0;
                }
            }

            Texture2D newTexture = new Texture2D(width, height);
            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }

        public override void UpdateTexture(Texture2D texture, int portID)
        {
            if (portID == 0) baseTexture = texture;
            else maskTexture = texture;

            UpdateTexture();
        }

        public override void UpdateTexture()
        {
            if (maskTexture == null) maskTexture = Texture2D.whiteTexture;
            base.UpdateTexture();
        }

        public override object[] Serialize()
        {
            return new object[] { threshold };
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) UpdateThreshold((int)data[0], false);
        }
    }
}
