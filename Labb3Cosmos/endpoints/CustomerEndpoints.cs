using Labb3Cosmos.Data.Enteties;
using Labb3Cosmos.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.StaticAssets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;

namespace Labb3Cosmos.Controllers;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(IHost host)
    {

        var app = host.Services.GetRequiredService<IApplicationBuilder>()
                    as WebApplication;

        //Create

        app!.MapPost("/api/customer", async (
            Customer customer, CosmosService db) =>
        {
            var created = await db.CreateCustomerAsync(customer);
            return Results.Created($"/api/customers/{created.Id}", created);
        });


        //Read

        app.MapGet("api/customers", async (CosmosService db) =>
            Results.Ok(await db.GetAllCustomersAsync()));

        app.MapGet("api/customer/{id}", async (
            string id, CosmosService db) =>
        {
            var customer = await db.GetCustomerAsync(id);

            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });

        app.MapGet("api/customers/search", async (
            string? customerName,
            string? salespersonName,
            CosmosService db) =>
        {
            if (!string.IsNullOrEmpty(customerName))
                return Results.Ok(await db.SearchCustomerNameAsync(customerName));

            if (!string.IsNullOrEmpty(salespersonName))
                return Results.Ok(await db.SearchSalespersonNameAsync(salespersonName));

            return Results.BadRequest("Please enter a customer or salespersons name");

        });


        //Update

        app.MapPut("api/customers/{id}", async (
            string id, Customer updated, CosmosService db) =>
        {
            var result = await db.UpdateCustomerAsync(id, updated);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        });


        //Delete

        app.MapDelete("api/customers/{id}", async (
            string id, CosmosService db) =>
        {
            var deleted = await db.DeleteCustomerAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        });

    }
}

