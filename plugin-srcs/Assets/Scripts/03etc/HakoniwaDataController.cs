using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class HakoniwaDataController : MonoBehaviour
    {
        public HakoniwaDataManager hakoniwaDataManager = new HakoniwaDataManager();

        //GameObject
        public GameObject robot;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(new Vector3((float)(hakoniwaDataManager.CameraPosition_x) ,(float)(hakoniwaDataManager.CameraPosition_y), (float)(hakoniwaDataManager.CameraPosition_z)));
            robot.transform.localPosition = new Vector3((float)(hakoniwaDataManager.CameraPosition_x) ,(float)(hakoniwaDataManager.CameraPosition_y), (float)(hakoniwaDataManager.CameraPosition_z));
            //robot.transform.localPosition = new Vector3((float)(hakoniwaDataManager.CameraPosition_x) ,(float)(hakoniwaDataManager.CameraPosition_y), (float)(hakoniwaDataManager.CameraPosition_z));
        }
    }
}