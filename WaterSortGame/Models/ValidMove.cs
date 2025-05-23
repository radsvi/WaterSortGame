﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class ValidMove // prejmenovat na SolvingStep
    {
        public ValidMove(PositionPointer source, PositionPointer target, LiquidColorNew[,] gameState, bool isTargetSingleColor = false)
        {
            GameState = Models.GameState.CloneGridStatic(gameState);
            Target = target;
            Source = source; // mam to v tomhle poradi kvuli eventum
            Liquid = gameState[source.X, source.Y];

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
        public PositionPointer Target { get; private protected set; }
        public bool IsTargetSingleColor { get; private set; }
        public LiquidColorNew Liquid { get; private protected set; }
        public float Priority { get; set; } = 0; // higher weight means better move
        public LiquidColorNew[,] GameState { get; set; }
        public int SolutionValue { get; set; }
        //public int MaxSolutionValue { get; set; }

        //public static bool operator ==(ValidMove first, ValidMove second)
        private static bool OperatorOverload(ValidMove first, ValidMove second)
        {
            //Debug.WriteLine($"first.Source.X [{first.Source.X}] == second.Source.X [{second.Source.X}] && first.Source.Y [{first.Source.Y}] == second.Source.Y [{second.Source.Y}]");
            //Debug.WriteLine($"&& first.Target.X [{first.Target.X}] == second.Target.X[{second.Target.X}] && first.Target.Y [{first.Target.Y}] == second.Target.Y [{second.Target.Y}]");
            //Debug.WriteLine($"&& first.Liquid.Name [{first.Liquid.Name}] == second.Liquid.Name [{second.Liquid.Name}]");
            
            //Debug.WriteLine($"[{first.Source.X}] == [{second.Source.X}] && [{first.Source.Y}] == [{second.Source.Y}]");
            //Debug.WriteLine($"&& [{first.Target.X}] == [{second.Target.X}] && [{first.Target.Y}] == [{second.Target.Y}]");
            //Debug.WriteLine($"&& [{first.Liquid.Name}] == [{second.Liquid.Name}]");
            if (first.Source.X == second.Source.X && first.Source.Y == second.Source.Y
                && first.Target.X == second.Target.X && first.Target.Y == second.Target.Y
                && first.Liquid.Name == second.Liquid.Name)
            {
                return true;
            }
            return false;
        }
        public static bool operator ==(ValidMove first, ValidMove second)
        {
            return OperatorOverload(first, second);
        }
        public static bool operator !=(ValidMove first, ValidMove second)
        {
            return !OperatorOverload(first, second);
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
    }
    internal class NullValidMove : ValidMove
    {
        public NullValidMove() : base(true) {
            GameState = new LiquidColorNew[0,0];
            Target = new NullPositionPointer();
            Source = new NullPositionPointer();
            Liquid = new NullLiquidColorNew();
        }
    }
}
