using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


namespace Com.Wulfram3 {
    public class GameManager : Photon.PunBehaviour {

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public GameObject pulseShellPrefab;

        public GameObject explosionPrefab;

        public GameObject hullBar;

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
            PhotonNetwork.LeaveRoom();
        }

        public void Start() {
            if (playerPrefab == null) {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            } else {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                if (PlayerMovementManager.LocalPlayerInstance == null) {
                    Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                } else {
                    Debug.Log("Ignoring scene load for " + Application.loadedLevelName);
                }
            }
        }

        [PunRPC]
        public void SpawnPulseShell(Vector3 pos, Quaternion rotation) {
            if (PhotonNetwork.isMasterClient) {
                PhotonNetwork.Instantiate(pulseShellPrefab.name, pos, rotation, 0);
            }
        }

        public void SpawnExplosion(Vector3 pos) {
            if (PhotonNetwork.isMasterClient) {
                PhotonNetwork.Instantiate(explosionPrefab.name, pos, Quaternion.identity, 0);
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