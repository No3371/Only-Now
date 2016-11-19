using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using DialogSystem.Data;


namespace DialogSystem.Runtime
{
    public class DialogSystem : MonoBehaviour
    {
        static string dialogSystemPath = "Plugins/DialogSystem";

        public static DialogSystem instance;
        public static List<Speaker> activeSpeakers;
        Speaker playerSpeaker;
        Dictionary<string, Speaker> speakerMap = new Dictionary<string, Speaker>();

        public const float textDelayGlobalFactor = 1.1f;

        DataProfile profileCache;

        Stack<GameObject> DialogBubblePool = new Stack<GameObject>();
        GameObject OptionsDialogReserve;

        Dict_Tree AllTreeCache;
        DialogTree toJump = null;


        enum SystemState
        {
            Initializing,
            Standby,
            Loading,
            Display, //Planned: Enter to skip dialog delay
            ReadyForNext,
            AwaitingInput, //e.g., OptionsDialog
            EndingDialog           
        }

        static SystemState systemState;


        void CacheAllTree()
        {
            DialogTree[] temp = Resources.FindObjectsOfTypeAll<DialogTree>();
            for (int i = 0; i < temp.Length; i++)
            {
                AllTreeCache.Add(temp[i].Name, temp[i]);
            }
        }
        // Use this for initialization
        void Start()
        {
            if (instance == null) instance = this;
            if (instance != this) Destroy(this);
            DontDestroyOnLoad(this);

            systemState = SystemState.Initializing;
            BubblePooling();
        }

        void LoadDialogTree (DialogTree toLoad)
        {
            systemState = SystemState.Loading;
            loadedTreeCache = toLoad;
            currentDialog = loadedTreeCache.treeStartNode;
            systemState = SystemState.ReadyForNext;
        }

        void LoadDialogTree()
        {
            systemState = SystemState.Loading;
            loadedTreeCache = pendingTrees.Dequeue();
            currentDialog = loadedTreeCache.treeStartNode;
            systemState = SystemState.ReadyForNext;
        }

        // Update is called once per frame
        void Update()
        {
            
            switch (systemState)
            {
                case SystemState.Initializing:
                    // Shouldn't happen
                    break;
                case SystemState.Standby:
                    if (toJump != null)
                    {
                        LoadDialogTree(toJump);
                        toJump = null;
                    }
                    else {
                        if (pendingTrees.Count == 0) return;
                        else if (pendingTrees.Count >= 1)
                        {
                            LoadDialogTree();
                        }
                    }
                    break;
                case SystemState.Loading:
                    break;
                case SystemState.Display:
                    //Make speaker display if no current dialog displaying 
                    //If a dialog is displaying, but receiver is not done, do nothing
                    //Goes to ReadyForNext or AwaitingInput (OptionsDialog) when receiver is done
                    if (receiver == null)
                    {
                        receiver = new DisplayDoneReceiver();

                        if (currentDialog is OptionsDialog)
                        {
                            speakerMap[(currentDialog as OptionsDialog).Speaker].Display(currentDialog as OptionsDialog, receiver);
                            systemState = SystemState.AwaitingInput;
                        }
                        else if (currentDialog is SimpleDialog)
                        {
                            speakerMap[(currentDialog as SimpleDialog).Speaker].Display(currentDialog as SimpleDialog, receiver);
                        }
                        else { }
                    }
                    else if (receiver.done)
                    {
                        if (currentDialog is SimpleDialog) systemState = SystemState.ReadyForNext;
                    }
                    else { }
                    break;
                case SystemState.AwaitingInput:
                    //Do nothing until player select a options (receiver.done)
                    if (receiver.done)
                    {
                        if (currentDialog is OptionsDialog)
                            systemState = SystemState.ReadyForNext;
                    }
                    
                    break;
                case SystemState.ReadyForNext:
                    //Only change currentDialog at this state 
                    //goes to Display (SimpleDialog, OptionsDialog) or EndingDialog (End Node)
                    if (currentDialog is OptionsDialog) currentDialog = DialogNodeHandler(currentDialog, receiver.optionsIndex);
                    else currentDialog = DialogNodeHandler(currentDialog);
                    receiver = null;                    

                    if (currentDialog == null) //Returned by DialogNodeHandler when passed in node is TreeEndNode/JumpNode
                    {
                        systemState = SystemState.EndingDialog;
                    }
                    else
                    {
                        systemState = SystemState.Display;
                    }
                    break;
                case SystemState.EndingDialog:
                    //Goes to Standby
                    receiver = null;
                    systemState = SystemState.Standby;
                    break;
            }

        }

