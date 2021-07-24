﻿using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog
{
    public class BlogPostAccess
    {
        [JsonPropertyName("date")] public DateTime Date { get; set; }

        [JsonIgnore] public string DateString => Date.ToString("yyyy-MM");

        [JsonPropertyName("count")] public int Count { get; set; }
    }
}