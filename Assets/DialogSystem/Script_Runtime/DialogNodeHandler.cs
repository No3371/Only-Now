using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorySystem.Runtime;
using StorySystem.Data;

namespace StorySystem.Runtime.DialogNodeHandlers{
	public class DialogNodeHandler {

		StorySystemManager storySystemInstance;

		public void Handle (BaseDialogNode node, StorySystemManager system, ConversationResolver resolver){
            if (node is ConversationStartNode) 
				ConversationStartNodeHandler(node, system, resolver);
            else if (node is ConversationEndNode) 
				ConversationEndNodeHandler(node, system, resolver);
            else if (node is OptionsDialogNode) 
				OptionsDialogNodeHandler(node as OptionsDialogNode);
            else if (node is SimpleDialogNode) 
				SimpleDialogHandler(node as SimpleDialogNode);
            else if (node is RandomOutputNode) 
				RandomOutputNodeHandler(node as RandomOutputNode);
            
		}

		public void ConversationStartNodeHandler (BaseDialogNode node, StorySystemManager system, ConversationResolver resolver){
			
		}
		public void ConversationEndNodeHandler (BaseDialogNode node, StorySystemManager system, ConversationResolver resolver){
			
		}

		public void SimpleDialogNodeHandler (BaseDialogNode node, StorySystemManager system, ConversationResolver resolver){

		}        
		
		BaseDialogNode ConversationJumpNodeHandler(ConversationJumpNode node, StorySystemManager system, ConversationResolver resolver)
        {
            if (storySystemInstance.allConvCaches.ContainsKey(node.targetConvID)) resolver.availableJump = storySystemInstance.allConvCaches[node.targetConvID];
            else Debug.Log("DialogSystem:: TreeJumpNodeHandler:: Can't get any tree with the key: " + node.targetConvID);
            return null;
        }

        BaseDialogNode RandomOutputNodeHandler(RandomOutputNode node)
        {
            return node.children[Random.Range(0, node.children.Count)];
        }
        ///<Summary>
        /// Return the selection the player has made.
        ///</Summary>
        BaseDialogNode OptionsDialogNodeHandler(OptionsDialogNode node, int selection = -1)
        {
            //call display dialog method
            //call draw options method
            //set selection to player selection
            return node.children[selection];
        }

        BaseDialogNode SimpleDialogHandler(SimpleDialogNode node)
        {
            //call display dialog method
            return node.children[0];
        }





	}
}