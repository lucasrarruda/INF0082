using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
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
        char[] delimiters = new char[]{' ', '\n', '\r', '\t'};
        string[] textArray = text.Split(delimiters);
        Task<int>[] arrayFrequence = new Task<int>[words.Length];
        
        for(int i = 0; i < words.Length; i++)
        {
            arrayFrequence[i] = FindingWordRepetition(words[i], textArray);
        }

        for(int i = 0; i < words.Length; i++)
        {
            Console.WriteLine($"{words[i]}: {await arrayFrequence[i]}");
        }
    }

    static async Task<int> FindingWordRepetition(string palavra, string[] textArray)
    {
        int frequence = 0;
        await Task.Run(() =>
        {
            foreach(string currentWord in textArray)
            {
                if(currentWord.Equals(palavra, StringComparison.OrdinalIgnoreCase))
                {
                    frequence++;
                }
            }
        });
        return frequence;
    }
}