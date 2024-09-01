using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Obtive os resultados abaixo, mostrando que o tempo de compartilhamento devido 
// ao async await não compesou o uso de assincronicidade para resolver esse problema.

// Tempo de duração sequencial: 3662,2 ms
// Tempo de duração com Parallel: 669,6 ms
// Tempo de duração com Async/Await: 3808 ms
// Speedup com Parallel.Foreach 5,4692353643966545
// Speedup com Async/Await 0,9617121848739495


// Tempo de duração sequencial: 34741,2 ms
// Tempo de duração com Parallel: 3335,4 ms
// Tempo de duração com Async/Await: 40346,8 ms
// Speedup com Parallel.Foreach 10,415902140672781
// Speedup com Async/Await 0,861064570176569

class Program
{
    static readonly string ENDPOINT = "https://pokeapi.co/api/v2/pokemon/";
    static readonly string POKEFILE = "pokedex.txt";

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
        // Sequencial:
        List<long> sequencialTimes = new List<long>();
        for (int i = 0; i < 5; i++)
        {
            DeletePokeFile();
            sequencialTimes.Add(PeformSequencial(pokemonIds));
        }
        double sequencialTime = sequencialTimes.Average();
        Console.WriteLine($"Tempo de duração sequencial: {sequencialTime} ms");

        // Usando Parallel.Foreach
        List<long> parallelForeachTimes = new List<long>();
        for (int i = 0; i < 5; i++)
        {
            DeletePokeFile();
            parallelForeachTimes.Add(PeformParallel(pokemonIds));
        }
        double parallelForeachTime = parallelForeachTimes.Average();
        Console.WriteLine($"Tempo de duração com Parallel: {parallelForeachTime} ms");

        // Usando Async/Await
        List<long> asyncAwaitTimes = new List<long>();
        for (int i = 0; i < 5; i++)
        {
            DeletePokeFile();
            Task<long> time = PeformAsyncAwait(pokemonIds);
            asyncAwaitTimes.Add(time.GetAwaiter().GetResult());
        }
        double asyncAwaitTime = asyncAwaitTimes.Average();
        Console.WriteLine($"Tempo de duração com Async/Await: {asyncAwaitTime} ms");

        Console.WriteLine($"Speedup com Parallel.Foreach {sequencialTime/parallelForeachTime}");
        Console.WriteLine($"Speedup com Async/Await {sequencialTime/asyncAwaitTime}");
    }

    static void DeletePokeFile()
    {
        string file = Path.Combine(Directory.GetCurrentDirectory(), POKEFILE);
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }

    static long PeformSequencial(int[] ids)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach(int id in ids)
        {
            Task<Pokemon> pokemonRequest = GetPokemonAsync(id);
            Pokemon pokemon = pokemonRequest.GetAwaiter().GetResult();
            RecordPokemon(pokemon);
        }
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    static long PeformParallel(int[] ids)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        object fileLock = new object();
        Parallel.ForEach(ids, id =>
        {
            Task<Pokemon> pokemonRequest = GetPokemonAsync(id);
            Pokemon pokemon = pokemonRequest.GetAwaiter().GetResult();
            lock(fileLock)
            {
                RecordPokemon(pokemon);
            }
        });
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    static async Task<long> PeformAsyncAwait(int[] ids)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach(int id in ids)
        {
            Pokemon pokemon = await GetPokemonAsync(id);
            await RecordPokemonAsync(pokemon);
        }
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    private static async Task<Pokemon> GetPokemonAsync(int id)
    {
        Pokemon? pokemon = default;
        using (HttpClient clientApi = new HttpClient())
        {
            try
            {
                HttpResponseMessage responseApi = await clientApi.GetAsync($"{ENDPOINT}{id}");
                responseApi.EnsureSuccessStatusCode();

                string responseBody = await responseApi.Content.ReadAsStringAsync();
                pokemon = JsonConvert.DeserializeObject<Pokemon>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        return pokemon;
    }

    private static void RecordPokemon(Pokemon pokemon)
    {
        using(StreamWriter file = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), POKEFILE), append: true))
        {
            file.WriteLine(pokemon.Output());
        }
    }

    private static async Task RecordPokemonAsync(Pokemon pokemon)
    {
        using(StreamWriter file = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), POKEFILE), append: true))
        {
            await file.WriteLineAsync(pokemon.Output());
        }
    }
};

public class Pokemon
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("types")]
    public List<PokemonType> Types { get; set; }

    public string Output()
    {
        return $"{Name},{string.Join(",",Types.Select(t => t.Type.Name))}";
    }
}

public class PokemonType
{
    [JsonProperty("type")]
    public TypeDetail Type { get; set; }
}

public class TypeDetail
{
    [JsonProperty("name")]
    public string Name { get; set; }
}