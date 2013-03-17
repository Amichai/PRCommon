using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class GradientEval  {
        public List<IntPoint> GetPoints() {
            throw new NotImplementedException();
        }

        public Func<IntPoint, int[][], bool, double> gradientFunc;

        public Func<int[][], List<double>> Eval {
            get {
                ///Pick n points at random
                ///Walk the gradiet defined by the gradient function
                ///Return the longest average gradient
                
                
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }
    }
}
