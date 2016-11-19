using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{

    public class SimpleDialog : DialogNode
    {
        [SerializeField]
        string text;
        [SerializeField]
        string speaker;

        public string Text { get { return text; } set { text = value; } }
        public string Speaker { get { return text; } set { text = value; } }

        public static SimpleDialog Create(string speaker, string text, DialogNode parent)
        {
            SimpleDialog created = ScriptableObject.CreateInstance<SimpleDialog>();

            created.Speaker = speaker;
            created.Text = text;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }
    }
}