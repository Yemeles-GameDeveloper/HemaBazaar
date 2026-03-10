﻿using System.Text.Json.Serialization;

namespace HemaBazaar.MVC.Models
{
    public class JwtTokenResponseModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expireDate")]
        public DateTime ExpireDate { get; set; }
    }
}
