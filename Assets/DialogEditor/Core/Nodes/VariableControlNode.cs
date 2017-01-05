using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;
using StorySystem.Data;

namespace DialogNodeEditor
{

    [Node(false, "VariableControl/Variable Control Node", new Type[] { typeof(ConversationCanvas) })]
    public class VariableControlNode : BaseVariableNode
    {
        private const string Id = "VariableControlNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(VariableControlNode); } }

        private const int StartValue = 54;
        private const int SizeValue = 52;

        [SerializeField]
        public List<VariableControlItem> operations;

        public override Node Create(Vector2 pos)
        {
            VariableControlNode node = CreateInstance<VariableControlNode>();

            node.rect = new Rect(pos.x, pos.y, 360, 84);
            node.name = "Variable Control Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 50);
            
            node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 50);

            node.operations = new List<VariableControlItem>();

            node.AddNewOperation();

            return node;
        }

        protected internal override void NodeGUI()
        {            
            DrawOperations();

            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();

                    GUILayout.Space(5);
                    if (GUILayout.Button("Add New Operation"))
                    {
                        AddNewOperation();
                    }

                GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();

                    GUILayout.Space(5);
                    if (GUILayout.Button("Remove Last Operation"))
                    {
                        RemoveLastOperation();
                    }

                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void RemoveLastOperation()
        {
            if (operations.Count > 1)
            {
                VariableControlItem opt = operations.Last();
                operations.Remove(opt);
                rect = new Rect(rect.x, rect.y, rect.width, rect.height - SizeValue);
            }
        }

        private void DrawOperations()
        {
            for (int i = 0; i < operations.Count; i++)
            {                    
                VariableControlItem opt = operations[i];

                Rect temp = EditorGUILayout.BeginVertical(GUILayout.Height(40)); 
                    GUILayout.Space(8); 
                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();  
                            GUILayout.FlexibleSpace();
                            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;
                            GUILayout.Label(i.ToString(), GUILayout.Width(20));
                            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleLeft;
                            GUILayout.FlexibleSpace();
                        GUILayout.EndVertical();                        
                        GUILayout.BeginVertical();  
                            GUILayout.BeginHorizontal();
                                opt.type = (VariableType) EditorGUILayout.Popup( (int) opt.type, new string[] { "Integer", "Float", "String", "Bool"}, GUILayout.MaxWidth(80));
                                opt.key = EditorGUILayout.TextField(opt.key);
                            GUILayout.EndHorizontal(); 
                            GUILayout.BeginHorizontal();
                                switch (opt.type)
                                    {
                                        case VariableType.Float:
                                            opt.opt = (VariableOperation) EditorGUILayout.Popup( (int) opt.opt, new string[] { "+", "-", "*", "÷", "=" }, GUILayout.MaxWidth(30));                                                                            
                                            opt.vFloat = EditorGUILayout.FloatField(opt.vFloat);
                                            break;
                                        case VariableType.String:
                                            GUILayout.Label("Set to", GUILayout.Width(40));
                                            opt.vString = EditorGUILayout.TextField(opt.vString);
                                            break;
                                        case VariableType.Int:
                                            opt.opt = (VariableOperation) EditorGUILayout.Popup( (int) opt.opt, new string[] { "+", "-", "*", "÷", "=" }, GUILayout.MaxWidth(30));
                                            opt.vInt = EditorGUILayout.IntField(opt.vInt);
                                            break;
                                        case VariableType.Bool:
                                            GUILayout.Label("IsTrue", GUILayout.Width(40));
                                            opt.vBool = EditorGUILayout.Toggle(opt.vBool);
                                            break;
                                    }
                            GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);
                GUILayout.EndVertical();
                Handles.color = new Color(1, 1, 1, 0.4f);
                if (i > 0) Handles.DrawLine(new Vector2(temp.left, temp.top), new Vector2(temp.right, temp.top));
            }
        }

        private void AddNewOperation()
        {
            VariableControlItem opt = new VariableControlItem("key", VariableOperation.Set, 0);
            rect = new Rect(rect.x, rect.y, rect.width, rect.height + SizeValue);
            operations.Add(opt);
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