using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataEditor.CollectionData{
	[System.Serializable]
	public class CollectionDataEntry : ScriptableObject{
		public string itemID;
		public string itemBasicDescription;
		public Sprite itemThumbnail;
		public Sprite itemImage;

		public List<string> tags;

		public static CollectionDataEntry Create(string itemID){
			CollectionDataEntry created = ScriptableObject.CreateInstance<CollectionDataEntry>();
			created.itemID = itemID;
			created.itemBasicDescription = "";
			created.itemThumbnail = null;
			created.itemImage = null;
			return created;
		}
		public static CollectionDataEntry Create(string itemID, string desc){
			CollectionDataEntry created = ScriptableObject.CreateInstance<CollectionDataEntry>();
			created.itemID = itemID;
			created.itemBasicDescription = desc;
			created.itemThumbnail = null;
			created.itemImage = null;
			return created;
		}

		
	}
}