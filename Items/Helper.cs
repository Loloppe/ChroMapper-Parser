using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TMPro;

namespace Parser.Items
{
    internal class Helper
    {
        public static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Name"
            };

            TextBox textBox = new TextBox
            {
                Size = new System.Drawing.Size(size.Width - 10, 23),
                Location = new System.Drawing.Point(5, 5),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&OK",
                Location = new System.Drawing.Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&Cancel",
                Location = new System.Drawing.Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
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

        public static int VerifySelection(HashSet<BeatmapObject> selection)
        {
            if (selection.Count > 0)
            {
                if (selection.All(x => x is BeatmapNote))
                {
                    Parser.type.Dropdown.value = 0;
                    return 0;
                }
                else if (selection.All(x => x is MapEvent))
                {
                    Parser.type.Dropdown.value = 1;
                    return 1;
                }
                else if (selection.All(x => x is BeatmapObstacle))
                {
                    Parser.type.Dropdown.value = 2;
                    return 2;
                }
            }

            return -1;
        }

        public static void CopySelection(HashSet<BeatmapObject> selection, int type)
        {
            if(type == -1)
            {
                Debug.Log("Parser: Invalid selection");
                return;
            }

            var option = new TMP_Dropdown.OptionData
            {
                text = Parser.name.InputField.text
            };

            Parser.dropdown.Dropdown.options.Add(option);
            Parser.dropdown.Dropdown.value = Parser.dropdown.Dropdown.options.Count - 1;
            Parser.dropdown.Dropdown.RefreshShownValue();

            switch (type)
            {
                case 0:
                    Parser.data.NoteOptions.Add(option);
                    List<BeatmapNote> notes = new List<BeatmapNote>();
                    selection.ToList().ForEach(x => notes.Add((BeatmapNote)x));
                    Parser.data.Notes.Add(notes);
                    return;
                case 1:
                    Parser.data.EventOptions.Add(option);
                    List<MapEvent> events = new List<MapEvent>();
                    selection.ToList().ForEach(x => events.Add((MapEvent)x));
                    Parser.data.Events.Add(events);
                    return;
                case 2:
                    Parser.data.ObstacleOptions.Add(option);
                    List<BeatmapObstacle> obstacles = new List<BeatmapObstacle>();
                    selection.ToList().ForEach(x => obstacles.Add((BeatmapObstacle)x));
                    Parser.data.Obstacles.Add(obstacles);
                    return;
            }
        }

        public static void PasteSelection()
        {
            if (Parser.dropdown.Dropdown.options.Count > 0)
            {
                switch(Parser.type.Dropdown.value)
                {
                    case 0:
                        if (Parser.data.Notes[Parser.dropdown.Dropdown.value].Count > 0)
                        {
                            List<BeatmapNote> toSpawn = new List<BeatmapNote>(Parser.data.Notes[Parser.dropdown.Dropdown.value]);

                            foreach (var no in toSpawn)
                            {
                                var n = new BeatmapNote(no.ConvertToJson());
                                n.Time = (n.Time - toSpawn[0].Time) + Parser.atsc.CurrentBeat;
                                Parser._notesContainer.SpawnObject(n, false, false);
                            }

                            BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).RefreshPool(true);
                        }
                        return;
                    case 1:
                        if (Parser.data.Events[Parser.dropdown.Dropdown.value].Count > 0)
                        {
                            List<MapEvent> toSpawn = new List<MapEvent>(Parser.data.Events[Parser.dropdown.Dropdown.value]);

                            foreach (var ev in toSpawn)
                            {
                                var e = new MapEvent(ev.ConvertToJson());
                                e.Time = (e.Time - toSpawn[0].Time) + Parser.atsc.CurrentBeat;
                                Parser._eventsContainer.SpawnObject(e, false, false);
                            }

                            BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Event).RefreshPool(true);
                        }
                        return;
                    case 2:
                        if (Parser.data.Obstacles[Parser.dropdown.Dropdown.value].Count > 0)
                        {
                            List<BeatmapObstacle> toSpawn = new List<BeatmapObstacle>(Parser.data.Obstacles[Parser.dropdown.Dropdown.value]);

                            foreach (var obs in toSpawn)
                            {
                                var ob = new BeatmapObstacle(obs.ConvertToJson());
                                ob.Time = (ob.Time - toSpawn[0].Time) + Parser.atsc.CurrentBeat;
                                Parser._obstaclesContainer.SpawnObject(ob, false, false);
                            }

                            BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Obstacle).RefreshPool(true);
                        }
                        return;
                }
            }

            Debug.Log("Parser: Nothing to paste");
        }
    }
}
