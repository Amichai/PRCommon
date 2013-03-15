using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public interface IEvalFunc {
        List<IntPoint> GetPoints();
        Func<int[][], List<double>> Eval { get; set; }
    }

    public class PixelEval : IEvalFunc {
        public int I;
        public int J;

        public List<IntPoint> GetPoints() {
            return new List<IntPoint>() { new IntPoint() { X = I, Y = J }};
        }

        public PixelEval(int i, int j) {
            this.I = i;
            this.J = j;
            this.Eval = k => new List<double>() { k[I][J] };
        }
        public Func<int[][], List<double>> Eval { get; set; }
    }

    public class PixelSubset : IEvalFunc {
        List<IntPoint> Points { get; set; }

        public List<IntPoint> GetPoints() {
            return Points;
        }

        public PixelSubset(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            this.Eval = k => k.SubsetEval(points).ToList();
        }

        public Func<int[][], List<double>> Eval { get; set; }
    }
}
