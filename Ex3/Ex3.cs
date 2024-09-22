using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {

        // Obter o número de pedidos (P) e de cozinheiros (C)
        string[] entrada = Console.ReadLine().Split();
        int P = int.Parse(entrada[0]);
        int C = int.Parse(entrada[1]);

        var pedidos = new List<(string prato, int tempo)>();

        for (int i = 0; i < P; i++)
        {
            string[] pedido = Console.ReadLine().Split(',');
            string prato = pedido[0].Trim();
            int tempo = int.Parse(pedido[1].Trim());
            pedidos.Add((prato, tempo));
        }

        // Continue a implementação
        List<Chef> chefs = new();
        for(int i = 0; i < C; i++)
        {
            chefs.Add(new Chef());
        }

        Manager manager = new(chefs);
        List<Task> waitingOrder = new();

        foreach(var orderRequested in pedidos)
        {
            Order order = new()
            {
                Name = orderRequested.prato,
                PreparationTime = orderRequested.tempo
            };
            waitingOrder.Add(manager.RequestOrder(order));
        }

        await Task.WhenAll(waitingOrder);
    }

    public class Order
    {
        public required string Name { get; set; }
        public required int PreparationTime { get; set; }
    }

    public class Chef
    {
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public async Task Cook(Order order)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                Console.WriteLine($"O prato '{order.Name}' começou a ser preparado.");
                await Task.Delay(order.PreparationTime);
                Console.WriteLine($"O prato '{order.Name}' está pronto.");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }

    public class Manager
    {
        private List<Chef> _chefs;
        private int _chefRound = 0;

        public Manager(List<Chef> chefs)
        {
            _chefs = chefs;
        }

        public async Task RequestOrder(Order order)
        {
            Chef selectedChef = _chefs[_chefRound];
            
            _chefRound = (_chefRound + 1) % _chefs.Count;   

            await selectedChef.Cook(order);
        }
    }
}
