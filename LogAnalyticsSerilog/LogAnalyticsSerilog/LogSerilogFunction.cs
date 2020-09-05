using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace LogAnalyticsSerilog
{
    public class LogSerilogFunction
    {
        private readonly IDoSomeMath _doSomeMath;

        /// <summary>Constructor</summary>
        public LogSerilogFunction(IDoSomeMath doSomeMath)
        {
            _doSomeMath = doSomeMath;

        }

        [FunctionName("LogAddress")]
        public IActionResult LogAddressAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            Address address = CreateAddress();

            log.LogInformation("This is an address {@TheAddress}", address);

            return new OkObjectResult("Logged an address!");
        }

        [FunctionName("LogPerson")]
        public IActionResult LogPersonAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            Address address = CreateAddress();
            Person person = CreatePerson(address);

            log.LogInformation("This is an person {@ThePerson}", person);

            return new OkObjectResult("Logged a person!");
        }


        [FunctionName("LoggerTests")]
        public IActionResult LoggerTesting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            // https://docs.microsoft.com/en-us/azure/azure-functions/functions-host-json#logging
            log.LogDebug("This is a LogDebug message.");
            log.LogDebug("This is {myCount} a LogDebug message using structured logging", 42);

            log.LogInformation("This is a LogInformation message.");
            log.LogInformation("This is {myCount} a LogInformation message using structured logging", 42);

            log.LogWarning("This is a LogWarning message.");
            log.LogWarning("This is {myCount} a LogWarning message using structured logging", 42);

            log.LogError("This is a LogError message.");
            log.LogError("This is {myCount} a LogError message using structured logging", 42);
            log.LogError(new ArgumentException("Outer exception here", new FileNotFoundException("Inner exception", "SomeFileName.xml")), "This is a LogError with an exception");

            log.LogCritical("This is a LogCritical message.");
            log.LogCritical("This is {myCount} a LogLogCriticalError message using structured logging", 42);
            log.LogCritical(new ArgumentException("Outer exception here", new FileNotFoundException("Inner exception", "SomeFileName.xml")), "This is a LogCritical with an exception");

            return new OkObjectResult("ILogger tests ran");
        }

        [FunctionName("LoggingWithScope")]
        public IActionResult LoggingWithScopeTesting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            using (log.BeginScope("MyScope is great!"))
            {
                log.LogInformation("Something happened.");
                log.LogInformation("Something else happened {myCount} times!", 78);
                log.LogCritical(new ArgumentException("Outer scope exception here", new FileNotFoundException("Inner scope exception", "DatFile.dat")), "Scope! This is a LogCritical with an exception");
            }
            
            return new OkObjectResult("ILogger logging with scope ran");
        }



        [FunctionName("ExceptionTest")]
        public IActionResult ExceptionTesting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            decimal answer = _doSomeMath.DivideTenByInput(0);

            return new OkObjectResult($"Exception test should have failed, but returned {answer} instead!");
        }

        private Person CreatePerson(Address address)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            var person = new Person
            {
                FirstName = rand.Next(1, 100) > 50 ? "John" : "Jane",
                LastName = $"LastName{rand.Next(1, 5000)}",
                Age = rand.Next(12, 95),
                // Age = new Age(rand.Next(12, 95)),
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
