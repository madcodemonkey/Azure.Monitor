using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LogAnalyticsDirect
{
    public class LogDirectFunction
    {
        private readonly ILogAnalyticsService _logAnalyticsService;

        public LogDirectFunction(ILogAnalyticsService logAnalyticsService)
        {
            _logAnalyticsService = logAnalyticsService;
        }

        [FunctionName("LogAddress")]
        public async Task<IActionResult> LogAddressAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            Address address = CreateAddress();

            await _logAnalyticsService.SendDataAsync(address);

            return new OkObjectResult("Logged an address!");
        }

        [FunctionName("LogPerson")]
        public async Task<IActionResult> LogPersonAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log) {
            Address address = CreateAddress();
            Person person = CreatePerson(address);

            await _logAnalyticsService.SendDataAsync(person);

            return new OkObjectResult("Logged a person!");
        }


        private Person CreatePerson(Address address)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            var person = new Person
            {
                FirstName = $"FirstName{rand.Next(1, 5000)}",
                LastName =  $"LastName{rand.Next(1, 5000)}",
                Age = rand.Next(12, 65),
                Address = address,
                TimeCreated = DateTime.Now
            };

            return person;
        }

        private Address CreateAddress()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            var address = new Address
            {
                Line1 = $"{rand.Next(1000, 5000)} Happy street",
                Line2 = $"Apartment {rand.Next(1, 500)}",
                City = "Walla Walla",
                State = "Washington",
                PostalCode = "99362",
                TimeCreated = DateTime.Now
            };

            return address;
        }
    }
}
