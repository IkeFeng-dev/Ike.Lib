using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test.Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Type netFwPolicy2Type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(netFwPolicy2Type);
            INetFwRule newRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            newRule.Name = "Open Port 12345";
            newRule.Description = "Allow inbound traffic through port 12345";
            newRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            newRule.LocalPorts = "12345";
            newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            newRule.Enabled = true;
            newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            newRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;

            fwPolicy2.Rules.Add(newRule);


            Console.WriteLine("Port 12345 is now open for inbound traffic.");
            //while (true)
            //{
            //    Console.WriteLine("Enter port number:");
            //    string port = Console.ReadLine();
            //    Type netFwPolicy2Type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            //    INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(netFwPolicy2Type);

            //    foreach (INetFwRule rule in fwPolicy2.Rules)
            //    {
            //        if (rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN && rule.Enabled)
            //        {
            //            if (rule.Protocol == (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP &&
            //                rule.LocalPorts != null && rule.LocalPorts.Contains(port))
            //            {
            //                Console.WriteLine("Found Rule: " + rule.Name);
            //                Console.WriteLine("Description: " + rule.Description);
            //                Console.WriteLine("serviceName: " + rule.serviceName);
            //                Console.WriteLine("ApplicationName: " + rule.ApplicationName);
            //                Console.WriteLine("Action: " + rule.Action);
            //                Console.WriteLine("Direction: " + rule.Direction);
            //                Console.WriteLine("EdgeTraversal: " + rule.EdgeTraversal);
            //                Console.WriteLine("Enabled: " + rule.Enabled);
            //                Console.WriteLine("Grouping: " + rule.Grouping);
            //                Console.WriteLine("IcmpTypesAndCodes: " + rule.IcmpTypesAndCodes);
            //                Console.WriteLine("InterfaceTypes: " + rule.InterfaceTypes);
            //                Console.WriteLine("LocalPorts: " + rule.LocalPorts);
            //                Console.WriteLine("LocalAddresses: " + rule.LocalAddresses);
            //                Console.WriteLine("RemotePorts: " + rule.RemotePorts);
            //                Console.WriteLine("RemoteAddresses: " + rule.RemoteAddresses);
            //                Console.WriteLine("Profiles: " + rule.Profiles);
            //                Console.WriteLine("Protocol: " + rule.Protocol);
            //                Console.WriteLine("==================================================");
            //            }
            //        }
            //    }
            //}
        }
    }
}
