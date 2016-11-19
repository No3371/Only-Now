using UnityEngine;
using UnityEditor;
using System.Collections;
using NodeEditorFramework;
using System.Collections.Generic;

using DialogSystem.Runtime;


namespace DialogSystem.Data
{
    public class Converter
    {
        public static void Save(string path, NodeEditorFramework.NodeCanvas canvas)
        {
            DialogTree tree = ConvertToDialogTree(canvas);

            AssetDatabase.CreateAsset(tree, path);
            foreach (DialogNode n in tree.nodes)
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
                UnityEditor.AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
                subAsset.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        private static void Recursive(Node processing, DialogNode currentRef, DialogTree tree)
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
                DialogNode creating = null;
                if (processing.Outputs[i].GetNodeAcrossConnection() == null)
                {
                    Debug.Log("Converter: This connect to nothing. Returnning.");
                    return;

                }
                if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == null)
                {
                    Debug.Log("Converter: Warnning! Recursive() receiving null translated type.");
                    return;

                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(TreeEndNode))
                {
                    creating = TreeEndNode.Create(currentRef);
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(SimpleDialog))
                {
                    creating = (SimpleDialog.Create(((DialogNodeEditor.DialogNode)processing.Outputs[i].GetNodeAcrossConnection()).SpeakerID, ((DialogNodeEditor.DialogNode)processing.Outputs[i].GetNodeAcrossConnection()).SpeakingText, currentRef));
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(OptionsDialog))
                {
                    creating = (OptionsDialog.Create(((DialogNodeEditor.DialogMultiOptionsNode)processing.Outputs[i].GetNodeAcrossConnection()).SpeakerID, ((DialogNodeEditor.DialogMultiOptionsNode)processing.Outputs[i].GetNodeAcrossConnection()).SpeakingText, currentRef, ((DialogNodeEditor.DialogMultiOptionsNode)processing.Outputs[i].GetNodeAcrossConnection()).GetAllOptions().ToArray()));
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(SystemControlNode))
                {
                    switch (((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).currentSelection)
                    {
                        case 0:
                            creating = (DialogSystem.Data.SystemControlNode.Create(((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).key, ((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).float_Value, ((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).float_operation_type, currentRef));
                            break;
                        case 1:
                            creating = (DialogSystem.Data.SystemControlNode.Create(((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).key, ((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).string_Value, currentRef));
                            break;
                        case 2:
                            creating = (DialogSystem.Data.SystemControlNode.Create(((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).key, ((DialogNodeEditor.ControlNode)processing.Outputs[i].GetNodeAcrossConnection()).bool_Value, currentRef));
                            break;
                    }
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(DialogSystem.Data.SystemConditionalNode))
                {
                    switch (((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).currentSelection)
                    {
                        case 0:
                            creating = (DialogSystem.Data.SystemConditionalNode.Create(((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).key, ((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).float_Value, ((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).float_operation_type, currentRef));
                            break;
                        case 1:
                            creating = (DialogSystem.Data.SystemConditionalNode.Create(((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).key, ((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).string_Value, currentRef));
                            break;
                        case 2:
                            creating = (DialogSystem.Data.SystemConditionalNode.Create(((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).key, ((DialogNodeEditor.SystemConditionalNode)processing.Outputs[i].GetNodeAcrossConnection()).bool_Value, currentRef));
                            break;
                    }
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(DialogSystem.Data.RandomNode))
                {
                    creating = RandomNode.Create(currentRef);
                }
                else if (Translate(processing.Outputs[i].GetNodeAcrossConnection().GetType()) == typeof(DialogSystem.Data.TreeJumpNode))
                {
                    creating = TreeJumpNode.Create(currentRef, ((DialogNodeEditor.DialogJumpNode)processing.Outputs[i].GetNodeAcrossConnection()).targetName);
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

        public static DialogTree ConvertToDialogTree(NodeEditorFramework.NodeCanvas canvas)
        {
            DialogTree tree = DialogTree.Create();

            //Start from the start node.
            foreach (Node n in canvas.nodes)
            {
                if (n is DialogNodeEditor.DialogStartNode)
                {
                    (canvas as DialogNodeEditor.DialogTreeCanvas).dialogStartNode = n as DialogNodeEditor.DialogStartNode;
                    tree.Name = (n as DialogNodeEditor.DialogStartNode).dialogName;
                }
            }
            Node processing = (canvas as DialogNodeEditor.DialogTreeCanvas).dialogStartNode;
            TreeStartNode currentRef = tree.treeStartNode;
            //
            Recursive(processing, currentRef, tree);

            tree.Description = (canvas as DialogNodeEditor.DialogTreeCanvas).description;

            return tree;

        }

        public static int TryDialogTree(NodeEditorFramework.NodeCanvas canvas)
        {
            DialogTree tree = DialogTree.Create();

            //Start from the start node.

            foreach(Node n in canvas.nodes)
            {
                if (n is DialogNodeEditor.DialogStartNode) (canvas as DialogNodeEditor.DialogTreeCanvas).dialogStartNode = n as DialogNodeEditor.DialogStartNode;
            }

            Node processing = (canvas as DialogNodeEditor.DialogTreeCanvas).dialogStartNode;
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

            if (passIn == typeof(DialogNodeEditor.DialogNode)) return typeof(DialogSystem.Data.SimpleDialog);
            else if (passIn == typeof(DialogNodeEditor.DialogStartNode)) return typeof(DialogSystem.Data.TreeStartNode);
            else if (passIn == typeof(DialogNodeEditor.DialogMultiOptionsNode)) return typeof(DialogSystem.Data.OptionsDialog);
            else if (passIn == typeof(DialogNodeEditor.ControlNode)) return typeof(DialogSystem.Data.SystemControlNode);
            else if (passIn == typeof(DialogNodeEditor.SystemConditionalNode)) return typeof(DialogSystem.Data.SystemConditionalNode);
            else if (passIn == typeof(DialogNodeEditor.DialogEndNode)) return typeof(DialogSystem.Data.TreeEndNode);
            else if (passIn == typeof(DialogNodeEditor.RandomNode)) return typeof(DialogSystem.Data.RandomNode);
            else if (passIn == typeof(DialogNodeEditor.DialogJumpNode)) return typeof(DialogSystem.Data.TreeJumpNode);
            else return null;
        }

        private void Connect(DialogNode parent, DialogNode child)
        {
            child.parents.Add(parent);
            parent.children.Add(child);
        }
    }
}
