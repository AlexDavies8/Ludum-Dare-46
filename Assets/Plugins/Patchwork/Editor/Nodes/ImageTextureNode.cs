using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace Plugins.Patchwork
{
    public class ImageTextureNode : NodeBase
    {
        ObjectField textureField;

        public override void CreateNode()
        {
            title = "Image Texture";

            outputPort = NodeFactory.GeneratePort(this, Direction.Output, typeof(Texture2D), Port.Capacity.Multi);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            textureField = new ObjectField();
            textureField.objectType = typeof(Texture2D);
            inputContainer.Add(textureField);

            textureField.RegisterCallback<ChangeEvent<Object>>(e => UpdateTexture());

            RefreshExpandedState();
            RefreshPorts();
        }

        public override Texture2D ApplyNode()
        {
            Texture2D fileTex = textureField.value as Texture2D;

            if (fileTex == null) return Texture2D.whiteTexture;

            if (fileTex.width == 0 || fileTex.height == 0) return Texture2D.whiteTexture;

            RenderTexture tmp = RenderTexture.GetTemporary(fileTex.width, fileTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(fileTex, tmp);

            RenderTexture prev = RenderTexture.active;

            Texture2D newTex = new Texture2D(fileTex.width, fileTex.height);

            newTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            newTex.Apply();

            RenderTexture.active = prev;

            RenderTexture.ReleaseTemporary(tmp);

            return newTex;
        }

        public override object[] Serialize()
        {
            return new object[] { AssetDatabase.GetAssetPath(textureField.value as Texture2D) };
        }

        public override void Deserialize(object[] data)
        {
            if (data.Length > 0) textureField.value = AssetDatabase.LoadAssetAtPath<Texture2D>(data[0].ToString());
        }
    }
}