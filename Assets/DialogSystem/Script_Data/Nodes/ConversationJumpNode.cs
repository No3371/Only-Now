using UnityEngine;
using System.Collections;

namespace StorySystem.Data
{
    public class ConversationJumpNode : BaseConvControlNode
    {
        [SerializeField]
        public string targetConvID;

        public static ConversationJumpNode Create(DialogNodeEditor.ConversationJumpNode editorNode, BaseDialogNode parent)
        {
            ConversationJumpNode created = ScriptableObject.CreateInstance<ConversationJumpNode>();

            created.nodeType = "ConversationJumpNode";
            created.targetConvID = editorNode.targetID;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }        

        public override void Handle(Runtime.StorySystemManager system, StorySystem.Runtime.ConversationResolver resolver){
            resolver.availableJump = system.allConvCaches[targetConvID];
        }
    }
}