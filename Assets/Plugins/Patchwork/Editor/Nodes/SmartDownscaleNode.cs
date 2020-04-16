using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class SmartDownscaleNode : NodeBase
    {
        int scale = 2;
        Slider scaleSlider;
        IntegerField scaleField;

        public override void CreateNode()
        {
            title = "Smart Downscale";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            //X Offset
            VisualElement xSizeContainer = NodeFactory.CreateIntegerInput("Downscale Multiplier", 2, 8, SetScale, out scaleSlider, out scaleField);
            inputContainer.Add(xSizeContainer);

            UpdateControls(false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void SetScale(int v)
        {
            scale = v;

            UpdateControls();
        }

        void UpdateControls(bool updateTexture = true)
        {
            scaleField.value = scale;
            scaleSlider.value = scale;

            if (updateTexture) UpdateTexture();
        }

        public override Texture2D ApplyNode()
        {
            Color[] pixels = baseTexture.GetPixels();

            int width = baseTexture.width / scale;
            int height = baseTexture.height / scale;
            Color[] newPixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float r = 0, g = 0, b = 0;
                    for (int ox = 0; ox < scale; ox++)
                    {
                        for (int oy = 0; oy < scale; oy++)
                        {
                            int oi = x * scale + ox + (y * scale + oy) * baseTexture.width;
                            Color col = pixels[oi];
                            r += col.r;
                            g += col.g;
                            b += col.b;
                        }
                    }
                    r /= scale * scale;
                    g /= scale * scale;
                    b /= scale * scale;
                    float comp = r * r + g * g + b * b;

                    float closestDist = float.MaxValue;
                    Color closest = Color.white;

                    for (int ox = 0; ox < scale; ox++)
                    {
                        for (int oy = 0; oy < scale; oy++)
                        {
                            int oi = x * scale + ox + (y * scale + oy) * baseTexture.width;
                            Color col = pixels[oi];
                            float ocomp = col.r * col.r + col.g * col.g + col.b * col.b;
                            float dist = Mathf.Abs(ocomp - comp);
                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                closest = col;
                            }
                        }
                    }

                    newPixels[x + y * width] = closest;
                }
            }

            Texture2D newTexture = new Texture2D(width, height);
            newTexture.SetPixels(newPixels);
            newTexture.Apply();
            return newTexture;
        }

        public override object[] Serialize()
        {
            return new object[] { scale};
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) scale = (int)data[0];

            UpdateControls(false);
        }
    }
}
