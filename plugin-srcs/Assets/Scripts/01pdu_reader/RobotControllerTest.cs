using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    enum MotorType {
        MotorType_Left = 0,
        MotorType_Right,
        MotorType_Num
    }
    public class RobotControllerTest : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private IRobotPartsMotor [] motors = new IRobotPartsMotor[(int)MotorType.MotorType_Num];
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;
        private IPduWriter pdu_writer;
        public float steering_sensitivity = 1.5f;                // ?o???l

        public string topic_type = "geometry_msgs/Twist";
        public string topic_name = "cmd_vel";
        public string robot_name;
        public int update_cycle = 10;
        public float motor_interval_distance = 0.160f; // 16cm

        private int count = 0;

        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[1];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name;
            cfg[0].topic_type_name = this.topic_type;
            cfg[0].sub = true;
            return cfg;
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
                this.pdu_io = PduIoConnector.Get(robot_name);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + robot_name);
                }
                var pdu_writer_name = robot_name + "_" + this.topic_name + "Pdu";
                this.pdu_writer = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }
            }
            this.count = 0;
        }

        public static float motorFowardForceScale = 1.0f;
        public static float motorRotateForceScale = 10.0f;
        public void DoControl()
        {
            this.count++;
            if (this.count < this.update_cycle)
            {
                return;
            }
            this.count = 0;
            double target_velocity;
            double target_rotation_angle_rate;
            Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
            Debug.Log(this.pdu_writer.GetWriteOps().Ref("linear").GetDataFloat64("x"));
            Debug.Log(this.pdu_writer.GetWriteOps().Ref("angular").GetDataFloat64("z"));
        }
        public IoMethod io_method = IoMethod.RPC;
        public CommMethod comm_method = CommMethod.UDP;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[1];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.WRITE;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name;
            configs[0].value.type = this.topic_type;
            configs[0].value.class_name = ConstantValues.pdu_reader_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[0].value.pdu_size = ConstantValues.Twist_pdu_size;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();
            return configs;
        }
    }
}

