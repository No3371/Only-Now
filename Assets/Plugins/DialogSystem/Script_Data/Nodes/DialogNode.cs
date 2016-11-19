using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace DialogSystem.Data
{
    public class DialogNode : ScriptableObject
    {
        static int childrenLimit = 1;

        public List<DialogNode> parents = new List<DialogNode>();
        public List<DialogNode> children = new List<DialogNode>();

        public bool HasChildren()
        {
            return (children.Count > 0);
        }

        public void AddChildren(DialogNode dialog)
        {
            if (dialog.GetType() == typeof(TreeStartNode))
                Debug.Log("Dialog System: Warning! Trying to add Start Node to children.");
            if (!IfChildrenFull()) children.Add(dialog);
            else Debug.Log("Dialog System: Fail, children count of the node has reach limit.");
        }

        public bool IfChildrenFull()
        {
            return children.Count > childrenLimit;
        }
    }


    /*
    public class RandomDialog : Dialog
    {
        Dialog parent; Dialog Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        List<Dialog> children = new List<Dialog>();
        
        public RandomDialog()
        {

        }

        public RandomDialog(string text)
        {
            Text = text;
        }

        public void AddOptions(Dialog dialog)
        {
            children.Add(dialog);
        }

        public void RemoveOptions(int id)
        {
            children.Remove(children[id]);
        }

        public new bool HasChildren()
        {
            return (children.Count > 0);
        }
    }
    */
}
