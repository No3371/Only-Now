using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DialogSystem.Data
{
    [System.Serializable]
    public class DataProfile
    {
        [SerializeField]
        Dict_float dictFloat = new Dict_float();

        [SerializeField]
        Dict_string dictString = new Dict_string();

        [SerializeField]
        Dict_bool dictBool = new Dict_bool();
        
        public void Set(string k, float v)
        {
            if (dictFloat.ContainsKey(k)) dictFloat[k] = v;
            else dictFloat.Add(k, v);
        }

        public void Set(string k, string v)
        {
            if (dictString.ContainsKey(k)) dictString[k] = v;
            else dictString.Add(k, v);
        }

        public void Set(string k, bool v)
        {
            if (dictBool.ContainsKey(k)) dictBool[k] = v;
            else dictBool.Add(k, v);
        }

        public float GetFloat(string k)
        {
            return dictFloat[k];
        }

        public string GetString(string k)
        {
            return dictString[k];
        }

        public bool GetBool(string k)
        {
            return dictBool[k];
        }

        public bool TryGetFloat(string k)
        {
            return dictFloat.ContainsKey(k);
        }

        public bool TryGetString(string k)
        {
            return dictString.ContainsKey(k);
        }

        public bool TryGetBool(string k)
        {
            return dictBool.ContainsKey(k);
        }


    }
}
