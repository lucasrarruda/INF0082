using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
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
    }
}
