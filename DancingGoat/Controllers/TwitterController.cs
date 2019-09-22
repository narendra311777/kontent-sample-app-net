﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DancingGoat.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DancingGoat.Controllers
{
    public class TwitterController : Controller
    {
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // TODO: Investigate the usage of filters in asp.net core
        // [ChildActionOnly]
        public ActionResult Tweet(Tweet item)
        {
            var selectedTheme = item.Theme.FirstOrDefault()?.Name?.ToLower() ?? "light";
            var displayOptions = item.DisplayOptions.ToList();
            var hideThread = displayOptions.Any(o => o.Codename.Equals("hide_thread"));
            var hideMedia = displayOptions.Any(o => o.Codename.Equals("hide_media"));
            var options = $"&hide_thread={hideThread}&hide_media={hideMedia}";
            var tweetLink = item.TweetLink;

            var tweetResponse = Client.GetAsync(
                $"https://publish.twitter.com/oembed?url={tweetLink}&theme={selectedTheme}" + options).Result;
            var jsonResponse = JObject.Parse(tweetResponse.Content.ReadAsStringAsync().Result);

            return PartialView((object)jsonResponse?.Property("html")?.Value.ToObject<string>());
        }
    }
}