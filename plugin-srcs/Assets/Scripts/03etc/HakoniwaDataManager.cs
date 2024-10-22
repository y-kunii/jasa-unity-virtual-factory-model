using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class HakoniwaDataManager : MonoBehaviour
    {
        // pdu position data
        public double CameraPosition_x;
        public double CameraPosition_y;
        public double CameraPosition_z;

        // pdu rotate data
        public double CameraQuaternion_x;
        public double CameraQuaternion_y;
        public double CameraQuaternion_z;
        public double CameraQuaternion_w;

        // misc
        public PositionAndRotationController positionAndRotationController = new PositionAndRotationController();

        void Start()
        {
            
        }

        void Update()
        {
            Debug.Log("QQQQQQQQQQQQQ " + positionAndRotationController.position_x);
            CameraPosition_x = positionAndRotationController.position_x;
            CameraPosition_y = positionAndRotationController.position_y;
            CameraPosition_z = positionAndRotationController.position_z;

            CameraQuaternion_x = positionAndRotationController.quaternion_x;
            CameraQuaternion_y = positionAndRotationController.quaternion_y;
            CameraQuaternion_z = positionAndRotationController.quaternion_z;
            CameraQuaternion_w = positionAndRotationController.quaternion_w;

        }

    }
}
