﻿namespace CodingTracker.Models;
internal class CodingSession
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime Duration { get; set; }

    internal void CalculateDuration()
    {

    }
}
