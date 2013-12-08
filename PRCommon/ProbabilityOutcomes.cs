using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class ProbabilityOutcomes {
        public ProbabilityOutcomes() {
            this.outcomes = new Dictionary<double, LabelProbOutcome>();
        }
        private Dictionary<double, LabelProbOutcome> outcomes;

        private double inc = .01;

        public void Add(double prob, bool result) {
            double i = Math.Round(prob, 1);
            if (!this.outcomes.ContainsKey(i)) {
                this.outcomes[i] = new LabelProbOutcome();
            }
            this.outcomes[i].Add(result);
        }

        public double Get(double prob) {
            double i = Math.Round(prob, 1);
            if (!this.outcomes.ContainsKey(i) || this.outcomes[i].Total < 10) {
                return .1;
            }
            return this.outcomes[i].Accuracy;
        }
    }

    public class LabelProbOutcome {
        public LabelProbOutcome() {
            this.Correct = 0;
            this.Incorrect = 0;
        }
        public int Correct { get; set; }
        public int Incorrect { get; set; }
        public int Total {
            get {
                return Correct + Incorrect;
            }
        }

        public double Accuracy {
            get {
                return (double)Correct / (double)Total;
            }
        }

        internal void Add(bool result) {
            if (result) {
                this.Correct++;
            } else {
                this.Incorrect++;
            }
        }
    }
}
