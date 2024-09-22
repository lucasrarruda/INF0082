using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {

        // Obter a quantidade de usuários e atualizações do sensor
        string[] entrada = Console.ReadLine().Split();
        int usuarios = int.Parse(entrada[0]);
        int atualizacoes = int.Parse(entrada[1]);

        // Obter a quantidade de leituras realizadas por cada usuário
        int[] leituras = new int[usuarios];
        for(int i = 0; i < usuarios; i++) {
            leituras[i] = int.Parse(Console.ReadLine());
        }

        // Continue a implementação
        Sensor sensor = new(atualizacoes);
        List<User> users = new();

        for(int i = 0; i < leituras.Length; i++)
        {
            users.Add(new User(i + 1, leituras[i], sensor));
        }

        await sensor.Await();
        foreach(User use in users)
        {
            await use.Await();
        }
    }

    public class User
    {
        private Task _currentTask;
        private int _readingNumber = 0;
        private Sensor _sensor;
        private int _id = 0;

        public User(int id, int readingNumber, Sensor sensor)
        {
            _id = id;
            _readingNumber = readingNumber;
            _sensor = sensor;
            _currentTask = Task.Run(() => ReaderFunc());
        }

        public async Task Await()
        {
            await _currentTask;
        }

        private async Task ReaderFunc()
        {
            for(int i = 0; i < _readingNumber; i++)
            {
                double currentTemp = _sensor.Temperature;
                Console.WriteLine($"Usuário {_id}: Temperatura lida: {currentTemp}°C");
                await Task.Delay(GetRandomDelay());
            }
        }

        private int GetRandomDelay()
        {
            Random randomNumber = new();
            return randomNumber.Next(500, 2000);
        }
    }

    public class Sensor
    {
        Task _currentTask;
        ReaderWriterLockSlim _lock = new();

        private double _temperature = 0.0;
        public double Temperature
        {
            get 
            {
                double temp;
                _lock.EnterReadLock();
                temp = _temperature;
                _lock.ExitReadLock();
                return temp;
            }
            set
            {
                _lock.EnterWriteLock();
                _temperature = value;
                _lock.ExitWriteLock();
            }
        }

        public Sensor(int updateNumber)
        {
            _currentTask = Task.Run(() => UpdatingFunc(updateNumber));
        }

        public async Task Await()
        {
            await _currentTask;
        }

        private async Task UpdatingFunc(int updateNumber)
        {
            for(int i = 0; i < updateNumber; i++)
            {
                double currentTemp = Temperature = GetRandomTemperature();
                Console.WriteLine($"[Sensor] Temperatura atualizada: {currentTemp}°C");
                await Task.Delay(GetRandomDelay());
            }
        }

        private double GetRandomTemperature()
        {
            Random randomNumber = new();
            return randomNumber.NextDouble() * randomNumber.Next(1, 40);
        }

        private int GetRandomDelay()
        {
            Random randomNumber = new();
            return randomNumber.Next(500, 2000);
        }
    }
}
