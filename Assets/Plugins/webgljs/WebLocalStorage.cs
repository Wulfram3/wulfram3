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
<<<<<<< HEAD
=======

>>>>>>> ef5921745e1ba675bdf67d3d3d48c3d26a1c7cd1
        [DllImport("__Internal")]
        public static extern string GetValue(string key);

        [DllImport("__Internal")]
        public static extern void SetValue(string key, string value);
    }
}