using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class FeatureSuccess {
        public FeatureSuccess() {
            this.SuccessRate = new Dictionary<string, PastValues>();
            this.Overall = new PastTrials();
            this.LabelSuccess = new Dictionary<string, PastTrials>();
            this.TrialCounter = 0;
        }

        public Dictionary<string, PastValues> SuccessRate { get; set; }
        public string BestGuess { get; set; }

        public int TrialCounter { get; set; }

        public void Trial(string label, Dictionary<string, double> probabilities, string guess) {
            if (probabilities == null || !probabilities.ContainsKey(label)) return;
            this.BestGuess = guess;
            this.TrialCounter++;
            if (SuccessRate.ContainsKey(label)) {
                SuccessRate[label].Add(probabilities[label]);
            } else {
                SuccessRate[label] = new PastValues(probabilities[label]);
            }

            if (!LabelSuccess.ContainsKey(label)) {
                LabelSuccess[label] = new PastTrials();
            }
            if (label == BestGuess) {
                Overall.Add(true);
                LabelSuccess[label].Add(true);
            } else {
                Overall.Add(false);
                LabelSuccess[label].Add(false);

                if (BestGuess != null) {
                    if (!LabelSuccess.ContainsKey(BestGuess)) {
                        LabelSuccess[BestGuess] = new PastTrials();
                    }
                    LabelSuccess[BestGuess].Add(false);
                }
            }
        }

        public override string ToString() {
            string output = string.Empty;
            output += TrialCounter.ToString() + " total trials.\n";
            output += "Total success rate: " + Overall.LastN() + "\n";
            foreach (var a in LabelSuccess) {
                output += string.Format("{0} success for label {1}\n", a.Value.LastN(), a.Key);
            }
            return output;
        }

        public PastTrials Overall { get; set; }
        public Dictionary<string, PastTrials> LabelSuccess;
    }
    public class PastTrials {
        List<bool> results;
        private int count { get; set; }

        public int Count() {
            return results.Count();
        }

        public PastTrials(int count = 100) {
            this.results = new List<bool>(count);
            this.count = count;
        }

        public void Add(bool result) {
            this.results.Add(result);
            if (results.Count() > count) {
                results.RemoveAt(0);
            }
        }

        public double LastN2 {
            get {
                return LastN();
            }
            set {

            }
        }

        public double LastN() {
            return (double)results.Where(i => i == true).Count() / results.Count();
        }
    }
}
