using Assets.InternalApis.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Interfaces
{
    public interface IUserController
    {
        event Action<WulframPlayer, string> LoginCompleted;

        event Action<string> RegisterUserCompleted;

        void UpdateUserData();

        void RecordUnitKill(UnitType type);

        void RecordUnitDeploy(UnitType type);

        void RecordPlayerDeath(UnitType type);

        WulframPlayer GetWulframPlayerData();

        void LoginUser(string username, string password);

        void RegisterUser(string username, string password, string email);

    }
}