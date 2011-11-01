using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Two10.AzureSugar;

namespace Two10.Azure.Arr
{
    public class WebRole : RoleEntryPoint
    {
        public string FarmRoleName { get; set; }

        public string PublicUrl { get; set; }

        public string StorageConnectionString { get; set; }

        private IEnumerable<RoleInstance> Instances
        {
            get
            {
                return RoleEnvironment.Roles[this.FarmRoleName].Instances;
            }
        }

        public override bool OnStart()
        {
            this.FarmRoleName = "WebRole1";
            this.PublicUrl = "two10ra.cloudapp.net";
            this.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=two10ra;AccountKey=dmIMUY1mg/qPeOgGmCkO333L26cNcnUA1uMcSSOFMB3cB8LkdDkh02RaYTPLBL8qMqnqazqd6uMxI2bJJEnj0g==";

            Log("ArrWebRole", "OnStart");
            IISconfiguration.UpdateBindingInformation(this.PublicUrl);
            return base.OnStart();
        }

        public override void Run()
        {
            Log("ArrWebRole", "Start");
            IISconfiguration.GetOrCreateFarm("farm");
            while (true)
            {
                try
                {

                    foreach (var role in RoleEnvironment.Roles)
                    {
                        Log("Role", role.Key);

                        foreach (var instance in role.Value.Instances)
                        {
                            Log("Instance", instance.Id);
                            foreach (var endpoint in instance.InstanceEndpoints)
                            {
                                Log("ArrWebRole", endpoint.Key + " = " + endpoint.Value.IPEndpoint.ToString());
                            }
                        }
                    }
                    Log("ArrWebRole", "1");
                    var endpoints = this.Instances.Select(i => i.InstanceEndpoints["Internal"].IPEndpoint).ToArray();
                    IISconfiguration.AddServers("farm", endpoints);
                    Log("ArrWebRole", "2");
                }
                catch (Exception ex)
                {
                    Log("ArrWebRole", ex.ToString());
                }

                Thread.Sleep(60000);
            }
        }

        private void Log(string role, string message)
        {
            using (var logcontext = new AzureSugarTableContext(CloudStorageAccount.Parse(this.StorageConnectionString)))
            {
                logcontext.CreateTable<Log>();
                var log = logcontext.Create<Log>();
                log.Role = role;
                log.Message = message;
            }
        }

    }
}
