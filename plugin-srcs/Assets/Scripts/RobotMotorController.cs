using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts.TestDriver
{
    public class RobotMotorController : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        public GameObject realRobot;
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;

        public int update_cycle = 10;
        public string topic_name = "cmd_vel";
        public string roboname = "SampleRobot";
        private int count = 0;

        public RosTopicMessageConfig[] getRosConfig()
        {
            return new RosTopicMessageConfig[0];
        }
        public void Initialize(System.Object obj)
        {
            GameObject tmp = null;
            try
            {
                tmp = obj as GameObject;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Initialize error: " + e.Message);
                return;
            }

            if (this.root == null)
            {
                this.root = tmp;
                this.root_name = string.Copy(this.root.transform.name);
                this.pdu_io = PduIoConnector.Get(roboname);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + roboname);
                }
                var pdu_io_name = roboname + "_" + this.topic_name + "Pdu";
                this.pdu_reader = this.pdu_io.GetReader(pdu_io_name);
                if (this.pdu_reader == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_io_name);
                }
            }
            this.count = 0;
        }
        public double delta_vel = 0.1;
        public double delta_angle = 0.1;
        private void Update()
        {
            robotRotateAndPostionSync();
            RobotControllerNoObstacle();
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.target_velocity += delta_vel;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.target_velocity -= delta_vel;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.target_rotation_angle_rate += delta_angle;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.target_rotation_angle_rate -= delta_angle;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space key is pressed");
                this.target_velocity = 0;
                this.target_rotation_angle_rate = 0;
            }
        }

        public double target_velocity;
        public double target_rotation_angle_rate;
        public void DoControl()
        {
            this.count++;
            if (this.count < this.update_cycle)
            {
                return;
            }
            this.count = 0;
            target_velocity = 10f;
            //Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB" + target_velocity);
            //this.pdu_reader.GetWriteOps().Ref("linear").SetData("x", target_velocity * DifferentialMotorController.motorFowardForceScale);
            //this.pdu_reader.GetWriteOps().Ref("angular").SetData("z", target_rotation_angle_rate * DifferentialMotorController.motorRotateForceScale);
        }
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            return new RoboPartsConfigData[0];
        }

        private int goalListCount = 0;
        private bool isObstacleState = false;
        private Transform goalPoint;
        public List<Transform> goalList;
        private Vector3 heading;
        public float RotateSpeed = 10f;
        public bool canMoveRobot = false;
        void Start()
        {
            goalPoint = goalList[goalListCount];
            heading = goalPoint.position - this.transform.position;
            rotateIsOn = true;
        }
        public void robotRotateAndPostionSync()
        {
            //this.transform.localPosition = realRobot.transform.localPosition;
            this.transform.position = realRobot.transform.position + new Vector3(0, 0, 1);
            this.transform.localRotation = realRobot.transform.localRotation;
            this.transform.Rotate(0, -90, 0);
        }
        private bool rotateIsOn;
        private bool runningIsOn;
        public void RobotControllerNoObstacle()
        {
            Ray ray = new Ray(transform.position, this.transform.forward);
            Debug.DrawRay(gameObject.transform.position, this.transform.forward * 30, Color.blue, 0.01f);
            Debug.DrawRay(gameObject.transform.position, this.transform.right * 30, Color.blue, 0.01f);
            Debug.DrawRay(gameObject.transform.position, - this.transform.right * 30, Color.blue, 0.01f);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.name == goalPoint.transform.name)
                {
                    Vector3 RobotFoward = this.transform.forward;
                    heading = goalPoint.position - this.transform.position;
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
                this.target_velocity = 0;
                this.target_rotation_angle_rate = 0;
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
                        this.target_rotation_angle_rate += delta_angle;
                        rotateIsOn = false;
                    }
                }
                else
                {
                    //Debug.Log(diffRotaion.x);
                    //Debug.Log(diffRotaion.z);
                    //this.transform.Rotate(0, 0, 0);
                    this.target_rotation_angle_rate = 0;
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
                this.target_velocity += delta_vel;
                runningIsOn = false;
            }
            float dis = Vector3.Distance(goalPoint.position, this.transform.position);
            //Debug.Log(Mathf.Abs(dis));
            if (Mathf.Abs(dis) < 1.6f)
            {
                if (goalList.Count > goalListCount)
                {
                    goalListCount++;
                    goalPoint = goalList[goalListCount];
                    heading = goalPoint.position - this.transform.position;
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

