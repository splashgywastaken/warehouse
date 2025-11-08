using UnityEngine;
using UnityEngine.Events;


namespace Warehouse.Services.Camera
{
    public interface ICameraNotifier
    {
        public event System.Action MoveStarted;
        public event System.Action MoveEnded;
        public event System.Action<Vector3> PositionChanged;
        public event System.Action<Quaternion> RotationChanged;
    }
}
