using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Two10.Azure.Arr
{
    public class WebRole : RoleEntryPoint
    {
        public string RoleName
        {
            get
            {
                return "WebRole1";
            }
        }

        private IEnumerable<RoleInstance> Instances
        {
            get
            {
                return RoleEnvironment.Roles[this.RoleName].Instances;
            }
        }

        public override bool OnStart()
        {
            Log("OnStart");

            return base.OnStart();
        }

        public override void Run()
        {
            Log("Start");
            IISconfiguration.GetOrCreateFarm("farm");
            while (true)
            {
                try
                {
                    var endpoints = this.Instances.Select(i => i.InstanceEndpoints["Internal"].IPEndpoint).ToArray();
                    Log("Endpoints = " + endpoints.Length);
                    IISconfiguration.AddServers("farm", endpoints);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }

                Thread.Sleep(60000);
            }
        }

        private void Log(string message)
        {
            using (var file = new StreamWriter("log.log", true))
            {
                file.WriteLine(message);
            }
            System.Diagnostics.Trace.Write(message);
        }

    }
}
