using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.FSM
{
    public sealed class DFSA<T, S> : IScored
        where T : class
    {
        Object dfsaID;
        DFSAState<T, S> initialState;
        public DFSA(DFSAState<T, S> initialState, double score)
        {
            this.initialState = initialState;
            this.score = score;
        }

        public DFSA(DFSAState<T, S> initialState)
        {
            this.initialState = initialState;
            this.score = Double.NaN;
        }

        public double score;
        public double Score()
        {
            return score;
        }

        public DFSAState<T, S> InitialState()
        {
            return initialState;
        }

        public void SetInitialState(DFSAState<T, S> initialState)
        {
            this.initialState = initialState;
        }

        public ISet<DFSAState<T, S>> States()
        {
            ISet<DFSAState<T, S>> visited = new HashSet<DFSAState<T, S>>();
            List<DFSAState<T, S>> toVisit = new List<DFSAState<T, S>>();
            toVisit.Add(InitialState());
            ExploreStates(toVisit, visited);
            return visited;
        }

        private static void ExploreStates(List<DFSAState<T, S>> toVisit, ISet<DFSAState<T, S>> visited)
        {
            while (!toVisit.IsEmpty())
            {
                DFSAState<T, S> state = toVisit.Get(toVisit.Size() - 1);
                toVisit.RemoveAt(toVisit.Size() - 1);
                if (!visited.Contains(state))
                {
                    toVisit.AddRange(state.SuccessorStates());
                    visited.Add(state);
                }
            }
        }

        public DFSA(Object dfsaID)
        {
            this.dfsaID = dfsaID;
            this.score = 0;
        }

        private static void PrintTrieDFSAHelper(DFSAState<T, S> state, int level)
        {
            if (state.IsAccepting())
            {
                return;
            }

            ICollection<T> inputs = state.ContinuingInputs();
            foreach (T input in inputs)
            {
                DFSATransition<T, S> transition = state.Transition(input);
                Console.Out.Write(level);
                Console.Out.Write(input);
                for (int i = 0; i < level; i++)
                {
                    Console.Out.Write(@"   ");
                }

                Console.Out.Write(transition.Score());
                Console.Out.Write(@" ");
                Console.Out.WriteLine(input);
                PrintTrieDFSAHelper(transition.Target(), level + 1);
            }
        }

        public static void PrintTrieDFSA(DFSA<T, S> dfsa)
        {
            Console.Error.WriteLine(@"DFSA: " + dfsa.dfsaID);
            PrintTrieDFSAHelper(dfsa.InitialState(), 2);
        }

        public void PrintAttFsmFormat(TextWriter w)
        {
            Queue<DFSAState<T, S>> q = new Queue<DFSAState<T, S>>();
            ISet<DFSAState<T, S>> visited = new HashSet<DFSAState<T, S>>();
            q.Enqueue(initialState);
            while (q.Count > 0 && q.Peek() != null)
            {
                DFSAState<T, S> state = q.Dequeue();
                if (state == null || visited.Contains(state))
                    continue;
                visited.Add(state);
                if (state.IsAccepting())
                {
                    w.Write(state.ToString() + @"\t" + state.Score() + @"\n");
                    continue;
                }

                SortedSet<T> inputs = new SortedSet<T>(state.ContinuingInputs());
                foreach (T input in inputs)
                {
                    DFSATransition<T, S> transition = state.Transition(input);
                    DFSAState<T, S> target = transition.Target();
                    if (!visited.Contains(target))
                        q.Enqueue(target);
                    w.Write(state.ToString() + @"\t" + target.ToString() + @"\t" + transition.GetInput() + @"\t" + transition.Score() + @"\n");
                }
            }
        }

        private static void PrintTrieAsRulesHelper(DFSAState<T, S> state, string prefix, TextWriter w)
        {
            if (state.IsAccepting())
            {
                return;
            }

            ICollection<T> inputs = state.ContinuingInputs();
            foreach (T input in inputs)
            {
                DFSATransition<T, S> transition = state.Transition(input);
                DFSAState<T, S> target = transition.Target();
                Set<T> inputs2 = target.ContinuingInputs();
                bool allTerminate = true;
                foreach (T input2 in inputs2)
                {
                    DFSATransition<T, S> transition2 = target.Transition(input2);
                    DFSAState<T, S> target2 = transition2.Target();
                    if (target2.IsAccepting())
                    {
                        w.Write(prefix + @" --> " + input + @" " + input2 + @"\n");
                    }
                    else
                    {
                        allTerminate = false;
                    }
                }

                if (!allTerminate)
                {
                    string newPrefix = prefix + @"_" + input;
                    w.Write(prefix + @" --> " + input + @" " + newPrefix + @"\n");
                    PrintTrieAsRulesHelper(transition.Target(), newPrefix, w);
                }
            }
        }

        public static void PrintTrieAsRules(DFSA<T, S> dfsa, TextWriter w)
        {
            PrintTrieAsRulesHelper(dfsa.InitialState(), dfsa.dfsaID.ToString(), w);
        }
    }
}
