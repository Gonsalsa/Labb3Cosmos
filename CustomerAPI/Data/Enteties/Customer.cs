using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace CustomerAPI.Data.Enteties;

public class Customer
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("adress")]
    public string Adress {  get; set; } = string.Empty;

    [JsonPropertyName("salesperson")]
    public Salesperson Salesperson { get; set; } = new();



    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }


    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt {  get; set; } = DateTime.Now;

}
