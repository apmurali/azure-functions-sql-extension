// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Sql.Samples.Common;
using Microsoft.Azure.WebJobs.Extensions.Sql.Samples.InputBindingSamples;
using Microsoft.Azure.WebJobs.Extensions.Sql.Tests.Integration;
using Xunit;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace Microsoft.Azure.WebJobs.Extensions.Sql.Tests.Performance
{
    [Collection("Performanetests")]
    public class SqlInputBindingPerformanceTests : IntegrationTestBase
    {

        public SqlInputBindingPerformanceTests() : base()
        {
        }

        private async Task<HttpResponseMessage> SendInputRequest(string functionName, string query = "")
        {
            string requestUri = $"http://localhost:{this.Port}/api/{functionName}/{query}";

            return await this.SendGetRequest(requestUri);
        }

        [Benchmark]
        public async Task<HttpResponseMessage> GetProductsTest()
        {
            int n = 0;
            int cost = 100;
            this.StartFunctionHost(nameof(GetProducts));

            // Generate T-SQL to insert n rows of data with cost
            Product[] products = GetProductsWithSameCost(n, cost);
            this.InsertProducts(products);

            // Run the function
            return await this.SendInputRequest("getproducts", cost.ToString());
        }

        private static Product[] GetProductsWithSameCost(int n, int cost)
        {
            var result = new Product[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = new Product
                {
                    ProductID = i,
                    Name = "test",
                    Cost = cost
                };
            }
            return result;
        }

        private void InsertProducts(Product[] products)
        {
            if (products.Length == 0)
            {
                return;
            }

            var queryBuilder = new StringBuilder();
            foreach (Product p in products)
            {
                queryBuilder.AppendLine($"INSERT INTO dbo.Products VALUES({p.ProductID}, '{p.Name}', {p.Cost});");
            }

            this.ExecuteNonQuery(queryBuilder.ToString());
        }
    }

    public class Program
    {
        public static void Main(string[] _)
        {
            BenchmarkRunner.Run<SqlInputBindingPerformanceTests>(
                ManualConfig
                    .Create(DefaultConfig.Instance)
                    .WithOptions(ConfigOptions.DisableOptimizationsValidator));
        }
    }
}
