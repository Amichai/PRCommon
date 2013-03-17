using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public interface IEvalFunc {
        List<IntPoint> GetPoints();
        List<IntPoint> GetPoint();
        Func<int[][], List<double>> Eval { get; set; }
        IEvalFunc Ref { get; set; }
    }

    public class EvalBase {
        public int IndexVal { get; set; }
        public static int Counter = 0;

        public static Dictionary<Func<int[][], List<double>>, List<double>> HashedResults = new Dictionary<Func<int[][], List<double>>, List<double>>();
        public EvalBase() {
            IndexVal = Counter++;
            //HashedResults = new Dictionary<Func<int[][], List<double>>, List<double>>();
        }
    }

    public class PixelEval : EvalBase, IEvalFunc {
        public int I;
        public int J;

        public List<IntPoint> GetPoint() {
            return GetPoints();
        }

        public List<IntPoint> GetPoints() {
            return new List<IntPoint>() { new IntPoint() { X = I, Y = J }};
        }

        public PixelEval(int i, int j) {
            this.I = i;
            this.J = j;
            this.Eval = k => new List<double>() { k[I][J] };
        }
        public IEvalFunc Ref { get; set; }
        public Func<int[][], List<double>> Eval { get; set; }
    }
    /*
    public class PixelSubset : EvalBase, IEvalFunc {
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
        public Func<int[][], List<double>> Eval { get; set; }
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
        public Func<int[][], List<double>> Eval { get; set; }
    }
    */
    public class PixelDiff : EvalBase, IEvalFunc {
        List<IntPoint> Points { get; set; }

        public List<IntPoint> GetPoint() {
            return Points.Take(1).ToList();
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            return pts;
        }

        public PixelDiff(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            if (Points.Count() > 1) throw new Exception();
            this.Eval = i => diffEval(i, points);
        }

        public IEvalFunc Ref { get; set; }

        private List<double> diffEval(int[][] input, List<IntPoint> pts) {
            if (pts.Count() > 1) {
                pts = pts.Take(1).ToList();
                //throw new Exception();
            }
            double output = 1;
            foreach (var p in pts) {
                double refEval;
                if (HashedResults.ContainsKey(Ref.Eval)) {
                    refEval = HashedResults[Ref.Eval].Average();
                } else {
                    refEval = Ref.Eval(input).Average();
                }
                output = refEval - input[p.X][p.Y];
            }
            var vec = new List<double>() { output }; 
            HashedResults[Eval] = vec;
            return vec;
        }

        public Func<int[][], List<double>> Eval { get; set; }
    }

    public class PixelSum : EvalBase, IEvalFunc {
        List<IntPoint> Points { get; set; }

        public List<IntPoint> GetPoint() {
            return Points.Take(1).ToList();
        }

        public List<IntPoint> GetPoints() {
            var pts = Points;
            pts.AddRange(Ref.GetPoints());
            return pts;
        }

        public PixelSum(List<IntPoint> points) {
            ///CHeck that all the points are unique and delete duplicates
            this.Points = points;
            if (points.Count() > 1) throw new Exception();
            this.Eval = i => sumEval(i, points);
        }

        public IEvalFunc Ref { get; set; }

        private List<double> sumEval(int[][] input, List<IntPoint> pts) {
            if (pts.Count() > 1) {
                pts = pts.Take(1).ToList();
                //throw new Exception();
            }


            double output = 1;
            foreach (var p in pts) {
                double refEval;
                if (HashedResults.ContainsKey(Ref.Eval)) {
                    refEval = HashedResults[Ref.Eval].Average();
                } else {
                    refEval = Ref.Eval(input).Average();
                }
                output = refEval + input[p.X][p.Y];
            }
            var vec = new List<double>() { output };
            HashedResults[Eval] = vec;
            return vec;
        }

        public Func<int[][], List<double>> Eval { get; set; }
    }
}
