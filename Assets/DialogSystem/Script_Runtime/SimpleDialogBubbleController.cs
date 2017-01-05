using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace StorySystem.Runtime
{
    public class SimpleDialogBubbleController : MonoBehaviour
    {

        Text textComp;

        // Use this for initialization
        void OnEnable()
        {
            textComp = this.GetComponent<Text>();
        }

        void OnDisable()
        {
        }

        IEnumerator pop;
        string str;
        StorySystemManager.DisplayDoneReceiver receiver;

        public void StartPopping(string text, StorySystemManager.DisplayDoneReceiver receiver)
        {
            this.str = text;
            pop = PopText(str);
            StartCoroutine(pop);
            this.receiver = receiver;
        }

        IEnumerator PopText(string str)
        {
            for (int i = 1; i < str.Length; i++)
            {
                this.textComp.text = str.Substring(0, i);
                //TODO: fix this 
                //yield return new WaitForSeconds(StorySystemManager.textDelay);
            }
            yield break;
        }

        IEnumerator BubbleFading(float delay, float duration)
        {
            yield return new WaitForSeconds(delay);
            for (int i = 0; i < duration/0.01f; i++)
            {
                this.textComp.color = new Color(this.textComp.color.r, this.textComp.color.g, this.textComp.color.b, this.textComp.color.a - 1/(duration*100));
                yield return new WaitForSeconds(0.01f);
            }
            BeforeDeactivate();
            this.gameObject.SetActive(false);
            yield break;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(StorySystemManager.instance.inputSet.Confirm)) InteractHandler();
        }

        /// <summary>
        /// Skip text popping, instantly display whole sentence. 
        /// </summary>
        void InteractHandler()
        {
            StopCoroutine(pop);
            this.textComp.text = str;
            receiver.done = true;
        }

        /// <summary>
        /// Things to do before deactiving the dialog.
        /// </summary>
        void BeforeDeactivate()
        {

        }
    }
}