using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class CircleTextureNode : NodeBase
    {
        int size = 16;
        Slider sizeSlider;
        IntegerField sizeField;

        public override void CreateNode()
        {
            title = "Circle Texture";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            //X Offset
            VisualElement sizeContainer = NodeFactory.CreateIntegerInput("Size (px)", 1, 128, SetXSize, out sizeSlider, out sizeField);
            inputContainer.Add(sizeContainer);

            UpdateControls(false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void SetXSize(int v)
        {
            size = v;

            UpdateControls();
        }

        void UpdateControls(bool updateTexture = true)
        {
            sizeField.value = size;
            sizeSlider.value = size;

            if (updateTexture) UpdateTexture();
        }

        public override Texture2D ApplyNode()
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[texture.width * texture.height];

            float halfWidth = (texture.width - 1) * 0.5f;
            float halfHeight = (texture.height - 1) * 0.5f;

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    int i = x + y * texture.width;

                    float sqrDist = (x - halfWidth) * (x - halfWidth) + (y - halfHeight) * (y - halfHeight);
                    if (sqrDist <= texture.width * texture.width * 0.25f)
                    {
                        pixels[i] = Color.white;
                    }
                    else
                    {
                        pixels[i] = new Color(0, 0, 0, 0);
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        public override object[] Serialize()
        {
            return new object[] { size};
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) size = (int)data[0];

            UpdateControls(false);
        }
    }
}
