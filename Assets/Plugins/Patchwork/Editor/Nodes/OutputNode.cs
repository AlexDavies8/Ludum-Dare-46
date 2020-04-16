using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.Patchwork
{
    public class OutputNode : NodeBase
    {
        Image previewImage;

        public override void CreateNode()
        {
            title = "Output";

            var inputPort = NodeFactory.GeneratePort(this, Direction.Input, typeof(Texture2D), Port.Capacity.Single);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            previewImage = new Image();
            extensionContainer.Add(previewImage);

            previewImage.style.width = 200;
            previewImage.style.height = 200;

            RefreshExpandedState();
            RefreshPorts();
        }

        public override void UpdateTexture(Texture2D texture, int portID)
        {
            baseTexture = texture;
            if (baseTexture == null) baseTexture = Texture2D.whiteTexture;

            baseTexture.filterMode = FilterMode.Point;
            previewImage.image = baseTexture;

            RefreshExpandedState();
        }
    }
}
