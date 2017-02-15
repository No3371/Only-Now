using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataEditor.InfoLogData{

	[System.Serializable]
	public class InfoLogEntry : ScriptableObject{

		public enum EntryType{
			Article,
			Gallery,
			Comment
		}

		public EntryType entryType;
		public int index;
		public int ID;
		public string briefDescription;
		public string contentDescription;
		public bool hasAttachments;
		public List<Sprite> attachedImages;
		public List<string> imageCaptions;

		public static InfoLogEntry Create(){
			InfoLogEntry created = ScriptableObject.CreateInstance<InfoLogEntry>();
			created.index = -1;
			created.entryType = EntryType.Article;
			created.briefDescription = "Some Desc";
			created.attachedImages = new List<Sprite>();
			created.ID = created.GetInstanceID();
			created.imageCaptions = new List<string>();
			return created;
		}				
	}
}