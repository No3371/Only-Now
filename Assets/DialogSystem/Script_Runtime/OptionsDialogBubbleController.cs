using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace StorySystem.Runtime
{

    public class OptionsDialogBubbleController : MonoBehaviour
    {

        bool controlling;
        bool Controlling { get { return controlling; } set { controlling = value; } }

        int currentIndex = 0;
        int currentAmount = 0;

        //Assigned in Inspector
        public List<GameObject> optionsPool;

        StorySystemManager.DisplayDoneReceiver receiver;

        // Use this for initialization
        void OnEnable()
        {
            controlling = true;
        }

        void OnDisable()
        {
            controlling = false;
        }


        // Update is called once per frame
        void Update()
        {
            if (!controlling) return;

            if (Input.GetKeyDown(StorySystemManager.instance.inputSet.Up)) PreviousSelection();
            else if (Input.GetKeyDown(StorySystemManager.instance.inputSet.Down)) NextSelection();
            else if (Input.GetKeyDown(StorySystemManager.instance.inputSet.Confirm)) InteractHandler();
        }

        void InteractHandler()
        {
            receiver.optionsIndex = currentIndex;
            receiver.done = true;
            BeforeDeactivate();
        }

        /// <summary>
        /// Things to do after the player make the decision, before deactiving the dialog.
        /// </summary>
        void BeforeDeactivate()
        {
            foreach (GameObject g in optionsPool)
            {
                g.SetActive(false);
                currentAmount = 0;
            }
        }

        public void FillOptions(params string[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                optionsPool[i].GetComponentInChildren<Text>().text = text[i];
                if (i == 0) ToggleHighlight(true, optionsPool[i]);
                else ToggleHighlight(false, optionsPool[i]);
                currentAmount += 1;
                optionsPool[i].SetActive(true);
            }
        }

        void ToggleHighlight(bool toggle, GameObject option)
        {
            if (toggle)
            {
                option.GetComponentInChildren<Text>().color = new Color(255, 255, 255, 255);
                option.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 255);
            }
            else if (!toggle)
            {
                option.GetComponentInChildren<Text>().color = new Color(255, 255, 255, 100);
                option.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 100);
            }

        }

        void NextSelection()
        {
            ToggleHighlight(false, optionsPool[currentIndex]);
            currentIndex += 1;
            currentIndex = Mathf.Clamp(currentIndex, 0, currentAmount);
            ToggleHighlight(true, optionsPool[currentIndex]);
        }

        void PreviousSelection()
        {
            ToggleHighlight(false, optionsPool[currentIndex]);
            currentIndex -= 1;
            currentIndex = Mathf.Clamp(currentIndex, 0, currentAmount);
            ToggleHighlight(true, optionsPool[currentIndex]);
        }
    }
}