using System;
using System.Collections.Generic;
using Azure.Identity;
using CustomerAPI.Data.Enteties;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Labb3Cosmos.Functions;

public class CustomerChangeFeed
{
    private readonly ILogger<CustomerChangeFeed> _logger;

    public CustomerChangeFeed(ILogger<CustomerChangeFeed> logger)
      => _logger = logger;


    [Function("CustomerChangeFeed")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "%CosmosDbName%",
        containerName: "%CosmosDbContainer%",
        Connection = "CosmosDbConnection",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Customer> input)
    {
        if (input == null || input.Count == 0)
            return;


        foreach (var customer in input)
        {
            _logger.LogInformation($"Change detected for customer: {customer.Name}");

            await SendEmailAsync(customer);
        }

        
    }

    private async Task SendEmailAsync(Customer customer)
    {
        var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        var client = new SendGridClient(apiKey);

        var from = Environment.GetEnvironmentVariable("EmailFrom");
        var to = Environment.GetEnvironmentVariable("EmailTo");

        var message = new SendGridMessage
        {
            From = new EmailAddress(from, "CRM System"),
            Subject = $"Kunduppdatering: {customer.Name}",
            HtmlContent = $"""
                <h2>CRM-system: Kunduppdatering</h2>
                <p>Hej {customer.Salesperson.Name},</p>
                <p>Du är ansvarig for följande kund:</p>
                <table border='1' cellpadding='8' style='border-collapse:collapse'>
                <tr><td><b>Namn</b></td>
                <td>{customer.Name}</td></tr>
                <tr><td><b>Titel</b></td>
                <td>{customer.Title}</td></tr>
                <tr><td><b>Telefon</b></td>
                <td>{customer.PhoneNumber}</td></tr>
                <tr><td><b>Email</b></td>
                <td>{customer.Email}</td></tr>
                <tr><td><b>Adress</b></td>
                <td>{customer.Adress}</td></tr>
                </table>
                <p>Vänliga hälsningar,<br/>CRM-systemet</p>
                """
        };

        message.AddTo(new EmailAddress(to));

        var response = await client.SendEmailAsync(message);

        if (response.IsSuccessStatusCode)
            _logger.LogInformation($"Email sent to {to}");
        else
            _logger.LogError($"Email failed. Status: {response.StatusCode}");
    }
}

