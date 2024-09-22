using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        // Leitura do número de regiões
        int R = int.Parse(Console.ReadLine());

        var votos = new List<(int votosChapaA, int votosChapaB)>();

        // Leitura dos votos em cada região
        for (int i = 0; i < R; i++)
        {
            string[] entrada = Console.ReadLine().Split();
            int votosRegiaoA = int.Parse(entrada[0]);
            int votosRegiaoB = int.Parse(entrada[1]);
            votos.Add((votosRegiaoA, votosRegiaoB));
        }

        // Continue a implementação

    }
}
