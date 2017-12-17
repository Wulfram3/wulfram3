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

			void Start() {
				gameManager = FindObjectOfType<GameManager>();
			}


        public void Madoka(Action<string> output, string[] args)
        {
            output.Invoke("no");
        
        }

        public void Print(Action<string> output, string[] args)
        {
            output.Invoke("Printing all arguments:");
            foreach (var argument in args)
            {
                output.Invoke(argument);
            }
        }

        public void ColorHelloWorld(Action<string> output, string[] args)
        {
				
				transform.DetachChildren ();
				Destroy (gameObject);
				PhotonNetwork.Destroy (this.gameObject);
            	output.Invoke("Spawning Scout".AddColor(Color.red));

				if (FindObjectOfType<RepairPad>().transform.position != null){
					GameObject go = PhotonNetwork.Instantiate(this.playerPrefab.name, FindObjectOfType<RepairPad>().transform.position + new Vector3(0,5,0) , Quaternion.identity, 0);
				}

        }
    }
}
}