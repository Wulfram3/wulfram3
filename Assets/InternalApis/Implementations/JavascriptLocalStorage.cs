using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Plugins.webgljs;
using Assets.InternalApis.Interfaces;

namespace Assets.InternalApis.Implementations
{
    public class JavascriptLocalStorage : IInternalStorage
    {
        public string GetValue(string key)
        {
            try
            {
                return WebLocalStorage.GetValue(key);
            }
            catch (System.Exception ex)
            {

                return "null";
            }
            
        }

        public void SetValue(string key, string data)
        {
            try
            {
                WebLocalStorage.SetValue(key, data);
            }
            catch (System.Exception ex)
            {
            }
            
        }
    }
}