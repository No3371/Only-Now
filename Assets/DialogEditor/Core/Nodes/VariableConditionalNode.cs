using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;
using VariableSystem;

namespace DialogNodeEditor
{

    [Node(false, "VariableControl/Variable Conditional Node", new Type[] { typeof(ConversationCanvas) })]
    public class VariableConditionalNode : BaseVariableNode
    {
        private const string Id = "VariableConditionalNodeNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(VariableConditionalNode); } }

        private const int StartValue = 84;
        private const int SizeValue = 54;

        [SerializeField]
        public List<VariableConditionItem> conditions = new List<VariableConditionItem>();

        public VariableConditionGroupType condGroup;

        public override Node Create(Vector2 pos)
        {
            VariableConditionalNode node = CreateInstance<VariableConditionalNode>();

            node.rect = new Rect(pos.x, pos.y, 360, 120);
            node.name = "Variable Conditional Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);
            
            node.CreateOutput("IfMatch", "DialogForward", NodeSide.Right, 34);
            node.CreateOutput("IfNotMatch", "DialogForward", NodeSide.Right, 56);

            node.AddNewCondition();

            return node;
        }

        protected internal override void NodeGUI()
        {            
            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal(GUILayout.Height(40));
                    GUILayout.Space(34);
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                        GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleLeft;
                        GUILayout.Label("Condition Mode");
                        GUI.skin.GetStyle("Label").alignment = TextAnchor.UpperLeft;
                        condGroup = (VariableConditionGroupType) EditorGUILayout.Popup((int) condGroup, new string[] { "And", "Or"}, GUILayout.Width(100));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Width(24));
                        GUILayout.Space(2);
                        GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;
                        GUILayout.Label("✔", GUILayout.Width(24), GUILayout.Height(18));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("✘", GUILayout.Width(24), GUILayout.Height(18));
                        GUI.skin.GetStyle("Label").alignment = TextAnchor.UpperLeft;
                    GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(8);
                DrawConditions();
                GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                        GUILayout.Space(5);
                        if (GUILayout.Button("Add New Condition")) AddNewCondition();
                    GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                        GUILayout.Space(5);
                        if (GUILayout.Button("Remove Last Condition")) RemoveLastCondition();
                    GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void RemoveLastCondition()
        {
            if (conditions.Count > 1)
            {
                VariableConditionItem cond = conditions.Last();
                conditions.Remove(cond);
                rect = new Rect(rect.x, rect.y, rect.width, rect.height - SizeValue);
            }
        }

        private void DrawConditions()
        {
            for (int i = 0; i < conditions.Count; i++)
            {                    
                VariableConditionItem cond = conditions[i];                

                Rect temp = EditorGUILayout.BeginVertical(GUILayout.Height(40)); 
                    GUILayout.Space(4); 
                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();  
                            GUILayout.FlexibleSpace();
                            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;
                            if (condGroup == VariableConditionGroupType.And) GUILayout.Label("&&", GUILayout.Width(20));
                            else if (condGroup == VariableConditionGroupType.Or) GUILayout.Label("||", GUILayout.Width(20));
                            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleLeft;
                            GUILayout.FlexibleSpace();
                        GUILayout.EndVertical();                        
                        GUILayout.BeginVertical();  
                            GUILayout.BeginHorizontal();
                                cond.type = (VariableType) EditorGUILayout.Popup( (int) cond.type, new string[] { "Integer", "Float", "String", "Bool"}, GUILayout.MaxWidth(80));
                                cond.key = EditorGUILayout.TextField(cond.key);
                            GUILayout.EndHorizontal(); 
                            GUILayout.BeginHorizontal();
                                switch (cond.type)
                                    {
                                        case VariableType.Float:
                                            cond.cond = (VariableCondition) EditorGUILayout.Popup( (int) cond.cond, new string[] { "==", "!=", ">", "<", ">=", "<=" }, GUILayout.MaxWidth(30));                                                                            
                                            cond.vFloat = EditorGUILayout.FloatField(cond.vFloat);
                                            break;
                                        case VariableType.String:
                                            cond.cond = (VariableCondition) EditorGUILayout.Popup( (int) cond.cond, new string[] { "==", "!="}, GUILayout.MaxWidth(30));  
                                            cond.vString = EditorGUILayout.TextField(cond.vString);
                                            break;
                                        case VariableType.Bool:
                                            GUILayout.Label("IsTrue?", GUILayout.Width(50));
                                            cond.vBool = EditorGUILayout.Toggle(cond.vBool);
                                            break;
                                    }
                            GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(12); 
                GUILayout.EndVertical();
                Handles.color = new Color(1, 1, 1, 0.4f);
                Handles.DrawLine(new Vector2(temp.left, temp.top), new Vector2(temp.right, temp.top));
                
            }
        }

        private void AddNewCondition()
        {
            VariableConditionItem opt = new VariableConditionItem("StorySystem", "key", VariableCondition.IsEqual, 0);
            rect = new Rect(rect.x, rect.y, rect.width, rect.height + SizeValue);
            conditions.Add(opt);
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