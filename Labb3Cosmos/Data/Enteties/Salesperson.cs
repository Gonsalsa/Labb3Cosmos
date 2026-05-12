using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Labb3Cosmos.Data.Enteties;

public class Salesperson
{
    [JsonPropertyName("name")]
    public string Name { get; set;  } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber {  get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
