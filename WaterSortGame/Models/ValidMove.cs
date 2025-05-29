using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal enum StringFormat
    {
        Names,
        Numbers
    }
    internal class ValidMove // prejmenovat na SolvingStep
    {
        public ValidMove(PositionPointer source, PositionPointer target, LiquidColorNew[,] gameState, bool isTargetSingleColor = false)
        {
            GameState = Models.GameState.CloneGridStatic(gameState);
            Target = target;
            Source = source; // mam to v tomhle poradi kvuli eventum
            Liquid = gameState[source.X, source.Y];
            StepNumber = stepCounter++;
            //Hash = GetHashCode();
            //ReadableHash = GameStateToInt(GameState);

            IsTargetSingleColor = isTargetSingleColor;
            //CalculatePriority();
        }
        public ValidMove() {}
        private protected ValidMove(bool nullMove)
        {
            StepNumber = -1;
        }
        public ValidMove(LiquidColorNew[,] gameState)
        {
            GameState = gameState;
            StepNumber = stepCounter++;
        }
        //public PositionPointer Source { get; private set; }
        private PositionPointer source;
        public PositionPointer Source
        {
            get { return source; }
            private protected set
            {
                if (value != source)
                {
                    source = value;
                    CalculatePriority();
                }
            }
        }
        private static protected int stepCounter = 0;
        public int StepNumber { get; set; }
        public bool Visited { get; set; }
        public bool FullyVisited { get; set; } // True means that all children were visited.
        public PositionPointer Target { get; private protected set; }
        public bool IsTargetSingleColor { get; private set; }
        public LiquidColorNew Liquid { get; private protected set; }
        public float Priority { get; set; } = 0; // higher weight means better move
        public LiquidColorNew[,] GameState { get; set; }
        public int SolutionValue { get; set; }
        public int Hash { get; private set; }
        [Obsolete] public string ReadableHash { get; private set; }
        [Obsolete] public string ReadableGameState { get; private set; }
        //public int MaxSolutionValue { get; set; }

        //public static bool operator ==(ValidMove first, ValidMove second)
        //private static bool EqualsOverload(ValidMove first, ValidMove second)
        //{
        //    //Debug.WriteLine($"first.Source.X [{first.Source.X}] == second.Source.X [{second.Source.X}] && first.Source.Y [{first.Source.Y}] == second.Source.Y [{second.Source.Y}]");
        //    //Debug.WriteLine($"&& first.Target.X [{first.Target.X}] == second.Target.X[{second.Target.X}] && first.Target.Y [{first.Target.Y}] == second.Target.Y [{second.Target.Y}]");
        //    //Debug.WriteLine($"&& first.Liquid.Name [{first.Liquid.Name}] == second.Liquid.Name [{second.Liquid.Name}]");

        //    //Debug.WriteLine($"[{first.Source.X}] == [{second.Source.X}] && [{first.Source.Y}] == [{second.Source.Y}]");
        //    //Debug.WriteLine($"&& [{first.Target.X}] == [{second.Target.X}] && [{first.Target.Y}] == [{second.Target.Y}]");
        //    //Debug.WriteLine($"&& [{first.Liquid.Name}] == [{second.Liquid.Name}]");
        //    if (first.Source.X == second.Source.X && first.Source.Y == second.Source.Y
        //        && first.Target.X == second.Target.X && first.Target.Y == second.Target.Y
        //        && first.Liquid.Name == second.Liquid.Name)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public static bool operator ==(ValidMove first, ValidMove second)
        //{
        //    return EqualsOverload(first, second);
        //}
        //public static bool operator !=(ValidMove first, ValidMove second)
        //{
        //    return !EqualsOverload(first, second);
        //}
        public bool Equals(LiquidColorNew[,] otherGameState)
        {
            //var jedna = GameStateToInt(GameState);
            //var dva = GameStateToInt(otherGameState);


            return GameStateToInt(GameState) == GameStateToInt(otherGameState);
        }
        private void CalculatePriority()
        {
            float newPriority = 0;
            // Singular colors have slightly higher priority than stacked colors so that they are picked first when there is a choise between the two.
            newPriority += (GameState.GetLength(1) - Source.NumberOfRepeatingLiquids) / 10f;

            // if target is not empty tube (but rather the same color), give it higher priority, but only if all stacked liquids fit
            if (Target is not null)
            {
                if (Target.Y > 0 && Source.NumberOfRepeatingLiquids <= (GameState.GetLength(1) - Target.Y))
                {
                    newPriority++;
                }
            }
            Priority = newPriority;
        }
        public override int GetHashCode()
        {
            var intGameState = GameStateToInt(GameState);
            //var hash = intGameState.GetHashCode();
            //var hashSlow = GameState.GetHashCode();
            
            return intGameState.GetHashCode();

            //return GameState.GetHashCode();
        }
        public void UpdateHash()
        {
            Hash = GetHashCode();
            ReadableHash = GameStateToInt(GameState);
            //ReadableGameState = GameStateToString(GameState);
            ReadableGameState = GameStateToString(GameState, StringFormat.Numbers);

        }
        //private static List<string> GameStateToInt(LiquidColorNew[,] gameState)
        //{
        //    List<string> intGameState = new List<string>();
        //    for (int x = 0; x < gameState.GetLength(0); x++)
        //    {
        //        string tubeString = string.Empty;
        //        for (int y = gameState.GetLength(1) - 1; y >= 0; y--)
        //        {
        //            if (gameState[x, y] is not null)
        //                //tubeInt += (int)gameState[x, y].Name * (int)Math.Pow(100,y);
        //                tubeString += ((int)gameState[x, y].Name).ToString("00");
        //        }
        //        intGameState.Add(tubeString);
        //    }
        //    intGameState.Sort();
        //    return intGameState;
        //}
        private static string GameStateToInt(LiquidColorNew[,] gameState)
        {
            List<string> intGameState = new List<string>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                string tubeString = string.Empty;
                for (int y = gameState.GetLength(1) - 1; y >= 0; y--)
                {
                    if (gameState[x, y] is not null)
                        //tubeInt += (int)gameState[x, y].Name * (int)Math.Pow(100,y);
                        tubeString += ((int)gameState[x, y].Name).ToString("00");
                }
                intGameState.Add(tubeString);
            }
            intGameState.Sort();
            string stringGameState = string.Empty;
            foreach (var tube in intGameState)
            {
                stringGameState += tube.ToString() + "-";
            }
            stringGameState = stringGameState.Substring(0, stringGameState.Length - 1);
            return stringGameState;
        }
        private static string GameStateToString(LiquidColorNew[,] gameState, StringFormat format = StringFormat.Names)
        {
            return Models.GameState.GameStateToString(gameState, format);
        }
    }
    internal class NullValidMove : ValidMove
    {
        public NullValidMove() : base(true) {
            GameState = new LiquidColorNew[0,0];
            Target = new NullPositionPointer();
            Source = new NullPositionPointer();
            Liquid = new NullLiquidColorNew();
            FullyVisited = true; // changing this to true because NullValidMove shouldn't even be even considered as a valid move.
        }
    }
}
