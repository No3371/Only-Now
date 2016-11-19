using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using DialogSystem.Data;

namespace DialogSystem.Runtime
{
    public class Speaker : MonoBehaviour
    {
        public List<DialogTree> TriggerDialogs;

        public string ID;
        
        public GameObject dialogBubble;
        public SimpleBubbleController dialogControl;
        public GameObject optionsBubble;
        public OptionsBubbleController optionsControl;

        float GetDuration(int length)
        {
            return length * DialogSystem.textDelayGlobalFactor * DialogSystem.textDelay;
        }

        public void Display(OptionsDialog dialog, DialogSystem.DisplayDoneReceiver receiver)
        {
            RefreshDialog();
            optionsControl.FillOptions(dialog.optionsText.ToArray());
        }

        public void Display (SimpleDialog dialog, DialogSystem.DisplayDoneReceiver receiver)
        {
            RefreshDialog();
            dialogControl.StartPopping(dialog.Text, receiver);            
        }
        
        /// <summary>
        /// Called when text popping finished, by PoppingDoneNotifier().
        /// </summary>

        public void RefreshDialog()
        {
            if (optionsBubble != null) optionsControl = optionsBubble.GetComponent<OptionsBubbleController>();
            if (dialogBubble != null) dialogControl = dialogBubble.GetComponent<SimpleBubbleController>();
        }

        public void ScaleByDialogLength()
        {

        }

        void OnEnable()
        {
            DialogSystem.instance.ActivateSpeaker(this);
        }

        void OnDisable()
        {
            DialogSystem.instance.DeactivateSpeaker(this);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
