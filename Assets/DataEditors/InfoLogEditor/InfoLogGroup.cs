using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataEditor.InfoLogData{


	[System.Serializable]
	public class Dict_InfoLogEntry : SerializableDictionary<string, InfoLogEntry>{}

	[System.Serializable]
	public class InfoLogGroup : ScriptableObject{
		
		public List<InfoLogEntry> entries;

		public string groupTag;

		public string additionalDescription;

		public static InfoLogGroup Create(string tag = ""){
			InfoLogGroup created = ScriptableObject.CreateInstance<InfoLogGroup>();
			created.groupTag = tag;
			created.additionalDescription = "";
			created.entries = new List<InfoLogEntry>();
			return created;
		}				
	}
}