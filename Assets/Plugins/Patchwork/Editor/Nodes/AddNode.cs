using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Plugins.Patchwork
{
    public class AddNode : NodeBase
    {
        Texture2D bottomTexture;

        public override void CreateNode()
        {
            title = "Add";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "A";
            inputContainer.Add(inputPort);

            var maskPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            maskPort.portName = "B";
            inputContainer.Add(maskPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        public override Texture2D ApplyNode()
        {
            Color[] pixels = baseTexture.GetPixels();
            Color[] bottomPixels = bottomTexture.GetPixels();

            int width = Mathf.Max(baseTexture.width, bottomTexture.width);
            int height = Mathf.Max(baseTexture.height, bottomTexture.height);

            Color[] newPixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float xfac = (float)x / width;
                    float yfac = (float)y / height;

                    int x1 = (int)(xfac * baseTexture.width);
                    int y1 = (int)(yfac * baseTexture.height);
                    int x2 = (int)(xfac * bottomTexture.width);
                    int y2 = (int)(yfac * bottomTexture.height);

                    int i1 = x1 + y1 * baseTexture.width;
                    int i2 = x2 + y2 * bottomTexture.width;

                    float r1 = pixels[i1].r;
                    float g1 = pixels[i1].g;
                    float b1 = pixels[i1].b;
                    float r2 = bottomPixels[i2].r;
                    float g2 = bottomPixels[i2].g;
                    float b2 = bottomPixels[i2].b;

                    int i = x + y * width;

                    newPixels[i].r = r1 + r2;
                    newPixels[i].g = g1 + g2;
                    newPixels[i].b = b1 + b2;

                    newPixels[i].a = pixels[i1].a + bottomPixels[i2].a;
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
            else bottomTexture = texture;

            UpdateTexture();
        }

        public override void UpdateTexture()
        {
            if (bottomTexture == null) bottomTexture = Texture2D.whiteTexture;
            base.UpdateTexture();
        }
    }
}