using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // Obter a capacidade do estacionamento (C) e o número de veículos (V)
        string[] entrada = Console.ReadLine().Split();
        int C = int.Parse(entrada[0]);
        int V = int.Parse(entrada[1]);

        // Continue a implementação
        ParkingLot parkingLot = new ParkingLot(C);

        Task[] tasks = new Task[V];
        for (int i = 0; i < V; i++)
        {
            Vehicle vehicle = new Vehicle(i + 1, parkingLot);
            tasks[i] = Task.Run(() => vehicle.TryPark());
        }

        Task.WaitAll(tasks);
    }

    class ParkingLot
    {
        private SemaphoreSlim semaphore;
        private int availableSpaces;
        private readonly object lockObj = new object();

        public ParkingLot(int capacity)
        {
            availableSpaces = capacity;
            semaphore = new SemaphoreSlim(capacity, capacity);
        }

        public async Task<bool> Enter(Vehicle vehicle)
        {
            Console.WriteLine($"Veículo {vehicle.Id} esperando para entrar...");

            if (await semaphore.WaitAsync(1000))
            {
                lock (lockObj)
                {
                    availableSpaces--;
                    Console.WriteLine($"Veículo {vehicle.Id} entrou. Vagas disponíveis: {availableSpaces}");
                }
                return true;
            }
            else
            {
                Console.WriteLine($"Veículo {vehicle.Id} não conseguiu vaga e vai tentar novamente.");
                return false;
            }
        }

        public void Exit(Vehicle vehicle)
        {
            lock (lockObj)
            {
                availableSpaces++;
                Console.WriteLine($"Veículo {vehicle.Id} saiu. Vagas disponíveis: {availableSpaces}");
            }
            semaphore.Release();
        }
    }

    class Vehicle
    {
        public int Id { get; }
        private ParkingLot parkingLot;

        public Vehicle(int id, ParkingLot parkingLot)
        {
            Id = id;
            this.parkingLot = parkingLot;
        }

        public async Task TryPark()
        {
            Random random = new Random();

            while (true)
            {
                if (await parkingLot.Enter(this))
                {
                    int parkingTime = random.Next(2000, 5000);
                    await Task.Delay(parkingTime);
                    parkingLot.Exit(this);
                    break;
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}

