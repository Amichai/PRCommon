using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class PixelProd : EvalBase, IEvalFunc {
        public IntPoint? GetPoint() {
            return Points.Take(1).Single();
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            return pts;
        }

        public PixelProd(IntPoint point) {
            this.Points = new List<IntPoint>() { point };
            this.Eval = i => prodEval(i, point);
        }

        public int PointCount() {
            if (FuncPixelCount.ContainsKey(Ref.Eval)) {
                return FuncPixelCount[Ref.Eval] + this.Points.Count();
            } else {
                return this.Points.Count() + Ref.PointCount();
            }
        }

        private double prodEval(int[][] input, IntPoint p) {
            double output = 1;
            double refEval;
            if (HashedResults.ContainsKey(Ref.Eval)) {
                refEval = HashedResults[Ref.Eval];
            } else {
                refEval = Ref.Eval(input);
            }
            output = refEval * input[p.X][p.Y];
            HashedResults[Eval] = output;
            return output;
        }
    }
}
