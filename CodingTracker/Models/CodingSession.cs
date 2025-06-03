namespace CodingTracker.Models;
internal class CodingSession
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }

    internal CodingSession(int id, DateTime startTime, DateTime endTime)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        Duration = CalculateDuration();
    } 
    internal TimeSpan CalculateDuration()
    {
        if (EndTime < StartTime)
        {
            throw new ArgumentException("EndTime cannot be earlier than StartTime.");
        }
        return EndTime - StartTime;
    }
}
