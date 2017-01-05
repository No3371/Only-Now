using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{
    [Node(false, "ConvControl/Conversation Start Node", new Type[] { typeof(ConversationCanvas) })]
    public class ConversationStartNode : BaseConvControlNode
    {
        private const string Id = "conversationStartNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(ConversationStartNode); } }

        public string convID = "ID of this conversation";
        public string description = "";

        public override Node Create(Vector2 pos)
        {
            ConversationStartNode node = CreateInstance<ConversationStartNode>();

            node.rect = new Rect(pos.x, pos.y, 220, 150);
            node.name = "Conversation Start Node";

            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(GUILayout.Width(200));
                    GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label("Conversation ID");
                    convID = EditorGUILayout.TextField(convID);
                    GUILayout.Space(8);
                    GUILayout.Label("Description");
                    description = EditorGUILayout.TextArea(description, GUILayout.Height(60));
                    GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleLeft;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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