        void BubblePooling(int amount = 5)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject bb = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/DialogBubble"));
                bb.transform.SetParent(GameObject.Find("Canvas").transform);
                bb.SetActive(false);
                DialogBubblePool.Push(bb);
            }

            OptionsDialogReserve = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/OptionsDialogBubble"));
            OptionsDialogReserve.SetActive(false);    
        }

        public void ActivateSpeaker(Speaker speaker)
        {
            activeSpeakers.Add(speaker);
            speakerMap.Add(speaker.ID, speaker);
            speaker.dialogBubble = DialogBubblePool.Pop();
        }

        public void DeactivateSpeaker(Speaker speaker)
        {
            activeSpeakers.Remove(speaker);
            speakerMap.Remove(speaker.ID);
            DialogBubblePool.Push(speaker.dialogBubble);
            speaker.dialogBubble = null;
        }

        #region DialogOngoing

        public static float textDelay = 0.1f;
        DisplayDoneReceiver receiver;
        Queue<DialogTree> pendingTrees = new Queue<DialogTree>();
        DialogTree loadedTreeCache;
        DialogNode currentDialog;
        
        public void ActivateTree(DialogTree tree)
        {
            pendingTrees.Enqueue(tree);
        }

        public void ActivateTrees(DialogTree[] trees)
        {
            foreach(DialogTree tree in trees) pendingTrees.Enqueue(tree);
        }
        
        class DialogStyle
        {
            Color fore;
            Color bg;

            public DialogStyle()
            {
                this.fore = Color.white;
                this.bg = Color.black;
            }

            public DialogStyle(Color fore, Color32 bg)
            {
                this.fore = fore;
                this.bg = bg;
            }
        }

        #endregion
        DialogNode DialogNodeHandler(DialogNode node)
        {
            if (node is TreeStartNode) return node.children[0];
            else if (node is TreeEndNode) return null;
            else if (node is SystemConditionalNode) return ConditionalNodeHandler(node as SystemConditionalNode);
            else if (node is SystemControlNode) return ControlNodeHandler(node as SystemControlNode);
            else if (node is OptionsDialog) return OptionsDialogHandler(node as OptionsDialog);
            else if (node is SimpleDialog) return SimpleDialogHandler(node as SimpleDialog);
            else if (node is TreeJumpNode) return TreeJumpNodeHandler(node as TreeJumpNode);
            else if (node is RandomNode) return RandomNodeHandler(node as RandomNode);
            else return null;
        }

        DialogNode DialogNodeHandler(DialogNode node, int selection)
        {
            if (node is TreeStartNode) return node.children[0];
            else if (node is TreeEndNode) return null;
            else if (node is SystemConditionalNode) return ConditionalNodeHandler(node as SystemConditionalNode);
            else if (node is SystemControlNode) return ControlNodeHandler(node as SystemControlNode);
            else if (node is OptionsDialog) return OptionsDialogHandler(node as OptionsDialog, selection);
            else if (node is SimpleDialog) return SimpleDialogHandler(node as SimpleDialog);
            else if (node is TreeJumpNode) return TreeJumpNodeHandler(node as TreeJumpNode);
            else if (node is RandomNode) return RandomNodeHandler(node as RandomNode);
            else return null;
        }

        public class DisplayDoneReceiver
        {
            public bool done = false;
            public int optionsIndex = 0;
        }

        /// <summary>
        /// Return the check result as children index. 
        /// </summary>
        /// <returns>
        /// If the condition is met, return 0, otherwise return 1;
        /// </returns>

        DialogNode TreeJumpNodeHandler(TreeJumpNode node)
        {
            if (AllTreeCache.ContainsKey(node.targetName)) toJump = AllTreeCache[node.targetName];
            else Debug.Log("DialogSystem:: TreeJumpNodeHandler:: Can't get any tree with the key: " + node.targetName);
            return null;
        }

        DialogNode ConditionalNodeHandler(SystemConditionalNode node)
        {
            if (ProfileCheck(node)) return node.children[0];
            else if (!ProfileCheck(node)) return node.children[1];
            else return null; //Fallback
        }

        DialogNode ControlNodeHandler(SystemControlNode node)
        {
            ProfileSet(node);
            return node.children[0];
        }

        DialogNode RandomNodeHandler(RandomNode node)
        {
            return node.children[Random.Range(0, node.children.Count)];
        }
        ///<Summary>
        /// Return the selection the player has made.
        ///</Summary>
        DialogNode OptionsDialogHandler(OptionsDialog node, int selection = -1)
        {
            //call display dialog method
            //call draw options method
            //set selection to player selection
            return node.children[selection];
        }

        DialogNode SimpleDialogHandler(SimpleDialog node)
        {
            //call display dialog method
            return node.children[0];
        }
        
        bool ProfileCheck(SystemConditionalNode node)
        {
            switch (node.Type)
            {
                case "float":
                    if (profileCache.TryGetFloat(node.Key)) return node.Check(profileCache.GetFloat(node.Key));
                    break;
                case "string":
                    if (profileCache.TryGetString(node.Key)) return node.Check(profileCache.GetString(node.Key));
                    break;
                case "bool":
                    if (profileCache.TryGetBool(node.Key)) return node.Check(profileCache.GetBool(node.Key));
                    break;
                default:
                    Debug.Log("Fail to match Conditional Node Type. Returning false.");
                    return false;
            }
            Debug.Log("ConditionCheck Fail! Should have returned in condition type matching.");
            return false;
        }

        void ProfileSet(SystemControlNode node)
        {
            switch (node.Type)
            {
                case "float":
                    if (node.floatOperationType != 4)
                    {
                        if (profileCache.TryGetFloat(node.Key))
                        {
                            switch (node.floatOperationType)
                            {
                                case 0:
                                    profileCache.Set(node.Key, profileCache.GetFloat(node.Key) + node.FloatValue);
                                    break;
                                case 1:
                                    profileCache.Set(node.Key, profileCache.GetFloat(node.Key) - node.FloatValue);
                                    break;
                                case 2:
                                    profileCache.Set(node.Key, profileCache.GetFloat(node.Key) * node.FloatValue);
                                    break;
                                case 3:
                                    profileCache.Set(node.Key, profileCache.GetFloat(node.Key) / node.FloatValue);
                                    break;
                            }
                        }
                        else if (node.floatOperationType == 4)
                        {
                            Debug.Log("DialogSystem: ControlNodeHandler: ProfileSet:\n The designated key does not exist in the profileCache, can't perform the action.");
                        }
                        else
                        {
                            Debug.Log("DialogSystem: ControlNodeHandler: ProfileSet:\n The designated operation index > 4, this should be an error.");
                        }
                    }
                    else profileCache.Set(node.Key, node.FloatValue);
                    break;
                case "string":
                    profileCache.Set(node.Key, node.StringValue);
                    break;
                case "bool":
                    profileCache.Set(node.Key, node.BoolValue);
                    break;
                default:
                    Debug.Log("Failed to match Control Node Type. Returning.");
                    break;
            }
            Debug.Log("Profile Control Fail! Should have returned in condition type matching.");
            return;
        }

        #region Input
        public class InputKeySet
        {
            public KeyCode Confirm;
            public KeyCode Up;
            public KeyCode Down;

            public InputKeySet(KeyCode confirm = KeyCode.F, KeyCode up = KeyCode.W, KeyCode down = KeyCode.S)
            {
                Confirm = confirm;
                Up = up;
                Down = down;
            }
        }

        public InputKeySet inputSet = new InputKeySet();
        #endregion
    }
}
