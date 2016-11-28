using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DebugConsole;
using System.Threading;

namespace DebugConsoleExample
{
    public partial class Form1 : Form
    {
        DebuggingConsole console;
        List<int> keys = new List<int>();
        RenderInformation lastr, r;
        Thread thread;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<CommandDescriptor> cmds = new List<CommandDescriptor>();
            cmds.Add(new CommandDescriptor("help", "lists all availble commands", false, CommandHandler_Help));
            cmds.Add(new CommandDescriptor("clear", "clears console window", false, CommandHandler_Clear));
            cmds.Add(new CommandDescriptor("add", "adds numbers", false, CommandHandler_Add));
            cmds.Add(new CommandDescriptor("threadTest", "adds numbers result after 5 seconds", true, CommandHandler_AddThreaded));
            cmds.Add(new CommandDescriptor("exec", "executes a given command (just for fun :D)", false, CommandHandler_Exec));
            cmds.Add(new CommandDescriptor("echo", "outputs a given text", false, CommandHandler_Echo));

            console = new DebuggingConsole(cmds.ToArray(), new DebugConsole.Color(255, 255, 255));
            console.Open();
            thread = new Thread(() => { Loop(); });
            thread.IsBackground = true;
            thread.Start();
        }

        private void CommandHandler_Exec(object sender, ExecuteCommandArgs e)
        {
            if (e.Args.Length > 0)
            {
                List<string> newArgs = new List<string>(e.Args);
                newArgs.RemoveAt(0);
                console.ExecuteCommand(e.Args[0], newArgs.ToArray());
            }
        }

        private void CommandHandler_Echo(object sender, ExecuteCommandArgs e)
        {
            if (e.Args.Length > 0)
            {
                console.WriteLine(e.Args[0], 0, 0, 0);
            }
        }

        private void CommandHandler_Help(object sender, ExecuteCommandArgs args)
        {
            foreach (KeyValuePair<string, CommandDescriptor> command in console.Commands)
            {
                console.WriteLine(command.Key + ": " + command.Value.Description, 0, 0, 0);
            }
        }

        private void CommandHandler_Clear(object sender, ExecuteCommandArgs args)
        {
            console.Clear();
        }

        private void CommandHandler_Add(object sender, ExecuteCommandArgs args)
        {
            try
            {
                int erg = 0;
                for (int i = 0; i < args.Args.Length; i++)
                {
                    erg += Convert.ToInt32(args.Args[i]);
                }
                console.WriteLine("Result: " + erg.ToString(), 0, 255, 0);
            }
            catch(Exception e)
            {
                console.WriteLine("Argument Exception: " + e.Message, 255, 0, 0);
            }
        }

        private void CommandHandler_AddThreaded(object sender, ExecuteCommandArgs args)
        {
            try
            {
                if (args.Args.Length > 0)
                {
                    int erg = 0;
                    for (int i = 0; i < args.Args.Length; i++)
                    {
                        erg += Convert.ToInt32(args.Args[i]);
                    }
                    Thread.Sleep(5000);
                    console.WriteLine("Result: " + erg.ToString(), 0, 255, 0);
                }
                else
                {
                    console.WriteLine("Command \"threadTest\" requires two or more arguments!", 255, 0, 0);
                }
            }
            catch (Exception e)
            {
                console.WriteLine("Argument Exception: " + e.Message, 255, 0, 0);
            }
        }


        private void Loop()
        {
            int sleep = 15;
            while (console.IsOpen)
            {
                lock (keys)
                {
                    if (keys.Count > 0)
                    {
                        for (int i = 0; i < keys.Count; i++)
                        {
                            if (keys[i] < 0)
                                console.Update(sleep + 2, keys[i] == -2, keys[i] == -1);
                            else
                                console.Update(sleep + 2, (char)keys[i], false, false);

                            keys.RemoveAt(0);
                            i--;
                        }
                    }
                    else
                    {
                        console.Update(sleep + 2, false, false);
                    }
                }
            
                lastr = r;
                r = console.RenderingInfo;
    

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)UpdateControls);
                }
                else
                {
                    UpdateControls();
                }
                Thread.Sleep(sleep);
            }
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            lock (keys)
            {
                if (e.KeyCode == Keys.Left)
                    keys.Add(-2);
                else if (e.KeyCode == Keys.Right)
                    keys.Add(-1);
                /*else
                {
                    if (e.Shift || e.KeyValue < 65 || e.KeyValue > 90)
                        keys.Add(e.KeyValue);
                    else if (e.Shift && e.KeyValue == 188)
                        keys.Add(59);
                    else
                        keys.Add(e.KeyValue + 32);
                }*/
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            console.Close();
            try
            {
                thread.Abort();
            }
            catch
            {

            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (keys)
            {
                keys.Add(e.KeyChar);
            }
        }

        private void UpdateControls()
        {
            if (lastr != null && r != null)
            {
                if (lastr.Lines.Length != r.Lines.Length)
                {
                    tbOutput.Clear();
                    for (int i = 0; i < r.Lines.Length; i++)
                    {
                        int index = tbOutput.TextLength;
                        tbOutput.AppendText(r.Lines[i] + Environment.NewLine);
                        tbOutput.Select(index, r.Lines[i].Length);
                        tbOutput.SelectionColor = System.Drawing.Color.FromArgb(r.LineColors[i].A, r.LineColors[i].B, r.LineColors[i].G, r.LineColors[i].B);
                        tbOutput.DeselectAll();
                    }
                }

                if (lastr.CommandLine != r.CommandLine)
                {
                    tbInput.Text = r.CommandLine;
                    tbInput.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    tbInput.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    AutoCompleteStringCollection auto = new AutoCompleteStringCollection();
                    auto.AddRange(r.AutoComplete);
                    tbInput.AutoCompleteCustomSource = auto;
                }
            }
        }
    }
}
