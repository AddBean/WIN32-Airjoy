using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirJoy.ServiceLogic
{
    class UserModel
    {
            public bool res { get; set; }//返回的登录结果;
            public string name { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public bool androidState { get; set; }
            public string androidIp { get; set; }
            public string androidupdatetime { get; set; }
            public bool pcState { get; set; }
            public string pcIp { get; set; }
            public string pcupdatetime { get; set; }
            public string addtime { get; set; }
        
    }
}
