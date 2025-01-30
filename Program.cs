using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using LabMod5;

// Lucas Rocha Arruda

// Execuções! Os exercícios estão depois desse bloco de código.

// Preparing inputs
var numbers = InputHelper.GenerateRandomIntArray((int)Math.Pow(2, 28));
var strings = InputHelper.GenerateRandomStringArray((int)Math.Pow(2, 22), 100);
var lists = InputHelper.GenerateRandomNestedLists(100, (int)Math.Pow(2, 20));

// Header
Console.WriteLine($"Exercise,Result,SpentTimeInMs");

// Executing exercise 1
var stopwatch = Stopwatch.StartNew();
var result = SumEvenNumbers(numbers);
stopwatch.Stop();
Console.WriteLine($"Sum Even Numbers, {result}, {stopwatch.ElapsedMilliseconds}");

// Executing exercise 1 with Linq
stopwatch = Stopwatch.StartNew();
result = SumEvenNumbersLinq(numbers);
stopwatch.Stop();
Console.WriteLine($"Sum Even Numbers Linq, {result}, {stopwatch.ElapsedMilliseconds}");

// Executing exercise 1 with Plinq
stopwatch = Stopwatch.StartNew();
result = SumEvenNumbersPLinq(numbers);
stopwatch.Stop();
Console.WriteLine($"Sum Even Numbers PLinq, {result}, {stopwatch.ElapsedMilliseconds}");


/// Executing exercise 2
stopwatch = Stopwatch.StartNew();
result = CountLongStrings(strings);
stopwatch.Stop();
Console.WriteLine($"CountLongStrings, {result}, {stopwatch.ElapsedMilliseconds}");

/// Executing exercise 2 with linq
stopwatch = Stopwatch.StartNew();
result = CountLongStringsLinq(strings);
stopwatch.Stop();
Console.WriteLine($"CountLongStrings Linq, {result}, {stopwatch.ElapsedMilliseconds}");

/// Executing exercise 2 with Plinq
stopwatch = Stopwatch.StartNew();
result = CountLongStringsPLinq(strings);
stopwatch.Stop();
Console.WriteLine($"CountLongStrings PLinq, {result}, {stopwatch.ElapsedMilliseconds}");


/// Executing exercise 3
stopwatch = Stopwatch.StartNew();
result = FindMaxInNestedLists(lists);
stopwatch.Stop();
Console.WriteLine($"FindMaxInNestedLists, {result}, {stopwatch.ElapsedMilliseconds}");

/// Executing exercise 3 with linq
stopwatch = Stopwatch.StartNew();
result = FindMaxInNestedListsLinq(lists);
stopwatch.Stop();
Console.WriteLine($"FindMaxInNestedListsLinq, {result}, {stopwatch.ElapsedMilliseconds}");

/// Executing exercise 3 with Plinq
stopwatch = Stopwatch.StartNew();
result = FindMaxInNestedListsPLinq(lists);
stopwatch.Stop();
Console.WriteLine($"FindMaxInNestedListsPLinq, {result}, {stopwatch.ElapsedMilliseconds}");

/// Executing exercise 4
stopwatch = Stopwatch.StartNew();
bool plinq = false;
await WordCountMapReduce.Start(plinq);
stopwatch.Stop();
Console.WriteLine($"WordCountMapReduce, -, {stopwatch.ElapsedMilliseconds}");

/// Executing exercise 4 with Plinq
stopwatch = Stopwatch.StartNew();
plinq = true;
await WordCountMapReduce.Start(plinq);
stopwatch.Stop();
Console.WriteLine($"WordCountMapReducePLinq, -, {stopwatch.ElapsedMilliseconds}");

Console.WriteLine("The end.");

/**
 * Ex. 1
 */
static long SumEvenNumbers(int[] numbers)
{
    long sum = 0;
    foreach (var number in numbers)
    {
        if (number % 2 == 0)
        {
            sum += number;
        }
    }
    return sum;
}

static long SumEvenNumbersLinq(int[] numbers)
{
    // IMPLEMENT ME!
    return numbers.Where(n => n % 2 == 0).Sum(n => (long)n);
}

static long SumEvenNumbersPLinq(int[] numbers)
{
    // IMPLEMENT ME!
    return numbers.AsParallel().Where(n => n % 2 == 0).Sum(n => (long)n);
}


/**
 * Exercise #2
 */
