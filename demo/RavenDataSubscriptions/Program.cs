using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace RavenDataSubscriptions
{
    class Order
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public bool Processed { get; set; }
    }

    class Program
    {
        private static IDocumentStore DocumentStore { get; set; }
        private static IDocumentSession DocumentSession { get; set; }

        static void Main(string[] args)
        {
            using (var store = new DocumentStore {ConnectionStringName = "RavenDB"}.Initialize())
            {
                CreateDocuments(store);
                DocumentStore = store;

                var id = 0L;
                using (var session = store.OpenSession())
                {
                    var subscriptionInfo = session.Load<SubscriptionInfo>(SubscriptionInfo.DocumentId);
                    if (subscriptionInfo == null)
                    {
                        subscriptionInfo = new SubscriptionInfo();
                        subscriptionInfo.SubscriptionId = CreateSubscription(store);
                        session.Store(subscriptionInfo);
                        session.SaveChanges();
                    }

                    id = subscriptionInfo.SubscriptionId;
                }
                
                var orders = store.Subscriptions.Open<Order>(id, new SubscriptionConnectionOptions()
                {
                    BatchOptions = new SubscriptionBatchOptions
                    {
                        MaxDocCount = 16 * 1024,
                        MaxSize = 4 * 1024 * 1024,
                        AcknowledgmentTimeout = TimeSpan.FromMinutes(3)
                    },
                    IgnoreSubscribersErrors = false,
                    ClientAliveNotificationInterval = TimeSpan.FromSeconds(30),
                    Strategy = SubscriptionOpeningStrategy.WaitForFree
                });

                orders.BeforeBatch += Orders_BeforeBatch;
                orders.BeforeAcknowledgment += Orders_BeforeAcknowledgment;
                orders.AfterAcknowledgment += Orders_AfterAcknowledgment;
                orders.AfterBatch += Orders_AfterBatch;

                orders.Subscribe(x =>
                {
                    SendOrderReceivedEmail(x);
                    DocumentSession.Store(x);
                    x.Processed = true;

                    Console.WriteLine($"Processed {x.Id} {x.Customer}");
                });

                var configs = store.Subscriptions.GetSubscriptions(start: 0, take: 10);

                Console.ReadLine();
                orders.Dispose();
                store.Subscriptions.Release(id);
                store.Subscriptions.Delete(id);
                Console.WriteLine("subscription deleted");
                Console.ReadLine();
                store.DatabaseCommands.GlobalAdmin.DeleteDatabase(databaseName: "SubscriptionsTest", hardDelete: true);
            }
        }

        private static void Orders_BeforeBatch()
        {
            DocumentSession = DocumentStore.OpenSession();
            Console.WriteLine("Before batch");
        }

        private static bool Orders_BeforeAcknowledgment()
        {
            Console.WriteLine("Before ack");
            DocumentSession.SaveChanges();
            return true;
        }

        private static void Orders_AfterAcknowledgment(Etag lastProcessedEtag)
        {
            Console.WriteLine($"last etag: {lastProcessedEtag}");
        }

        private static void Orders_AfterBatch(int documentsProcessed)
        {
            Console.WriteLine($"processed {documentsProcessed} documents");
            DocumentSession?.Dispose();
        }

        private static void SendOrderReceivedEmail(Order order)
        {
            //send email
        }

        private static long CreateSubscription(IDocumentStore store)
        {
            return store.Subscriptions.Create(new SubscriptionCriteria<Order>
            {
                KeyStartsWith = "orders/",
                PropertiesMatch = new Dictionary<Expression<Func<Order, object>>, RavenJToken>()
                {
                    {x => x.Processed, false}
                },
                PropertiesNotMatch = new Dictionary<Expression<Func<Order, object>>, RavenJToken>()
                {
                    {x => x.Customer, "Fred Doe"}
                },
                StartEtag = Etag.Empty
            });
        }

        private static void CreateDocuments(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                session.Store(new Order
                {
                    Id = "orders/1",
                    Customer = "John Doe",
                    Amount = 10.01m,
                    Email = "john@doe.com"
                });

                session.Store(new Order
                {
                    Id = "orders/2",
                    Customer = "Jane Doe",
                    Amount = 10.02m,
                    Email = "jane@doe.com"
                });

                session.Store(new Order
                {
                    Id = "orders/3",
                    Customer = "Fred Doe",
                    Amount = 10.03m,
                    Email = "fred@doe.com"
                });

                /*for (int i = 4; i < 1000; i++)
                {
                    session.Store(new Order
                    {
                        Id = $"orders/{i}",
                        Customer = $"Robot {i}",
                        Amount = 10.15m
                    });
                }*/

                session.SaveChanges();
            }
        }
    }

    class SubscriptionInfo
    {
        public const string DocumentId = "admin/subscription-info";
        public string Id = DocumentId;
        public long SubscriptionId { get; set; }
    }
}