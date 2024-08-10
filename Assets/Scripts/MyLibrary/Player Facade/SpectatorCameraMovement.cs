using UnityEngine;

namespace MyLibrary.PlayerFacade
{
    public class SpectatorCameraMovement
    {
        SpectatorCameraMovementProperties spectatorCameraMovementProperties;
        public SpectatorCameraMovement(SpectatorCameraMovementProperties spectatorCameraMovementProperties)
        {
            this.spectatorCameraMovementProperties = spectatorCameraMovementProperties;
        }
        Vector3 direction;
        Vector3 playerEulerAngles;
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        public System.Tuple<Vector3, Vector3> ConstructMovement(Transform directMovement, bool isRun)
        {
                    // Handle camera rotation using mouse input
            yaw += Input.GetAxis("Mouse X") * spectatorCameraMovementProperties.lookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * spectatorCameraMovementProperties.lookSpeed;
            pitch = Mathf.Clamp(pitch, -90f, 90f);  // Limit pitch to prevent flipping

            playerEulerAngles = new Vector3(pitch, yaw, 0.0f);

            float moveSpeedCurrent = isRun ? spectatorCameraMovementProperties.fastMoveSpeed : spectatorCameraMovementProperties.moveSpeed;

            switch (spectatorCameraMovementProperties.direction)
            {
                case DirectionMovement.FORWARD:
                direction = directMovement.forward;
                break;

                case DirectionMovement.BACKWARD:
                direction = -directMovement.forward;
                break;

                case DirectionMovement.LEFT:
                direction = -directMovement.right;
                break;

                case DirectionMovement.RIGHT:
                direction = directMovement.right;
                break;

                case DirectionMovement.UP:
                direction = directMovement.up;
                break;

                case DirectionMovement.DOWN:
                direction = -directMovement.up;
                break;
            }
            return new System.Tuple<Vector3, Vector3>(moveSpeedCurrent * Time.deltaTime * direction, playerEulerAngles);
        }

        public Vector3 DestroyMovement() => Vector3.zero;
    }

    public enum DirectionMovement
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    [System.Serializable]
    public class SpectatorCameraMovementProperties
    {
        public float moveSpeed = 10.0f;         // Speed of camera movement
        public float fastMoveSpeed = 50.0f;     // Speed when holding the "Shift" key
        public float lookSpeed = 2.0f;          // Speed of mouse look
        [HideInInspector] public DirectionMovement direction;   // Speed of mouse look
    }
}
    