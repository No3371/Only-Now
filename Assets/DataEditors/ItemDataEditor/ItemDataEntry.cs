using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemDataEditor.Data{
	[System.Serializable]
	public class ItemDataEntry : ScriptableObject{
		public string itemID;
		public string itemBasicDescription;
		public Sprite itemThumbnail;
		public Sprite itemImage;

		public List<string> tags;

		public static ItemDataEntry Create(string itemID){
			ItemDataEntry created = ScriptableObject.CreateInstance<ItemDataEntry>();
			created.itemID = itemID;
			created.itemBasicDescription = "";
			created.itemThumbnail = null;
			created.itemImage = null;
			return created;
		}
		public static ItemDataEntry Create(string itemID, string desc){
			ItemDataEntry created = ScriptableObject.CreateInstance<ItemDataEntry>();
			created.itemID = itemID;
			created.itemBasicDescription = desc;
			created.itemThumbnail = null;
			created.itemImage = null;
			return created;
		}

		
	}
}