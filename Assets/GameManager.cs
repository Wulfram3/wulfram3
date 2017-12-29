using Assets.InternalApis;
using Assets.InternalApis.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Wulfram3
{
    public class GameManager : Photon.PunBehaviour
    {
        public GameObject pulseShellPrefab;

        public GameObject flakShellPrefab;

        public GameObject explosionPrefab;

        public GameObject hullBar;

        public GameObject fuelBar;

        public GameObject cargoPrefab;

        public Material redcolor;
        public Material bluecolor;

        public GameObject playerInfoPanelPrefab;
        public Transform[] spawnPointsBlue;
        public Transform[] spawnPointsRed;
     


        [HideInInspector]
        public Camera normalCamera;

        //[HideInInspector]
        public Camera overheadCamera;

        private TargetInfoController targetChangeListener;


        #region Photon Messages




        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            var userControler = DepenencyInjector.Resolve<IUserController>();
            var discordApi = DepenencyInjector.Resolve<IDiscordApi>();
            PhotonNetwork.LeaveRoom();
            StartCoroutine(discordApi.PlayerLeft(userControler.GetWulframPlayerData().userName));
        }

        public void Start()
        {
            Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);

            if (PlayerMovementManager.LocalPlayerInstance == null) {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate


                //team start
                PunTeams.UpdateTeamsNow();

                int redPlayers = PunTeams.PlayersPerTeam[PunTeams.Team.red].Count;
                int bluePlayers = PunTeams.PlayersPerTeam[PunTeams.Team.blue].Count;

                Debug.Log("Number of Red players: " + redPlayers);
                Debug.Log("Number of Blue players: " + bluePlayers);
                if (bluePlayers > redPlayers) {
                    Debug.Log("Spawn red tank");
                    Transform selectedSpawnPoint = spawnPointsRed[0];
                    GameObject player = PhotonNetwork.Instantiate("RedTank", selectedSpawnPoint.position, selectedSpawnPoint.rotation, 0);
                    PhotonNetwork.player.SetTeam(PunTeams.Team.red);
                } else {
                    Debug.Log("Spawn blue tank");
                    Transform selectedSpawnPoint = spawnPointsBlue[0];
                    GameObject player = PhotonNetwork.Instantiate("PlayerTank", selectedSpawnPoint.position, selectedSpawnPoint.rotation, 0);
                    PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
                }
            } else {
                Debug.Log("Ignoring scene load for " + Application.loadedLevelName);
            }
            normalCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            overheadCamera = GameObject.FindGameObjectWithTag("OverheadCamera").GetComponent<Camera>();
            overheadCamera.enabled = false; //set disabled so that it does't render in the background
        }

        [PunRPC]
        public void SpawnPulseShell(Vector3 pos, Quaternion rotation)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Instantiate(pulseShellPrefab.name, pos, rotation, 0);
            }
        }
        public void SpawnFlakShell(Vector3 pos, Quaternion rotation)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Instantiate(flakShellPrefab.name, pos, rotation, 0);
            }
        }

        public void SpawnExplosion(Vector3 pos)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Instantiate(explosionPrefab.name, pos, Quaternion.identity, 0);
            }
        }

        //laser stuff autocannon
        /*public void DrawLine(Vector3 startPos, Vector3 endPos)
		{ 
			if (PhotonNetwork.isMasterClient) {
				//PhotonNetwork.Instantiate(lineRender, startPos, Quaternion.identity, 0);

			}

		}*/


        public void UnitsHealthUpdated(HitPointsManager hitpointsManager)
        {
            if (hitpointsManager.tag.Equals("Player") && hitpointsManager.photonView.isMine)
            {
                SetHullBar((float)hitpointsManager.health / (float)hitpointsManager.maxHealth);
            }
            if (PhotonNetwork.isMasterClient && hitpointsManager.health <= 0 && !hitpointsManager.tag.Equals("Player"))
            {
                PhotonNetwork.Destroy(hitpointsManager.gameObject);
                SpawnExplosion(hitpointsManager.transform.position);
            }
        }

        public void SetHullBar(float level)
        {
            hullBar.GetComponent<LevelController>().SetLevel(level);
        }

        public void FuelLevelUpdated(FuelManager fuelManager) {
            SetFuelBar((float) fuelManager.fuel / (float) fuelManager.maxFuel);
        }

        public void SetFuelBar(float level) {
            fuelBar.GetComponent<LevelController>().SetLevel(level);
        }

        public void SetCurrentTarget(GameObject go)
        {
            if (targetChangeListener != null)
            {
                targetChangeListener.TargetChanged(go);
            }
        }

        public void AddTargetChangeListener(TargetInfoController tic)
        {
            targetChangeListener = tic;
        }

        public void DestroyNow(GameObject go)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Destroy(go);
                SpawnExplosion(go.transform.position);
            }
        }

        public void Respawn(PlayerMovementManager player)
        {


            Vector3 spawnPos = new Vector3(0f, 5f, 0f);
            Quaternion spawnRotation = Quaternion.identity;

            if (FindObjectOfType<RepairPad>() != null)
            {
                // TODO: Player will have to hope that all repair pads don't die by the time they click.
                overheadCamera.enabled = true;
                normalCamera.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                RepairPad.Spawn(this, player, Vector3.zero);
            }

        }

        [PunRPC]
        public void RequestPickUpCargo(CargoManager cargoManager) {
            if (PhotonNetwork.isMasterClient) {
                if (cargoManager.pickedUpCargo != "") {
                    return;
                }
                Cargo cargo = FindCargoInRange(cargoManager.transform.position, 5f);
                if (cargo != null) {
                    cargoManager.photonView.RPC("SetPickedUpCargo", PhotonTargets.All, cargo.content);
                    if (cargo.GetComponentInParent<PlayerMovementManager>() != null) {
                    }
                    PhotonNetwork.Destroy(cargo.gameObject);
                }
            }
        }

        private Cargo FindCargoInRange(Vector3 position, float scanRadius) {
            Transform closestTarget = null;
            float minDistance = scanRadius + 10f;
            var cols = Physics.OverlapSphere(position, scanRadius);
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols) {
                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody) && col.attachedRigidbody.GetComponentInParent<Cargo>() != null) {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }

            foreach (Rigidbody rb in rigidbodies) {
                Transform target = rb.transform;

                float distance = Vector3.Distance(position, target.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            if (closestTarget == null) {
                return null;
            }
            return closestTarget.GetComponentInParent<Cargo>();
        }

        public void PickUpCargo(CargoManager cargoManager) {
            photonView.RPC("RequestPickUpCargo", PhotonTargets.MasterClient, cargoManager);
        }

        [PunRPC]
        public void RequestDropCargo(CargoManager cargoManager) {
            if (PhotonNetwork.isMasterClient) {
                string pickedUpCargo = cargoManager.pickedUpCargo;
                if (pickedUpCargo == "") {
                    return;
                }
                cargoManager.photonView.RPC("SetPickedUpCargo", PhotonTargets.All, "");
                GameObject go = PhotonNetwork.Instantiate(cargoPrefab.name, cargoManager.transform.position, Quaternion.identity, 0);
                go.GetComponent<Cargo>().photonView.RPC("SetContent", PhotonTargets.All, pickedUpCargo);
            }
        }

        public void DropCargo(CargoManager cargoManager) {
            photonView.RPC("RequestDropCargo", PhotonTargets.MasterClient, cargoManager);
        }

        #endregion

        #region Private Methods

        public override void OnJoinedRoom()
        {


            Debug.Log("Sent Post!' ");
            // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.room.PlayerCount == 1)
            {
                Debug.Log("We load the 'Playground' ");

                // #Critical
                // Load the Room Level. 
                PhotonNetwork.LoadLevel("Playground");

            }
        }

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("Playground");
        }


        #endregion
    }
}