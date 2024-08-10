using UnityEngine;

namespace MyLibrary
{
    public class SpectatorCameraFacade
    {
        SpectatorCameraMovement spectatorCameraMovement;
        SpectatorCameraRotateObject spectatorCameraRotateObject;
        SpectatorCameraRestoreObject spectatorCameraRestoreObject;

        SpectatorCameraProperties spectatorCameraProperties;
        public SpectatorCameraFacade(SpectatorCameraProperties spectatorCameraProperties)
        {
            this.spectatorCameraProperties = spectatorCameraProperties;
            spectatorCameraMovement = new SpectatorCameraMovement(spectatorCameraProperties.spectatorCameraMovementProperties);
            spectatorCameraRotateObject = new SpectatorCameraRotateObject(spectatorCameraProperties.spectatorCameraRotateObjectProperties);
            spectatorCameraRestoreObject = new SpectatorCameraRestoreObject(spectatorCameraProperties.spectatorCameraRestoreObjectProperties);
        }

        public System.Tuple<Vector3, Vector3> ConstructMovement(Transform directMovement, bool isRun)   => spectatorCameraMovement.ConstructMovement(directMovement, isRun);
        public Vector3 DestroyMovement()   => spectatorCameraMovement.DestroyMovement();


        public Quaternion ConstructRotateObject() => spectatorCameraRotateObject.ConstructRotateObject();
        public Quaternion DestroyRotateObject() => spectatorCameraRotateObject.DestroyRotateObject();


        public string ConstructRestoreObject(string name) => spectatorCameraRestoreObject.ConstructRestoreObject(name);
    }

    [System.Serializable]
    public class SpectatorCameraProperties
    {
        public SpectatorCameraMovementProperties spectatorCameraMovementProperties;
        public SpectatorCameraRotateObjectProperties spectatorCameraRotateObjectProperties;
        public SpectatorCameraRestoreObjectProperties spectatorCameraRestoreObjectProperties;
    }
}

