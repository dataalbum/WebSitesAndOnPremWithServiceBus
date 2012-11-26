using System;
using WebSitesWithOnPremIntegration.Core;

namespace WebSitesWithOnPremIntegration.ServiceBusListener
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBusHelper
                .Setup()
                .Subscribe<Customer>((customer) =>
                {
                    CustomerService.Add(customer);

                    Console.WriteLine("Customer {0} from {1}, {2} saved",
                        customer.Name,
                        customer.City,
                        customer.Country);
                });
        }
    }
}
