using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamCounter : MonoBehaviour {

    public Text redTeamCounter;

    public Text blueTeamCounter;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }
    public void OnJoinedRoom()
    {

        this.UpdateTeams();
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        this.UpdateTeams();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        this.UpdateTeams();
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        this.UpdateTeams();
    }

    public void UpdateTeams()
    {
        redTeamCounter.text = PunTeams.PlayersPerTeam[PunTeams.Team.red].Count.ToString();
        blueTeamCounter.text = PunTeams.PlayersPerTeam[PunTeams.Team.blue].Count.ToString();

    }
}
