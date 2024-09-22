using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
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
        List<Task> regions = new List<Task>();
        VotingMachine voteMachine = new();
        Random random = new();

        foreach(var currentVotes in votos)
        {
            regions.Add(Task.Run(() => 
            {
                int delay = random.Next(500, 2000);
                Task.Delay(delay);
                voteMachine.AddVotes("A", currentVotes.votosChapaA);
                Task.Delay(delay);
                voteMachine.AddVotes("B", currentVotes.votosChapaB);
            }));
        }

        await Task.WhenAll(regions);
        voteMachine.ShowWinner();
    }

    public class VotingMachine
    {
        private ConcurrentDictionary<string, int> _voting = new ConcurrentDictionary<string, int>();

        public void AddVotes(string politicalParty, int votes)
        {
            _voting.AddOrUpdate(politicalParty, votes, (key, currentValue) => currentValue + votes);
        }

        public void ShowWinner()
        {
            int votingA = 0;
            int votingB = 0;
            _voting.TryGetValue("A", out votingA);
            _voting.TryGetValue("B", out votingB);
            
            Console.WriteLine($"Chapa A: {votingA} Votos");
            Console.WriteLine($"Chapa B: {votingB} Votos");
            string winner = votingA >= votingB ? "A": "B";
            Console.WriteLine($"A chapa {winner} venceu a eleição!");
        }
    }
}
