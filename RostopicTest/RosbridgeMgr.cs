using System;
using System.Collections.Generic;
using System.Windows;
using Rosbridge.Client;
using Newtonsoft.Json.Linq;

namespace RostopicTest
{
    public class RosbridgeMgr
    {
        private MessageDispatcher? _md;
        private MainViewModel? _mainViewModel;
        private bool _isConnected;

        public bool IsConnected { get; set; }

        List<Subscriber> _subscribers;

        private RosbridgeMgr()
        {
            _subscribers = new List<Subscriber>();
            IsConnected = false;
        }
        private static readonly Lazy<RosbridgeMgr> _instance = new Lazy<RosbridgeMgr>(() => new RosbridgeMgr());
        public static RosbridgeMgr Instance => _instance.Value;

        public void SetMainModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public async void Connect(string uri)
        {
            if (IsConnected)
            {
                foreach (var s in _subscribers)
                {
                    s.UnsubscribeAsync().Wait();
                }
                IsConnected = false;
                _subscribers.Clear();

                if (_md is not null)
                {
                    await _md.StopAsync();
                    _md = null;
                }
            }
            else
            {
                try
                {
                    _md = new MessageDispatcher(new Socket(new Uri(uri)), new MessageSerializerV2_0());
                    _md.StartAsync().Wait();

                    SubscribeMsg("/joint_states", "sensor_msgs/JointState");

                    //foreach (var tuple in _rosbrdgModel.GetSubscribeTopics())
                    //{
                    //    SubscribeMsg(tuple.Item1, tuple.Item2);
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                         "Error!! Could not connect to the rosbridge server", MessageBoxButton.OK, MessageBoxImage.Error);
                    _md = null;
                    return;
                }

                IsConnected = true;
            }
            //ToggleConnected();
        }

        public void Capture()
        {
            ServiceCallMsg("/zivid_camera/capture", "[]");
        }

        public void Start_Action()
        {
            string topic = "/camera01_result0a_action/goal";
            string msg_type = "plaif_vision_msgs/VisionActionGoal";
            string msg = "{}";

            PublishMsg(topic, msg_type, msg);
        }

        public async void PublishMsg(string topic, string msg_type, string msg)
        {
            if (_md is null) return;

            var pb = new Rosbridge.Client.Publisher(topic, msg_type, _md);
            await pb.PublishAsync(JObject.Parse(msg));
        }

        private void _subscriber_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string msg = e.Message["msg"]!.ToString();

            switch (e.Message["topic"]!.ToString())
            {
            }

        }

        public async void SubscribeMsg(string topic, string msg_type)
        {
            if (_md is null) return;

            var s = new Subscriber(topic, msg_type, _md);
            s.MessageReceived += _subscriber_MessageReceived;
            await s.SubscribeAsync();
            _subscribers.Add(s);
        }

        public async void ServiceCallMsg(string topic, string msg)
        {
            if (_md is null) return;

            var sc = new ServiceClient(topic, _md);
            JArray argsList = JArray.Parse(msg);
            var dynamics = argsList.ToObject<List<dynamic>>();
            if (dynamics != null)
            {
                var result = await sc.Call(dynamics);
            }
        }
    }
}
