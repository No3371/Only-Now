using UnityEngine;
using System.Collections.Generic;

namespace StorySystem.Data
{
    public class RandomOutputNode : BaseConvControlNode
    {
        public static RandomOutputNode Create(BaseDialogNode parent)
        {
            RandomOutputNode created = ScriptableObject.CreateInstance<RandomOutputNode>();
            
            created.nodeType = "RandomOutput";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public BaseDialogNode RandomOne()
        {
            return this.children[Random.Range(0, this.children.Count)];
        }
    }
}