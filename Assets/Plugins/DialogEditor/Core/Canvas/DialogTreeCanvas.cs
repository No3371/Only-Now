using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using UnityEngine;

namespace DialogNodeEditor
{
    [NodeCanvasType("DialogTreeCanvas")]
    public class DialogTreeCanvas : NodeCanvas
    {
        [SerializeField]
        public DialogStartNode dialogStartNode;

        [SerializeField]
        public string description = "";

        public override void BeforeSavingCanvas()
        {
            foreach (Node node in this.nodes)
            {
                if (node is DialogStartNode)
                {
                    dialogStartNode = node as DialogStartNode;
                    description = (node as DialogStartNode).description;
                }
            }
            if (dialogStartNode == null && nodes.Count > 0) UnityEditor.EditorUtility.DisplayDialog("Warning!", "Dialog Editor cant find any dialog start node in this canvas. The canvas will still get saved, but please add the start node.", "Close");
        }
    }
}
