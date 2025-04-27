using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class TreeNode<T> where T : SolutionStep // temporary limitation. change later..
    {
        public string Data { get; private set; }
        public TreeNode<T>? Parent { get; private set; }
        public TreeNode<T>? Child { get; private set; }
        public TreeNode<T>? Sibling { get; private set; }
        public TreeNode(string data, TreeNode<T>? sender = null, TreeNode<T>? child = null, TreeNode<T>? sibling = null)
        {
            this.Data = data;
            this.Parent = sender;
            this.Child = child;
            this.Sibling = sibling;
        }
        //private void GenerateNextGameState(LiquidColorNew[,] gameState, SolutionStep move, SolutionStepsOld previousStepReferer = null)
        //{
        //    var currentState = MainWindowVM.GameState.CloneGrid(gameState);

        //    gameState[move.Target.X, move.Target.Y] = gameState[move.Source.X, move.Source.Y];
        //    gameState[move.Source.X, move.Source.Y] = null;

        //    var upcomingStep = new SolutionStepsOld(currentState, move, this.Previous);
        //    SolvingStepsOLD.Add(upcomingStep);


        //    MainWindowVM.GameState.SetGameState(gameState);

        //    MainWindowVM.DrawTubes();
        //    MainWindowVM.OnChangingGameState();
        //}
    }
    //internal class TreeNode<T>
    //{

    //}
}
