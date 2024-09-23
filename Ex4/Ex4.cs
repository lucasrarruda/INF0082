using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

class Program
{
    static async Task Main(string[] args)
    {
        // Leitura do número total de pedidos (N), vendedores (V) e entregadores (E)
        string[] entrada = Console.ReadLine().Split();
        int N = int.Parse(entrada[0]);
        int V = int.Parse(entrada[1]);
        int E = int.Parse(entrada[2]);

        // Continue a implementação
        BlockingCollection<Order> orders = [];
        int orderNumber = N / V;
        List<Task> deliveries = [];
        List<Task> vendors = [];

        for (int i = 0; i < E; i++)
        {
            deliveries.Add(new Delivery((i + 1), orders).CurrentTask);
        }

        for (int i = 0; i < V; i++)
        {
            vendors.Add(new Vendor((i + 1), orderNumber, orders).CurrentTask);
        }

        await Task.WhenAll(vendors);
        orders.CompleteAdding();
        await Task.WhenAll(deliveries);
    }

    public static class OrderIDGenerator
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private static int _id = 1;

        public static int GetNewId()
        {
            _semaphore.Wait();
            int newId = _id++;
            _semaphore.Release();
            return newId;
        }
    }

    public class Vendor
    {
        private BlockingCollection<Order> _orders;
        private int _id = 0;
        private int _ordersNumber = 0;
        public Task CurrentTask { get; internal set; }

        public Vendor(int id, int ordersNumber, BlockingCollection<Order> orders)
        {
            _id = id;
            _ordersNumber = ordersNumber;
            _orders = orders;
            CurrentTask = Task.Run(() => Produce());
        }

        private async Task Produce()
        {
            Random randomDelay = new();
            for(int i = 0; i < _ordersNumber; i++)
            {
                Order newOrder = new();
                Console.WriteLine($"Vendedor {_id}: Pedido {newOrder.ID} criado.");
                _orders.Add(newOrder);
                await Task.Delay(randomDelay.Next());
            }
        }
    }

    public class Order
    {
        public int ID { get; internal set; }

        public Order()
        {
            ID = OrderIDGenerator.GetNewId();
        }
    }

    public class Delivery
    {
        private BlockingCollection<Order> _orders;
        private int _id = 0;

        public Task CurrentTask { get; internal set; }

        public Delivery(int id, BlockingCollection<Order> orders)        
        {
            _id = id;
            _orders = orders;
            CurrentTask = Task.Run(() => Deliver());
        }

        private void Deliver()
        {
            foreach (var order in _orders.GetConsumingEnumerable())
            {
                Console.WriteLine($"Entregador {_id}: Pedido {order.ID} entregue.");
            }
        }
    }
}
