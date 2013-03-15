using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class PastValuesVec : List<PastValues> {
        PastValues pastDisances = new PastValues();
        public PastValuesVec(List<double> eval) {
            for(int i=0; i< eval.Count(); i++){
                this.Add(new PastValues(eval[i]));
            }
        }

        public PastValuesVec() { }

        public double Variance() {
            return pastDisances.Average();
        }

        public double Compare(List<double> list) {
            if (list.Count() != this.Count()) throw new Exception();
            var distance = this.Distance(list);
            if (pastDisances.Count < 1) return 0;
            return pastDisances.Compare(distance);
        }

        public List<double> AveragePosition() {
            List<double> vec = new List<double>();
            foreach (var a in this) {
                vec.Add(a.Average());
            }
            return vec;
        }

        public double Distance(List<double> list) {
            if (list.Count() != this.Count()) throw new Exception();
            double sum = 0;
            foreach (var val in this) {
                sum += (list.Average() - val.Average()).Sqrd();
            }
            return Math.Sqrt(sum);
        }

        public void Add(List<double> list) {
            if (this.Count() == 0) {
                for (int i = 0; i < list.Count(); i++) {
                    this.Add(new PastValues(list[i]));
                }
                return;
            }
            if (list.Count() != this.Count()) throw new Exception();
            for (int i = 0; i < list.Count(); i++) {
                this[i].Add(list[i]);
            }
            var distance = this.Distance(list);
            pastDisances.Add(distance);
        }
    }

    public class PastValues {
        List<double> pastReturnValues = new List<double>();
        public void Add(double val, int addDepth = 2) {
            //pastReturnValues.Add(val);
            Count++;
            Sum += val;
            if (addDepth > 0) {
                if (differencesFromMean == null) {
                    differencesFromMean = new PastValues();
                }
                differencesFromMean.Add((Average() - val) * (Average() - val), --addDepth);
            }
        }

        public double Compare(double val) {
            double distanceFromTheAverage = Math.Abs(val - Average());
            double sd = this.Moment(1);
            return this.Moment(2) / (distanceFromTheAverage * sd + (.1 / Count));
        }

        public int Count = 0;

        public double Sum = 0;

        public double Average() {
            return Sum / Count;
        }

        public double Moment(int depth = 1) {
            PastValues inspection = this;
            for (int i = 0; i < depth; i++) {
                inspection = inspection.differencesFromMean;
            }
            return inspection.Average();
        }

        PastValues differencesFromMean;
        public PastValues() {

        }
        public PastValues(double p) {
            this.Add(p);
        }
        ///Rolling standard deviation
        ///Rolling moments, etc.
    }
}
