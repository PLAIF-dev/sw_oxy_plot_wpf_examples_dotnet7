using Rosbridge.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RostopicTest
{
    public class MainViewModel : ViewModelBase
    {
        MainViewModel()
        {
            var rosmgr = RosbridgeMgr.Instance;
            rosmgr.SetMainModel(this);

            rosmgr.Connect("ws://192.168.1.36:9090");
            rosmgr.SubscribeMsg("/joint_states", "sensor_msgs/JointState", _subscriber_MessageReceived);
        }

        public void _subscriber_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string msg = e.Message["msg"]!.ToString();

            switch (e.Message["topic"]!.ToString())
            {
                case "/joint_states":
                    var sec  = Int32.Parse(e.Message["msg"]!["header"]!["stamp"]!["secs"]!.ToString());
                    var nsec = Int32.Parse(e.Message["msg"]!["header"]!["stamp"]!["nsecs"]!.ToString());
                    var seq  = Int32.Parse(e.Message["msg"]!["header"]!["seq"]!.ToString());
                    var pos1 = Double.Parse(e.Message["msg"]!["position"]![0]!.ToString());
                    var pos2 = Double.Parse(e.Message["msg"]!["position"]![1]!.ToString());
                    var pos3 = Double.Parse(e.Message["msg"]!["position"]![2]!.ToString());
                    var pos4 = Double.Parse(e.Message["msg"]!["position"]![3]!.ToString());
                    var pos5 = Double.Parse(e.Message["msg"]!["position"]![4]!.ToString());
                    var pos6 = Double.Parse(e.Message["msg"]!["position"]![5]!.ToString());
                    break;
            }
        }
    }
}
