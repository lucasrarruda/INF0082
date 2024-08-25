using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        string fileName = Console.ReadLine();
        int N = int.Parse(Console.ReadLine());

        string[] words = new string[N];

        for (int i = 0; i < N; i++)
        {
            words[i] = Console.ReadLine();
        }

        string text;
        try
        {
            text = File.ReadAllText(fileName);
        }
        catch (Exception e)
        {
            Console.WriteLine("Erro ao ler o arquivo: ", e.Message);
            return;
        }

        // Continue a Implementação
        // ...
    }
}