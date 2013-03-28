using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {

    /*
    public class Pixelset : EvalBase, IEvalFunc {
        List<IntPoint> Points { get; set; }

        public List<IntPoint> GetPoints() {
            return Points;
        }

        public PixelSubset(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            this.Eval = k => k.SubsetEval(points).ToList();
        }
        public IEvalFunc Ref { get; set; }
    }

    public class PixelQuot : EvalBase, IEvalFunc { 
        List<IntPoint> Points { get; set; }

        public List<IntPoint> GetPoints() {
            return Points;
        }

        public PixelQuot(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            this.Eval = i => multEval(i, points);
        }

        private List<double> multEval(int[][] input, List<IntPoint> pts) {
            int midPoint = (int)Math.Ceiling(pts.Count() / 2.0);
            double sum1 = 0;
            for (int i = 0; i < midPoint; i++ ) {
                var p = pts[i];
                sum1 += input[p.X][p.Y];
            }

            double sum2 = 0;
            for (int i = midPoint; i < pts.Count(); i++) {
                var p = pts[i];
                sum2 += input[p.X][p.Y];
            }

            if (sum1 + sum2 == 0) {
                return new List<double>() { 0};
            }
            double quot = sum1 / (sum1 + sum2);
            return new List<double>() { quot };
        }
        public IEvalFunc Ref { get; set; }
    }
    */
}
