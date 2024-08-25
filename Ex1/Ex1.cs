using System.Diagnostics;

/* Resposta:
* O speedup só piorou com o aumento de threads para valores de até 100.000.000.
* Curiosamente o speedup piora de no aumento de 2 para 4 threads, mas retorna a crescer
* a medida que o número de threads aumenta para 8 e 16. Veja um exemplo de execução
* para N = 100.000.000:
* Speedup 2 threads -> 0,03402489626556016
* Speedup 4 threads -> 0,02527743526510481
* Speedup 8 threads -> 0,028873239436619718
* Speedup 16 threads -> 0,03299798792756539
*/

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

        long tempoSequencial = CalcularTempoMedioSimulacao(N);
        double speedup2 = (double)tempoSequencial / CalcularTempoMedioSimulacao(N, 2);
        double speedup4 = (double)tempoSequencial / CalcularTempoMedioSimulacao(N, 4);
        double speedup8 = (double)tempoSequencial / CalcularTempoMedioSimulacao(N, 8);
        double speedup16 = (double)tempoSequencial / CalcularTempoMedioSimulacao(N, 16);

        Console.WriteLine($"Speedup 2 threads -> {speedup2}");
        Console.WriteLine($"Speedup 4 threads -> {speedup4}");
        Console.WriteLine($"Speedup 8 threads -> {speedup8}");
        Console.WriteLine($"Speedup 16 threads -> {speedup16}");
    }

    static long CalcularTempoMedioSimulacao(int N, int numeroThreads = 1)
    {
        long tempoTotal = 0;
        for(int i = 0; i < 5; i++)
        {
            tempoTotal += ExecutarSimulacao(N, numeroThreads);
        }
        long tempoMedio = tempoTotal / 5;
        Console.WriteLine($"Tempo médio para N={N} e {numeroThreads} threads -> {tempoMedio} ms\n\n");
        return tempoMedio;
    }

    static long ExecutarSimulacao(int N, int numeroThreads)
    {
        long somatorio;
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (numeroThreads == 1)
        {
            somatorio = SomatorioDosQuadradosSequencial(N);
        }
        else
        {
            somatorio = SomatorioDosQuadradosParalelo(N, numeroThreads);
        }
        stopwatch.Stop();

        Console.WriteLine($"Resultado do somatório para N={N} -> {somatorio} ({stopwatch.ElapsedMilliseconds} ms)");
        return stopwatch.ElapsedMilliseconds;
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

    static long SomatorioDosQuadradosParalelo(int N, int numeroThreads)
    {
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