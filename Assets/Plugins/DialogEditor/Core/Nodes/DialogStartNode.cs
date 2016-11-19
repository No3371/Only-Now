using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{
    [Node(false, "Dialog/Dialog Start Node", new Type[] { typeof(DialogTreeCanvas) })]
    public class DialogStartNode : BaseDialogNode
    {
        private const string Id = "dialogStartNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(DialogStartNode); } }

        public string dialogName = "Name to save as...";
        public string description = "Brief description of the dialog tree...";

        public override Node Create(Vector2 pos)
        {
            DialogStartNode node = CreateInstance<DialogStartNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 84);
            node.name = "Dialog Start Node";

            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();
            dialogName = EditorGUILayout.TextField("Dialog Name", dialogName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            description = EditorGUILayout.TextArea(description, GUILayout.Height(40));
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