using UnityEngine;
using UnityEditor;
using System.Collections;
using NodeEditorFramework;
using System.Collections.Generic;

using StorySystem.Runtime;


namespace StorySystem.Data
{
    public class Converter
    {
        public static void Save(string path, NodeEditorFramework.NodeCanvas canvas)
        {
            Conversation tree = ConvertToNewConv(canvas);

            AssetDatabase.CreateAsset(tree, path);
            foreach (BaseDialogNode n in tree.nodes)
            {
                AddSubAsset(n, tree);
            }
            Debug.Log("Converter:: Save:: Created a new dialog tree.");

            AssetDatabase.SaveAssets();
        }

        public static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
        {
            if (subAsset != null && mainAsset != null)
            {
                AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
                subAsset.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        private static void Recursive(Node processing, BaseDialogNode currentRef, Conversation tree)
        {
            if (processing == null)
            {
                Debug.Log("Converter: Recursive(): Why bother ask me to process a null object? Returnning.");
                return;
            }


            //If skip this check, the recursion will stuck, as pretty much execute in the for().
            //Will prevent going deeper at End Node
            if (processing.Outputs.Count == 0)
            {
                return;
            }
            
            
            for (int i = 0; i < processing.Outputs.Count; i++)
            {
                BaseDialogNode creating = null;
                if (processing.Outputs[i].GetNodeAcrossConnection() == null)
                {
                    Debug.Log("Converter: This connect to nothing. Returning.");
                    return;

                }
                if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == null)
                {
                    Debug.Log("Converter: Warning! Recursive() receiving null translated type.");
                    return;

                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(ConversationEndNode))
                {
                    creating = ConversationEndNode.Create(currentRef);
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(SimpleDialogNode))
                {
                    creating = SimpleDialogNode.Create(((DialogNodeEditor.SimpleDialogNode)processing.Outputs[i].GetNodeAcrossConnection()), currentRef);
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(OptionsDialogNode))
                {
                    creating = OptionsDialogNode.Create(((DialogNodeEditor.OptionsDialogNode)processing.Outputs[i].GetNodeAcrossConnection()), currentRef);
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(VariableControlNode))
                {
                    creating = VariableControlNode.Create((DialogNodeEditor.VariableControlNode) processing.Outputs[i].GetNodeAcrossConnection());
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(VariableConditionalNode))
                {
                    creating = VariableConditionalNode.Create((DialogNodeEditor.VariableConditionalNode) processing.Outputs[i].GetNodeAcrossConnection());

                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(RandomOutputNode))
                {
                    creating = RandomOutputNode.Create(currentRef);
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(ConversationJumpNode))
                {
                    creating = ConversationJumpNode.Create(((DialogNodeEditor.ConversationJumpNode)processing.Outputs[i].GetNodeAcrossConnection()), currentRef);
                }
                                    
                tree.AddNode(creating);
                Recursive(processing.Outputs[i].GetNodeAcrossConnection(), creating, tree);
            }
        }

        static int tryNodeCount = 0;
        public static int tryOutputCount = 1;
        private static void TryRecursive(Node processing, bool top = false)
        {
            if (processing == null) return;
            if (top)
            {
                tryNodeCount = 0;
                tryOutputCount = 1;
            }
            
            for (int i = 0; i < processing.Outputs.Count; i++)
            {                  
                TryRecursive(processing.Outputs[i].GetNodeAcrossConnection());
            }

            tryOutputCount += processing.Outputs.Count;
            tryNodeCount += 1;
            return;            
        }

        public static Conversation ConvertToNewConv(NodeEditorFramework.NodeCanvas canvas)
        {
            Conversation conv = Conversation.Create(canvas as DialogNodeEditor.ConversationCanvas);

            Node processing = (canvas as DialogNodeEditor.ConversationCanvas).convStartNode;
            ConversationStartNode currentRef = conv.startNode;
            //
            Recursive(processing, currentRef, conv);

            return conv;

        }

        ///Check if the canvas is close.
        public static int TryDialogTree(NodeEditorFramework.NodeCanvas canvas)
        {
            Node processing = (canvas as DialogNodeEditor.ConversationCanvas).convStartNode;
            //

            TryRecursive(processing, true);
            
            Debug.Log("Converter:: TryDialogTree:: Try Result: " + tryNodeCount + " nodes," + tryOutputCount + "Outputs");

            return tryNodeCount;
        }

        public static System.Type Translate(System.Type passIn)
        {
            if (!passIn.IsSubclassOf(typeof(DialogNodeEditor.BaseDialogNode)))
            {
                Debug.Log("Converter: Warnning! Trying to convert a object not from node editor. Returning null.");
                return null;
            }

            if (passIn == typeof(DialogNodeEditor.SimpleDialogNode)) return typeof(StorySystem.Data.SimpleDialogNode);
            else if (passIn == typeof(DialogNodeEditor.OptionsDialogNode)) return typeof(StorySystem.Data.OptionsDialogNode);
            else if (passIn == typeof(DialogNodeEditor.ConversationStartNode)) return typeof(StorySystem.Data.ConversationStartNode);
            else if (passIn == typeof(DialogNodeEditor.ConversationEndNode)) return typeof(StorySystem.Data.ConversationEndNode);
            else if (passIn == typeof(DialogNodeEditor.ConversationJumpNode)) return typeof(StorySystem.Data.ConversationJumpNode);
            else if (passIn == typeof(DialogNodeEditor.VariableControlNode)) return typeof(StorySystem.Data.VariableControlNode);
            else if (passIn == typeof(DialogNodeEditor.VariableConditionalNode)) return typeof(StorySystem.Data.VariableConditionalNode);
            else if (passIn == typeof(DialogNodeEditor.RandomOutputNode)) return typeof(StorySystem.Data.RandomOutputNode);
            //else if (passIn == typeof(DialogNodeEditor.Action_JumpNode)){} //TODO_BA action jump translation}}
            //else if (passIn == typeof(DialogNodeEditor.Action_MoveNode)){} //TODO_BA action jump translation}}
            //else if (passIn == typeof(DialogNodeEditor.Action_MoveToWaypointNode)){} //TODO_BA action jump translation}}
            //else if (passIn == typeof(DialogNodeEditor.Action_ShakeNode)){} //TODO_BA action jump translation}}
            //else if (passIn == typeof(DialogNodeEditor.Action_InstantAnimationNode)){} //TODO_BA action jump translation}}
            else return null;
        }

        private void Connect(BaseDialogNode parent, BaseDialogNode child)
        {
            child.parents.Add(parent);
            parent.children.Add(child);
        }
    }
}
