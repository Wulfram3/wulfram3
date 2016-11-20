using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using System;

public class HostGame : MonoBehaviour {
    List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
    bool matchCreated;
    NetworkMatch networkMatch;

    void Awake() {
        networkMatch = gameObject.AddComponent<NetworkMatch>();
    }

    void OnGUI() {
        // You would normally not join a match you created yourself but this is possible here for demonstration purposes.
        if (GUILayout.Button("Create Room")) {
            networkMatch.CreateMatch("NewRoom", 4, true, "", "", "", 0, 0, OnMatchCreate);
        }

        if (GUILayout.Button("List rooms")) {
            networkMatch.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        }

        if (matchList.Count > 0)
        {
            GUILayout.Label("Current rooms");
        }
        foreach (var match in matchList)
        {
            if (GUILayout.Button(match.name))
            {
                networkMatch.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
            }
        }
    }

    private void OnMatchCreate(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");
            matchCreated = true;
            Utility.SetAccessTokenForNetwork(responseData.networkId, responseData.accessToken);
            NetworkServer.Listen(responseData, 9000);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        if (success && responseData != null)
        {
            networkMatch.JoinMatch(responseData[0].networkId, "", "", "", 0, 0, OnMatchJoined);
        }
    }

    private void OnMatchJoined(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            Debug.Log("Join match succeeded");
            if (matchCreated)
            {
                Debug.LogWarning("Match already set up, aborting...");
                return;
            }
            Utility.SetAccessTokenForNetwork(responseData.networkId, responseData.accessToken);
            NetworkClient myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.Connect(responseData);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    //public void OnMatchCreate(CreateMatchResponse matchResponse) {
    //    if (matchResponse.success) {
    //        Debug.Log("Create match succeeded");
    //        matchCreated = true;
    //        Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
    //        NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
    //    } else {
    //        Debug.LogError("Create match failed");
    //    }
    //}

    //public void OnMatchList(ListMatchResponse matchListResponse) {
    //    if (matchListResponse.success && matchListResponse.matches != null) {
    //        networkMatch.JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);
    //    }
    //}

    //public void OnMatchJoined(JoinMatchResponse matchJoin) {
    //    if (matchJoin.success) {
    //        Debug.Log("Join match succeeded");
    //        if (matchCreated) {
    //            Debug.LogWarning("Match already set up, aborting...");
    //            return;
    //        }
    //        Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
    //        NetworkClient myClient = new NetworkClient();
    //        myClient.RegisterHandler(MsgType.Connect, OnConnected);
    //        myClient.Connect(new MatchInfo(matchJoin));
    //    } else {
    //        Debug.LogError("Join match failed");
    //    }
    //}

    public void OnConnected(NetworkMessage msg) {
        Debug.Log("Connected!");
    }
}
