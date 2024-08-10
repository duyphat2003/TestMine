using UnityEngine;

namespace MyLibrary.PlayerFacade
{
    public class SpectatorCameraRotateObject
    {
        SpectatorCameraRotateObjectProperties spectatorCameraRotateObjectProperties;

        public SpectatorCameraRotateObject(SpectatorCameraRotateObjectProperties spectatorCameraRotateObjectProperties)
        {
            this.spectatorCameraRotateObjectProperties = spectatorCameraRotateObjectProperties;
        }

        Quaternion directionRotate;
        public Quaternion ConstructRotateObject()
        {
            switch(spectatorCameraRotateObjectProperties.rotateDirection)
            {
                case RotateDirection.UP:
                directionRotate =  Quaternion.AngleAxis(spectatorCameraRotateObjectProperties.rotationSpeedY * Time.deltaTime, Vector3.up);
                break;

                case RotateDirection.DOWN:
                directionRotate =  Quaternion.AngleAxis(spectatorCameraRotateObjectProperties.rotationSpeedY * Time.deltaTime, -Vector3.up);
                break;
                
                case RotateDirection.RIGHT:
                directionRotate =  Quaternion.AngleAxis(spectatorCameraRotateObjectProperties.rotationSpeedX * Time.deltaTime, Vector3.right);
                break;
                case RotateDirection.LEFT:
                directionRotate =  Quaternion.AngleAxis(spectatorCameraRotateObjectProperties.rotationSpeedX * Time.deltaTime, -Vector3.right);
                break;

                case RotateDirection.FORWARD:
                directionRotate =  Quaternion.AngleAxis(spectatorCameraRotateObjectProperties.rotationSpeedZ * Time.deltaTime, Vector3.forward);
                break;
                
                case RotateDirection.BACKWARD:
                directionRotate =  Quaternion.AngleAxis(spectatorCameraRotateObjectProperties.rotationSpeedZ * Time.deltaTime, -Vector3.forward);
                break;
            }
            return directionRotate;
        }
        
        public Quaternion DestroyRotateObject() => Quaternion.Euler(0, 0, 0);
    }

    [System.Serializable]
    public class SpectatorCameraRotateObjectProperties
    {
        public float rotationSpeedX = 10.0f;  // Speed of rotation around X axis
        public float rotationSpeedY = 10.0f;  // Speed of rotation around Y axis
        public float rotationSpeedZ = 10.0f;  // Speed of rotation around Z axis    [HideInInspector] public GameObject objectInteracting;
        [HideInInspector] public RotateDirection rotateDirection;
    }

    public enum RotateDirection
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
}
