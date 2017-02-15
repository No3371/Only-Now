using UnityEngine;
using System.Collections.Generic;
using StorySystem.Runtime;
using VariableSystem;

namespace StorySystem.Data
{
    public class VariableConditionalNode : BaseVariableNode
    {
        VariableConditionGroupType condGroup;

        [SerializeField]
        List<VariableConditionItem> conditions = new List<VariableConditionItem>();

        public bool Check()
        {
            switch (condGroup){
                case VariableConditionGroupType.And:
                    bool temp = true;
                    for (int i = 0; i < conditions.Count; i++){
                        temp = temp & conditions[i].Check();
                    }
                    return temp;
                case VariableConditionGroupType.Or:
                    for (int i = 0; i < conditions.Count; i++){
                        if (conditions[i].Check()) return true;
                    }
                    return false;
                default:
                    return false; 
            }
        }

        public static VariableConditionalNode Create(DialogNodeEditor.VariableConditionalNode editorNode)
        {
            VariableConditionalNode created = ScriptableObject.CreateInstance<VariableConditionalNode>();

            created.conditions = new List<VariableConditionItem>(editorNode.conditions);
            created.condGroup = editorNode.condGroup;

            return created;
        }

        public override void Handle(StorySystemManager system, ConversationResolver resolver){
            Check();
            resolver.currentNodeCache = this.children[0];
        }
    }

}