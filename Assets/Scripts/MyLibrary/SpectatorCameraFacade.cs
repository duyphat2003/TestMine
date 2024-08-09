using MyLibrary;
using UnityEngine;

    public class SpectatorCameraFacade
    {
        SpectatorCameraProperties spectatorCameraProperties;
        public SpectatorCameraFacade(SpectatorCameraProperties spectatorCameraProperties)
        {
            this.spectatorCameraProperties = spectatorCameraProperties;
        }

        public System.Tuple<Vector3, Vector3> ConstructMovement(Transform directMovement, bool isRun)   => spectatorCameraProperties.spectatorCameraMovement.ConstructMovement(directMovement, isRun);
        public Vector3 DestroyMovement()   => spectatorCameraProperties.spectatorCameraMovement.DestroyMovement();


        public Quaternion ConstructRotateObject() => spectatorCameraProperties.spectatorCameraRotateObject.ConstructRotateObject();
        public Quaternion DestroyRotateObject() => spectatorCameraProperties.spectatorCameraRotateObject.DestroyRotateObject();


        public string ConstructRestoreObject(string name) => spectatorCameraProperties.spectatorCameraRestoreObject.ConstructRestoreObject(name);
    }

    [System.Serializable]
    public class SpectatorCameraProperties
    {
        public SpectatorCameraMovement spectatorCameraMovement;
        public SpectatorCameraRotateObject spectatorCameraRotateObject;
        public SpectatorCameraRestoreObject spectatorCameraRestoreObject;
    }
