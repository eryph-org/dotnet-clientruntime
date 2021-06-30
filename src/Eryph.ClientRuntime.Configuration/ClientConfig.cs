using System;
using System.Collections.Generic;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Configuration
{
    public class ClientConfig
    {

        public string DefaultClientId { get; set; }


        public IEnumerable<ClientData> Clients { get; set; }

        public IDictionary<string, Uri> Endpoints { get; set; }
    }
}