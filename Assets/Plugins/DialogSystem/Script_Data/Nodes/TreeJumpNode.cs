using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{
    public class TreeJumpNode : SystemNode
    {
        [SerializeField]
        public string targetName;

        public static TreeJumpNode Create(DialogNode parent, string targetName)
        {
            TreeJumpNode created = ScriptableObject.CreateInstance<TreeJumpNode>();

            created.NodeType = SystemNodeType.Jump;
            created.targetName = targetName;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }        
    }
}