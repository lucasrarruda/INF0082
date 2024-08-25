using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;

class Program {
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());

        // Continue a Implementação
        if (N <= 0)
        {
            Console.WriteLine("Informe um valor maior que zero.");
            return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        // long somatorio = SomatorioDosQuadradosSequencial(N);
        long somatorio = SomatorioDosQuadradosParalelo(N);
        stopwatch.Stop();

        Console.WriteLine($"Resultado do somatório para N={N} -> {somatorio} ({stopwatch.ElapsedMilliseconds} ms)");
    }

    static long SomatorioDosQuadradosSequencial(int N)
    {
        long somatorio = 0;
        
        for(int i = 0; i < N; i++)
        {
            somatorio += i * i;
        }

        return somatorio;
    }

    static long SomatorioDosQuadradosParalelo(int N)
    {
        const int numeroThreads = 1;
        Thread[] arrayThreads = new Thread[numeroThreads];
        long[] arraySomatorios = new long[numeroThreads];
        long somatorio = 0;
        int elementosPorThreads = N / numeroThreads;
        int restoElementosPorThread = N % numeroThreads;

        for(int t = 0; t < numeroThreads; t++)
        {
            int indiceInicio = t * elementosPorThreads + Math.Min(restoElementosPorThread, t);
            int indiceFim = indiceInicio + elementosPorThreads + (t < restoElementosPorThread? 1: 0);
            int indiceThread = t;
            arrayThreads[indiceThread] = new Thread(() =>
            {
                for(int i = indiceInicio; i < indiceFim; i++)
                {
                    arraySomatorios[indiceThread] += (int)Math.Pow(i + 1, 2);
                }
            });
            arrayThreads[indiceThread].Start();
        }

        for(int t = 0; t < numeroThreads; t++)
        {
            arrayThreads[t].Join();
            somatorio += arraySomatorios[t];
        }

        return somatorio;
    }
}