using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorySystem.Runtime;
using StorySystem.Data;

namespace StorySystem.Runtime.ConversationStates{

	public abstract class ConversationState{

		public ConversationResolver stateContainer;

		public abstract void Handle();
	
	}

	public class sInitializing : ConversationState{
		public override void Handle(){
			
		}
	}

	public class sStandby : ConversationState{

		public sStandby(ConversationResolver resolver){
			stateContainer = resolver;
		}

		public override void Handle(){
			if (stateContainer.availableJump != null)
			{
				stateContainer.ChangeState(new sLoadingConversation(stateContainer, stateContainer.availableJump));
				stateContainer.SetConvToJumpNext(null);
			}
			else {
				if (stateContainer.convQueue.Count == 0) return;
				else if (stateContainer.convQueue.Count >= 1)
				{
					stateContainer.ChangeState(new sLoadingConversation(stateContainer, stateContainer.convQueue.Dequeue()));
				}
			}
		}
	}

	public class sLoadingConversation : ConversationState{

		Conversation toLoad;

		public sLoadingConversation(ConversationResolver resolver, Conversation conv){
			this.stateContainer = resolver;
			this.toLoad = conv;
		}

		public override void Handle(){
			stateContainer.currentNodeCache = toLoad.startNode;
			stateContainer.ChangeState(new sFillingBubble());
		}
	}

	public class sFillingBubble : ConversationState{

		public sFillingBubble(){

		}

		public override void Handle(){
			
		}
	}

	//TODO: sWaitingForInputOrExpire
	
}