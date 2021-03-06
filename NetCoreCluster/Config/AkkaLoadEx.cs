using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using AkkaConfig = Akka.Configuration.Config;

namespace NetCoreCluster.Config
{
    public class AkkaLoadEx
    {
        public static ConcurrentDictionary<string, IActorRef> ActorList = new ConcurrentDictionary<string, IActorRef>();

        public static IActorRef RegisterActor(string name, IActorRef actorRef)
        {
            if (ActorList.ContainsKey(name)) throw new Exception("이미 등록된 액터입니다.");
            ActorList[name] = actorRef;
            return actorRef;
        }

        public static IActorRef ActorSelect(string name)
        {
            if (ActorList.ContainsKey(name))
                return ActorList[name];

            return null;
        }

        public static AkkaConfig Load(string environment)
        {
            if (environment.ToLower() != "production")
            {
                environment = "Development";
            }

            return LoadConfig(environment, "akka{0}.conf");
        }

        private static AkkaConfig LoadConfig(string environment, string configFile)
        {
            string akkaip = Environment.GetEnvironmentVariable("CLUSTER_IP") ?? "127.0.0.1";
            string akkaport = Environment.GetEnvironmentVariable("CLUSTER_PORT") ?? "5100";
            string akkaseed = Environment.GetEnvironmentVariable("CLUSTER_SEEDS") ?? "127.0.0.1:5100";
            string roles = Environment.GetEnvironmentVariable("CLUSTER_ROLES") ?? "akkanet";

            var configFilePath = string.Format(configFile, environment.ToLower() != "production" ? string.Concat(".", environment) : "");
            if (File.Exists(configFilePath))
            {
                string akkaseed_array;
                if (akkaseed.Split(",").Length > 1)
                {
                    akkaseed_array = akkaseed.Split(",").Aggregate("[",
                        (current, seed) => current + @"""" + seed + @""", ");
                    akkaseed_array += "]";
                }
                else
                {
                    akkaseed_array = "[\"" + akkaseed + "\"]";
                }

                string roles_array;
                if (roles.Split(",").Length > 1)
                {
                    roles_array = roles.Split(",").Aggregate("[",
                        (current, role) => current + @"""" + role + @""", ");
                    roles_array += "]";
                }
                else
                {
                    roles_array = "[\"" + roles + "\"]";
                }

                string customConfig = @"
                akka {
	                remote {
		                log-remote-lifecycle-events = debug
                        dot-netty.tcp {
                            port = $akkaport
			                hostname = $akkaip
                        }
                    }
	                cluster {
                        seed-nodes = $akkaseed
                        roles = $roles
                    }
                }    
                ".Replace("$akkaport", akkaport)
                                .Replace("$akkaip", akkaip)
                                .Replace("$akkaseed", akkaseed_array)
                                .Replace("$roles", roles_array);

                AkkaConfig injectedClusterConfigString = customConfig;

                string configText = File.ReadAllText(configFilePath);

                var akkaConfig = ConfigurationFactory.ParseString(configText);

                var finalConfig = injectedClusterConfigString.WithFallback(akkaConfig);

                Console.WriteLine($"=== AkkaConfig:{configFilePath}\r\n{finalConfig}\r\n===");
                return finalConfig;
            }
            return ConfigurationFactory.Empty;
        }
    }
}
