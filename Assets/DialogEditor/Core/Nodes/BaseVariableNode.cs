using System;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{
    [Node(true, "Dialog/Base Variable Node", new Type[] { typeof(ConversationCanvas) })]
    public abstract class BaseVariableNode : BaseDialogNode
    {
        private const string Id = "BaseVariableNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(BaseVariableNode); } }

        public override Node Create(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        protected internal override void NodeGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("A variable node.");

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