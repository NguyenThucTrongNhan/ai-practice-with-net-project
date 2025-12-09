namespace AlgorithmConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        public static IEnumerable<string> AllPrefixes(int prefixLength, IEnumerable<string> words)
        {
            return words.Where(w => w.Length >= prefixLength)
                .Select(x => x[..prefixLength])
                .Distinct();
        }
        public static int Count(int[] tree)
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            var internalNode = new HashSet<int>();
            for (int child = 0; child < tree.Length; child++) {
                int parent =tree[child];
                if(parent != -1) internalNode.Add(parent);
            }

        }
    }
}
