using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class PixelEval : EvalBase, IEvalFunc {
        public int I;
        public int J;

        public IntPoint? GetPoint() {
            return GetPoints().Single();
        }

        public List<IntPoint> GetPoints() {
            return new List<IntPoint>() { new IntPoint() { X = I, Y = J } };
        }

        public int PointCount() {
            return 1;
        }

        public PixelEval(int i, int j) {
            this.I = i;
            this.J = j;
            this.Eval = k => k[I][J] ;
        }
    }
}
