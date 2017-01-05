using UnityEngine;
using System.Collections;

namespace StorySystem.Data
{
    public class ConversationStartNode : BaseConvControlNode
    {
        public string convId;

        public string description;

        public static ConversationStartNode Create(DialogNodeEditor.ConversationStartNode editorNode)
        {
            ConversationStartNode created = ScriptableObject.CreateInstance<ConversationStartNode>();

            created.nodeType = "ConversationStartNode";
            created.convId = editorNode.convID;
            created.description = editorNode.description;

            return created;
        }
    }
}