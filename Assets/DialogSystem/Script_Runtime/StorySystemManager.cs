using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using StorySystem.Data;


namespace StorySystem.Runtime
{
    public class StorySystemManager : MonoBehaviour
    {
        static string dialogSystemPath = "Plugins/DialogSystem";

        public static StorySystemManager instance;
        public static List<Speaker> activeSpeakers;
        Speaker playerSpeaker;
        Dictionary<string, Speaker> speakerMap = new Dictionary<string, Speaker>();

        public const float textDelayGlobalFactor = 1.1f;

        public VariableProfileCache variableProfileCache;

        Stack<GameObject> DialogBubblePool = new Stack<GameObject>();
        GameObject OptionsDialogReserve;

        Conversation toJump = null;

        public Dict_Conv allConvCaches = new Dict_Conv();

        void CacheAllConversations()
        {
            Conversation[] temp = Resources.FindObjectsOfTypeAll<Conversation>();
            for (int i = 0; i < temp.Length; i++)
            {
                allConvCaches.Add(temp[i].conversationID, temp[i]);
            }
        }

        // Use this for initialization
        void Start()
        {
            if (instance == null) instance = this;
            if (instance != this) Destroy(this);
            DontDestroyOnLoad(this);
            BubblePooling();
        }

        // Update is called once per frame
        void Update()
        {           
  

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
