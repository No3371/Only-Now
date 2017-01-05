using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEngine;

namespace DialogNodeEditor
{
    [NodeCanvasType("ConversationCanvas")]
    public class ConversationCanvas : NodeCanvas
    {
        [SerializeField]
        public ConversationStartNode convStartNode;

        [SerializeField]
        public string description = "";

        public override void BeforeSavingCanvas()
        {
            foreach (Node node in this.nodes)
            {
                if (node is ConversationStartNode)
                {
                    convStartNode = node as ConversationStartNode;
                    description = (node as ConversationStartNode).description;
                }
            }
            if (convStartNode == null && nodes.Count > 0) UnityEditor.EditorUtility.DisplayDialog("Warning!", "Dialog Editor cant find any dialog start node in this canvas. The canvas will still get saved, but please add the start node.", "Close");
        }
    }
}
