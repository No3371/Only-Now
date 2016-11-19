using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{
    public class TreeEndNode : SystemNode
    {
        public static TreeEndNode Create(DialogNode parent)
        {
            TreeEndNode created = ScriptableObject.CreateInstance<TreeEndNode>();

            created.NodeType = SystemNodeType.End;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }
    }
}