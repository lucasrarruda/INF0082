using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Ler N da entrada
        int N = int.Parse(Console.ReadLine());

        double[] sequence = new double[N];

        // Inicializar o array com reais aleatórios entre 0 e 100
        Random rand = new Random();
        for (int i = 0; i < N; i++)
        {
            sequence[i] = rand.NextDouble() * 100;
        }

        // Continue a Implementação
        Stopwatch stopwatch = Stopwatch.StartNew();
        Task<double> media = CalcularMedia(sequence);
        Task<double> mediana = CaclularMediana(sequence);
        Task<double> variancia = CalcularVariancia(sequence, media);
        Task<double> desvioPadrao = CalcularDesvioPadrao(sequence, media);
        stopwatch.Stop();

        Console.WriteLine($"Para N = {N} temos:\n");
        Console.WriteLine($"- Média = {await media}");
        Console.WriteLine($"- Mediana = {await mediana}");
        Console.WriteLine($"- Variância = {await variancia}");
        Console.WriteLine($"- Desvio Padrão = {await desvioPadrao}");
        Console.WriteLine($"Tempo: {stopwatch.ElapsedMilliseconds} ms");
    }

    static async Task<double> CalcularMedia(double[] array)
    {
        double total = 0;
        double media = 0.0;
        await Task.Run(() =>
        {
            foreach(double item in array)
            {
                total += item;
            }
            media = total / array.Length;
        });
        
        return media;
    }

    static async Task<double> CaclularMediana(double[] array)
    {
        int indiceMeio = array.Length / 2;
        await Task.Run(() => Array.Sort(array));

        if (array.Length % 2 == 0)
        {
            return (array[indiceMeio - 1] + array[indiceMeio]) / 2;
        }
        else
        {
            return array[indiceMeio];
        }
    }

    static async Task<double> CalcularVariancia(double[] array, Task<double> media)
    {
        double somaQuadrados = 0;

        foreach (double item in array)
        {
            double diferenca = item - await media;
            somaQuadrados += (diferenca) * (diferenca);
        }

        return somaQuadrados / array.Length;
    }

    static async Task<double> CalcularDesvioPadrao(double[] array, Task<double> media)
    {
        return Math.Sqrt(await CalcularVariancia(array, media));
    }
}