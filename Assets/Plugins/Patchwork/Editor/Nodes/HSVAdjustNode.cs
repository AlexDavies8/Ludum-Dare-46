using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class HSVAdjustNode : NodeBase
    {
        int hue = 0;
        IntegerField hueField;
        Slider hueSlider;

        int saturation = 100;
        IntegerField saturationField;
        Slider saturationSlider;

        int value = 100;
        IntegerField valueField;
        Slider valueSlider;

        public override void CreateNode()
        {
            title = "HSV Adjust";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            //Hue
            VisualElement hueContainer = NodeFactory.CreateIntegerInput("Hue (°)", -180, 180, SetHue, out hueSlider, out hueField);
            inputContainer.Add(hueContainer);

            //Saturation
            VisualElement saturationContainer = NodeFactory.CreateIntegerInput("Saturation (%)", 0, 200, SetSaturation, out saturationSlider, out saturationField);
            inputContainer.Add(saturationContainer);

            //Value
            VisualElement valueContainer = NodeFactory.CreateIntegerInput("Value (%)", 0, 200, SetValue, out valueSlider, out valueField);
            inputContainer.Add(valueContainer);

            UpdateControls(false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void SetHue(int v)
        {
            hue = v;

            UpdateControls();
        }

        void SetSaturation(int v)
        {
            saturation = v;

            UpdateControls();
        }

        void SetValue(int v)
        {
            value = v;

            UpdateControls();
        }

        void UpdateControls(bool updateTexture = true)
        {
            hueField.value = hue;
            hueSlider.value = hue;
            saturationField.value = saturation;
            saturationSlider.value = saturation;
            valueField.value = value;
            valueSlider.value = value;

            if (updateTexture) UpdateTexture();
        }

        public override Texture2D ApplyNode()
        {
            Color[] pixels = baseTexture.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                float h, s, v;
                Color.RGBToHSV(pixels[i], out h, out s, out v);

                h = (h + 1 + hueField.value / 360f) % 1;
                s *= (saturationField.value / 100f);
                v *= (valueField.value / 100f);

                Color col = Color.HSVToRGB(h, s, v);
                col.a = pixels[i].a;
                pixels[i] = col;
            }

            Texture2D newTexture = new Texture2D(baseTexture.width, baseTexture.height);
            newTexture.SetPixels(pixels);
            newTexture.Apply();
            return newTexture;
        }

        public override object[] Serialize()
        {
            return new object[] { hueField.value, saturationField.value, valueField.value };
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) hue = (int)data[0];
            if (data.Length > 1) saturation = (int)data[1];
            if (data.Length > 2) value = (int)data[2];

            UpdateControls(false);
        }
    }
}