using UnityEngine;
using System.Collections.Generic;

using DialogSystem.Data;

namespace DialogSystem.Data
{
    [System.Serializable]
    public class DialogTree : ScriptableObject
    {
        public TreeStartNode treeStartNode;
        public List<DialogNode> nodes = new List<DialogNode>();

        public string Name;
        public string Description;
                
        public static DialogTree Create()
        {
            DialogTree created = ScriptableObject.CreateInstance<DialogTree>();
            created.treeStartNode = TreeStartNode.Create();
            created.nodes.Add(created.treeStartNode);
            return created;
        }

        public void AddNode(DialogNode node)
        {
            nodes.Add(node);
        }
    }
}
