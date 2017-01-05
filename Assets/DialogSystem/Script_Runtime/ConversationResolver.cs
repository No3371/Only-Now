using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorySystem.Runtime.ConversationStates;
using StorySystem.Data;

namespace StorySystem.Runtime{
	///Instantiated by DialogSystem to resolve and play a conversation.
	///Every resolver and player can only process 1 conversation at a time.
	public class ConversationResolver : MonoBehaviour {

		static Queue<ConversationResolver> resolverPool;

		public static ConversationResolver GetInstanceFromPool(){
			if (resolverPool == null) resolverPool = new Queue<ConversationResolver>();
			return resolverPool.Dequeue();
		}

		StorySystemManager storySystemInstance;

		bool CheckInit(){			

			return true;
		}

		

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		void OnEnable()
		{
			state = new sInitializing();
			
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			state.Handle();
		}

#region ConversationControl
		///Only MainConversations can control DialogSystem settings, and ones will interrupt all other conversations.
		bool isMainConversation = false;
		public Queue<Conversation> convQueue;
		public Conversation availableJump;
		///Add a conversation to the queue of the resolver.
        public void ResolveConversation (Conversation conv)
        {
            convQueue.Enqueue(conv);
        }

		///Add some conversations to the queue of the resolver.
        public void ResolveConversations (Conversation[] convs)
        {
            foreach(Conversation conv in convs) convQueue.Enqueue(conv);
        }

		public void OnNodeHandled(){
			currentNodeCache = nodeOutputCache;
			nodeOutputCache = null;
			currentNodeCache.Handle(storySystemInstance, this);
		}
		
		public void SetConvToJumpNext(Conversation conv){
			if (conv == null) availableJump = null;
			else availableJump = conv;
		}

#endregion



#region DialogPlayer

		Speaker speaker;

		public BaseDialogNode currentNodeCache, nodeOutputCache;

		GameObject bubblePrefab;

		IEnumerator WaitForBeforeNextDialog(float seconds){
		yield return new WaitForSeconds(seconds);
		}


#endregion



#region StateManagement

		ConversationState state;

		public void ChangeState (ConversationState state){
			this.state = state;
		}

#endregion
	}

}
