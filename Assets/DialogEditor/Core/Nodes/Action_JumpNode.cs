using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{

    [Node(false, "Action/Jump", new Type[] { typeof(ConversationCanvas) })]
    public class Action_JumpNode : BaseActionNode
    {
        private const string Id = "Action_JumpNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(Action_JumpNode); } }

        private const int StartValue = 84;
        private const int SizeValue = 54;

        public int jumpTimes;
        public float jumpHeight;
        


        public override Node Create(Vector2 pos)
        {
            Action_JumpNode node = CreateInstance<Action_JumpNode>();

            node.rect = new Rect(pos.x, pos.y, 280, 90);
            node.name = "Action_Jump Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);
            
            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {            
            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Execute Next Node", GUILayout.Width(120));
                    delayNextNode = EditorGUILayout.Popup(delayNextNode, new string[] { "When Action Finished", "At The Same Time"}, GUILayout.Width(150));
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Performer ID", GUILayout.Width(120));
                    performerID = EditorGUILayout.TextField(performerID);
                GUILayout.EndHorizontal();

                Rect temp = GUILayoutUtility.GetLastRect();
                Handles.DrawLine(new Vector2(temp.xMin, temp.yMax + 6), new Vector2(temp.xMax, temp.yMax + 6));

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Do");
                    jumpHeight = EditorGUILayout.FloatField(jumpHeight, GUILayout.Width(40));
                    GUILayout.Label("Units High Jump");
                    jumpTimes = EditorGUILayout.IntField(jumpTimes, GUILayout.Width(30));
                    GUILayout.Label("Times");
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