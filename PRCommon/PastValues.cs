﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PRCommon {
    public class PastValuesVec : List<PastValues> {
        PastValues pastDistances = new PastValues();
        public PastValuesVec(List<double> eval) {
            for(int i=0; i< eval.Count(); i++){
                this.Add(new PastValues(eval[i]));
            }
        }

        public PastValuesVec() { }

        public double Variance() {
            return pastDistances.Average();
        }

        public double Compare(List<double> list) {
            if (list.Count() != this.Count()) throw new Exception();
            var distance = this.Distance(list);
            if (pastDistances.Count < 1) return 0;
            return pastDistances.Compare(distance);
        }

        public double Compare2(List<double> list) {
            double prod = 1;
            for (int i = 0; i < list.Count(); i++) {
                prod *= this[i].Compare(list[i]);
            }
            return prod;
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
            pastDistances.Add(distance);
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

        private XElement serializeMoment() {
            var moment = new XElement("Moment");
            moment.Add(new XAttribute("Count", Count));
            moment.Add(new XAttribute("Sum", Sum));
            return moment;
        }

        internal XElement Serialize() {
            var root = new XElement("PastValues");
            PastValues inspection = this;
            while (inspection != null) {
                var moment = inspection.serializeMoment();
                root.Add(moment);
                inspection = inspection.differencesFromMean;
            }
            return root;
        }
    }
}
