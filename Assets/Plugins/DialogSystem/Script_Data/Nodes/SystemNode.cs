using UnityEngine;
using System.Collections;

namespace DialogSystem.Data
{
    public class SystemNode : DialogNode
    {
        [System.Serializable]
        public enum SystemNodeType
        {
            Default, //Should never be used in normal situation
            TextDelay, //Planned
            DialogStyle, //Planned
            DialogDelay, //Planned
            RandomChooser, //Planned
            Control,
            Conditional,
            Jump,
            Start,
            End
        }

        public SystemNodeType NodeType = SystemNodeType.Default;
    }
}