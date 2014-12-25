﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApiCore
{
    public class SessionInfo
    {
        public int AppId;
        public int Permissions;
        public string SessionId;
        public string MemberId;
        public string Secret;
        public string Signature;
        public int Expire;
    }
}
