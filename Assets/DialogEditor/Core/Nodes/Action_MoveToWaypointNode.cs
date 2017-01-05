﻿using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{

    [Node(false, "Action/Move To Waypoint", new Type[] { typeof(ConversationCanvas) })]
    public class Action_MoveToWaypointNode : BaseActionNode
    {
        private const string Id = "MoveToWaypointNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(Action_MoveToWaypointNode); } }

        private const int StartValue = 84;
        private const int SizeValue = 54;


        ///Move left if 0, Right if 1
        public string wayPointID;

        public float speed;
        
        public override Node Create(Vector2 pos)
        {
            Action_MoveToWaypointNode node = CreateInstance<Action_MoveToWaypointNode>();

            node.rect = new Rect(pos.x, pos.y, 334, 90);
            node.name = "Action_Move To Waypoint Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);
            
            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            return node;
        }

        protected internal override void NodeGUI()
        {            
            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Execute Next Node", GUILayout.Width(140));
                    delayNextNode = EditorGUILayout.Popup(delayNextNode, new string[] { "When Action Finished", "At The Same Time"});
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Performer ID", GUILayout.Width(140));
                    performerID = EditorGUILayout.TextField(performerID);
                GUILayout.EndHorizontal();

                Rect temp = GUILayoutUtility.GetLastRect();
                Handles.DrawLine(new Vector2(temp.left, temp.bottom + 6), new Vector2(temp.right, temp.bottom + 6));

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Move To", GUILayout.Width(54));
                    wayPointID = EditorGUILayout.TextField(wayPointID, GUILayout.ExpandWidth(true));
                    GUILayout.Label("At", GUILayout.Width(20));
                    speed = EditorGUILayout.FloatField(speed, GUILayout.Width(40));
                    GUILayout.Label("Unit/s");
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