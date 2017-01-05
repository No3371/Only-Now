using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StorySystem.Runtime{

	///Attach the script to player gameobject so it can talk to Speakers.
	public class ConversationActivator : MonoBehaviour {

		///Radius to look for a speaker around the activator.
		public static float talkRadius = 2f;

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void Update()
		{
			if (Input.GetAxis("Interact") > 0)
				SearchForSpeaker();
		}

		void SearchForSpeaker(){
			Collider2D[] candidates = Physics2D.OverlapCircleAll(this.transform.position, talkRadius);
			for (int i = 0; i < candidates.Length; i++){
				if (candidates[i].GetComponent<Speaker>())
					candidates[i].GetComponent<Speaker>().StartConversation();
			}
		}
	}
}