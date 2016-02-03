using System;
using System.Collections.Generic;
using System.Linq;

namespace robotgrid
{
    /// <summary>
    /// Cosnole app to simulate robot movment on mars surface
    /// It's something that was put together quickly, I am not in love with static pattern infact it was more painful cause of everything is static.  
    /// I just wanted to write something quickly without providing a visual interface. 
    /// in real world I would be using services, inject dependencies and let methods return something meaningful instead of priting messages on console. 
    /// You will see lot of exceptions thrown, I understand its expensive but was trying to cover edge cases without doing a lot. 
    /// There is default list of commands that are being added you can add custom commands e.g "Back, B, 2, -1"
    /// 
    /// </summary>


    public static class Program
    {
        public static List<Command> AvailableCommands { get; set; }
        public static Position CurrentPosition;
        public static int[,] grid = null;

        public static void Main(string[] args)
        {
            if (AvailableCommands == null)
            {
                AvailableCommands = new List<Command>()
                {
                    new Command {Code = "F",Magnitude =1, Name ="Forward",Type = CommandType.Movement },
                    new Command {Code = "R",Magnitude =90, Name ="Right",Type = CommandType.Orientation },
                    new Command {Code = "L",Magnitude =-90, Name ="Left",Type = CommandType.Orientation },
                };
            }

            Console.WriteLine("press 1 to execute a command and 2 to add a new command");
            var c = Console.ReadLine();
            if (c != null)
            {
                if (c == "1")
                    ExecuteCommand();
                else if (c == "2")
                    AddCommand();
                else
                {
                    Console.WriteLine("Invalid Value to start program, enter '1' to start again.");
                    var i = Console.ReadLine();
                    if (i == "1")
                        Main(null);
                }
            }
        }

        public static void ExecuteCommand()
        {
            Console.WriteLine("al right lets move our robot on mars.");
            if (grid == null)
            {
                Console.WriteLine("we haven't set outer dimensions of our grid yet. enter x and y coordinates, seperated by ','  e.g. 5,3'");
                var input = Console.ReadLine();
                var dims = input.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
                if (dims == null || dims.Length != 2)
                {
                    Console.WriteLine("error validating imput");
                    Main(null);
                }
                int xis = int.Parse(dims[0]);
                int yis = int.Parse(dims[1]);
                if (xis > 49 || yis > 49)
                {
                    throw new InvalidOperationException("No Coordinates should be over 50");
                }
                grid = new int[xis + 1, yis + 1];
            }
            Console.WriteLine("enter starting coordinates (x-y-D)");
            var crd = Console.ReadLine();
            SetStartingPoint(crd);
            Console.WriteLine("Starting point is set, now issue set of instructions");
            var instructions = Console.ReadLine();
            if (instructions.Length > 99)
                throw new InvalidOperationException("Length of instruction should be under 100");
            MoveRobot(instructions.ToCharArray());


            Console.WriteLine("Final cooridnates " + CurrentPosition.x.ToString()
                + "-" + CurrentPosition.y.ToString()
                + "-" + CurrentPosition.Direction);

            Main(null);

        }

