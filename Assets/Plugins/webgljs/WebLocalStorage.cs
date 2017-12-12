using Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Assets.Plugins.webgljs
{
    public class WebLocalStorage : MonoBehaviour
    {
        [DllImport("__Internal")]
        public static extern string GetValue(string key);


        [DllImport("__Internal")]
        public static extern void SetValue(string key, string value);
    }
}