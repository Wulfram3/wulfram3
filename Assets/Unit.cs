using Assets.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class Unit : MonoBehaviour {

        public string team;

        public string name;

        public PunTeams.Team unitTeam;

        public UnitType unitType;

        // Use this for initialization
        void Start() {
            switch (team.ToLower())
            {
                case "blue":
                    unitTeam = PunTeams.Team.blue;
                    break;
                case "red":
                    unitTeam = PunTeams.Team.red;
                    break;
                default:
                    unitTeam = PunTeams.Team.none;
                    break;
            }

            switch (name.ToLower())
            {
                case "tank":
                    unitType = UnitType.Tank;
                    break;
                case "scout":
                    unitType = UnitType.Scout;
                    break;
                default:
                    unitType = UnitType.None;
                    break;
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
