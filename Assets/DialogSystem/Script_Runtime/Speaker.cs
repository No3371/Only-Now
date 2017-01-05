using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using StorySystem.Data;

namespace StorySystem.Runtime
{
    public class Speaker : MonoBehaviour
    {
        public List<Conversation> TriggerDialogs;

        public string ID;
        
        public GameObject dialogBubble;
        public SimpleDialogBubbleController dialogControl;
        public GameObject optionsBubble;
        public OptionsDialogBubbleController optionsControl;

        public void Display(OptionsDialogNode dialog, StorySystemManager.DisplayDoneReceiver receiver)
        {
            RefreshDialog();
            optionsControl.FillOptions(dialog.optionsText.ToArray());
        }

        public void Display (SimpleDialogNode dialog, StorySystemManager.DisplayDoneReceiver receiver)
        {
            RefreshDialog();
            dialogControl.StartPopping(dialog.DialogText, receiver);            
        }
        
        /// <summary>
        /// Called when text popping finished, by PoppingDoneNotifier().
        /// </summary>

        public void RefreshDialog()
        {
            if (optionsBubble != null) optionsControl = optionsBubble.GetComponent<OptionsDialogBubbleController>();
            if (dialogBubble != null) dialogControl = dialogBubble.GetComponent<SimpleDialogBubbleController>();
        }

        public void ScaleByDialogLength()
        {

        }

        void OnEnable()
        {
            StorySystemManager.instance.ActivateSpeaker(this);
        }

        void OnDisable()
        {
            StorySystemManager.instance.DeactivateSpeaker(this);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartConversation(){
            
        }
    }
}
