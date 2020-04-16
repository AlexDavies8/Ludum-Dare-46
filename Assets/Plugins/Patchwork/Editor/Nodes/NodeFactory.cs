using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Plugins.Patchwork
{
    public static class NodeFactory
    {
        public static Port GeneratePort(NodeBase node, Direction portDirection, Type type, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, type);
        }

        public static VisualElement CreateIntegerInput(string label, int minValue, int maxValue, Action<int> onValueChange, out Slider slider, out IntegerField field)
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.marginLeft = container.style.marginRight = 5;

            var valueLabel = new Label(label);
            valueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            container.Add(valueLabel);

            slider = new Slider(minValue, maxValue);
            slider.style.width = 60;
            slider.RegisterCallback<ChangeEvent<float>>(e => onValueChange((int)e.newValue));
            slider.style.marginLeft = 5;

            field = new IntegerField();
            field.style.width = 30;
            field.RegisterCallback<ChangeEvent<int>>(e => onValueChange(e.newValue));

            container.Add(SnapRightContainer(slider, field));

            return container;
        }

        public static VisualElement SnapRightContainer(params VisualElement[] children)
        {
            VisualElement container = new VisualElement();
            container.style.right = 0;
            container.style.marginLeft = StyleKeyword.Auto;
            container.style.display = DisplayStyle.Flex;
            container.style.flexDirection = FlexDirection.Row;

            foreach (VisualElement element in children)
            {
                container.Add(element);
            }

            return container;
        }
    }
}
