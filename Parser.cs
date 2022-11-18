// MIT License

// Copyright (c) 2022 Loloppe

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Parser.Items;
using UnityEngine;
using TMPro;
using System.IO;
using SimpleJSON;

namespace Parser
{
    [Plugin("Paster")]
    public class Parser
    {
        private UI.UI _ui;
        static internal BeatSaberSongContainer _beatSaberSongContainer;
        private NotesContainer _notesContainer;
        private EventsContainer _eventsContainer;
        private ObstaclesContainer _obstaclesContainer;
        static internal UIDropdown dropdown;
        static internal UIDropdown type;
        static internal Data data;

        [Init]
        private void Init()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            _ui = new UI.UI(this);
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex == 3)
            {
                _notesContainer = Object.FindObjectOfType<NotesContainer>();
                _eventsContainer = Object.FindObjectOfType<EventsContainer>();
                _obstaclesContainer = Object.FindObjectOfType<ObstaclesContainer>();
                _beatSaberSongContainer = Object.FindObjectOfType<BeatSaberSongContainer>();

                data = new Data
                {
                    Events = new List<List<MapEvent>>(),
                    Notes = new List<List<BeatmapNote>>(),
                    Obstacles = new List<List<BeatmapObstacle>>(),
                    NoteOptions = new List<TMP_Dropdown.OptionData>(),
                    EventOptions = new List<TMP_Dropdown.OptionData>(),
                    ObstacleOptions = new List<TMP_Dropdown.OptionData>()
                };

                MapEditorUI mapEditorUI = Object.FindObjectOfType<MapEditorUI>();
                _ui.AddMenu(mapEditorUI);
            }
        }

        public void Copy()
        {
            if(type.Dropdown.value == 0)
            {
                List<MapEvent> select = null;

                var selection = SelectionController.SelectedObjects;
                if (selection.Count > 0)
                {
                    if (selection.All(x => x is MapEvent))
                    {
                        select = new List<MapEvent>(selection.Cast<MapEvent>());
                    }
                    else
                    {
                        Debug.Log("Paster: Make sure to select only MapEvent");
                    }
                }

                if (select != null)
                {
                    var dd = new TMP_Dropdown.OptionData
                    {
                        text = Options.Parser.Name
                    };

                    dropdown.Dropdown.options.Add(dd);
                    dropdown.Dropdown.value = dropdown.Dropdown.options.Count - 1;
                    dropdown.Dropdown.RefreshShownValue();

                    data.EventOptions.Add(dd);

                    List<MapEvent> list = new List<MapEvent>();
                    foreach (var x in select)
                    {
                        list.Add(x);
                    }
                    data.Events.Add(list);
                }
            }
            else if (type.Dropdown.value == 1)
            {
                List<BeatmapNote> select = null;

                var selection = SelectionController.SelectedObjects;
                if (selection.Count > 0)
                {
                    if (selection.All(x => x is BeatmapNote))
                    {
                        select = new List<BeatmapNote>(selection.Cast<BeatmapNote>());
                    }
                    else
                    {
                        Debug.Log("Paster: Make sure to select only Notes");
                    }
                }

                if (select != null)
                {
                    var dd = new TMP_Dropdown.OptionData
                    {
                        text = Options.Parser.Name
                    };

                    dropdown.Dropdown.options.Add(dd);
                    dropdown.Dropdown.value = dropdown.Dropdown.options.Count - 1;
                    dropdown.Dropdown.RefreshShownValue();

                    data.NoteOptions.Add(dd);

                    List<BeatmapNote> list = new List<BeatmapNote>();
                    foreach (var x in select)
                    {
                        list.Add(x);
                    }
                    data.Notes.Add(list);
                }
            }
            else if (type.Dropdown.value == 2)
            {
                List<BeatmapObstacle> select = null;

                var selection = SelectionController.SelectedObjects;
                if (selection.Count > 0)
                {
                    if (selection.All(x => x is BeatmapObstacle))
                    {
                        select = new List<BeatmapObstacle>(selection.Cast<BeatmapObstacle>());
                    }
                    else
                    {
                        Debug.Log("Paster: Make sure to select only Obstacles");
                    }
                }

                if (select != null)
                {
                    var dd = new TMP_Dropdown.OptionData
                    {
                        text = Options.Parser.Name
                    };

                    dropdown.Dropdown.options.Add(dd);
                    dropdown.Dropdown.value = dropdown.Dropdown.options.Count - 1;
                    dropdown.Dropdown.RefreshShownValue();

                    data.ObstacleOptions.Add(dd);

                    List<BeatmapObstacle> list = new List<BeatmapObstacle>();
                    foreach (var x in select)
                    {
                        list.Add(x);
                    }
                    data.Obstacles.Add(list);
                }
            }
        }
        public void Paste()
        {
            var selection = SelectionController.SelectedObjects;

            if (dropdown.Dropdown.options.Count > 0)
            {
                if (type.Dropdown.value == 0)
                {
                    if (data.Events[dropdown.Dropdown.value].Count > 0)
                    {
                        List<MapEvent> toSpawn = new List<MapEvent>(data.Events[dropdown.Dropdown.value]);

                        foreach (var ev in toSpawn)
                        {
                            var e = new MapEvent(ev.ConvertToJson());
                            AudioTimeSyncController atsc = Object.FindObjectOfType<AudioTimeSyncController>();
                            e.Time += atsc.CurrentBeat - toSpawn[0].Time;
                            _eventsContainer.SpawnObject(e, false, false);
                            selection.Add(e);
                        }

                        BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Event).RefreshPool(true);
                    }
                    else
                    {
                        Debug.Log("Paster: No events available");
                    }
                }
                else if (type.Dropdown.value == 1)
                {
                    if (data.Notes[dropdown.Dropdown.value].Count > 0)
                    {
                        List<BeatmapNote> toSpawn = new List<BeatmapNote>(data.Notes[dropdown.Dropdown.value]);

                        foreach (var note in toSpawn)
                        {
                            var no = new BeatmapNote(note.ConvertToJson());
                            AudioTimeSyncController atsc = Object.FindObjectOfType<AudioTimeSyncController>();
                            no.Time += atsc.CurrentBeat - toSpawn[0].Time;
                            _notesContainer.SpawnObject(no, false, false);
                            selection.Add(no);
                        }

                        BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).RefreshPool(true);
                    }
                    else
                    {
                        Debug.Log("Paster: No notes available");
                    }
                }
                else if (type.Dropdown.value == 2)
                {
                    if (data.Obstacles[dropdown.Dropdown.value].Count > 0)
                    {
                        List<BeatmapObstacle> toSpawn = new List<BeatmapObstacle>(data.Obstacles[dropdown.Dropdown.value]);

                        foreach (var obs in toSpawn)
                        {
                            var ob = new BeatmapObstacle(obs.ConvertToJson());
                            AudioTimeSyncController atsc = Object.FindObjectOfType<AudioTimeSyncController>();
                            ob.Time += atsc.CurrentBeat - toSpawn[0].Time;
                            _obstaclesContainer.SpawnObject(ob, false, false);
                            selection.Add(ob);
                        }

                        BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Obstacle).RefreshPool(true);
                    }
                    else
                    {
                        Debug.Log("Paster: No obstacles available");
                    }
                }
            }
        }

        public void Remove()
        {
            if (dropdown.Dropdown.options.Count > 0)
            {
                if (type.Dropdown.value == 0)
                {
                    data.Events.RemoveAt(dropdown.Dropdown.value);
                    data.EventOptions.RemoveAt(dropdown.Dropdown.value);
                }
                else if (type.Dropdown.value == 1)
                {
                    data.Notes.RemoveAt(dropdown.Dropdown.value);
                    data.NoteOptions.RemoveAt(dropdown.Dropdown.value);
                }
                else if (type.Dropdown.value == 2)
                {
                    data.Obstacles.RemoveAt(dropdown.Dropdown.value);
                    data.ObstacleOptions.RemoveAt(dropdown.Dropdown.value);
                }
                dropdown.Dropdown.options.RemoveAt(dropdown.Dropdown.value);
                dropdown.Dropdown.value--;
                if(dropdown.Dropdown.value < 0)
                {
                    dropdown.Dropdown.value = 0;
                }
                dropdown.Dropdown.RefreshShownValue();
            }
        }

        public void Rename()
        {
            if (dropdown.Dropdown.options.Count > 0)
            {
                dropdown.Dropdown.options[dropdown.Dropdown.value].text = Options.Parser.Name;
                dropdown.Dropdown.RefreshShownValue();
            }
        }

        public void Load()
        {
            try
            {
                var path = System.AppDomain.CurrentDomain.BaseDirectory + "/Plugins/Parser/" + Options.Parser.Name + ".json";
                data = new Data
                {
                    Events = new List<List<MapEvent>>(),
                    Notes = new List<List<BeatmapNote>>(),
                    Obstacles = new List<List<BeatmapObstacle>>(),
                    EventOptions = new List<TMP_Dropdown.OptionData>(),
                    NoteOptions = new List<TMP_Dropdown.OptionData>(),
                    ObstacleOptions = new List<TMP_Dropdown.OptionData>()
                };
                dropdown.Dropdown.options.Clear();
                JsonData da = new JsonData();
                da = ReadFromJsonFile<JsonData>(path);

                for (int j = 0; j < da.Events.Count; j++)
                {
                    data.Events.Add(new List<MapEvent>());
                    data.EventOptions.Add(new TMP_Dropdown.OptionData(da.EventOptions[j]));
                    for (int i = 0; i < da.Events[j].Count; i++)
                    {
                        if(da.Events[j][i].CustomData != null)
                        {
                            List<char> charsToRemove = new List<char>() { '\"' };
                            da.Events[j][i].CustomData = Filter(da.Events[j][i].CustomData, charsToRemove);
                            MapEvent e = new MapEvent(da.Events[j][i].Time, da.Events[j][i].Type, da.Events[j][i].Value, JSONNode.Parse(da.Events[j][i].CustomData));
                            data.Events[j].Add(e);
                        }
                        else
                        {
                            MapEvent e = new MapEvent(da.Events[j][i].Time, da.Events[j][i].Type, da.Events[j][i].Value);
                            data.Events[j].Add(e);
                        }
                    }
                }

                for (int j = 0; j < da.Notes.Count; j++)
                {
                    data.Notes.Add(new List<BeatmapNote>());
                    data.NoteOptions.Add(new TMP_Dropdown.OptionData(da.NoteOptions[j]));
                    for (int i = 0; i < da.Notes[j].Count; i++)
                    {
                        if (da.Notes[j][i].CustomData != null)
                        {
                            List<char> charsToRemove = new List<char>() { '\"' };
                            da.Notes[j][i].CustomData = Filter(da.Notes[j][i].CustomData, charsToRemove);
                            BeatmapNote n = new BeatmapNote(da.Notes[j][i].Time, da.Notes[j][i].LineIndex, da.Notes[j][i].LineLayer, da.Notes[j][i].Type, da.Notes[j][i].CutDirection, JSONNode.Parse(da.Notes[j][i].CustomData));
                            data.Notes[j].Add(n);
                        }
                        else
                        {
                            BeatmapNote n = new BeatmapNote(da.Notes[j][i].Time, da.Notes[j][i].LineIndex, da.Notes[j][i].LineLayer, da.Notes[j][i].Type, da.Notes[j][i].CutDirection);
                            data.Notes[j].Add(n);
                        }
                    }
                }

                for (int j = 0; j < da.Obstacles.Count; j++)
                {
                    data.Obstacles.Add(new List<BeatmapObstacle>());
                    data.ObstacleOptions.Add(new TMP_Dropdown.OptionData(da.ObstacleOptions[j]));
                    for (int i = 0; i < da.Obstacles[j].Count; i++)
                    {
                        if (da.Obstacles[j][i].CustomData != null)
                        {
                            List<char> charsToRemove = new List<char>() { '\"' };
                            data.Obstacles[j][i].CustomData = Filter(data.Obstacles[j][i].CustomData, charsToRemove);
                            BeatmapObstacle e = new BeatmapObstacle(da.Obstacles[j][i].Time, da.Obstacles[j][i].LineIndex, da.Obstacles[j][i].Type, da.Obstacles[j][i].Duration, da.Obstacles[j][i].Width, JSONNode.Parse(da.Obstacles[j][i].CustomData));
                            data.Obstacles[j].Add(e);
                        }
                        else
                        {
                            BeatmapObstacle e = new BeatmapObstacle(da.Obstacles[j][i].Time, da.Obstacles[j][i].LineIndex, da.Obstacles[j][i].Type, da.Obstacles[j][i].Duration, da.Obstacles[j][i].Width);
                            data.Obstacles[j].Add(e);
                        }
                    }
                }

                dropdown.Dropdown.value = 1;
                dropdown.Dropdown.value = 0;
                type.Dropdown.value = 1;
                type.Dropdown.value = 0;
                dropdown.Dropdown.RefreshShownValue();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "/Plugins/Parser/");
                var path = System.AppDomain.CurrentDomain.BaseDirectory + "/Plugins/Parser/" + Options.Parser.Name + ".json";

                List<List<Event>> events = new List<List<Event>>();
                List<string> eventsOptions = new List<string>();

                for (int j = 0; j < data.Events.Count; j++)
                {
                    events.Add(new List<Event>());
                    eventsOptions.Add(data.EventOptions[j].text);
                    for (int i = 0; i < data.Events[j].Count; i++)
                    {
                        Event e = new Event(data.Events[j][i]);
                        events[j].Add(e);
                    }
                }

                List<List<Note>> notes = new List<List<Note>>();
                List<string> notesOptions = new List<string>();

                for (int j = 0; j < data.Notes.Count; j++)
                {
                    notes.Add(new List<Note>());
                    notesOptions.Add(data.NoteOptions[j].text);
                    for (int i = 0; i < data.Notes[j].Count; i++)
                    {
                        Note e = new Note(data.Notes[j][i]);
                        notes[j].Add(e);
                    }
                }

                List<List<Obstacle>> obstacles = new List<List<Obstacle>>();
                List<string> obstaclesOptions = new List<string>();

                for (int j = 0; j < data.Obstacles.Count; j++)
                {
                    obstacles.Add(new List<Obstacle>());
                    obstaclesOptions.Add(data.ObstacleOptions[j].text);
                    for (int i = 0; i < data.Obstacles[j].Count; i++)
                    {
                        Obstacle e = new Obstacle(data.Obstacles[j][i]);
                        obstacles[j].Add(e);
                    }
                }

                JsonData jsonData = new JsonData
                {
                    Events = events,
                    Notes = notes,
                    Obstacles = obstacles,
                    EventOptions = eventsOptions,
                    NoteOptions = notesOptions,
                    ObstacleOptions = obstaclesOptions
                };

                WriteToJsonFile(path, jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = Newtonsoft.Json.JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static string Filter(string str, List<char> charsToRemove)
        {
            foreach (char c in charsToRemove)
            {
                str = str.Replace(c.ToString(), System.String.Empty);
            }

            return str;
        }

        [Exit]
        private void Exit() { }
    }
}
