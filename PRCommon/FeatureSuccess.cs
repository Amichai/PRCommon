using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PRCommon {
    public class FeatureSuccess {
        public FeatureSuccess() {
            this.SuccessRate = new Dictionary<string, PastValues>();
            this.Overall = new PastTrials();
            this.LabelSuccess = new Dictionary<string, PastTrials>();
            this.TrialCounter = 0;
            this.Outcomes = new ProbabilityOutcomes();
        }

        public Dictionary<string, PastValues> SuccessRate { get; set; }
        public string BestGuess { get; set; }

        public int TrialCounter { get; set; }
        public ProbabilityOutcomes Outcomes;

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
            updateOutcomes(label, probabilities, guess);
        }

        private void updateOutcomes(string label, Dictionary<string, double> probabilities, string guess) {
            if (guess == null || !probabilities.ContainsKey(guess)) {
                return;
            }
            bool result = label == guess;
            this.Outcomes.Add(probabilities[guess], result);
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

        public XElement ToXml() {
            XElement root = new XElement("Success");
            root.Add(new XElement("Overall", this.Overall.ToXml()));
            XElement labelSuccess = new XElement("LabelSuccess");
            foreach (var l in this.LabelSuccess) {
                labelSuccess.Add(new XElement("Label"+ l.Key, l.Value.ToXml()));
            }
            root.Add(labelSuccess);
            return root;
        }
    }

    public class PastTrials {
        private Monoticity monoticity { get; set; }

        Queue<bool> results;
        private int count { get; set; }

        public int Count() {
            return results.Count();
        }

        public PastTrials(int count = 100) {
            this.results = new Queue<bool>(count);
            this.count = count;
            this.monoticity = new Monoticity();
            this.RunningExponential = 1;
            this.RunningGeometric = 0;
        }

        public void Add(bool result) {
            this.results.Enqueue(result);
            if (results.Count() > count) {
                results.Dequeue();
            }
            this.monoticity.Add(this.LastN());
            total++;
            if (result) {
                correct++;
                this.RunningGeometric += total;
                this.RunningExponential = this.RunningExponential * .8 + .2;
            } else {
                this.RunningExponential = this.RunningExponential * .8;
            }
        }

        public double Max {
            get {
                return monoticity.Max();
            }
        }

        public double Monoticity {
            get {
                return monoticity.Get();
            }
        }

        /// <summary>Last 100</summary>
        public double LastN() {
            if (results.Count() == 0) return 0;
            return (double)results.Where(i => i == true).Count() / results.Count();
        }

        public double LastN(int n) {
            if (results.Take(n).Count() == 0) return 0;
            return (double)results.Take(n).Where(i => i == true).Count() / results.Take(n).Count();
        }

        private double correct = 0;
        private double total = 0;

        public double RunningExponential;
        public double RunningGeometric;

        public double Probability() {
            if (total == 0) {
                return 0;
            }
            return correct / total;
        }

        internal XElement ToXml() {
            XElement root = new XElement("PastTrials");
            root.Add(new XAttribute("TotalSeen", this.total));
            root.Add(new XAttribute("Last100", this.LastN()));
            root.Add(new XAttribute("BestProbability", this.monoticity.Max()));
            root.Add(new XAttribute("RunningGeometric", this.RunningGeometric));
            root.Add(new XAttribute("RunningExponential", this.RunningExponential));
            root.Add(new XAttribute("OverallProbability", this.Probability()));
            return root;
        }
    }
}
