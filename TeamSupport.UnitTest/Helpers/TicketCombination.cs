using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.UnitTest.Helpers
{
    /// <summary>
    /// hash values representing three words
    /// </summary>
    struct Triple   // struct to avoid new/delete of Tuple
    {
        public int item1;
        public int item2;
        public int item3;
    }

    class CombinationCount<T>
    {
        public T _combination;
        public int Count { get; private set; }

        public CombinationCount(T combination)
        {
            Count = 1;
            _combination = combination;
        }

        public void Increment() { ++Count; }

        public override int GetHashCode() { return _combination.GetHashCode(); }
    }

    struct TicketHash
    {
        public int _ticketID;
        public int[] _wordHash;

        public TicketHash(int ticketID, int[] hashCodes)
        {
            _ticketID = ticketID;
            _wordHash = hashCodes;
        }

        public void Counts(Dictionary<int, int> counts)
        {
            // increment the word counts
            foreach (int hash in _wordHash)
            {
                if (!counts.ContainsKey(hash))
                    counts[hash] = 1;
                else
                    ++counts[hash];
            }
        }
    }

    class TicketCombination
    {
        Dictionary<int, string> _unique;
        List<TicketHash> _tickets;
        int[] _filteredWords;
        Dictionary<int, CombinationCount<Triple>> _tripleCount;


        public TicketCombination()
        {
            _unique = new Dictionary<int, string>();
            _tickets = new List<TicketHash>();
            _tripleCount = new Dictionary<int, CombinationCount<Triple>>();
        }

        public void Add(TicketText ticketText)
        {
            // find unique set of words in ticket by HashCode
            Dictionary<int, string> ticketWords = StopList.UniqueWords(ticketText.Description);

            // add these to the global set
            AddToGlobalUniqueSet(ticketWords);

            // create the ticket with ID and words hash codes
            int[] hashCodes = ticketWords.Select(w => w.Key).ToArray();
            TicketHash ticketHash = new TicketHash(ticketText.TicketID, hashCodes);
            _tickets.Add(ticketHash);
        }

        void AddToGlobalUniqueSet(Dictionary<int, string> ticketWords)
        {
            foreach (KeyValuePair<int, string> pair in ticketWords)
                StopList.CheckedAdd(pair.Key, pair.Value, _unique);
        }

        public void Reduce(int minWordOccurrenceCount)
        {
            // count how many times each word (represented by HashCode) appears
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (TicketHash ticket in _tickets)
                ticket.Counts(counts);

            _filteredWords = (counts.Where(c => c.Value > minWordOccurrenceCount)).Select(w => w.Key).ToArray();
        }

        public void CountTriples()
        {
            // count the triples for only the words in _filteredWords
            foreach (TicketHash ticket in _tickets)
                TripleStuff(_filteredWords.Intersect<int>(ticket._wordHash).ToArray());
        }

        void TripleStuff(int[] keys)
        {
            Array.Sort(keys);
            Triple triple;
            for (int i = 0; i < keys.Length; ++i)
            {
                triple.item1 = keys[i];
                for (int j = 0; j < i; ++j)
                {
                    triple.item2 = keys[j];
                    for (int k = 0; k < j; ++k)
                    {
                        triple.item3 = keys[k];
                        Update(triple);
                    }
                }
            }
        }

        void Update(Triple triple)
        {
            if (_tripleCount.TryGetValue(triple.GetHashCode(), out CombinationCount<Triple> tripleCount))
                tripleCount.Increment();
            else
                _tripleCount[triple.GetHashCode()] = new CombinationCount<Triple>(triple);
        }

        public void Dump()
        {
            int i = 0;
            foreach (int hashCode in _filteredWords)
            {
                if (++i < 8)
                    Debug.Write($"{_unique[hashCode]}, ");
                else
                {
                    Debug.WriteLine($"{_unique[hashCode]}, ");
                    i = 0;
                }
            }
            Debug.WriteLine(' ');

            var enumerators = _tripleCount.Select(p => p.Value).OrderByDescending(t => t.Count).Take(200);
            foreach (var enumerator in enumerators)
            {
                Triple combination = enumerator._combination;
                //Debug.WriteLine($"{enumerator.Count} ({_unique[combination.item1]}, {_unique[combination.item2]})");
                Debug.WriteLine($"{enumerator.Count} ({_unique[combination.item1]}, {_unique[combination.item2]}, {_unique[combination.item3]})");
            }
        }
    }

}
