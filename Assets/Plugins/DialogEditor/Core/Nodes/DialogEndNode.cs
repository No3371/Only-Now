using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{
    [Node(false, "Dialog/Dialog End Node", new Type[] { typeof(DialogTreeCanvas) })]
    public class DialogEndNode : BaseDialogNode
    {
        private const string Id = "dialogEndNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(DialogEndNode); } }

        public string dialogName = "";

        public override Node Create(Vector2 pos)
        {
            DialogEndNode node = CreateInstance<DialogEndNode>();

            node.rect = new Rect(pos.x, pos.y, 300, 48);
            node.name = "Dialog End Node";

            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {
            GUILayout.Label("This dialog tree ends here.");
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