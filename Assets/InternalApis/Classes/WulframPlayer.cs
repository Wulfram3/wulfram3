using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.InternalApis.Classes
{

////    UserName: String,
////    email: String,
////    firstName: String,
////    lastName: String,
////    minutesPlayed: Number,
////    level: Number,
////    minutesPlayed: Number,
////    createdDate: {
////        type: Date,
////        default: Date.now
////},
////    score: {
////        type: Number,
////        default: [0]
////    },
////    scores: {
////        tankDeaths: Number,
////        scoutDeaths: Number,
////        tankKills: Number,
////        scoutKills: Number,
////        powercellKills: Number,
////        repairpadKills: Number,
////        refullpadKills: Number,
////        flakturretKills: Number,
////        gunturretKills: Number,
////        misslelauncherKills: Number,
////        skypumpKills: Number,
////        darklightKills: Number,
////        unitsHealed: Number,
////        powercellDeployed: Number,
////        repairpadDeployed: Number,
////        refullpadDeployed: Number,
////        flakturretDeployed: Number,
////        gunturretDeployed: Number,
////        misslelauncherDeployed: Number,
////        skypumpDeployed: Number,
////        darklightDeployed: Number,
////    }

    public class WulframPlayer 
    {
        public int id { get; set; }

        public string Username { get; set; }

        public string email { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public int minutesPlayed { get; set; }

        public int level { get; set; }

        public DateTime createdDate { get; set; }

        public decimal score { get; set; }

        public PlayerScore scores { get; set; }

    }

    public class PlayerScore
    {
        public decimal tankDeaths { get; set; }

        public decimal scoutDeaths { get; set; }

        public decimal tankKills { get; set; }

        public decimal scoutKills { get; set; }

        public decimal powercellKills { get; set; }

        public decimal repairpadKills { get; set; }

        public decimal refullpadKills { get; set; }

        public decimal flakturretKills { get; set; }

        public decimal gunturretKills { get; set; }

        public decimal misslelauncherKills { get; set; }

        public decimal skypumpKills { get; set; }

        public decimal darklightKills { get; set; }

        public decimal unitsHealed { get; set; }

        public decimal powercellDeployed { get; set; }

        public decimal repairpadDeployed { get; set; }

        public decimal refullpadDeployed { get; set; }

        public decimal flakturretDeployed { get; set; }

        public decimal gunturretDeployed { get; set; }

        public decimal misslelauncherDeployed { get; set; }

        public decimal skypumpDeployed { get; set; }

        public decimal darklightDeployed { get; set; }
    }
}