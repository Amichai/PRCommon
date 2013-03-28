using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public interface IEvalFunc {
        int PointCount();
        List<IntPoint> GetPoints();
        IntPoint? GetPoint();
        Func<int[][], double> Eval { get; set; }
        IEvalFunc Ref { get; set; }
    }

    public class EvalBase {
        public static Random Rand = new Random();
        public Func<int[][], double> Eval { get; set; }
        public List<IntPoint> Points { get; set; }
        public int IndexVal { get; set; }
        public static int Counter = 0;

        public static Dictionary<Func<int[][], double>, double> HashedResults = new Dictionary<Func<int[][], double>, double>();
        public EvalBase() {
            IndexVal = Counter++;
        }

        public IEvalFunc Ref { get; set; }

        public static Dictionary<Func<int[][], double>, int> FuncPixelCount = new Dictionary<Func<int[][], double>, int>();
    }
}
