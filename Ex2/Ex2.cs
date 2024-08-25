using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
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
        // ...

    }
}