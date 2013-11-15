using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class ScoredObject<T> : IScored
    {
        private double score;
        public double Score()
        {
            return score;
        }

        public virtual void SetScore(double score)
        {
            this.score = score;
        }

        private T object_renamed;
        public virtual T Object()
        {
            return object_renamed;
        }

        public virtual void SetObject(T object_renamed)
        {
            this.object_renamed = object_renamed;
        }

        public ScoredObject(T object_renamed, double score)
        {
            this.object_renamed = object_renamed;
            this.score = score;
        }

        public override string ToString()
        {
            return object_renamed + @" @ " + score;
        }
    }
}
