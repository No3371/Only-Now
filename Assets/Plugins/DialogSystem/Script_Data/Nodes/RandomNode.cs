using UnityEngine;
using System.Collections.Generic;

namespace DialogSystem.Data
{
    public class RandomNode : SystemNode
    {

        public static RandomNode Create(DialogNode parent)
        {
            RandomNode created = ScriptableObject.CreateInstance<RandomNode>();
            
            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public DialogNode RandomOne()
        {
            return this.children[Random.Range(0, this.children.Count)];
        }
    }
}