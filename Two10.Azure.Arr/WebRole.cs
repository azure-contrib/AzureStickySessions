#region Copyright (c) 2011 two10 degrees
//
// (C) Copyright 2011 two10 degrees
//      All rights reserved.
//
// This software is provided "as is" without warranty of any kind,
// express or implied, including but not limited to warranties as to
// quality and fitness for a particular purpose. Active Web Solutions Ltd
// does not support the Software, nor does it warrant that the Software
// will meet your requirements or that the operation of the Software will
// be uninterrupted or error free or that any defects will be
// corrected. Nothing in this statement is intended to limit or exclude
// any liability for personal injury or death caused by the negligence of
// Active Web Solutions Ltd, its employees, contractors or agents.
//
#endregion

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
