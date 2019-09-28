using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Herbivore_data_collector
{
    public partial class Form1 : Form
    {
        public List<string> list = new List<string>();
        public Dictionary<string, Dictionary<string, int>> paths = new Dictionary<string, Dictionary<string, int>>();
        public static string savefile = "G:/Users/Jozhus/Desktop/Files/Downloads/Herbiboar data.txt";
        public List<string> currentpath;
        public Form1()
        {
            InitializeComponent();
            updateData();
            currentpath = new List<string>();
            foreach (string x in list)
            {
                output.AppendText(x + Environment.NewLine);
            }
        }

        private void End_Click(object sender, EventArgs e)
        {
            if (output.Text.Length > 0)
            {
                if (output.Text.Substring(output.Text.Length - 1) == ">")
                {
                    string recentline = output.Lines[output.Lines.Length - 1];
                    output.Text = output.Text.Trim().Remove(output.Text.Length - 1);
                    System.IO.StreamWriter file = File.AppendText(savefile);
                    file.WriteLine(recentline.Trim().Remove(recentline.Length - 1));
                    file.Close();
                    interpret(recentline);
                    output.AppendText(Environment.NewLine);
                }
                currentpath = new List<string>();
            }
            prediction.Clear();
            foreach (string key in paths.Keys)
            {
                paths[key].Remove("");
            }
        }

        private void pointClick(object sender, EventArgs e)
        {
            string point = ((Button)sender).Text;
            currentpath.Add(point);
            output.AppendText(point + ">");
            prediction.Clear();
            decimal total = 0;
            if (paths.ContainsKey(point))
            {
                foreach (KeyValuePair<string, int> future in paths[point])
                {
                    if (currentpath.Count() > 1 && currentpath[currentpath.Count - 2] == future.Key)
                    {
                        continue;
                    }
                    total += future.Value;
                }
                foreach (KeyValuePair<string, int> future in paths[point])
                {
                    if (currentpath.Count() > 1 && currentpath[currentpath.Count - 2] == future.Key)
                    {
                        continue;
                    }
                    prediction.AppendText(future.Key + "(" + future.Value + ") " + (future.Value * 100 / total).ToString("N2") + "%" + Environment.NewLine);
                }
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            output.Clear();
            currentpath = new List<string>();
        }

        private void updateData()
        {
            string[] lines;
            var fileStream = new FileStream(@savefile, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                    interpret(line);
                }
            }
            lines = list.ToArray();
        }

        private void interpret(string line)
        {
            String[] path = line.Split('>');
            for (int i = 0; i < path.Length - 1; i++)
            {
                Dictionary<string, int> points;
                int count;
                if (!paths.TryGetValue(path[i], out points))
                {
                    points = new Dictionary<string, int>();
                    paths[path[i]] = points;
                }
                if(!paths[path[i]].TryGetValue(path[i + 1], out count))
                {
                    paths[path[i]][path[i + 1]] = 0;
                }
                paths[path[i]][path[i + 1]]++;
                
            }
        }
    }
}
