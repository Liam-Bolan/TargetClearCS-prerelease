//Skeleton Program code for the AQA A Level Paper 1 Summer 2025 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetClearCS
{
    internal class Program
    {
        static Random RGen = new Random();

        static void Main(string[] args)
        {
            List<int> NumbersAllowed = new List<int>();
            List<int> Targets;
            int MaxNumberOfTargets = 20;
            int MaxTarget;
            int MaxNumber;
            bool TrainingGame;
            string difficulty = "";
            Console.Write("Enter y to play the training game, anything else to play a random game: ");
            string Choice = Console.ReadLine().ToLower();
            Console.WriteLine();
            if (Choice == "y")
            {
                MaxNumber = 1000;
                MaxTarget = 1000;
                TrainingGame = true;
                Targets = new List<int> { -1, -1, -1, -1, -1, 23, 9, 140, 82, 121, 34, 45, 68, 75, 34, 23, 119, 43, 23, 119 };
            }
            else
            {
                Console.Write("Select difficulty (standard, easy, medium, hard): ");
                difficulty = Console.ReadLine();
                while (difficulty != "standard" && difficulty != "easy" && difficulty != "medium" && difficulty != "hard")
                {
                    Console.Write("\nInvalid difficulty, enter again: ");
                    difficulty = Console.ReadLine();
                }
                Console.WriteLine();
                MaxNumber = 10;
                MaxTarget = 50;
                TrainingGame = false;
                Targets = CreateTargets(MaxNumberOfTargets, MaxTarget);
            }
            NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber, difficulty);
            PlayGame(Targets, NumbersAllowed, TrainingGame, MaxTarget, MaxNumber, difficulty);
            Console.ReadLine();
        }

        static void PlayGame(List<int> Targets, List<int> NumbersAllowed, bool TrainingGame, int MaxTarget, int MaxNumber, string difficulty)
        {
            int Score = 0;
            bool GameOver = false;
            string UserInput;
            List<string> UserInputInRPN;
            while (!GameOver)
            {
                DisplayState(Targets, NumbersAllowed, Score);
                Console.Write("Enter an expression: ");
                UserInput = Console.ReadLine();
                string answer = UserInput.ToUpper();
                if (answer == "QUIT" || GameOver)
                {
                    Console.WriteLine("\nGame over!");
                    Console.WriteLine($"Final score: {Score}");
                    System.Threading.Thread.Sleep(2000);
                    Environment.Exit(0);
                }
                Console.WriteLine();
                if (CheckIfUserInputValid(UserInput))
                {
                    UserInputInRPN = ConvertToRPN(UserInput);
                    if (CheckNumbersUsedAreAllInNumbersAllowed(NumbersAllowed, UserInputInRPN, MaxNumber))
                    {
                        if (CheckIfUserInputEvaluationIsATarget(Targets, UserInputInRPN, ref Score))
                        {
                            RemoveNumbersUsed(UserInput, MaxNumber, NumbersAllowed);
                            NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber, difficulty);
                        }
                    }
                }
                if(!CheckIfUserInputValid(UserInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid infix notation");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                }
                Score--;
                if (Targets[0] != -1)
                {
                    GameOver = true;
                }
                else
                {
                    UpdateTargets(Targets, TrainingGame, MaxTarget);
                }
               
            } 
        }

        static bool CheckIfUserInputEvaluationIsATarget(List<int> Targets, List<string> UserInputInRPN, ref int Score)
        {
            int UserInputEvaluation = EvaluateRPN(UserInputInRPN);
            bool UserInputEvaluationIsATarget = false;
            if (UserInputEvaluation != -1)
            {
                for (int Count = 0; Count < Targets.Count; Count++)
                {
                    if (Targets[Count] == UserInputEvaluation)
                    {
                        Score += 2;
                        Targets[Count] = -1;
                        UserInputEvaluationIsATarget = true;
                        
                    }
                }
                if (!UserInputEvaluationIsATarget)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Equation does not evaluate to a target");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                }
            }
            return UserInputEvaluationIsATarget;
        }

        static void RemoveNumbersUsed(string UserInput, int MaxNumber, List<int> NumbersAllowed)
        {
            List<string> UserInputInRPN = ConvertToRPN(UserInput);
            List<int> LargeNums = new List<int>();

            LargeNums.Add(25);
            LargeNums.Add(50);
            LargeNums.Add(75);
            LargeNums.Add(100);

            foreach (string Item in UserInputInRPN)
            {
                if (CheckValidNumber(Item, MaxNumber))
                {
                    if (NumbersAllowed.Contains(Convert.ToInt32(Item)))
                    {
                        NumbersAllowed.Remove(Convert.ToInt32(Item));
                    }
                }
            }
            foreach(int largenum in LargeNums)
            {
                if (NumbersAllowed.Contains(largenum))
                {
                    NumbersAllowed.Remove(largenum);
                }
            }
        }

        static void UpdateTargets(List<int> Targets, bool TrainingGame, int MaxTarget)
        {
            for (int Count = 0; Count < Targets.Count - 1; Count++)
            {
                Targets[Count] = Targets[Count + 1];
            }
            Targets.RemoveAt(Targets.Count - 1);
            if (TrainingGame)
            {
                Targets.Add(Targets[Targets.Count - 1]);
            }
            else
            {
                Targets.Add(GetTarget(MaxTarget));
            }
        }

        static bool CheckNumbersUsedAreAllInNumbersAllowed(List<int> NumbersAllowed, List<string> UserInputInRPN, int MaxNumber)
        {
            List<int> Temp = new List<int>();
            foreach (int Item in NumbersAllowed)
            {
                Temp.Add(Item);
            }
            MaxNumber = 512;
            foreach (string Item in UserInputInRPN)
            {
                if (CheckValidNumber(Item, MaxNumber))
                {
                    if (Temp.Contains(Convert.ToInt32(Item)))
                    {
                        Temp.Remove(Convert.ToInt32(Item));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Number(s) used are not allowed!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine();
                        return false;
                    }
                }
            }
            return true;
        }

        static bool CheckValidNumber(string Item, int MaxNumber)
        {
            if (Regex.IsMatch(Item, "^[0-9]+$"))
            {
                int ItemAsInteger = Convert.ToInt32(Item);
                if (ItemAsInteger > 0 && ItemAsInteger <= MaxNumber)
                {
                    return true;
                }
            }
            return false;
        }

        static void DisplayState(List<int> Targets, List<int> NumbersAllowed, int Score)
        {
            DisplayTargets(Targets);
            DisplayNumbersAllowed(NumbersAllowed);
            DisplayScore(Score);
        }

        static void DisplayScore(int Score)
        {
            Console.WriteLine($"Current score: {Score}");
            Console.WriteLine();
            Console.WriteLine();
        }

        static void DisplayNumbersAllowed(List<int> NumbersAllowed)
        {
            Console.Write("Numbers available: ");
            foreach (int Number in NumbersAllowed)
            {
                Console.Write($"{Number}  ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static void DisplayTargets(List<int> Targets)
        {
            Console.Write("|");
            foreach (int T in Targets)
            {
                if (T == -1)
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write(T);
                }
                Console.Write("|");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static List<string> ConvertToRPN(string UserInput)
        {
            int Position = 0;
            Dictionary<string, int> Precedence = new Dictionary<string, int>
            {
                { "+", 2 }, { "-", 2 }, { "*", 4 }, { "/", 4 }
            };
            List<string> Operators = new List<string>();
            int Operand = GetNumberFromUserInput(UserInput, ref Position);
            List<string> UserInputInRPN = new List<string> { Operand.ToString() };
            Operators.Add(UserInput[Position - 1].ToString());
            while (Position < UserInput.Length)
            {
                Operand = GetNumberFromUserInput(UserInput, ref Position);
                UserInputInRPN.Add(Operand.ToString());
                if (Position < UserInput.Length)
                {
                    string CurrentOperator = UserInput[Position - 1].ToString();
                    while (Operators.Count > 0 && Precedence[Operators[Operators.Count - 1]] > Precedence[CurrentOperator])
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                    if (Operators.Count > 0 && Precedence[Operators[Operators.Count - 1]] == Precedence[CurrentOperator])
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                    Operators.Add(CurrentOperator);
                }
                else
                {
                    while (Operators.Count > 0)
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                }
            }
            return UserInputInRPN;
        }

        static int EvaluateRPN(List<string> UserInputInRPN)
        {
            List<string> S = new List<string>();
            while (UserInputInRPN.Count > 0)
            {
                while (!"+-*/".Contains(UserInputInRPN[0]))
                {
                    S.Add(UserInputInRPN[0]);
                    UserInputInRPN.RemoveAt(0);
                }
                double Num2 = Convert.ToDouble(S[S.Count - 1]);
                S.RemoveAt(S.Count - 1);
                double Num1 = Convert.ToDouble(S[S.Count - 1]);
                S.RemoveAt(S.Count - 1);
                double Result = 0;
                switch (UserInputInRPN[0])
                {
                    case "+":
                        Result = Num1 + Num2;
                        break;
                    case "-":
                        Result = Num1 - Num2;
                        break;
                    case "*":
                        Result = Num1 * Num2;
                        break;
                    case "/":
                        Result = Num1 / Num2;
                        break;
                }
                UserInputInRPN.RemoveAt(0);
                S.Add(Convert.ToString(Result));
            }
            if (Convert.ToDouble(S[0]) - Math.Truncate(Convert.ToDouble(S[0])) == 0.0)
            {
                return (int)Math.Truncate(Convert.ToDouble(S[0]));
            }
            else
            {
                return -1;
            }
        }

        static int GetNumberFromUserInput(string UserInput, ref int Position)
        {
            string Number = "";
            bool MoreDigits = true;
            while (MoreDigits)
            {
                if (Regex.IsMatch(UserInput[Position].ToString(), "[0-9]"))
                {
                    Number += UserInput[Position];
                }
                else
                {
                    MoreDigits = false;
                }
                Position++;
                if (Position == UserInput.Length)
                {
                    MoreDigits = false;
                }
            }
            if (Number == "")
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(Number);
            }
        }

        static bool CheckIfUserInputValid(string UserInput)
        {
            return Regex.IsMatch(UserInput, @"^([0-9]+[\+\-\*\/])+[0-9]+$");
        }

        static int GetTarget(int MaxTarget)
        {
            return RGen.Next(MaxTarget) + 1;
        }

        static int GetNumber(int MaxNumber)
        {
            return RGen.Next(MaxNumber) + 1;
        }

        static List<int> CreateTargets(int SizeOfTargets, int MaxTarget)
        {
            List<int> Targets = new List<int>();
            for (int Count = 1; Count <= 5; Count++)
            {
                Targets.Add(-1);
            }
            for (int Count = 1; Count <= SizeOfTargets - 5; Count++)
            {
                Targets.Add(GetTarget(MaxTarget));
            }
            return Targets;
        }

        static List<int> FillNumbers(List<int> NumbersAllowed, bool TrainingGame, int MaxNumber, string difficulty)
        {
            int[] LargeNums = { 25, 50, 75, 100 };
            Random rng = new Random();

            if (TrainingGame)
            {
                return new List<int> { 2, 3, 2, 8, 512 };
            }
            else if (difficulty == "standard")
            {
                while (NumbersAllowed.Count < 5)
                {
                    NumbersAllowed.Add(GetNumber(MaxNumber));
                }
                return NumbersAllowed;
            }
            else if(difficulty == "easy")
            {
                while(NumbersAllowed.Count < 4)
                {
                    NumbersAllowed.Add(GetNumber(MaxNumber));
                }
                int rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);
                
                return NumbersAllowed;
            }
            else if (difficulty == "medium")
            {
                while (NumbersAllowed.Count < 3)
                {
                    NumbersAllowed.Add(GetNumber(MaxNumber));
                }
                int rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);

                rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);

                return NumbersAllowed;

            }
            else if(difficulty == "hard")
            {
                while (NumbersAllowed.Count < 1)
                {
                    NumbersAllowed.Add(GetNumber(MaxNumber));
                }
                int rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);
                rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);
                rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);
                rannum = rng.Next(0, 4);

                NumbersAllowed.Add(LargeNums[rannum]);

                return NumbersAllowed;
            }
            return null;
        }
    }
}
