using System.Diagnostics;
using Polly;

namespace NamePairing
{
    public interface IMatchService
    {
        Dictionary<Participant, Participant>? GetPairs(IEnumerable<Participant> participants);
    }

    public class DifferenceMatchService : IMatchService
    {
        private static bool Allowed(Participant current, Participant candidate) {
            return current.Name != candidate.Name &&
                   !(current.Exclusions ?? new List<string>()).Contains(candidate.Name,
                       StringComparer.CurrentCultureIgnoreCase);
        }
        
        private static Dictionary<Participant, Participant> BuildPairs(IEnumerable<Participant> participants) {
            var all = participants.ToList().Shuffle();
            var recipients = new List<Participant>(all);
            var targets = new Dictionary<Participant, Participant>();
            var candidateSets = new Dictionary<Participant, List<Participant>>();
            foreach (var participant in all) {
                var candidates = recipients.Where(r => Allowed(participant, r)).ToList();
                candidateSets.Add(participant, candidates);
            }

            foreach (var candidateSet in candidateSets) {
                var firstCandidate = candidateSet.Value.Where(c => !targets.ContainsValue(c)).GetRandom();
                targets.Add(candidateSet.Key, firstCandidate);
            }

            return targets;
        }

        public Dictionary<Participant, Participant>? GetPairs(IEnumerable<Participant> participants) {
            var participantList = participants.ToList();
            var results = Policy
                .Handle<Exception>()
                .Retry(participantList.Count).ExecuteAndCapture(() => BuildPairs(participantList));
            return results.Outcome == OutcomeType.Successful ? results.Result : null;

            // throw results.FinalException;
        }
    }

    [Obsolete("Known issues with even numbers of participants, do not use.", true)]
    public class DumbMatchService : IMatchService
    {
        public Dictionary<Participant, Participant> GetPairs(IEnumerable<Participant> participants) {
            var all = participants.ToList().Shuffle();
            var recipients = new List<Participant>(all);
            var targets = new Dictionary<Participant, Participant>();
            foreach (var participant in all) {
                if (all.Count > 2) {
                    var pairTarget = recipients.FirstOrDefault(q =>
                        q.Name != participant.Name && (!targets.ContainsKey(q) || targets[q] != participant));
                    Debug.WriteLine($"Participant {participant.Name} found {(pairTarget?.Name ?? "none")}");
                    if (pairTarget != null) {
                        targets.Add(participant, pairTarget);
                        recipients.Remove(pairTarget);
                    }
                }
                else {
                    //TODO: need to cover this
                }
            }

            return targets;
        }
    }
}