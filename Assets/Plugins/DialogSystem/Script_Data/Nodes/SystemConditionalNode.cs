using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{
    public class SystemConditionalNode : SystemNode
    {
        [SerializeField]
        string key;
        [SerializeField]
        float float_Value;
        [SerializeField]
        int float_Operation_Type;
        [SerializeField]
        string string_Value;
        [SerializeField]
        bool bool_Value;
        [SerializeField]
        string type;

        public string Key { get { return key; } }
        public string Type { get { return type; } }

        public bool Check(float value)
        {
            switch (float_Operation_Type)
            {
                case 0:
                    return float_Value == value;
                case 1:
                    return float_Value > value;
                case 2:
                    return float_Value < value;
                default:
                    throw new System.Exception("Operation type invalid.");
                    break;
            }            
        }

        public bool Check(bool value)
        {
            return bool_Value == value;
        }
        public bool Check(string value)
        {
            return string_Value == value;
        }


        public static SystemConditionalNode Create()
        {
            SystemConditionalNode created = ScriptableObject.CreateInstance<SystemConditionalNode>();

            created.NodeType = SystemNodeType.Conditional;

            return created;
        }

        public static SystemConditionalNode Create(string key, float value, int optType, DialogNode parent)
        {
            SystemConditionalNode created = ScriptableObject.CreateInstance<SystemConditionalNode>();

            created.NodeType = SystemNodeType.Conditional;
            created.key = key;
            created.float_Value = value;
            created.float_Operation_Type = optType;
            created.type = "float";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public static SystemConditionalNode Create(string key, string value, DialogNode parent)
        {
            SystemConditionalNode created = ScriptableObject.CreateInstance<SystemConditionalNode>();

            created.NodeType = SystemNodeType.Control;
            created.key = key;
            created.string_Value = value;
            created.type = "string";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public static SystemConditionalNode Create(string key, bool value, DialogNode parent)
        {
            SystemConditionalNode created = ScriptableObject.CreateInstance<SystemConditionalNode>();

            created.NodeType = SystemNodeType.Conditional;
            created.key = key;
            created.bool_Value = value;
            created.type = "bool";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }
    }

}