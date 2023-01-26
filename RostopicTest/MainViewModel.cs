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

        }
    }
}
