using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class PixelQuot: EvalBase, IEvalFunc {
        public IntPoint? GetPoint() {
            return Points.Take(1).Single();
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            return pts;
        }

        public PixelQuot(IntPoint point, IEvalFunc refFunc) {
            this.Ref = refFunc;
            this.Points = new List<IntPoint>() { point };
            this.Eval = i => diffEval(i, point);
        }

        public int PointCount() {
            if (FuncPixelCount.ContainsKey(Ref.Eval)) {
                return FuncPixelCount[Ref.Eval] + this.Points.Count();
            } else {
                return this.Points.Count() + Ref.PointCount();
            }
        }

        private double diffEval(int[][] input, IntPoint p) {
            double output = 1;
            double refEval;
            if (HashedResults.ContainsKey(Ref.Eval)) {
                refEval = HashedResults[Ref.Eval];
            } else {
                refEval = Ref.Eval(input);
            }
            output = (refEval) / (refEval + input[p.X][p.Y] + .00001);
            HashedResults[Eval] = output;
            return output;
        }
    }
}
