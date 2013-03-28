using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class SymmetricalPixelDiff : EvalBase, IEvalFunc {
        public IntPoint? GetPoint() {
            return null;
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            pts.AddRange(Ref2.GetPoints());
            return pts;
        }

        public int PointCount() {
            int ref1Count, ref2Count;
            if (FuncPixelCount.ContainsKey(Ref.Eval)) {
                ref1Count = FuncPixelCount[Ref.Eval];
            } else {
                ref1Count = Ref.PointCount();
            }

            if (FuncPixelCount.ContainsKey(Ref2.Eval)) {
                ref2Count = FuncPixelCount[Ref2.Eval];
            } else {
                ref2Count = Ref2.PointCount();
            }
            return ref1Count + ref2Count;
        }

        public SymmetricalPixelDiff() {
            this.Eval = i => diffEval(i);
        }

        public IEvalFunc Ref2 { get; set; }

        private double diffEval(int[][] input) {
            double output = 1;
            double refEval;
            double ref2Eval;
            if (HashedResults.ContainsKey(Ref.Eval)) {
                refEval = HashedResults[Ref.Eval];
            } else {
                refEval = Ref.Eval(input);
            }

            if (HashedResults.ContainsKey(Ref2.Eval)) {
                ref2Eval = HashedResults[Ref2.Eval];
            } else {
                ref2Eval = Ref2.Eval(input);
            }
            output = refEval - ref2Eval;
            HashedResults[Eval] = output;
            return output;
        }
    }
}
