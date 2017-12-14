using Assets.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Interfaces
{
    public interface IUserController
    {

        void UpdateUserData();

        WulframPlayer GetWulframPlayerData();

    }
}