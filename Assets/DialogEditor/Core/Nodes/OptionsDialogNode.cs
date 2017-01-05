using System;
using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEditor;
using UnityEngine;

namespace DialogNodeEditor
{

    [Node(false, "Dialog/Options Dialog Node", new Type[] { typeof(ConversationCanvas) })]
    public class OptionsDialogNode : BaseDialogNode
    {
        private const string Id = "OptionsDialogNode";
        public override string GetID { get { return Id; } }
        public override Type GetObjectType { get { return typeof(OptionsDialogNode); } }

        private const int StartValue = 114;
        private const int SizeValue = 22;

        [SerializeField]
        List<DataHolderForOption> _options;
        [SerializeField] string speakerID;
        [SerializeField] string dialogText;
        public string DialogText { get { return dialogText; } set { dialogText = value; } }
        public string SpeakerID { get { return speakerID; } set { speakerID = value; } }

        public override Node Create(Vector2 pos)
        {
            OptionsDialogNode node = CreateInstance<OptionsDialogNode>();

            node.rect = new Rect(pos.x, pos.y, 360, 160);
            node.name = "Options Node";

            //Previous Node Connections
            node.CreateInput("Previous Node", "DialogForward", NodeSide.Left, 30);

            ////Next Node to go to
            //node.CreateOutput("Next Node", "DialogForward", NodeSide.Right, 30);

            node.SpeakerID = "";
            node.DialogText = "What I'll say...";

            node._options = new List<DataHolderForOption>();

            node.AddNewOption();

            return node;
        }

        protected internal override void NodeGUI()
        {
            SpeakerID = EditorGUILayout.TextField("Speaker ID", SpeakerID);
            GUILayout.Label("Dialog Text");
            DialogText = EditorGUILayout.TextArea(DialogText, GUILayout.Height(40));
            GUILayout.Space(5);
            DrawOptions();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.Space(5);
            if (GUILayout.Button("Add New Option"))
            {
                AddNewOption();
                IssueEditorCallBacks();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.Space(5);
            if (GUILayout.Button("Remove Last Option"))
            {
                Debug.Log("Remove options is clicked");
                RemoveLastOption();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void RemoveLastOption()
        {
            if (_options.Count > 1)
            {
                DataHolderForOption option = _options.Last();
                _options.Remove(option);
                Outputs[option.NodeOutputIndex].Delete();
                rect = new Rect(rect.x, rect.y, rect.width, rect.height - SizeValue);
            }
        }

        private void DrawOptions()
        {
            foreach (DataHolderForOption option in _options)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                option.OptionDisplay = EditorGUILayout.TextField("Option : ", option.OptionDisplay);
                GUILayout.Space(4);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private void AddNewOption()
        {
            DataHolderForOption option = new DataHolderForOption { OptionDisplay = "Write Here" };
            CreateOutput("Next Node", "DialogForward", NodeSide.Right,
                StartValue + _options.Count * SizeValue);
            option.NodeOutputIndex = Outputs.Count - 1;
            rect = new Rect(rect.x, rect.y, rect.width, rect.height + SizeValue);
            _options.Add(option);
        }

        //For Resolving the Type Mismatch Issue
        private void IssueEditorCallBacks()
        {
            DataHolderForOption option = _options.Last();
            NodeEditorCallbacks.IssueOnAddNodeKnob(Outputs[option.NodeOutputIndex]);
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
                default:
                    if (Outputs[_options[inputValue].NodeOutputIndex].GetNodeAcrossConnection() != default(Node))
                        return Outputs[_options[inputValue].NodeOutputIndex].GetNodeAcrossConnection() as BaseDialogNode;
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

        [Serializable]
        class DataHolderForOption
        {
            public string OptionDisplay;
            public int NodeOutputIndex;
        }

        public List<string> GetAllOptions()
        {
            return _options.Select(option => option.OptionDisplay).ToList();
        }
    }
}