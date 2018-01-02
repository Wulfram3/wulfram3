using System;
using AvalonAssets.Unity;
using UnityEngine;

// ReSharper disable CheckNamespace
namespace Com.Wulfram3 {
namespace AvalonAssets.Example
{
		public class ExampleCommand : Photon.PunBehaviour
    {
		private GameManager gameManager;
		public GameObject playerPrefab;
		public GameObject crimsonTank;

			void Start() {
				gameManager = FindObjectOfType<GameManager>();
			}


        public void Madoka(Action<string> output, string[] args)
        {
            output.Invoke("no");
        
        }

        public void Print(Action<string> output, string[] args)
        {
				PlayerMovementManager player = PlayerMovementManager.LocalPlayerInstance.GetComponent<PlayerMovementManager>();
				player.gameObject.GetComponent<CameraManager> ().Detach ();
				PhotonNetwork.Destroy (player.gameObject);
				output.Invoke("Spawning Red Tank".AddColor(Color.red));

				if (FindObjectOfType<RepairPad>().transform.position != null){
					GameObject go = PhotonNetwork.Instantiate(this.crimsonTank.name, FindObjectOfType<RepairPad>().transform.position + new Vector3(0,5,0) , Quaternion.identity, 0);
				}
        }

        public void ColorHelloWorld(Action<string> output, string[] args)
        {
				
				PlayerMovementManager player = PlayerMovementManager.LocalPlayerInstance.GetComponent<PlayerMovementManager>();
				player.gameObject.GetComponent<CameraManager> ().Detach ();
				PhotonNetwork.Destroy (player.gameObject);
            	output.Invoke("Spawning Red Scout".AddColor(Color.red));

				if (FindObjectOfType<RepairPad>().transform.position != null){
					GameObject go = PhotonNetwork.Instantiate(this.playerPrefab.name, FindObjectOfType<RepairPad>().transform.position + new Vector3(0,5,0) , Quaternion.identity, 0);
				}

        }
    }
}
}