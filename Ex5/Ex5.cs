using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static readonly string ENDPOINT = "https://pokeapi.co/api/v2/pokemon/"
    static void Main(string[] args)
    {
        string[] input = Console.ReadLine().Split(' ');
        int P = input.Length;
        int[] pokemonIds = new int[P];

        for (int i = 0; i < P; i++)
        {
            pokemonIds[i] = int.Parse(input[i]);
        }

        // Continue a Implementação
        // ...
    }
};

class Pokemon
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("types")]
    public List<string> Types { get; set; }
}