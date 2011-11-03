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
using System.Configuration;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Two10.Azure.Arr
{
    public class WebRole : RoleEntryPoint
    {
        public string RoleName
        {
            get
            {
                return ConfigurationManager.AppSettings["RoleName"];
            }
        }

        public string PublicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["PublicUrl"];
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
            IISconfiguration.UpdateBindingInformation(this.PublicUrl);
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
            System.Diagnostics.Trace.Write(message);
        }

    }
}
