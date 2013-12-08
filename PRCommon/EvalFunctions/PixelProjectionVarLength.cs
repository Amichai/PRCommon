using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class PixelProjectionVarLength : EvalBase, IEvalFunc {
        public IntPoint? GetPoint() {
            return Points.Take(1).Single();
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            foreach (var f in Features) {
                pts.AddRange(f.Projection.GetPoints());
            }
            return pts;
        }

        private double randomNormal(double mean, double stdDev) {
            double u1 = Rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = Rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            var randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        public bool GaussianDist { get; set; }

        public PixelProjectionVarLength(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            if (points.Count() > 1) throw new Exception();
            this.Eval = i => projEval(i, points);
        }

        public int PointCount() {
            throw new NotImplementedException();
        }

        List<double> weights = new List<double>();
        public List<Feature> Features { get; set; }

        private double projEval(int[][] input, List<IntPoint> pts) {
            if (weights.Count() == 0) {
                foreach (var f in Features) {
                    weights.Add(randomNormal(1, .5));
                }
            }
            double refEval;
            double output = 1;
            for (int i = 0; i < this.Features.Count(); i++) {
                var f = Features[i];
                if (HashedResults.ContainsKey(f.Projection.Eval)) {
                    refEval = HashedResults[f.Projection.Eval];
                } else {
                    refEval = f.Projection.Eval(input);
                }
                //output *= refEval * (double)f.SuccessRate.Overall.LastN() / successTotal;
                output *= refEval * this.weights[i];
            }
            HashedResults[Eval] = output;
            return output;
        }
    }
}
