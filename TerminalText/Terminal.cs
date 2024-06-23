using System;
using System.Collections.Generic;
using System.Timers;

namespace TerminalText
{
    internal class Terminal
    {
        public List<string> lines;

        public System.Timers.Timer fpsTimer;
        public int fps;
        public bool awaitingUserInput;

        private bool cursorVis;
        public bool cursorVisible
        {
            get
            {
                return cursorVis;
            }
            set
            {
                Console.CursorVisible = value;
                cursorVis = value;
            }
        }

        public Terminal(int _fps)
        {
            cursorVisible = false;

            Console.Clear();
            lines = new List<string>();
            Render();

            fps = _fps;
            awaitingUserInput = false;

            // Fuck off mr Hm i dont think you've set this variable before the end of this functoin (despite the fact SetFPS() does)
            fpsTimer = new System.Timers.Timer();
            SetFPS(_fps);

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);
        }

        void ProcessExit(object? source, EventArgs  e)
        {
            cursorVisible = true;
        }

        public void AddLine(string line)
        {
            lines.Add(TerminalFancyTextParser.ParseTextWithANSICodes(line));
        }

        public void SetFPS(int newFps)
        {
            fpsTimer = new System.Timers.Timer(1d / newFps);
            fpsTimer.Elapsed += RenderElapsedHandler;
            fpsTimer.AutoReset = true;
            fpsTimer.Enabled = true;
        }

        void RenderElapsedHandler(object? source, ElapsedEventArgs e)
        {
            Render();
        }

        public void Render()
        { 
            // erase all
            Console.Clear();
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
            Console.Write(TerminalANSICodes.BuildANSITag(TerminalANSICodes.ResetAll));
        }

        void ReadStart()
        {
            awaitingUserInput = true;
            fpsTimer.Enabled = false;
            // quickly render everything to prevent issues
            Render();
        }

        void ReadEnd()
        {
            awaitingUserInput = false;
            fpsTimer.Enabled = true;
        }

        public string ReadLine()
        {
            ReadStart();
            string input = Console.ReadLine() ?? "";
            ReadEnd();
            return input;
        }

        public string Question(string question, bool cursor = true)
        {
            bool prevThing = cursorVisible;

            ReadStart();
            Console.Write(TerminalFancyTextParser.ParseTextWithANSICodes(question));

            cursorVisible = cursor;
            string input = Console.ReadLine() ?? "";

            ReadEnd();
            Render();

            cursorVisible = prevThing;
            return input;
        }

        public ConsoleKeyInfo ReadKey()
        {
            ReadStart();
            ConsoleKeyInfo input = Console.ReadKey();
            ReadEnd();
            return input;
        }

        public string ChooseFromSelection(string[] selections, string pick = " \\<", bool doWrapping = true,
                                          ConsoleKey moveUp = ConsoleKey.UpArrow, ConsoleKey moveDown = ConsoleKey.DownArrow, 
                                          ConsoleKey select = ConsoleKey.Enter)
        {
            string[] selectionsDone = new string[selections.Length];
            string pickDone = TerminalFancyTextParser.ParseTextWithANSICodes(pick);

            if (selections.Length <= 0)
                throw new Exception("selections.Length must be greater than 0.");
            int selection = 0;
            ReadStart();
            for (int i = 0; i < selections.Length; i++)
            {
                selectionsDone[i] = TerminalFancyTextParser.ParseTextWithANSICodes(selections[i]);
                Console.WriteLine(selectionsDone[i] + (i == selection ? pickDone : ""));
            }
            while (true)
            {
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key == select)
                    break;
                int oldSelection = selection;
                if (input.Key == moveUp) 
                {
                    if (doWrapping && selection == 0)
                        selection = selections.Length - 1;
                    else
                        selection--;
                } 
                else if (input.Key == moveDown) 
                {
                    if (doWrapping && selection == selections.Length - 1)
                        selection = 0;
                    else
                        selection++;     
                }
                if (oldSelection != selection)
                {
                    Render();
                    for (int i = 0; i < selections.Length; i++)
                        Console.WriteLine(selectionsDone[i] + (i == selection ? pickDone : ""));
                }
            }
            ReadEnd();
            return selections[selection];   
        }
    }
}
