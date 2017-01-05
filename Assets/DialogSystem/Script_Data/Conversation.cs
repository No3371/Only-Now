using UnityEngine;
using System.Collections.Generic;

using StorySystem.Data;

namespace StorySystem.Data
{
    [System.Serializable]
    public class Conversation : ScriptableObject
    {
        public ConversationStartNode startNode;
        public List<BaseDialogNode> nodes = new List<BaseDialogNode>();

        public string conversationID;
        public string description;
                
        public static Conversation Create(DialogNodeEditor.ConversationCanvas canvas)
        {
            Conversation created = ScriptableObject.CreateInstance<Conversation>();

            created.startNode = ConversationStartNode.Create(canvas.convStartNode);

            created.conversationID = created.startNode.convId;
            created.description = created.startNode.description;

            return created;
        }

        public void AddNode(BaseDialogNode node)
        {
            nodes.Add(node);
        }
    }
}
