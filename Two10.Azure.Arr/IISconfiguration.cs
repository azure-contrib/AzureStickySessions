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

using Microsoft.Web.Administration;
using System;
using System.Net;

namespace Two10.Azure.Arr
{
    static class IISconfiguration
    {

        public static ConfigurationElement GetOrCreateFarm(string farmName)
        {

            using (var serverManager = new ServerManager())
            {
                var config = serverManager.GetApplicationHostConfiguration();
                var webFarmsSection = config.GetSection("webFarms");
                var webFarmsCollection = webFarmsSection.GetCollection();

                var el = FindElement(webFarmsCollection, "webFarm", "name", farmName);
                if (null != el)
                {
                    return el;
                }

                var webFarmElement = webFarmsCollection.CreateElement("webFarm");
                webFarmElement["name"] = farmName;
                webFarmsCollection.Add(webFarmElement);

                var applicationRequestRoutingElement = webFarmElement.GetChildElement("applicationRequestRouting");

                var affinityElement = applicationRequestRoutingElement.GetChildElement("affinity");
                affinityElement["useCookie"] = true;

                serverManager.CommitChanges();

                CreateReWriteRule(farmName);

                return webFarmElement;
            }
        }


        public static void AddServers(string farmName, IPEndPoint[] endpoints)
        {

            using (var serverManager = new ServerManager())
            {
                var config = serverManager.GetApplicationHostConfiguration();
                var webFarmsSection = config.GetSection("webFarms");
                var webFarmsCollection = webFarmsSection.GetCollection();
                var webFarmElement = FindElement(webFarmsCollection, "webFarm", "name", farmName);
                if (webFarmElement == null) throw new InvalidOperationException("Element not found!");

                var webFarmCollection = webFarmElement.GetCollection();
                foreach (var endpoint in endpoints)
                {
                    var server = FindElement(webFarmCollection, "server", "address", endpoint.Address.ToString());
                    if (null != server)
                    {
                        // server already exists
                        continue;
                    }

                    var serverElement = webFarmCollection.CreateElement("server");
                    serverElement["address"] = endpoint.Address.ToString();

                    var applicationRequestRoutingElement = serverElement.GetChildElement("applicationRequestRouting");
                    applicationRequestRoutingElement["httpPort"] = endpoint.Port;
                    webFarmCollection.Add(serverElement);
                }
                serverManager.CommitChanges();
            }
        }

        private static ConfigurationElement FindElement(ConfigurationElementCollection collection, string elementTagName, params string[] keyValues)
        {
            foreach (var element in collection)
            {
                if (String.Equals(element.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
                {
                    var matches = true;

                    for (var i = 0; i < keyValues.Length; i += 2)
                    {
                        var o = element.GetAttributeValue(keyValues[i]);
                        string value = null;
                        if (o != null)
                        {
                            value = o.ToString();
                        }

                        if (!String.Equals(value, keyValues[i + 1], StringComparison.OrdinalIgnoreCase))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return element;
                    }
                }
            }
            return null;
        }


        private static void CreateReWriteRule(string serverFarm)
        {

            using (var serverManager = new ServerManager())
            {
                var config = serverManager.GetApplicationHostConfiguration();
                var globalRulesSection = config.GetSection("system.webServer/rewrite/globalRules");
                var globalRulesCollection = globalRulesSection.GetCollection();

                var ruleElement = globalRulesCollection.CreateElement("rule");
                ruleElement["name"] = serverFarm;
                ruleElement["patternSyntax"] = @"Wildcard";
                ruleElement["stopProcessing"] = true;

                var matchElement = ruleElement.GetChildElement("match");
                matchElement["url"] = @"*";
                matchElement["ignoreCase"] = false;

                var actionElement = ruleElement.GetChildElement("action");
                actionElement["type"] = @"Rewrite";
                actionElement["url"] = string.Concat(@"http://", serverFarm, @"/{R:0}");
                globalRulesCollection.Add(ruleElement);

                serverManager.CommitChanges();
            }
        }

    }
}