        public static void MoveRobot(char[] insts)
        {
            foreach (var a in insts)
            {
                var cmd = AvailableCommands.FirstOrDefault(x => x.Code.ToLower() == a.ToString().ToLower());
                if (string.IsNullOrEmpty(cmd.Code))
                {
                    Console.WriteLine("Couldn't find command '" + a + "' moving on to next instruction");
                    continue;
                }
                if (cmd.Type == CommandType.Orientation)
                {
                    var degrees = Math.Abs(CurrentPosition.Direction.GetHashCode() + cmd.Magnitude);
                    if (degrees > 360)
                        degrees = degrees % 360;
                    else if (degrees == 0)
                        degrees = Directions.N.GetHashCode();
                    CurrentPosition.Direction = (Directions)degrees;
                }
                else
                {
                    MovementResult r;
                    switch (CurrentPosition.Direction)
                    {
                        case Directions.N:
                            r = CheckEdge(CurrentPosition.x, CurrentPosition.y, Directions.N);
                            if (r == MovementResult.Allowed)
                                CurrentPosition.y += (int)cmd.Magnitude;
                            else if (r == MovementResult.Edge)
                            {
                                HandleLostRobotEvent();
                            }
                            break;
                        case Directions.S:
                            r = CheckEdge(CurrentPosition.x, CurrentPosition.y, Directions.S);
                            if (r == MovementResult.Allowed)
                                CurrentPosition.y -= (int)cmd.Magnitude;
                            else if (r == MovementResult.Edge)
                            {
                                HandleLostRobotEvent();
                            }
                            break;
                        case Directions.W:
                            r = CheckEdge(CurrentPosition.x, CurrentPosition.y, Directions.W);
                            if (r == MovementResult.Allowed)
                                CurrentPosition.x -= (int)cmd.Magnitude;
                            else if (r == MovementResult.Edge)
                            {
                                HandleLostRobotEvent();
                            }
                            break;
                        case Directions.E:
                            r = CheckEdge(CurrentPosition.x, CurrentPosition.y, Directions.E);
                            if (r == MovementResult.Allowed)
                                CurrentPosition.x += (int)cmd.Magnitude;
                            else if (r == MovementResult.Edge)
                            {
                                HandleLostRobotEvent();
                            }
                            break;
                    }
                }
            }

        }

        public static void HandleLostRobotEvent()
        {
            Console.WriteLine("Robot lost, last known coordinates " + " " + CurrentPosition.x.ToString() + "-"
                                  + CurrentPosition.y.ToString() + "-" + CurrentPosition.Direction);
            Main(null);
        }
        public static MovementResult CheckEdge(int idx, int idy, Directions d)
        {
            if ((idx == 0 && d == Directions.W)
                || (idy == 0 && d == Directions.S)
                || (idx == grid.GetLength(0) - 1 && d == Directions.E)
                || (idy == grid.GetLength(1) - 1 && d == Directions.N))
            {
                if (grid[idx, idy] == -1)
                    return MovementResult.OutOfBounds;
                else
                    grid[idx, idy] = -1;
                return MovementResult.Edge;
            }
            return MovementResult.Allowed;

        }

        public static void SetStartingPoint(string crd)
        {
            if (!string.IsNullOrEmpty(crd))
            {
                string[] a = crd.Split(new char[] { '-' }, StringSplitOptions.None);
                if (a.Length == 3)
                {
                    CurrentPosition.x = int.Parse(a[0]);
                    CurrentPosition.y = int.Parse(a[1]);
                    Directions d;
                    if (Enum.TryParse<Directions>(a[2].ToUpper(), out d))
                        CurrentPosition.Direction = d;
                }
                else
                {
                    Console.WriteLine("enter starting coordinates (x-y-D)");
                    SetStartingPoint(Console.ReadLine());
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        static void AddCommand()
        {
            Console.WriteLine("great i see you are here to add a new command, lets get started");
            Console.WriteLine("Enter a name for your command");
            var name = Console.ReadLine();
            Console.WriteLine("Enter a Code (single character) for your command (Forward --> F");
            var code = Console.ReadLine();
            //validate to see if command already exists in list.
            Console.WriteLine("Enter type of command, 1 for orientation and 2 for Movement");
            CommandType t;
            if (!Enum.TryParse(Console.ReadLine(), true, out t))
            {
                throw new ArgumentException();
            }
            Console.WriteLine("Enter magnitude");
            double m;
            if (!double.TryParse(Console.ReadLine(), out m))
            {
                throw new InvalidCastException();
            }

            Command nc = new Command()
            {
                Code = code,
                Magnitude = m,
                Name = name,
                Type = t
            };
            AvailableCommands.Add(nc);
            Main(null);
        }
    }
    public struct Command
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public CommandType Type { get; set; }
        public double Magnitude { get; set; }
    }

    public enum MovementResult
    {
        Allowed,
        Edge,
        OutOfBounds
    }
    public enum CommandType
    {
        Orientation = 1,
        Movement = 2
    }

    public enum Directions
    {
        N = 360,
        E = 90,
        S = 180,
        W = 270,
    }

    public struct Position
    {
        public int x { get; set; }
        public int y { get; set; }
        public Directions Direction { get; set; }
    }
}
