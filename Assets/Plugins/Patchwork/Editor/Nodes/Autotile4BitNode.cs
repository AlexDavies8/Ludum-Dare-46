using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class Autotile4BitNode : NodeBase
    {
        int smoothing = 0;
        Slider smoothingSlider;
        IntegerField smoothingField;

        bool invert = false;
        Toggle invertToggle;

        public override void CreateNode()
        {
            title = "4 Bit Autotile";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            var smoothingContainer = NodeFactory.CreateIntegerInput("Smoothing (%)", 0, 100, newValue => SetSmoothing(newValue), out smoothingSlider, out smoothingField);
            inputContainer.Add(smoothingContainer);

            var invertContainer = new VisualElement();
            invertContainer.style.flexDirection = FlexDirection.Row;
            invertContainer.style.marginLeft = invertContainer.style.marginRight = 5;

            var invertLabel = new Label("Invert");
            invertLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            invertContainer.Add(invertLabel);

            invertToggle = new Toggle();
            invertToggle.value = false;
            invertToggle.RegisterCallback<ChangeEvent<bool>>(e => SetInvert(e.newValue));
            invertContainer.Add(NodeFactory.SnapRightContainer(invertToggle));

            inputContainer.Add(invertContainer);

            SetSmoothing(smoothing, false);
            SetInvert(invert, false);

            RefreshExpandedState();
            RefreshPorts();
        }

        void SetSmoothing(int v, bool update = true)
        {
            smoothing = v;

            smoothingSlider.value = smoothing;
            smoothingField.value = smoothing;

            if (update) UpdateTexture();
        }

        void SetInvert(bool v, bool update = true)
        {
            invert = v;

            invertToggle.value = invert;

            if (update) UpdateTexture();
        }

        public override object[] Serialize()
        {
            return new object[] { smoothing, invert };
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0)
            {
                SetSmoothing((int)data[0], false);
            }

            if (data.Length > 1)
            {
                SetInvert((bool)data[1], false);
            }
        }

        readonly byte[] bitmasks = new byte[] 
        { 
            0b0000, 0b1111, 0b1100, 0b0101,
            0b0110, 0b1001, 0b1010, 0b0011,
            0b1000, 0b0100, 0b0111, 0b1011,
            0b0010, 0b0001, 0b1101, 0b1110
        };

        public override Texture2D ApplyNode()
        {
            Texture2D texture = new Texture2D(baseTexture.width * 4, baseTexture.height * 4);

            Color[] basePixels = baseTexture.GetPixels();

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Color[] tilePixels = new Color[basePixels.Length];
                    int maskIndex = x + (3-y) * 4;

                    for (int px = 0; px < baseTexture.width; px++)
                    {
                        for (int py = 0; py < baseTexture.height; py++)
                        {
                            int i = px + py * baseTexture.width;

                            if (ShowPixel(px, py, baseTexture.width, baseTexture.height, bitmasks[maskIndex]))
                            {
                                tilePixels[i] = basePixels[i];
                            }
                            else
                            {
                                tilePixels[i] = new Color(0, 0, 0, 0);
                            }
                        }
                    }

                    texture.SetPixels(x * baseTexture.width, y * baseTexture.height, baseTexture.width, baseTexture.height, tilePixels);
                }
            }

            texture.Apply();

            return texture;
        }

        bool ShowPixel(int x, int y, int width, int height, byte mask)
        {
            bool a = (mask & (1 << 0)) != 0;
            bool b = (mask & (1 << 1)) != 0;
            bool c = (mask & (1 << 2)) != 0;
            bool d = (mask & (1 << 3)) != 0;

            int bitCount = new bool[]{ a, b, c, d }.Count( t => t);

            float halfWidth = (width - 1) * 0.5f;
            float halfHeight = (height - 1) * 0.5f;

            bool show = false;

            float sqrRadius = sqr(Mathf.Lerp(halfWidth * 1.42f, halfWidth, smoothing * 0.01f));

            if (x <= halfWidth && y > halfHeight && a)
            {
                show = true;
                if (bitCount == 1 || bitCount == 2 && d)
                {
                    float sqrDist = getsqrdist(x, y, 0, height - 1);
                    if (sqrDist > sqrRadius) show = false;
                }
            }
            if (x > halfWidth && y > halfHeight && b)
            {
                show = true;
                if (bitCount == 1 || bitCount == 2 && c)
                {
                    float sqrDist = getsqrdist(x, y, width - 1, height - 1);
                    if (sqrDist > sqrRadius) show = false;
                }
            }
            if (x <= halfWidth && y <= halfHeight && c)
            {
                show = true;
                if (bitCount == 1 || bitCount == 2 && b)
                {
                    float sqrDist = getsqrdist(x, y, 0, 0);
                    if (sqrDist > sqrRadius) show = false;
                }
            }
            if (x > halfWidth && y <= halfHeight && d)
            {
                show = true;
                if (bitCount == 1 || bitCount == 2 && a)
                {
                    float sqrDist = getsqrdist(x, y, width - 1, 0);
                    if (sqrDist > sqrRadius) show = false;
                }
            }

            if (bitCount == 3)
            {
                if (a && b && c)
                {
                    float sqrDist = getsqrdist(x, y, width - 1, 0);
                    if (sqrDist > sqrRadius) show = true;
                }
                if (a && b && d)
                {
                    float sqrDist = getsqrdist(x, y, 0, 0);
                    if (sqrDist > sqrRadius) show = true;
                }
                if (a && c && d)
                {
                    float sqrDist = getsqrdist(x, y, width - 1, height - 1);
                    if (sqrDist > sqrRadius) show = true;
                }
                if (b && c && d)
                {
                    float sqrDist = getsqrdist(x, y, 0, height - 1);
                    if (sqrDist > sqrRadius) show = true;
                }
            }

            if (invert) show = !show;

            return show;

            float sqr(float val)
            {
                return val * val;
            }

            float getsqrdist(float x1, float y1, float x2, float y2)
            {
                return sqr(x1-x2) + sqr(y1-y2);
            }
        }
    }
}
