using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Classes
{


    //    userName: String,
    //password:String,
    //email: String,
    //firstName: {
    //    type: String,
    //    default: ''
    //},
    //lastName: {
    //    type: String,
    //    default: ''
    //},
    //minutesPlayed: {
    //    type: Number,
    //    default: 0
    //},
    //level: {
    //    type: Number,
    //    default: 0
    //},
    //createdDate: {
    //    type: Date,
    //    default: Date.now
    //},
    //score: {
    //    type: Number,
    //    default: 0
    //},


    public class WulframPlayer 
    {
        public string _id { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public int minutesPlayed { get; set; }

        public int level { get; set; }

        public DateTime createdDate { get; set; }

        public PlayerScores scores { get; set; }

        public string type { get; set; }
    }

    //scores: {
    //    tankDeaths: {
    //        type: Number,
    //        default: 0
    //    },
    //    scoutDeaths: {
    //        type: Number,
    //        default: 0
    //    },
    //    tankKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    scoutKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    powercellKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    repairpadKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    refullpadKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    flakturretKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    gunturretKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    misslelauncherKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    skypumpKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    darklightKills: {
    //        type: Number,
    //        default: 0
    //    },
    //    unitsHealed: {
    //        type: Number,
    //        default: 0
    //    },
    //    powercellDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    repairpadDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    refullpadDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    flakturretDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    gunturretDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    misslelauncherDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    skypumpDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    //    darklightDeployed: {
    //        type: Number,
    //        default: 0
    //    },
    public class PlayerScores
    {
        public int tankDeaths { get; set; }

        public int scoutDeaths { get; set; }

        public int tankKills { get; set; }

        public int scoutKills { get; set; }

        public int powercellKills { get; set; }

        public int repairpadKills { get; set; }

        public int refullpadKills { get; set; }

        public int flakturretKills { get; set; }

        public int gunturretKills { get; set; }

        public int misslelauncherKills { get; set; }

        public int skypumpKills { get; set; }

        public int darklightKills { get; set; }

        public int unitsHealed { get; set; }

        public int powercellDeployed { get; set; }

        public int repairpadDeployed { get; set; }

        public int refullpadDeployed { get; set; }

        public int flakturretDeployed { get; set; }

        public int gunturretDeployed { get; set; }

        public int misslelauncherDeployed { get; set; }

        public int skypumpDeployed { get; set; }

        public int darklightDeployed { get; set; }

        public int cargoKills { get;  set; }
    }
}