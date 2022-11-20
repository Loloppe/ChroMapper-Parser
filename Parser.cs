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
using UnityEngine.SceneManagement;
using Parser.Items;
using UnityEngine;
using TMPro;
using System.IO;
using SimpleJSON;

namespace Parser
{
    [Plugin("Parser")]
    public class Parser
    {
        private UI.UI _ui;
        static internal BeatSaberSongContainer _beatSaberSongContainer;
        static internal NotesContainer _notesContainer;
        static internal EventsContainer _eventsContainer;
        static internal ObstaclesContainer _obstaclesContainer;
        static internal UIDropdown dropdown;
        static internal UIDropdown type;
        static internal UITextInput name;
        static internal Data data;
        static internal AudioTimeSyncController atsc;

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

                name = new UITextInput();
                atsc = Object.FindObjectOfType<AudioTimeSyncController>();

                MapEditorUI mapEditorUI = Object.FindObjectOfType<MapEditorUI>();
                _ui.AddMenu(mapEditorUI);
            }
        }

        public void Copy()
        {
            var selection = SelectionController.SelectedObjects;
            int type = Helper.VerifySelection(selection);
            Helper.CopySelection(selection, type);
        }

        public void Paste()
        {
            SelectionController.DeselectAll();
            Helper.PasteSelection();
        }

        public void Remove()
        {
            if (dropdown.Dropdown.options.Count > 0)
            {
                switch(type.Dropdown.value)
                {
                    case 0:
                        data.Notes.RemoveAt(dropdown.Dropdown.value);
                        data.NoteOptions.RemoveAt(dropdown.Dropdown.value);
                        break;
                    case 1:
                        data.Events.RemoveAt(dropdown.Dropdown.value);
                        data.EventOptions.RemoveAt(dropdown.Dropdown.value);
                        break;
                    case 2:
                        data.Obstacles.RemoveAt(dropdown.Dropdown.value);
                        data.ObstacleOptions.RemoveAt(dropdown.Dropdown.value);
                        break;
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
                dropdown.Dropdown.options[dropdown.Dropdown.value].text = name.InputField.text;
                dropdown.Dropdown.RefreshShownValue();
            }
        }

        public void SetName()
        {
            string text = name.InputField.text;
            Helper.ShowInputDialog(ref text);
            name.InputField.text = text;
            name.gameObject.SetActive(false);
            name.gameObject.SetActive(true);
        }

        public void Load()
        {
            try
            {
                var path = System.AppDomain.CurrentDomain.BaseDirectory + "/Plugins/Parser/" + name.InputField.text + ".json";
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
                da = Helper.ReadFromJsonFile<JsonData>(path);

                for (int j = 0; j < da.Events.Count; j++)
                {
                    data.Events.Add(new List<MapEvent>());
                    data.EventOptions.Add(new TMP_Dropdown.OptionData(da.EventOptions[j]));
                    for (int i = 0; i < da.Events[j].Count; i++)
                    {
                        if(da.Events[j][i].CustomData != null)
                        {
                            List<char> charsToRemove = new List<char>() { '\"' };
                            da.Events[j][i].CustomData = Helper.Filter(da.Events[j][i].CustomData, charsToRemove);
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
                            da.Notes[j][i].CustomData = Helper.Filter(da.Notes[j][i].CustomData, charsToRemove);
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
                            data.Obstacles[j][i].CustomData = Helper.Filter(data.Obstacles[j][i].CustomData, charsToRemove);
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
                var path = System.AppDomain.CurrentDomain.BaseDirectory + "/Plugins/Parser/" + name.InputField.text + ".json";
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

                
                foreach(var e in events)
                {
                    var first = e[0].Time;
                    foreach(var ev in e)
                    {
                        ev.Time -= first;
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

                foreach (var e in notes)
                {
                    var first = e[0].Time;
                    foreach (var ev in e)
                    {
                        ev.Time -= first;
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

                foreach (var e in obstacles)
                {
                    var first = e[0].Time;
                    foreach (var ev in e)
                    {
                        ev.Time -= first;
                    }
                }

                JsonData jsonData = new JsonData
                {
                    Events = events,
                    Notes = notes,
                    Obstacles = obstacles,
                    EventOptions = eventsOptions,
                    NoteOptions = notesOptions,
                    ObstacleOptions = obstaclesOptions,
                };

                Helper.WriteToJsonFile(path, jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [Exit]
        private void Exit() { }
    }
}
