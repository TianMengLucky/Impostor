﻿using Impostor.Api.Utils;

namespace Impostor.Api.Config
{
    public class ServerConfig
    {
        public const string Section = "Server";

        private string? _resolvedPublicIp;
        private string? _resolvedListenIp;
        private string? _resolvedFrpPublicIp;

        public string PublicIp { get; set; } = "127.0.0.1";

        public ushort PublicPort { get; set; } = 22023;

        public string FrpPublicIp { get; set; } = "127.0.0.1";

        public ushort FrpPublicPort { get; set; } = 22023;

        public string ListenIp { get; set; } = "0.0.0.0";

        public ushort ListenPort { get; set; } = 22023;

        public string ResolvePublicFrpIp()
        {
           return _resolvedFrpPublicIp ??= IpUtils.ResolveIp(FrpPublicIp); 
        }

        public string ResolvePublicIp()
        {
            return _resolvedPublicIp ??= IpUtils.ResolveIp(PublicIp);
        }

        public string ResolveListenIp()
        {
            return _resolvedListenIp ??= IpUtils.ResolveIp(ListenIp);
        }
    }
}
