using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class TreeNode<T> where T : ValidMove // temporary limitation. change later..
    {
        public T Data { get; private set; }
        public TreeNode<T>? Parent { get; private set; }
        public TreeNode<T>? FirstChild { get; private set; }
        public TreeNode<T>? NextSibling { get; private set; }
        public bool Visited { get; set; }
        public TreeNode(T data)
        {
            this.Data = data;
        }
        //public TreeNode(T data, TreeNode<T>? sender = null, TreeNode<T>? child = null, TreeNode<T>? sibling = null)
        //{
        //    this.Data = data;
        //    this.Parent = sender;
        //    this.Child = child;
        //    this.Sibling = sibling;
        //}
        //public void AddSibling(T siblingNode)
        //{
        //    NextSibling = new TreeNode<T>(siblingNode);
        //}
        //public void AddChild(T childNode)
        //{
        //    FirstChild = new TreeNode<T>(childNode);
        //}
        public void AddSibling(TreeNode<T> siblingNode)
        {
            NextSibling = siblingNode;
        }
        public void AddChild(TreeNode<T> childNode)
        {
            FirstChild = childNode;
            childNode.Parent = this;
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
}
