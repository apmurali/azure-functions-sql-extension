// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.Sql.SamplesOutOfProc.Common;
namespace Microsoft.Azure.WebJobs.Extensions.Sql.SamplesOutOfProc
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "addproduct")]
            [FromBody] Product prod,
            [Sql("dbo.Products", ConnectionStringSetting = "SqlConnectionString")] out Product product)
        {
            product = prod;
            return new CreatedResult($"/api/addproduct", product);
        }
    }
}
