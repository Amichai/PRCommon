using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class Monoticity {
        double maxVal = int.MinValue;
        PastValues ratio = new PastValues();
        int counter = 0;
        public void Add(double val) {
            counter++;
            if (counter < 10) return;
            if (val > maxVal) {
                this.maxVal = val;
            }
            if (this.maxVal == 0) return;
            this.ratio.Add(val / this.maxVal);
        }

        public double Get() {
            if (ratio.Count > 1) {
                return this.ratio.Average();
            } else {
                return 0;
            }
        }

        internal double Max() {
            return maxVal;
        }
    }
}
