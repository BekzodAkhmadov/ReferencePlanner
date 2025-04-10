﻿#nullable disable

using ConferenceDTO;

namespace FrontEnd.Services
{
    public interface IApiClient
    {
        Task<List<SessionResponse>> GetSessionsAsync();
        Task<SessionResponse> GetSessionAsync(int id);
        Task<List<SpeakerResponse>> GetSpeakersAsync();
        Task<SpeakerResponse> GetSpeakerAsync(int id);
        Task PutSessionAsync(Session session);
        Task<bool> AddAttendeeAsync(Attendee attendee);
        Task<AttendeeResponse> GetAttendeeAsync(string name);
        Task DeleteSessionAsync(int id);
        Task<List<SearchResult>> SearchAsync(string query);
    }
}
