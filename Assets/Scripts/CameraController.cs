using UnityEngine;

namespace Assets.Scenes.MainHall
{
    public class CameraController : MonoBehaviour
    {
        /*
         * CameraControlller
         * Credit where it is due I based this on "EXTENDED FLYCAM" By Desi Quintans (CowfaceGames.com), 17 August 2012.
         * who posted the code under a free public liscence for the unity community to use.
                
	    FEATURES
		    WASD/Arrows:    Movement
		              Q:    Drop
		              E:    Climb
                  Shift:    Move faster
                Control:    Move slower
                 Escape:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	    */

        public float cameraSensitivity = 45;
        public float climbSpeed = 100;
        public float deltaTimeReplacement = .003f; //cant move the camera properly when time is slowed/stopped if I rely on delta T
        public float fastMoveFactor = 10; //
        public float normalMoveSpeed = 200;

        private float rotationX;
        private float rotationY;
        public float slowMoveFactor = 0.50f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Cursor.lockState = Input.GetMouseButton(1) ? CursorLockMode.Locked : CursorLockMode.None;
            //only rotate if cursor is locked
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * deltaTimeReplacement;
                rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * deltaTimeReplacement;
                rotationY = Mathf.Clamp(rotationY, -90, 90);

                transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) *
                                      Input.GetAxis("Vertical") * deltaTimeReplacement;
                transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) *
                                      Input.GetAxis("Horizontal") * deltaTimeReplacement;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) *
                                      Input.GetAxis("Vertical") * deltaTimeReplacement;
                transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) *
                                      Input.GetAxis("Horizontal") * deltaTimeReplacement;
            }
            else
            {
                transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") *
                                      deltaTimeReplacement;
                transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") *
                                      deltaTimeReplacement;
            }


            if (Input.GetKey(KeyCode.Q)) transform.position += transform.up * climbSpeed * deltaTimeReplacement;
            if (Input.GetKey(KeyCode.E)) transform.position -= transform.up * climbSpeed * deltaTimeReplacement;

            if (Input.GetKeyDown(KeyCode.Escape))
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                    ? CursorLockMode.None
                    : CursorLockMode.Locked;
        }
    }
}