static long CountLongStrings(string[] strings)
{
    long count = 0;
    foreach (var str in strings)
    {
        if (str.Length > 5)
        {
            count++;
        }
    }
    return count;
}

static long CountLongStringsLinq(string[] strings)
{
    // IMPLEMENT ME!
    return strings.Count(str => str.Length > 5);
}

static long CountLongStringsPLinq(string[] strings)
{
    // IMPLEMENT ME!
    return strings.AsParallel().Count(str => str.Length > 5);
}

/**
 * Exercise 3
 */
static int FindMaxInNestedLists(List<List<int>> nestedLists)
{
    int max = int.MinValue;
    foreach (var list in nestedLists)
    {
        foreach (var number in list)
        {
            if (number > max)
            {
                max = number;
            }
        }
    }
    return max;
}

static int FindMaxInNestedListsLinq(List<List<int>> nestedLists)
{
    // IMPLEMENT ME!
    return nestedLists.SelectMany(list => list).Max();
}


static int FindMaxInNestedListsPLinq(List<List<int>> nestedLists)
{
    // IMPLEMENT ME!
    return nestedLists.AsParallel().SelectMany(list => list).Max();
}

// Exercise 5
static class WordCountMapReduce
{
    public static Task Start(bool usePlinq)
    {
        string url = "http://www.albahari.com/ispell/allwords.txt"; // Substitua pelo URL do arquivo desejado
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        var producerTask = ProduceAsync(url, channel.Writer);
        var consumerTask = ConsumeAsync(channel.Reader, usePlinq);

        return Task.WhenAll(producerTask, consumerTask);
    }

    static async Task ProduceAsync(string url, ChannelWriter<string> writer)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            await writer.WriteAsync(line);
        }

        writer.Complete();
    }

    static async Task ConsumeAsync(ChannelReader<string> reader, bool usePlinq)
    {
        var localCollection = new ConcurrentBag<string>();
        var wordCounts = new ConcurrentDictionary<string, long>();

        await foreach (var line in ReadLinesAsync(reader))
        {
            localCollection.Add(line);

            if (localCollection.Count >= 10000)
            {

                var partialWordCounts = usePlinq ? ProcessChunkPlinq(localCollection) : ProcessChunk(localCollection);
                MergeWordCounts(wordCounts, partialWordCounts);

                // DEBUG ONLY
                // Console.WriteLine($"Processado um chunk com {partialWordCounts.Sum(kvp => kvp.Value)} palavras.");
                localCollection.Clear();
            }
        }

        // Processar linhas restantes
        if (!localCollection.IsEmpty)
        {
            var partialWordCounts = ProcessChunk(localCollection);
            MergeWordCounts(wordCounts, partialWordCounts);

            // DEBUG ONLY
            //Console.WriteLine($"Processado o último chunk com {partialWordCounts.Sum(kvp => kvp.Value)} palavras.");
        }

        // Exibir o resultado final
        // DEBUG ONLY
        //Console.WriteLine("Ocorrências finais de palavras (Top 10):");
        foreach (var kvp in wordCounts.OrderByDescending(kvp => kvp.Value).Take(10))
        {
            // DEBUG ONLY
            //Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }

    static async IAsyncEnumerable<string> ReadLinesAsync(ChannelReader<string> reader)
    {
        while (await reader.WaitToReadAsync())
        {
            while (reader.TryRead(out var line))
            {
                yield return line;
            }
        }
    }

    static Dictionary<string, long> ProcessChunk(ConcurrentBag<string> chunk)
    {
        Dictionary<string, long> response = [];
        foreach (var line in chunk)
        {
            var words = line.Split(' ');
            foreach (var word in words)
            {
                if (word != "")
                {
                    response.TryGetValue(word, out long val);
                    val++;
                    response[word] = val;
                }
            }
        }
        return response;
    }

    static Dictionary<string, long> ProcessChunkPlinq(ConcurrentBag<string> chunk)
    {
        // IMPLEMENT ME!
        return chunk
            .AsParallel()
            .SelectMany(line => line.Split(' '))
            .Where(word => !string.IsNullOrEmpty(word))
            .GroupBy(word => word)
            .ToDictionary(group => group.Key, group => (long)group.Count());
    }

    static void MergeWordCounts(ConcurrentDictionary<string, long> globalCounts, Dictionary<string, long> partialCounts)
    {
        foreach (var kvp in partialCounts)
        {
            globalCounts.AddOrUpdate(kvp.Key, kvp.Value, (_, oldValue) => oldValue + kvp.Value);
        }
    }
}
