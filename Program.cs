/*
accountEndpoint and authKeyOrResourceToken must be updated in CosmosHandler() with your account info.

GetContainerAsync() GetDatabase() method must be updated with your database name.

ContainerProperties() must be updated with your container name and partition key.

Item class should match the schema of your document.

Run with either Block A or Block B uncommented but not both. Block B requires a continuation token that can be stored after running Block A once.
*/
namespace CosmosDbDotNetAllVersionsChangeFeed {

    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    class Program {
        public static async Task Main(string[] args) {
            Container container = await CosmosHandler.GetContainerAsync();

            // Block A
            // ChangeFeedStartFrom Now
            // FeedIterator<VersionsAndDeletesResponse> iteratorForTheEntireContainer = 
            // container.GetChangeFeedIterator<VersionsAndDeletesResponse>(
            //     ChangeFeedStartFrom.Now(), ChangeFeedMode.AllVersionsAndDeletes);

            // Block B
            // ChangeFeedStartFrom Continuation Token
            // string allVersionsContinuationToken = "";
            // FeedIterator<VersionsAndDeletesResponse> iteratorForTheEntireContainer = 
            //     container.GetChangeFeedIterator<VersionsAndDeletesResponse>(
            //         ChangeFeedStartFrom.ContinuationToken(allVersionsContinuationToken), ChangeFeedMode.AllVersionsAndDeletes);

            while (iteratorForTheEntireContainer.HasMoreResults)
            {
                FeedResponse<VersionsAndDeletesResponse> response = await iteratorForTheEntireContainer.ReadNextAsync();

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Console.WriteLine($"No new changes");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                else 
                {
                    foreach (VersionsAndDeletesResponse d in response)
                    {
                        Console.WriteLine($"Detected change {d}");
                    }
                }
            }
        }
    }
    public static class CosmosHandler { 
        public static readonly CosmosClient _client;

        static CosmosHandler() { 
            _client = new CosmosClient(
                // Cosmos DB Account Endpoint URI
                accountEndpoint: "",
                // Cosmos DB Primary key 
                authKeyOrResourceToken: ""
            );
        }

        public static async Task<Container> GetContainerAsync() { 
            Database database = _client.GetDatabase("");

            ContainerProperties properties = new(
                id: "",
                partitionKeyPath : ""
            );

            return await database.CreateContainerIfNotExistsAsync(
                containerProperties: properties
            );
        
        }
    }
    public class VersionsAndDeletesResponse {
        [JsonProperty("current")]
        public Item Current { get; set; }

        [JsonProperty("previous")]
        public Item Previous { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
    public class Item{
        [JsonProperty("id")]
        public string? Id { get; set; }

        public string? Value { get; set; }
        public string? Time {get;set;}
    }
    public class Metadata{
        [JsonProperty("operationType")]
        public string? OperationType { get; set; }

        [JsonProperty("timeToLiveExpired")]
        public Boolean TimeToLiveExpired { get; set; }
        }
}

