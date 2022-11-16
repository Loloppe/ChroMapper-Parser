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

namespace Parser
{
    [Plugin("Paster")]
    public class Parser
    {
        private UI.UI _ui;
        static public BeatSaberSongContainer _beatSaberSongContainer;
        private NotesContainer _notesContainer;
        private EventsContainer _eventsContainer;
        private ObstaclesContainer _obstaclesContainer;
        static public UIDropdown dropdown;
        static public UIDropdown type;
        static public List<TMP_Dropdown.OptionData> noteOptions;
        static public List<TMP_Dropdown.OptionData> eventOptions;
        static public List<TMP_Dropdown.OptionData> obstacleOptions;
        private List<List<MapEvent>> events;
        private List<List<BeatmapNote>> notes;
        private List<List<BeatmapObstacle>> obstacles;

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

                events = new List<List<MapEvent>>();
                notes = new List<List<BeatmapNote>>();
                obstacles = new List<List<BeatmapObstacle>>();
                noteOptions = new List<TMP_Dropdown.OptionData>();
                eventOptions = new List<TMP_Dropdown.OptionData>();
                obstacleOptions = new List<TMP_Dropdown.OptionData>();

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

                    eventOptions.Add(dd);

                    List<MapEvent> list = new List<MapEvent>();
                    foreach (var x in select)
                    {
                        list.Add(x);
                    }
                    events.Add(list);
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

                    noteOptions.Add(dd);

                    List<BeatmapNote> list = new List<BeatmapNote>();
                    foreach (var x in select)
                    {
                        list.Add(x);
                    }
                    notes.Add(list);
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

                    obstacleOptions.Add(dd);

                    List<BeatmapObstacle> list = new List<BeatmapObstacle>();
                    foreach (var x in select)
                    {
                        list.Add(x);
                    }
                    obstacles.Add(list);
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
                    if (events[dropdown.Dropdown.value].Count > 0)
                    {
                        List<MapEvent> toSpawn = new List<MapEvent>(events[dropdown.Dropdown.value]);

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
                    if (notes[dropdown.Dropdown.value].Count > 0)
                    {
                        List<BeatmapNote> toSpawn = new List<BeatmapNote>(notes[dropdown.Dropdown.value]);

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
                    if (obstacles[dropdown.Dropdown.value].Count > 0)
                    {
                        List<BeatmapObstacle> toSpawn = new List<BeatmapObstacle>(obstacles[dropdown.Dropdown.value]);

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
                    events.RemoveAt(dropdown.Dropdown.value);
                    eventOptions.RemoveAt(dropdown.Dropdown.value);
                }
                else if (type.Dropdown.value == 1)
                {
                    notes.RemoveAt(dropdown.Dropdown.value);
                    noteOptions.RemoveAt(dropdown.Dropdown.value);
                }
                else if (type.Dropdown.value == 2)
                {
                    obstacles.RemoveAt(dropdown.Dropdown.value);
                    obstacleOptions.RemoveAt(dropdown.Dropdown.value);
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

        [Exit]
        private void Exit() { }
    }
}
