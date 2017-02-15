using UnityEngine;
using System.Collections.Generic;
using StorySystem.Runtime;
using VariableSystem;

namespace StorySystem.Data
{
    public class VariableControlNode : BaseVariableNode
    {

        [SerializeField]
        List<VariableControlItem> operations = new List<VariableControlItem>();

        public static VariableControlNode Create(DialogNodeEditor.VariableControlNode editorNode)
        {
            VariableControlNode created = ScriptableObject.CreateInstance<VariableControlNode>();

            created.operations = new List<VariableControlItem>(editorNode.operations);

            return created;
        }


        ///Calculate and apply new variable values. Then assign the child node to currentNodeCache; 
        public override void Handle(StorySystemManager system, ConversationResolver resolver){
            for (int i = 0; i < operations.Count; i++){
                operations[i].Control();
                resolver.currentNodeCache = this.children[0];
            }
        }
    }
}