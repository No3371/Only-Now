using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{
    [Node(false, "Dialog/Simple Dialog Node", new Type[] { typeof(ConversationCanvas) })]
    public class SimpleDialogNode : BaseDialogNode
    {
        private const string Id = "DialogNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(SimpleDialogNode); } }

        [SerializeField] string speakerID;
        [SerializeField] string dialogText;
        public string DialogText { get { return dialogText; } set { dialogText = value; } }
        public string SpeakerID { get { return dialogText; } set { dialogText = value; } }

        public override Node Create(Vector2 pos)
        {
            SimpleDialogNode node = CreateInstance<SimpleDialogNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 110);
            node.name = "Simple Dialog";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);

            //Next Node to go to
            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            node.SpeakerID = "";
            node.DialogText = "What I'll say...";

            return node;
        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();

            speakerID = EditorGUILayout.TextField("SpeakerID", speakerID);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            dialogText = EditorGUILayout.TextArea(dialogText, GUILayout.Height(65));

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
