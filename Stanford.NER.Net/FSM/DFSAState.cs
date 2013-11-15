using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.FSM
{
    public sealed class DFSAState<T, S> : IScored
        where T : class
    {
        private S stateID;
        private IDictionary<T, DFSATransition<T, S>> inputToTransition;
        public bool accepting;
        private DFSA<T, S> dfsa;
        public double score;
        public double Score()
        {
            return score;
        }

        public void SetScore(double score)
        {
            this.score = score;
        }

        public DFSA<T, S> Dfsa()
        {
            return dfsa;
        }

        public void SetStateID(S stateID)
        {
            this.stateID = stateID;
        }

        public S StateID()
        {
            return stateID;
        }

        public void AddTransition(DFSATransition<T, S> transition)
        {
            inputToTransition.Put(transition.Input(), transition);
        }

        public DFSATransition<T, S> Transition(T input)
        {
            return inputToTransition.Get(input);
        }

        public ICollection<DFSATransition<T, S>> Transitions()
        {
            return inputToTransition.Values;
        }

        public ICollection<T> ContinuingInputs()
        {
            return inputToTransition.Keys;
        }

        public ISet<DFSAState<T, S>> SuccessorStates()
        {
            ISet<DFSAState<T, S>> successors = new HashSet<DFSAState<T, S>>();
            ICollection<DFSATransition<T, S>> transitions = inputToTransition.Values;
            foreach (DFSATransition<T, S> transition in transitions)
            {
                successors.Add(transition.GetTarget());
            }

            return successors;
        }

        public void SetAccepting(bool accepting)
        {
            this.accepting = accepting;
        }

        public bool IsAccepting()
        {
            return accepting;
        }

        public bool IsContinuable()
        {
            return !inputToTransition.IsEmpty();
        }

        public override string ToString()
        {
            return stateID.ToString();
        }

        private int hashCodeCache;
        public override int GetHashCode()
        {
            if (hashCodeCache == 0)
            {
                hashCodeCache = stateID.GetHashCode() ^ dfsa.GetHashCode();
            }

            return hashCodeCache;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }

            if (!(o is DFSAState<T, S>))
            {
                return false;
            }

            DFSAState<T, S> s = (DFSAState<T, S>)o;
            return dfsa.Equals(s.dfsa) && stateID.Equals(s.stateID);
        }

        public ISet<DFSAState<T, S>> StatesReachable()
        {
            ISet<DFSAState<T, S>> visited = new HashSet<DFSAState<T, S>>();
            List<DFSAState<T, S>> toVisit = new List<DFSAState<T, S>>();
            toVisit.Add(this);
            ExploreStates(toVisit, visited);
            return visited;
        }

        private void ExploreStates(List<DFSAState<T, S>> toVisit, ISet<DFSAState<T, S>> visited)
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

        public DFSAState(S id, DFSA<T, S> dfsa)
        {
            this.dfsa = dfsa;
            this.stateID = id;
            this.accepting = false;
            this.inputToTransition = new HashMap<T, DFSATransition<T, S>>();
            this.score = Double.NegativeInfinity;
        }

        public DFSAState(S id, DFSA<T, S> dfsa, double score)
            : this(id, dfsa)
        {
            SetScore(score);
        }
    }
}
