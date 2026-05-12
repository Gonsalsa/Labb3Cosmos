using Labb3Cosmos.Data.Enteties;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Labb3Cosmos.Service;

public class CosmosService
{
    public readonly Microsoft.Azure.Cosmos.Container container;

    public CosmosService()
    {
        var connString = Environment.GetEnvironmentVariable("CosmosDbConnection");
        var dbName = Environment.GetEnvironmentVariable("CosmosDbName");
        var containerName = Environment.GetEnvironmentVariable("CosmosDbContainer");

        var options = new CosmosClientOptions
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),

            ConnectionMode = ConnectionMode.Gateway
        };

        var client = new CosmosClient(connString, options);
        container = client.GetContainer(dbName, containerName);
    }


    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        customer.Id = Guid.NewGuid().ToString();
        customer.CreatedAt = DateTime.Now;
        customer.UpdatedAt = DateTime.Now;

        var response = await container.CreateItemAsync(customer, new PartitionKey(customer.Id));
        return response.Resource;
    }

    public async Task<Customer?> GetCustomerAsync(string id)
    {
        try
        {
            var response = await container.ReadItemAsync<Customer>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        var query = container.GetItemLinqQueryable<Customer>().ToFeedIterator();
        var result = new List<Customer>();

        while (query.HasMoreResults)
            result.AddRange(await query.ReadNextAsync());

        return result;
    }


    public async Task<Customer?> UpdateCustomerAsync(string id, Customer update)
    {
        var existing = await GetCustomerAsync(id);
        if (existing == null)
            return null;

        update.Id = id;
        update.CreatedAt = DateTime.Now;
        update.UpdatedAt = DateTime.Now;

        var response = await container.ReplaceItemAsync(update, id, new PartitionKey(id));
        return response.Resource;
    }

    public async Task<bool> DeleteCustomerAsync(string id)
    {
        try
        {
            await container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<List<Customer>> SearchCustomerNameAsync(string name)
    {
        var query = container
            .GetItemLinqQueryable<Customer>()
            .Where(c => c.Name.ToLower().Contains(name.ToLower()))
            .ToFeedIterator();

        var result = new List<Customer>();
        
        while(query.HasMoreResults)
            result.AddRange(await query.ReadNextAsync());

        return result;
    }

    public async Task<List<Customer>> SearchSalespersonNameAsync(string name)
    {
        var query = container
            .GetItemLinqQueryable<Customer>()
            .Where(c => c.Salesperson.Name.ToLower().Contains(name.ToLower()))
            .ToFeedIterator();

        var result = new List<Customer>();

        while (query.HasMoreResults)
            result.AddRange(await query.ReadNextAsync());

        return result;
    }



}
