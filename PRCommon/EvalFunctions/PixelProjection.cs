using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class PixelProjection : EvalBase, IEvalFunc {
        public IntPoint? GetPoint() {
            return Points.Take(1).Single();
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            pts.AddRange(Ref2.GetPoints());
            return pts;
        }

        public PixelProjection(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            if (points.Count() > 1) throw new Exception();
            this.Eval = i => projEval(i, points);
            this.weight = Rand.NextDouble();
        }

        private double weight;

        public int PointCount() {
            throw new NotImplementedException();
        }

        public IEvalFunc Ref2 { get; set; }

        private double projEval(int[][] input, List<IntPoint> pts) {
            if (pts.Count() > 1) {
                pts = pts.Take(1).ToList();
            }

            double output = 1;
            foreach (var p in pts) {
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
                output = this.weight * refEval +
                    (1 - this.weight) * ref2Eval;
            }
            HashedResults[Eval] = output;
            return output;
        }
    }
}
