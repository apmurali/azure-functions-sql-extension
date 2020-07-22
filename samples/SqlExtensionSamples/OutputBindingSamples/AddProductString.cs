﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using static SqlExtensionSamples.ProductUtilities;


namespace SqlExtensionSamples.OutputBindingSamples
{
    public static class AddProductString
    {
        [FunctionName("AddProductString")]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addproduct-string")]
            HttpRequest req,
        [Sql("Products", ConnectionStringSetting = "SQLServerAuthentication")] out string output)
        {
            var product = new Product
            {
                Name = req.Query["name"],
                ProductID = int.Parse(req.Query["id"]),
                Cost = int.Parse(req.Query["cost"])
            };
            output = product.toString();
            return new CreatedResult($"/api/addproduct", product.toString());
        }
    }
}
