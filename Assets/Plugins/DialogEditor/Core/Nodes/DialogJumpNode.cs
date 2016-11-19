using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DialogNodeEditor
{
    [Node(false, "Dialog/Dialog Jump Node", new Type[] { typeof(DialogTreeCanvas) })]
    public class DialogJumpNode : BaseSystemNode
    {
        private const string Id = "dialogJumpNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(DialogJumpNode); } }
        
        public string targetName;

        public override Node Create(Vector2 pos)
        {
            DialogJumpNode node = CreateInstance<DialogJumpNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 48);
            node.name = "Dialog Jump Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);

            return node;

        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Target Dialog Name");
            targetName = EditorGUILayout.TextField(targetName);

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