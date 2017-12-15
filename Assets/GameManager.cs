using Assets.InternalApis;
using Assets.InternalApis.Interfaces;
using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


namespace Com.Wulfram3 {
    public class GameManager : Photon.PunBehaviour {

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public GameObject pulseShellPrefab;

		public GameObject flakShellPrefab;

        public GameObject explosionPrefab;

        public GameObject hullBar;

        public GameObject playerInfoPanelPrefab;

        [HideInInspector]
        public Camera normalCamera;

        [HideInInspector]
        public Camera overheadCamera;

        private TargetInfoController targetChangeListener;

        #region Photon Messages


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer other) {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient) {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other) {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient) {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        #endregion


        #region Public Methods


        public void LeaveRoom() {
            var userControler = DepenencyInjector.Resolve<IUserController>();
            var discordApi = DepenencyInjector.Resolve<IDiscordApi>();
            PhotonNetwork.LeaveRoom();
            StartCoroutine(discordApi.PlayerLeft(userControler.GetWulframPlayerData().Username));
        }

        public void Start() {
            if (playerPrefab == null) {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            } else {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                if (PlayerMovementManager.LocalPlayerInstance == null) {
                    Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
					if (FindObjectOfType<RepairPad>().transform.position != null){
						GameObject go = PhotonNetwork.Instantiate(this.playerPrefab.name, FindObjectOfType<RepairPad>().transform.position + new Vector3(0,5,0) , Quaternion.identity, 0);
					}
                } else {
                    Debug.Log("Ignoring scene load for " + Application.loadedLevelName);
                }
                normalCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                overheadCamera = GameObject.FindGameObjectWithTag("OverheadCamera").GetComponent<Camera>();


            }
        }

        [PunRPC]
        public void SpawnPulseShell(Vector3 pos, Quaternion rotation) {
            if (PhotonNetwork.isMasterClient) {
                PhotonNetwork.Instantiate(pulseShellPrefab.name, pos, rotation, 0);
            }
        }
		public void SpawnFlakShell(Vector3 pos, Quaternion rotation) {
			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.Instantiate(flakShellPrefab.name, pos, rotation, 0);
			}
		}

        public void SpawnExplosion(Vector3 pos) {
            if (PhotonNetwork.isMasterClient) {
                PhotonNetwork.Instantiate(explosionPrefab.name, pos, Quaternion.identity, 0);
            }
        }

        public void UnitsHealthUpdated(HitPointsManager hitpointsManager) {
            if (hitpointsManager.tag.Equals("Player") && hitpointsManager.photonView.isMine) {
                SetHullBar((float)hitpointsManager.health / (float)hitpointsManager.maxHealth);
            }
            if (PhotonNetwork.isMasterClient && hitpointsManager.health <= 0 && !hitpointsManager.tag.Equals("Player")) {
                PhotonNetwork.Destroy(hitpointsManager.gameObject);
                SpawnExplosion(hitpointsManager.transform.position);
            }
        }

        public void SetHullBar(float level) {
            hullBar.GetComponent<LevelController>().SetLevel(level);
        }

        public void SetCurrentTarget(GameObject go) {
            if (targetChangeListener != null) {
                targetChangeListener.TargetChanged(go);
            }
        }

        public void AddTargetChangeListener(TargetInfoController tic) {
            targetChangeListener = tic;
        }

        public void DestroyNow(GameObject go) {
            if (PhotonNetwork.isMasterClient) {
                PhotonNetwork.Destroy(go);
                SpawnExplosion(go.transform.position);
            }
        }

        public void Respawn(PlayerMovementManager player) {


            Vector3 spawnPos = new Vector3(0f, 5f, 0f);
            Quaternion spawnRotation = Quaternion.identity;

            if (FindObjectOfType<RepairPad>() != null)
            {
                // TODO: Player will have to hope that all repair pads don't die by the time they click.
                overheadCamera.enabled = true;
                normalCamera.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                RepairPad.Spawn(this, player, Vector3.zero);
            }

        }


        #endregion

        #region Private Methods


        void LoadArena() {
            if (!PhotonNetwork.isMasterClient) {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("Playground");
        }


        #endregion
    }
}