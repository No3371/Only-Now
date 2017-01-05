using UnityEngine;
using System.Collections;

namespace StorySystem.Data
{
    public class ConversationEndNode : BaseConvControlNode
    {
        public static ConversationEndNode Create(BaseDialogNode parent)
        {
            ConversationEndNode created = ScriptableObject.CreateInstance<ConversationEndNode>();

            created.nodeType = "ConversationEnd";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }
    }
}