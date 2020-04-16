using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class ErodeNode: NodeBase
    {
        public override void CreateNode()
        {
            title = "Erode";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        public override Texture2D ApplyNode()
        {
            Color[] pixels = baseTexture.GetPixels();
            Color[] newPixels = new Color[pixels.Length];

            for (int x = 0; x < baseTexture.width; x++)
            {
                for (int y = 0; y < baseTexture.height; y++)
                {
                    int i = x + y * baseTexture.width;

                    int i1 = Mathf.Clamp(x - 1, 0, baseTexture.width - 1) + y * baseTexture.width;
                    int i2 = Mathf.Clamp(x + 1, 0, baseTexture.width - 1) + y * baseTexture.width;
                    int i3 = x + Mathf.Clamp(y - 1, 0, baseTexture.height - 1) * baseTexture.width;
                    int i4 = x + Mathf.Clamp(y + 1, 0, baseTexture.height - 1) * baseTexture.width;

                    if (pixels[i1].a < 0.01f)
                    {
                        newPixels[i] = new Color(0, 0, 0, 0);
                        continue;
                    }
                    if (pixels[i2].a < 0.01f)
                    {
                        newPixels[i] = new Color(0, 0, 0, 0);
                        continue;
                    }
                    if (pixels[i3].a < 0.01f)
                    {
                        newPixels[i] = new Color(0, 0, 0, 0);
                        continue;
                    }
                    if (pixels[i4].a < 0.01f)
                    {
                        newPixels[i] = new Color(0, 0, 0, 0);
                        continue;
                    }

                    newPixels[i] = pixels[i];
                }
            }

            Texture2D newTexture = new Texture2D(baseTexture.width, baseTexture.height);
            newTexture.SetPixels(newPixels);
            newTexture.Apply();
            return newTexture;
        }
    }
}