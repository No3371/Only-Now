using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DialogNodeEditor
{
    [Node(false, "Dialog/Control Node", new Type[] { typeof(DialogTreeCanvas) })]
    public class ControlNode : BaseSystemNode
    {
        private const string Id = "ControlNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(ControlNode); } }

        public string key = "Key";
        public float float_Value = 0f;
        public int float_operation_type = 4;

        public string string_Value = "";
        public bool bool_Value = false;
        public int currentSelection = 0;

        public override Node Create(Vector2 pos)
        {
            ControlNode node = CreateInstance<ControlNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 72);
            node.name = "Control Node";

            node.CreateInput("Previous", "DialogForward", NodeSide.Left, 30);
            node.CreateOutput("Next", "DialogForward", NodeSide.Right, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();

            currentSelection = EditorGUILayout.Popup(currentSelection, new string[] { "Set Number", "Set String", "Set Bool"});

            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            switch (currentSelection)
            {
                case 0:
                    key = EditorGUILayout.TextField(key);
                    float_operation_type = EditorGUILayout.Popup(float_operation_type, new string[] { "+", "-", "*", "÷", "=" }, GUILayout.MaxWidth(30));
                    float_Value = EditorGUILayout.FloatField(float_Value);
                    break;
                case 1:
                    key = EditorGUILayout.TextField(key);
                    string_Value = EditorGUILayout.TextField(string_Value);
                    break;
                case 2:
                    key = EditorGUILayout.TextField(key);
                    bool_Value = EditorGUILayout.Toggle(bool_Value);
                    break;
            }
            GUILayout.EndHorizontal();

        }

        public override BaseDialogNode Input(int inputValue)
        {
            switch (inputValue)
            {
                case (int)EDialogInputValue.Next:
                    if (Outputs[1].GetNodeAcrossConnection() != default(Node))
                        return Outputs[1].GetNodeAcrossConnection() as BaseDialogNode;
                    break;
                case (int)EDialogInputValue.Back:
                    if (Outputs[0].GetNodeAcrossConnection() != default(Node))
                        return Outputs[0].GetNodeAcrossConnection() as BaseDialogNode;
                    break;
            }
            return null;
        }

        public override bool IsBackAvailable()
        {
            return Outputs[0].GetNodeAcrossConnection() != default(Node);
        }

        public override bool IsNextAvailable()
        {
            return Outputs[1].GetNodeAcrossConnection() != default(Node);
        }
    }
}