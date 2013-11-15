using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.FSM
{
    public sealed class DFSATransition<T, S> : IScored
        where T : class
    {
        private Object transitionID;
        private DFSAState<T, S> source;
        protected DFSAState<T, S> target;
        private double score;
        private T input;
        private Object output;
        public DFSATransition(Object transitionID, DFSAState<T, S> source, DFSAState<T, S> target, T input, Object output, double score)
        {
            this.transitionID = transitionID;
            this.source = source;
            this.target = target;
            this.input = input;
            this.output = output;
            this.score = score;
        }

        public DFSAState<T, S> GetSource()
        {
            return source;
        }

        public DFSAState<T, S> Source()
        {
            return source;
        }

        public DFSAState<T, S> GetTarget()
        {
            return target;
        }

        public DFSAState<T, S> Target()
        {
            return target;
        }

        public Object GetID()
        {
            return transitionID;
        }

        public double Score()
        {
            return score;
        }

        public T GetInput()
        {
            return input;
        }

        public T Input()
        {
            return input;
        }

        public Object GetOutput()
        {
            return output;
        }

        public Object Output()
        {
            return output;
        }

        public override string ToString()
        {
            return @"[" + transitionID + @"]" + source + @" -" + input + @":" + output + @"-> " + target;
        }
    }
}
