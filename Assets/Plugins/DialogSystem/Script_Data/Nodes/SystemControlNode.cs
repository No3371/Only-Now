using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{
    public class SystemControlNode : SystemNode
    {
        [SerializeField]
        string key;
        [SerializeField]
        float float_Value;
        [SerializeField]
        int float_operation_type;
        [SerializeField]
        string string_Value;
        [SerializeField]
        bool bool_Value;
        [SerializeField]
        string type;

        public string Key { get { return key; } }
        public string Type { get { return type; } }
        public float FloatValue { get { return float_Value; } }
        public bool BoolValue { get { return bool_Value; } }
        public string StringValue { get { return string_Value; } }
        public int floatOperationType { get { return float_operation_type; } }

        public static SystemControlNode Create()
        {
            SystemControlNode created = ScriptableObject.CreateInstance<SystemControlNode>();

            created.NodeType = SystemNodeType.Control;

            return created;
        }

        /// <summary>
        /// The operation type list: +, -, x, ÷, =.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="operation">+, -, x, ÷, =</param>
        /// <param name="parent"></param>
        /// <returns></returns>

        public static SystemControlNode Create(string key, float value, int operation, DialogNode parent)
        {
            SystemControlNode created = ScriptableObject.CreateInstance<SystemControlNode>();

            created.NodeType = SystemNodeType.Control;
            created.key = key;
            created.float_Value = value;
            created.type = "float";
            created.float_operation_type = operation;

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public static SystemControlNode Create(string key, string value, DialogNode parent)
        {
            SystemControlNode created = ScriptableObject.CreateInstance<SystemControlNode>();

            created.NodeType = SystemNodeType.Control;
            created.key = key;
            created.string_Value = value;
            created.type = "string";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }

        public static SystemControlNode Create(string key, bool value, DialogNode parent)
        {
            SystemControlNode created = ScriptableObject.CreateInstance<SystemControlNode>();

            created.NodeType = SystemNodeType.Control;
            created.key = key;
            created.bool_Value = value;
            created.type = "bool";

            created.parents.Add(parent);
            parent.children.Add(created);

            return created;
        }
    }
}