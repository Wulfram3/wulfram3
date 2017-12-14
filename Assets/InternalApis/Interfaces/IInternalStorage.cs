using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Interfaces
{
    public interface IInternalStorage
    {
        string GetValue(string key);

        void SetValue(string key, string data);
    }
}
 