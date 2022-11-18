using System.Collections.Generic;
using TMPro;

namespace Parser.Items
{
    public class Data
    {
        public List<TMP_Dropdown.OptionData> NoteOptions { get; set; }
        public List<TMP_Dropdown.OptionData> EventOptions { get; set; }
        public List<TMP_Dropdown.OptionData> ObstacleOptions { get; set; }
        public List<List<MapEvent>> Events { get; set; }
        public List<List<BeatmapNote>> Notes { get; set; }
        public List<List<BeatmapObstacle>> Obstacles { get; set; }
    }

    public class JsonData
    {
        public List<string> NoteOptions { get; set; }
        public List<string> EventOptions { get; set; }
        public List<string> ObstacleOptions { get; set; }
        public List<List<Event>> Events { get; set; }
        public List<List<Note>> Notes { get; set; }
        public List<List<Obstacle>> Obstacles { get; set; }
    }

    public class Event
    {
        public float Time { get; set; }
        public int Type { get; set; }
        public int Value { get; set; }
        public string CustomData { get; set; }

        public Event()
        {

        }

        public Event(MapEvent ev)
        {
            Time = ev.Time;
            Type = ev.Type;
            Value = ev.Value;
            this.CustomData = ev.CustomData.ToString();
            if(this.CustomData == "null")
            {
                this.CustomData = null;
            }
        }
    }

    public class Note
    {
        public float Time { get; set; }
        public int LineIndex { get; set; }
        public int LineLayer { get; set; }
        public int Type { get; set; }
        public int CutDirection { get; set; }
        public string CustomData { get; set; }

        public Note()
        {

        }

        public Note(BeatmapNote no)
        {
            Time = no.Time;
            Type = no.Type;
            CutDirection = no.CutDirection;
            LineIndex = no.LineIndex;
            LineLayer = no.LineLayer;
            this.CustomData = no.CustomData.ToString();
            if (this.CustomData == "null")
            {
                this.CustomData = null;
            }
        }
    }

    public class Obstacle
    {
        public float Time { get; set; }
        public int Type { get; set; }
        public int LineIndex { get; set; }
        public int Width { get; set; }
        public float Duration { get; set; }
        public string CustomData { get; set; }

        public Obstacle()
        {

        }

        public Obstacle(BeatmapObstacle obs)
        {
            Time = obs.Time;
            Type = obs.Type;
            LineIndex = obs.LineIndex;
            Width = obs.Width;
            Duration = obs.Duration;
            this.CustomData = obs.CustomData.ToString();
            if (this.CustomData == "null")
            {
                this.CustomData = null;
            }
        }
    }
}
