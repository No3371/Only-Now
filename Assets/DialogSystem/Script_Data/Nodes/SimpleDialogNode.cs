using UnityEngine;
using StorySystem.Runtime;

namespace StorySystem.Data
{

    public class SimpleDialogNode : BaseDialogNode
    {
        [SerializeField]
        string dialogText;
        [SerializeField]
        string speakerID;

        public string DialogText { get { return dialogText; } set { dialogText = value; } }
        public string SpeakerID { get { return dialogText; } set { dialogText = value; } }

        public static SimpleDialogNode Create(DialogNodeEditor.SimpleDialogNode editorNode, BaseDialogNode parent)
        {
            SimpleDialogNode created = ScriptableObject.CreateInstance<SimpleDialogNode>();

            created.SpeakerID = editorNode.SpeakerID;
            created.DialogText = editorNode.DialogText;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public void Handle(StorySystemManager system, ConversationResolver resolver){
            
            resolver.currentNodeCache = this.children[0];
        }
    }
}