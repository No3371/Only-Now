using UnityEngine;
using System.Collections.Generic;

namespace DialogSystem.Data
{
    public class OptionsDialog : DialogNode
    {
        static int childrenLimit = 4;

        [SerializeField]
        string text;
        [SerializeField]
        string speaker;

        public string Text { get { return text; } set { text = value; } }
        public string Speaker { get { return text; } set { text = value; } }

        public List<string> optionsText = new List<string>();

        public static OptionsDialog Create(string speaker, string text, DialogNode parent)
        {
            OptionsDialog created = ScriptableObject.CreateInstance<OptionsDialog>();

            created.Speaker = speaker;
            created.Text = text;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public static OptionsDialog Create(string speaker, string text, DialogNode parent, string[] options)
        {
            OptionsDialog created = ScriptableObject.CreateInstance<OptionsDialog>();

            created.Speaker = speaker;
            created.Text = text;

            for (int i = 0; i < options.Length; i++)
            {
                created.optionsText.Add(options[i]);
            }

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public void AddChildren(params DialogNode[] dialogs)
        {
            for (int i = 0; i < dialogs.Length; i++)
            {
                children.Add(dialogs[i]);
            }

            if (children.Count < optionsText.Count) Debug.Log("Dialog System: Warning, a created options dialog has excess options text.");
        }
    }
}