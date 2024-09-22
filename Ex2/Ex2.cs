using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
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
    }
}
