using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DialogNodeEditor
{
    [Node(false, "ConvControl/Conversation Jump Node", new Type[] { typeof(ConversationCanvas) })]
    public class ConversationJumpNode : BaseConvControlNode
    {
        private const string Id = "ConversationJumpNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(ConversationJumpNode); } }
        
        public string targetID;

        public override Node Create(Vector2 pos)
        {
            ConversationJumpNode node = CreateInstance<ConversationJumpNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 48);
            node.name = "Conversation Jump Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);

            return node;

        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();

                GUILayout.Label("Target Conversation ID");
                targetID = EditorGUILayout.TextField(targetID);

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