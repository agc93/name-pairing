using System.Diagnostics;

namespace NamePairing
{
    public class MatchService
    {
        public Dictionary<Participant, Participant> GetPairs(IEnumerable<Participant> participants) {
            var all = participants.ToList().Shuffle();
            var recipients = new List<Participant>(all);
            var targets = new Dictionary<Participant, Participant>();
            foreach (var participant in all)
            {
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