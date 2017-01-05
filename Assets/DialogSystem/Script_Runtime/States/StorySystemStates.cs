using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorySystem.Runtime;
using StorySystem.Data;

namespace StorySystem.Runtime.StorySystemStates{

	public abstract class SystemState{

		public StorySystemManager stateContainer;

		public abstract void Handle();
	
	}

	public class sInitializing : SystemState{
		public override void Handle(){
			
		}
	}

	///Accept new request
	public class sIdle : SystemState{
		public override void Handle(){

		}
	}	

	public class sWorking : SystemState{
		public override void Handle(){

		}
	}
}