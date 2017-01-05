using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StorySystem.Runtime;


namespace StorySystem.Data
{
    public class BaseDialogNode : ScriptableObject
    {
        public string nodeType = "Base";

        static int childrenLimit = 1;

        public List<BaseDialogNode> parents = new List<BaseDialogNode>();
        public List<BaseDialogNode> children = new List<BaseDialogNode>();

        public bool HasChildren()
        {
            return (children.Count > 0);
        }

        public void AddChildren(BaseDialogNode dialog)
        {
            if (dialog.GetType() == typeof(ConversationStartNode))
                Debug.Log("Dialog System: Warning! Trying to add Start Node to children.");
            if (!IfChildrenFull()) children.Add(dialog);
            else Debug.Log("Dialog System: Fail, children count of the node has reach limit.");
        }

        public bool IfChildrenFull()
        {
            return children.Count > childrenLimit;
        }
		public virtual void Handle(StorySystemManager system, ConversationResolver resolver){

        }
    }
}
