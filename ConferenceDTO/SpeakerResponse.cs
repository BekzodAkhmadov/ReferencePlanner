namespace ConferenceDTO
{
    public class SpeakerResponse : Speaker
    {
        // TODO: Set order of JSON properties so this shows up last not first
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
