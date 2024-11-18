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
        public GameObject robotCamera;
        public GameObject realRobot;

        // misc
        private int goalListCount = 0;
        private bool isObstacleState = false;
        private Transform goalPoint;
        public List<Transform> goalList;
        private Vector3 heading;
        public float RotateSpeed = 10f;
        public bool canMoveRobot = false;
        private bool rotateIsOn;
        private bool runningIsOn;


        // Start is called before the first frame update
        void Start()
        {
            //goalPoint = goalList[goalListCount];
            //heading = goalPoint.position - this.transform.position;
            //rotateIsOn = true;
        }
        
        public int count_i = 0;
        // Update is called once per frame
        void Update()
        {
            //robot.transform.position = new Vector3((float)(hakoniwaDataManager.CameraPosition_x) ,(float)(hakoniwaDataManager.CameraPosition_y), (float)(hakoniwaDataManager.CameraPosition_z));
            robot.transform.localPosition = new Vector3((float)(hakoniwaDataManager.CameraPosition_x) , 0, (float)(hakoniwaDataManager.CameraPosition_z));
            //robot.transform.localPosition = new Vector3((float)(hakoniwaDataManager.CameraPosition_x) ,(float)(robot.transform.position.y), (float)(hakoniwaDataManager.CameraPosition_z));
            //robot.transform.Rotate(new Vector3((float)hakoniwaDataManager.CameraQuaternion_x, (float)hakoniwaDataManager.CameraQuaternion_y, (float)hakoniwaDataManager.CameraQuaternion_z));
            robot.transform.localRotation = Quaternion.Euler( (float)hakoniwaDataManager.CameraQuaternion_x, (float)hakoniwaDataManager.CameraQuaternion_y, (float)hakoniwaDataManager.CameraQuaternion_z);
            //robotRotateAndPostionSync();
            if (hakoniwaDataManager.buttonA_flag)
            {
                goalPoint = goalList[0];
                heading = goalPoint.position - this.transform.position;
                
                if (count_i < 1)
                {
                    rotateIsOn = true;
                    count_i++;
                }
            }
            if (goalPoint != null)
            {
                RobotControllerNoObstacle();
            }
        }
        public void robotRotateAndPostionSync()
        {
            //this.transform.localPosition = realRobot.transform.localPosition;
            this.transform.position = realRobot.transform.position + new Vector3(0, 0, 1);
            this.transform.localRotation = realRobot.transform.localRotation;
            this.transform.Rotate(0, -90, 0);
        }

        public void RobotControllerNoObstacle()
        {
            Ray ray = new Ray(robotCamera.transform.position, robotCamera.transform.forward);
            Debug.DrawRay(robotCamera.gameObject.transform.position, robotCamera.transform.forward * 30, Color.blue, 0.01f);
            Debug.DrawRay(robotCamera.gameObject.transform.position, robotCamera.transform.right * 30, Color.blue, 0.01f);
            Debug.DrawRay(robotCamera.gameObject.transform.position, - robotCamera.transform.right * 30, Color.blue, 0.01f);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                //Debug.Log(hit.transform.name == goalPoint.transform.name);
                if (hit.transform.name == goalPoint.transform.name)
                {
                    
                    Vector3 RobotFoward = robot.transform.forward;
                    heading = goalPoint.position - robotCamera.transform.position;
                    Vector3 diffRotaion = heading.normalized - RobotFoward;
                    if (Mathf.Abs(diffRotaion.x) < 1.404f)
                    {
                        canMoveRobot = true;
                    }
                }
            }

            if (canMoveRobot)
            {
                //this.transform.Rotate(0, 0, 0);
                hakoniwaDataManager.angular_z = 0.0f;
                hakoniwaDataManager.linear_x = 0.1f;
                //this.target_velocity = 0;
                //this.target_rotation_angle_rate = 0;
                runningIsOn = true;
                MoveRobotFoward();
            }
            else
            {
                //if (diffRotaion > 0.001f)
                //if (Mathf.Abs(diffRotaion.x) > 0.1f)
                if (!canMoveRobot)
                {
                    //this.transform.Rotate(0, 0.1f, 0);
                    if (rotateIsOn)
                    {
                        Debug.Log("rotateIsOn");
                        //this.target_rotation_angle_rate += delta_angle;
                        hakoniwaDataManager.angular_z = 0.1f;
                        rotateIsOn = false;
                    }
                }
                else
                {
                    //Debug.Log(diffRotaion.x);
                    //Debug.Log(diffRotaion.z);
                    //this.transform.Rotate(0, 0, 0);
                    //this.target_rotation_angle_rate = 0;
                    canMoveRobot = true;
                }
            }
        }
        public void MoveRobotFoward()
        {
            //this.transform.position += this.transform.forward * 0.01f;
            //this.transform.position += this.transform.forward * (float)delta_vel;
            if (runningIsOn)
            {
                //this.target_velocity += delta_vel;
                runningIsOn = false;
            }
            float dis = Vector3.Distance(goalPoint.position, robotCamera.transform.position);
            Debug.Log(dis);
            //Debug.Log(Mathf.Abs(dis));
            if (Mathf.Abs(dis) < 0.06f)
            {
                Debug.Log("GOAL OKKKKK");
                hakoniwaDataManager.linear_x = 0.0f;
                if (goalList.Count > goalListCount)
                {
                    goalListCount++;
                    goalPoint = goalList[goalListCount];
                    heading = goalPoint.position - robotCamera.transform.position;
                }
                else
                {
                    goalPoint = null;
                }
                canMoveRobot = false;
                rotateIsOn = true;
                isObstacleState = false;
            }
        }
    }
}