using UnityEngine;
using System.Collections.Generic;

namespace StorySystem.Data
{
    public class OptionsDialogNode : BaseDialogNode
    {
        static int childrenLimit = 4;

        [SerializeField]
        string dialogText;
        [SerializeField]
        string speakerID;

        public string DialogText { get { return dialogText; } set { dialogText = value; } }
        public string SpeakerID { get { return dialogText; } set { dialogText = value; } }

        public List<string> optionsText = new List<string>();

        public static OptionsDialogNode Create(DialogNodeEditor.OptionsDialogNode editorNode, BaseDialogNode parent)
        {
            OptionsDialogNode created = ScriptableObject.CreateInstance<OptionsDialogNode>();

            created.nodeType = "OptionsDialogNode";

            created.SpeakerID = editorNode.SpeakerID;
            created.DialogText = editorNode.DialogText;
            created.optionsText = new List<string>(editorNode.GetAllOptions().ToArray());

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }


        public void AddChildren(params BaseDialogNode[] dialogs)
        {
            for (int i = 0; i < dialogs.Length; i++)
            {
                children.Add(dialogs[i]);
            }

            if (children.Count < optionsText.Count) Debug.Log("Dialog System: Warning, a created options dialog has excess options text.");
        }
    }
}