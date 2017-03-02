using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonTypes;
using ConfigService;
using LIFE.API.Layer.Initialization;
using MySql.Data.MySqlClient;

namespace LIFE.Components.Services.AgentManagerService.Implementation
{



    public class AgentParameterFetcher : IAgentParameterFetcher
    {
        private readonly string _connectionString;

        public AgentParameterFetcher() {

            // connect to MARS ROCK
            // create ConfigService and connect to marsconfig container. This is due to convention. This LIFE container
            // should be linked to the marsconfig container and thus marsconfig should lead to the correct ip
            // as per /etc/hosts

            var marsConfigService = new ConfigServiceClient(MARSConfigServiceSettings.Address);
            Console.WriteLine($"AgentManager: Using {MARSConfigServiceSettings.Address} as MARSConfigService address.");

            // retreive ip, port, user and password of mariaDB to us as ROCK instance
            var rockIP = "mariadb"; // use k8s service name
            var rockUser = marsConfigService.Get("rock/serveruser");
            var rockPassword = marsConfigService.Get("rock/serverpassword");

            _connectionString = string.Format("Server={0};Uid={1};Pwd={2};",rockIP,rockUser,rockPassword);

        }

        public ConcurrentDictionary<string, string[]> GetParametersForInitConfig(AgentInitConfig agentInitConfig)
        {
            var agentDbParamArrays = new ConcurrentDictionary<string, string[]>();
            Parallel.ForEach (agentInitConfig.AgentInitParameters, param =>
            {
                if (param.MappingType !=
                    MappingType.ColumnParameterMapping)
                {
                    return;
                }

                // check if we already have this enumerator
                if (agentDbParamArrays.ContainsKey(param.ColumnName)) return;

                var sqlQuery =
                    $"SELECT {param.ColumnName} FROM imports.{param.TableName} LIMIT {agentInitConfig.RealAgentCount} OFFSET {agentInitConfig.AgentInitOffset}";
                try
                {
                    using (var mysqlConnection = new MySqlConnection(_connectionString))
                    {
                        mysqlConnection.Open();
                        var cmd = new MySqlCommand(sqlQuery, mysqlConnection);
                        var values = new List<string>();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // should be first column in result. TODO: Fix this in new MySQL.NET client for dotnet core.
                                values.Add(reader.GetString(0));
                            }
                        }

                        mysqlConnection.Close();
                        agentDbParamArrays.TryAdd(param.ColumnName, values.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occured: {ex.Message}");
                    throw ex;
                }
            });

            return agentDbParamArrays;
        }
    }
}