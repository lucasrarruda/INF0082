// Remova essa linha para desativar o print dos números e visualizar os tempos
// #define PRINT_NUMBERS

using System.Collections.Concurrent;
using System.Diagnostics;

class Program {
    static async Task Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        string[] input = Console.ReadLine().Split(' ');
        int[] sequence = new int[N];

        for(int i = 0; i < N; i++) {
            sequence[i] = int.Parse(input[i]);
        }

        // Continue a Implementação
        Stopwatch stopwatch = Stopwatch.StartNew();
        await TaskStrategy(sequence);
        stopwatch.Stop();
        Console.WriteLine($"Estratégia com Task: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();
        ParallelForStrategy(sequence);
        stopwatch.Stop();
        Console.WriteLine($"Estratégia com Parallel.For: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();
        ParallelForEachStrategy(sequence);
        stopwatch.Stop();
        Console.WriteLine($"Estratégia com Parallel.ForEach: {stopwatch.ElapsedMilliseconds} ms");
    }

    static async Task TaskStrategy(int[] sequence)
    {
        List<Task> primeNumbersTasks = new List<Task>();

        foreach(int value in sequence)
        {
            int currentValue = value;
            Task currentTask = Task.Run(async () =>
            {
                ConcurrentBag<int> primeNumbers = await CalcAllPrimeNumberAsync(currentValue);
                #if PRINT_NUMBERS
                Console.WriteLine($"Numeros primos até {currentValue}:");
                foreach(int prime in primeNumbers) Console.WriteLine(prime);
                #endif
            });

            primeNumbersTasks.Add(currentTask);
        }

        await Task.WhenAll(primeNumbersTasks);
    }

    static async Task<ConcurrentBag<int>> CalcAllPrimeNumberAsync(int N)
    {
        ConcurrentBag<int> primeNumbers = new ConcurrentBag<int>();
        List<Task> tasks = new List<Task>();

        for(int i = 0; i < N; i++)
        {
            int currentValue = i;
            tasks.Add(Task.Run(async () =>
            {
                if (await IsPrimeNumberAsync(currentValue))
                {
                    primeNumbers.Add(currentValue);
                }
            }));
        }

        await Task.WhenAll(tasks);

        return primeNumbers;
    }

    static async Task<bool> IsPrimeNumberAsync(int number)
    {
        if (number < 2) return false;

        return await Task.Run(() =>
        {
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        });
    }

    static void ParallelForStrategy(int[] sequence)
    {
        Parallel.For(0, sequence.Length + 1, i =>
        {
            ConcurrentBag<int> primeNumbers = CalcAllPrimeNumberParallelFor(i);
            #if PRINT_NUMBERS
            Console.WriteLine($"Numeros primos até {i}:");
            foreach(int prime in primeNumbers) Console.WriteLine(prime);
            #endif
        });
    }

    static ConcurrentBag<int> CalcAllPrimeNumberParallelFor(int N)
    {
        ConcurrentBag<int> primeNumbers = new ConcurrentBag<int>();
        
        Parallel.For(2, N + 1, i =>
        {
            if (IsPrimeNumberParallelFor(i))
            {
                primeNumbers.Add(i);
            }
        });

        return primeNumbers;
    }

    static bool IsPrimeNumberParallelFor(int number)
    {
        if (number < 2) return false;

        bool isPrime = true;
        Parallel.For(2, (int)Math.Sqrt(number) + 1, (i, state) =>
        {
            if (number % i == 0)
            {
                isPrime = false;
                state.Stop();
            }
        });
        return isPrime;
    }

    static void ParallelForEachStrategy(int[] sequence)
    {
        Parallel.For(0, sequence.Length + 1, i =>
        {
            ConcurrentBag<int> primeNumbers = CalcAllPrimeNumberParallelForEach(i);
            #if PRINT_NUMBERS
            Console.WriteLine($"Numeros primos até {i}:");
            foreach(int prime in primeNumbers) Console.WriteLine(prime);
            #endif
        });
    }

    static ConcurrentBag<int> CalcAllPrimeNumberParallelForEach(int N)
    {
        ConcurrentBag<int> primeNumbers = new ConcurrentBag<int>();
        
        List<int> elements = Enumerable.Range(2, N).ToList();
        Parallel.ForEach(elements, i =>
        {
            if (IsPrimeNumberParallelForEach(i))
            {
                primeNumbers.Add(i);
            }
        });

        return primeNumbers;
    }

    static bool IsPrimeNumberParallelForEach(int number)
    {
        if (number < 2) return false;

        bool isPrime = true;
        List<int> divisors = Enumerable.Range(2, (int)Math.Sqrt(number) - 1).ToList();
        Parallel.ForEach(divisors, (i, state) =>
        {
            if (number % i == 0)
            {
                isPrime = false;
                state.Stop();
            }
        });
        return isPrime;
    }
}