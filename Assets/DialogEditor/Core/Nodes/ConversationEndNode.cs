using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{
    [Node(false, "ConvControl/Conversation End Node", new Type[] { typeof(ConversationCanvas) })]
    public class ConversationEndNode : BaseConvControlNode
    {
        private const string Id = "ConversationEndNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(ConversationEndNode); } }

        public string dialogName = "";

        public override Node Create(Vector2 pos)
        {
            ConversationEndNode node = CreateInstance<ConversationEndNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 48);
            node.name = "Conversation End Node";

            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {
            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("This conversation ends here.");
            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleLeft;
        }

        public override BaseDialogNode Input(int inputValue)
        {
            switch (inputValue)
            {
                case (int)EDialogInputValue.Next:
                    if (Outputs[0].GetNodeAcrossConnection() != default(Node))
                        return Outputs[0].GetNodeAcrossConnection() as BaseDialogNode;
                    break;
            }
            return null;
        }

        public override bool IsBackAvailable()
        {
            return false;
        }

        public override bool IsNextAvailable()
        {
            return Outputs[0].GetNodeAcrossConnection() != default(Node);
        }
    }
}