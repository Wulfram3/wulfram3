using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class PlayerMovementManager : Photon.PunBehaviour {

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public GameObject pulseShellPrefab;
        public AudioClip jumpSource;
        public AudioClip landSource;
        public AudioClip takeoffSource;

        private TerrainCollider terrainCollider;

        public float timeBetweenShots = 3.0f;
        public float timeBetweenJumps = 3.0f;
        public float timestamp;
        public float jumptimestamp;
        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;
        public float minimumX = -360F;
        public float maximumX = 360F;
        public float minimumY = -60F;
        public float maximumY = 60F;
        float rotationX = 0F;
        float rotationY = 0F;
        float lastRotationX = 0F;
        float lastRotationY = 0F;
        Quaternion originalRotation;
        float jumpForce = 700f;
        float height = 0.5f; // tank's level above ground


        private bool isLanded = false;
        private bool requestLand = false;
        private float maxDistanceToLand = 0.3f; //max distance between ground and tank that allows to land

        private bool requestJump = false;

        // Use this for initialization
        void Start() {
            if (!photonView.isMine) {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                return;
            }


            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();

            originalRotation = transform.localRotation;

        }

        private void Awake() {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine) {
                PlayerMovementManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update() {
            if (!photonView.isMine)
                return;



            //Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 30, transform.position.z), Vector3.down);
            //RaycastHit hit;
            //if (terrainCollider.Raycast(ray, out hit, 100.0F)) {
            //    y = hit.point.y;
            //}


            //transform.Translate(x, 0, z);
            //Vector3 pos = transform.position;
            //pos.y = y + 1;
            //transform.position = pos;

            //float mx = Input.GetAxis("Mouse X") * sensitivityX;
            //float my = Input.GetAxis("Mouse Y") * sensitivityY;
            //float deltaX = mx - lastRotationX;
            //float deltaY = my - lastRotationY;
            //lastRotationX = mx;
            //lastRotationY = my;
            //Quaternion xQuaternion = Quaternion.AngleAxis(mx, Vector3.up);
            //Quaternion yQuaternion = Quaternion.AngleAxis(my, -Vector3.right);
            //transform.localRotation = transform.localRotation * xQuaternion * yQuaternion;
            //transform.Rotate(Vector3.up, mx);

            //NB Uncomment to enable mouse look
            if (!isLanded) {
                if (axes == RotationAxes.MouseXAndY) {
                    // Read the mouse input axis
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationX = ClampAngle(rotationX, minimumX, maximumX);
                    rotationY = ClampAngle(rotationY, minimumY, maximumY);
                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
                    transform.localRotation = originalRotation * xQuaternion * yQuaternion;
                } else if (axes == RotationAxes.MouseX) {
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationX = ClampAngle(rotationX, minimumX, maximumX);
                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    transform.localRotation = originalRotation * xQuaternion;
                } else {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = ClampAngle(rotationY, minimumY, maximumY);
                    Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                    transform.localRotation = originalRotation * yQuaternion;
                }
            }

            //pulse was here before

            //Fire Pulse
            if (Time.time >= timestamp && (Input.GetMouseButtonDown(0))) {
                CmdFirePulseShell();
                timestamp = Time.time + timeBetweenShots;
            }

            //Tank Jump
            if (Time.time >= jumptimestamp && (Input.GetKeyDown(KeyCode.Space))) {
                requestJump = true;
                jumptimestamp = Time.time + timeBetweenJumps;
            }

            if (Input.GetKey(KeyCode.LeftShift)) {
                // raise level
                height = Mathf.Min(height + 0.01f, 1.0f);
                if (isLanded && height > 0.001) {
                    TakeOff();
                }
            }

            if (!isLanded && Input.GetKey(KeyCode.LeftControl)) {
                // lower level
                height = Mathf.Max(height - 0.01f, 0f);
                if (height < 0.001) {
                    requestLand = true;
                }
            }

            if (requestLand) {
                if (CanLand()) {
                    Debug.Log("CanLand true");
                    Land();
                } else {
                    Debug.Log("CanLand false");
                }
            }

        }

        private bool CanLand() {
            if (height > maxDistanceToLand) {
                return false;
            }

            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down);
            RaycastHit hit;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            return terrainCollider.Raycast(ray, out hit, 2.0f) && hit.distance <= maxDistanceToLand;
        }

        private void Land() {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down);
            RaycastHit hit;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            if (terrainCollider.Raycast(ray, out hit, 2.0f)) {
                Vector3 fwd = transform.forward;
                Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
                transform.rotation = Quaternion.LookRotation(proj, hit.normal);
                transform.Translate(hit.point - transform.position);
                transform.Translate(Vector3.up * 0.15f); //fixme: distance is calculated from center of the tank model, this moves the tank 'right' amount so that tank does not get clipped with the ground
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true; //do not let physics forces affect this body
                isLanded = true;
                requestLand = false;
                AudioSource.PlayClipAtPoint(landSource, transform.position);
                GetComponent<AudioSource>().Stop();
            } else {
                //cannot land
                isLanded = false;
            }
        }

        private void TakeOff() {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false; //let physics forces affect this body again
            requestLand = false;
            isLanded = false;
            AudioSource.PlayClipAtPoint(takeoffSource, transform.position);
            GetComponent<AudioSource>().Play();
        }

        void CmdFirePulseShell() {
            GameObject pulseShell = GameObject.Instantiate(pulseShellPrefab);
            pulseShell.transform.position = transform.position;
            pulseShell.transform.rotation = transform.rotation;
            pulseShell.transform.Translate(Vector3.forward * 2.0f + Vector3.up * 0.2f);
            //NetworkServer.Spawn(pulseShell);
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(-transform.forward * 100f);
        }

        public void FixedUpdate() {
            if (!photonView.isMine)
                return;
            Rigidbody rb = GetComponent<Rigidbody>();

            float forceMultiplier = 2.0f;

            float x = Input.GetAxis("Horizontal") * 0.1f;
            float z = Input.GetAxis("Vertical") * 0.1f;

            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down);
            RaycastHit hit;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            if (terrainCollider.Raycast(ray, out hit, height * 2)) {
                Vector3 direction = Vector3.up; //transform.up
                rb.AddForce(direction * Mathf.Min(forceMultiplier / (Mathf.Max(hit.distance - height, 0.01f) / 2.0f), 20.0f));


                /*transform.rotation = Quaternion.LookRotation(proj, hit.normal);

                Quaternion target = Quaternion.FromToRotation(Vector3.up, hit.normal);
                if (z >= 0.01f)
                {
                    Vector3 fwd = transform.forward;
                    Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(proj, hit.normal), 0.05f);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(hit.normal), 0.05f);
                */
                Debug.DrawLine(transform.position, hit.point);

            }



            //Tank Jump
            if (requestJump) {
                AudioSource.PlayClipAtPoint(jumpSource, transform.position);
                rb.AddForce(transform.up * jumpForce);
                requestJump = false;
            }
            rb.AddRelativeForce(new Vector3(x, 0, z) * 50f);
            //rb.AddTorque(new Vector3(0, 0, -x) * 0.7f); //strafe rotation


            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, -transform.up, hit, height)) {
            //    Rigidbody rb = GetComponent<Rigidbody>();
            //    rb.velocity = new Vector3();
            //    rb.AddForce(transform.up * (forceMultiplier / (hit.distance / 2)));
            //}
        }

        public static float ClampAngle(float angle, float min, float max) {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
