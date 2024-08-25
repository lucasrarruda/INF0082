using System;
using System.Threading.Tasks;

class Program {
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        string[] input = Console.ReadLine().Split(' ');
        int[] sequence = new int[N];

        for(int i = 0; i < N; i++) {
            sequence[i] = int.Parse(input[i]);
        }

        // Continue a Implementação
        // ...
    }
}