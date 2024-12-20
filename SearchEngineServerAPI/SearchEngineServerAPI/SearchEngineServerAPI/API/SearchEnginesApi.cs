﻿using Newtonsoft.Json.Linq;
using SearchEngineServerAPI.Controllers;
using SearchEngineServerAPI.Models;
using SerpApi;
using System.Collections;

namespace SearchEngineServerAPI.API;

public class SearchEnginesApi
{
    private readonly HttpClient _client;
    private readonly ILogger<SearchEngineController> _logger;



    public SearchEnginesApi(HttpClient httpClient,ILogger<SearchEngineController> logger)
    {
        _client = httpClient;
        _logger = logger;   
    }

    string apiKey = "17b45974d83f633e2b5dddbd7a2b0853f2ea22688f655a09bb1e205184c52036"; //TODO - env file
    Hashtable ht = new Hashtable();

    public async Task<List<SearchResult>> PostDataFromSearchEngineExternal(string engineName, string query)
    {
        var numOrCount = engineName == "google" ? "num" : "count";
        ht.Add("engine", engineName);
        ht.Add("q", query);
        ht.Add(numOrCount, "5");

        try
        {
            GoogleSearch search = new GoogleSearch(ht, apiKey);
            JObject data = search.GetJson();
            var organic_results = data["organic_results"];

            return organic_results.Select(res => new SearchResult()
            {
                SearchEngineType = engineName,
                Title = res.Value<string>("title"),
                Link = res.Value<string>("link"),
                Snippet = res.Value<string>("snippet")
            }).ToList();
        }
        catch (SerpApiSearchException ex)
        {
            _logger.LogError("Error while fetching from SerpApi. Serp Exception " + ex);
            return new List<SearchResult>();
        }
    }

}



