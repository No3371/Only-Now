using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{
    public class TreeStartNode : SystemNode
    {
        public static TreeStartNode Create()
        {
            TreeStartNode created = ScriptableObject.CreateInstance<TreeStartNode>();

            created.NodeType = SystemNodeType.Start;

            return created;
        }
    }
}