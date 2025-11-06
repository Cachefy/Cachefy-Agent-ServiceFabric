using Agent.Models;
using System.Fabric;
using System.Text.Json;

namespace Agent.Services
{
    public class AgentService : IAgentService
    {
        private readonly FabricClient _fabricClient;

        public AgentService()
        {
            _fabricClient = new FabricClient();
        }

        public async Task<List<ServiceFabricAgentResponse>> SendOperation(string serviceName, Func<string, Task<ServiceFabricAgentResponse>> operation, string? id = null)
        {
            Uri applicationNameFilter = new Uri($"fabric:{serviceName}");

            List<ServiceFabricAgentResponse> results = new();

            // get partitions
            var partitions = await _fabricClient.QueryManager.GetPartitionListAsync(applicationNameFilter);

            foreach (var partition in partitions)
            {
                var replicas = await _fabricClient.QueryManager.GetReplicaListAsync(partition.PartitionInformation.Id);

                foreach (var replica in replicas)
                {
                    if (!string.IsNullOrEmpty(id) && replica.NodeName != id)
                    {
                        continue;
                    }

                    var nodes = await _fabricClient.QueryManager.GetNodeListAsync(replica.NodeName);
                    var node = nodes.FirstOrDefault()!;

                    // This gives you address JSON with listener names and URLs
                    var doc = JsonDocument.Parse(replica.ReplicaAddress);

                    var endpoints = doc.RootElement.GetProperty("Endpoints");

                    var replicaEndpoint = endpoints.EnumerateObject().FirstOrDefault();

                    var replicaUrl = replicaEndpoint.Value.GetString();

                    if (Uri.TryCreate(replicaUrl, UriKind.Absolute, out var uri))
                    {
                        var serviceUrl = $"http://{node.IpAddressOrFQDN}:{uri.Port}";

                        var result = await operation(serviceUrl);

                        if (result == null)
                        {
                            result = new ServiceFabricAgentResponse();
                        }

                        result.Id = node.NodeName;

                        result.ParametersDetails = new List<ParametersDetails>
                        {
                            new()
                            {
                                Name = "Partition details",
                                Parameters = new Dictionary<string, string> {
                                 { "Port",  uri.Port.ToString()},
                                 { "Health State",  partition.HealthState.ToString()},
                                }
                            },
                            new()
                            {
                                Name = "Replica details",
                                Parameters = new Dictionary<string, string> {
                                 { "Address",  uri.AbsoluteUri},
                                 { "Health State",  replica.HealthState.ToString()},
                            }
                            },
                            new()
                            {
                                Name = "Node details",
                                Parameters = new Dictionary<string, string> {
                                 { "Name", node.NodeName},
                                 { "Ip", node.IpAddressOrFQDN },
                                 { "Health State", node.HealthState.ToString()},
                                 { "Code version", node.CodeVersion.ToString()},
                            }
                            }
                        };

                        results.Add(result!);
                    }
                }
            }

            return results;
        }
    }
}
