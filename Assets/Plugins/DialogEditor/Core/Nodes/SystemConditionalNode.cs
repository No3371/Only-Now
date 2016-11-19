using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DialogNodeEditor
{
    [Node(false, "Dialog/System Conditional Node", new Type[] { typeof(DialogTreeCanvas) })]
    public class SystemConditionalNode : BaseSystemNode
    {
        private const string Id = "systemConditionalNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(SystemConditionalNode); } }

        public string key = "Key";
        public float float_Value = 0f;
        public int float_operation_type = 0;
        public string string_Value = "";
        public bool bool_Value = false;
        public int currentSelection = 0;

        public override Node Create(Vector2 pos)
        {
            SystemConditionalNode node = CreateInstance<SystemConditionalNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 96);
            node.name = "System Conditional Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);
            //Next Node to go to
            node.CreateOutput("IfMatch", "DialogForward", NodeSide.Bottom, 75);
            node.CreateOutput("IfNotMatch", "DialogForward", NodeSide.Bottom, 225);

            return node;

        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();

            currentSelection = EditorGUILayout.Popup(currentSelection, new string[] { "Check Number", "Check String", "Check Bool" });

            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            switch (currentSelection)
            {
                case 0:
                    key = EditorGUILayout.TextField(key);
                    float_operation_type = EditorGUILayout.Popup(float_operation_type, new string[] { "=", ">", "<"}, GUILayout.MaxWidth(30));
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

            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            GUIStyle tempLabel = GUI.skin.GetStyle("Label");
            tempLabel.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("✔", tempLabel, GUILayout.Width(150));
            GUILayout.Label("✘", tempLabel, GUILayout.Width(150));

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