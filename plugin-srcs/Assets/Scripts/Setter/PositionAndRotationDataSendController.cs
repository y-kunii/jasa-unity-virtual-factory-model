using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    public class PositionAndRotationDataSendController : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        public GameObject realRobot;
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;
        private IPduWriter pdu_writer;

        public int update_cycle = 10;
        public string topic_type = "geometry_msgs/Pose";
        public string topic_name = "pose_data";
        public string roboname = "CameraTestDriver";
        private int count = 0;

        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[1];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_type_name = this.topic_type;
            cfg[0].topic_message_name = this.topic_name;
            cfg[0].sub = false;
            cfg[0].pub_option = new RostopicPublisherOption();
            cfg[0].pub_option.cycle_scale = this.update_cycle;
            cfg[0].pub_option.latch = false;
            cfg[0].pub_option.queue_size = 1;
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
                this.pdu_io = PduIoConnector.Get(roboname);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + roboname);
                }
                var pdu_reader_name = roboname + "_" + this.topic_name + "Pdu";
                //this.pdu_reader = this.pdu_io.GetReader(pdu_io_name);
                this.pdu_reader = this.pdu_io.GetReader(pdu_reader_name);
                //if (this.pdu_reader == null)
                if (this.pdu_reader == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_reader_name);
                }
            }
            this.count = 0;
        }

        public double position_x = 0;
        public double position_y = 0;
        public double position_z = 0;

        public double quaternion_x = 0;
        public double quaternion_y = 0;
        public double quaternion_z = 0;
        public double quaternion_w = 0;
        public void DoControl()
        {
            this.count++;
            if (this.count < this.update_cycle)
            {
                return;
            }
            this.count = 0;

            // position
            this.pdu_reader.GetReadOps().Ref("position").SetData("x", position_x);
            this.pdu_reader.GetReadOps().Ref("position").SetData("y", position_y);
            this.pdu_reader.GetReadOps().Ref("position").SetData("z", position_z);

            // quaternion
            this.pdu_reader.GetReadOps().Ref("orientation").SetData("x", quaternion_x);
            this.pdu_reader.GetReadOps().Ref("orientation").SetData("y", quaternion_y);
            this.pdu_reader.GetReadOps().Ref("orientation").SetData("z", quaternion_z);
            this.pdu_reader.GetReadOps().Ref("orientation").SetData("w", quaternion_w);
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
            configs[0].value.class_name = ConstantValues.pdu_writer_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[0].value.pdu_size = ConstantValues.Bool_pdu_size;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();
            return configs;
        }
    }
}

