using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{

    [Node(true, "Action/Base Action Node", new Type[] { typeof(ConversationCanvas) })]
    public class BaseActionNode : BaseDialogNode
    {
        private const string Id = "BaseActionNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(BaseActionNode); } }

        private const int StartValue = 84;
        private const int SizeValue = 54;

        public int delayNextNode = 0;
        public string performerID;

        public override Node Create(Vector2 pos)
        {
            BaseActionNode node = CreateInstance<BaseActionNode>();

            node.rect = new Rect(pos.x, pos.y, 360, 120);
            node.name = "Action Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);
            
            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {            
            GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                        GUILayout.Label("Execute Next Node");
                        delayNextNode = EditorGUILayout.Popup(delayNextNode, new string[] { "When Action Finished", "At The Same Time"}, GUILayout.Width(150));
                    GUILayout.EndVertical();
                GUILayout.EndHorizontal();                

            GUILayout.EndVertical();
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
            return false;
        }
    }
}