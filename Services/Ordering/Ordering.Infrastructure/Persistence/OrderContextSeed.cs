using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() 
                {
                    UserName = "swn",
                    FirstName = "Mehmet",
                    LastName = "Ozkaya",
                    EmailAddress = "ezozkme@gmail.com",
                    AddressLine = "Bahcelievler",
                    Country = "Turkey",
                    TotalPrice = 350,
                    CVV="123",
                    CardName="Luis Coco Enriquez",
                    CardNumber="1234 1234 1234 1234",
                    Expiration="03/23",
                    LastModifiedBy="Luis",
                    State="Spain",
                    ZipCode="28002"
                }
            };
        }
    }
}
