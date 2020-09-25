using System;
using System.Collections.Generic;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Configuration
{
    public class ClientConfig
    {

        public string DefaultClientId { get; set; }


        public IEnumerable<ClientData> Clients { get; set; }

        public IDictionary<string, Uri> Endpoints { get; set; }
    }
}