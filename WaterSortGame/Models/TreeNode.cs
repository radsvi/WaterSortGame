using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WaterSortGame.Models
{
    internal class TreeNode<T> where T : ValidMove, new() // temporary limitation. change later..
    {

        public T Data { get; private protected set; }
        //public T Data { get; set; }
        public TreeNode<T>? Parent { get; private protected set; }
        public TreeNode<T>? FirstChild { get; private protected set; }
        public TreeNode<T>? NextSibling { get; private protected set; }
        private protected TreeNode()
        {
            Data = new T();
        }
        public TreeNode(T data)
        {
            Data = data;
        }
        public void AddSibling(TreeNode<T> siblingNode)
        {
            NextSibling = siblingNode;
            NextSibling.Parent = this.Parent;
        }
        public void AddChild(TreeNode<T> childNode)
        {
            FirstChild = childNode;
            FirstChild.Parent = this;
        }
        public void SwapData(TreeNode<T> otherNode)
        {
            T temp = Data;
            Data = otherNode.Data;
            otherNode.Data = temp;
        }
        //public override int GetHashCode()
        //{
        //    return Data.GetHashCode();
        //}


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
    internal class NullTreeNode : TreeNode<ValidMove>
    {
        public NullTreeNode(TreeNode<ValidMove> parent) : base()
        {
            Data = new NullValidMove();
            Parent = parent;
        }
    }
    static internal class TreeNodeHelper
    {
        //public static int CountSiblings(TreeNode<ValidMove> node)
        //{
        //    if (node.NextSibling is null) return 0;
        //    return 1 + CountSiblings(node.NextSibling);
        //}
        private static TreeNode<ValidMove> GetTailNode(TreeNode<ValidMove> currentNode)
        {
            while (currentNode != null && currentNode.NextSibling != null)
                currentNode = currentNode.NextSibling;
            return currentNode!;
        }
        private static TreeNode<ValidMove> Partition(TreeNode<ValidMove> head, TreeNode<ValidMove> tail)
        {
            TreeNode<ValidMove> pivot = head;
            TreeNode<ValidMove> iNode = head;
            TreeNode<ValidMove> jNode = head;

            while (jNode != null)
            {
                if (jNode.Data.Priority > pivot.Data.Priority)
                {
                    jNode.SwapData(iNode.NextSibling);

                    //DebugPrintAllSiblings(head);
                    iNode = iNode.NextSibling;
                }
                jNode = jNode.NextSibling;
            }
            iNode.SwapData(pivot);

            return iNode;
        }
        private static void QuickSortHelper(TreeNode<ValidMove> head, TreeNode<ValidMove> tail)
        {
            if (head == null || head == tail)
            {
                return;
            }
            //DebugPrint(head, tail);
            TreeNode<ValidMove> pivot = Partition(head, tail);
            QuickSortHelper(head, pivot);
            QuickSortHelper(pivot.NextSibling, tail);
        }
        public static TreeNode<ValidMove> QuickSort(TreeNode<ValidMove> head)
        {
            TreeNode<ValidMove> tail = GetTailNode(head);
            QuickSortHelper(head, tail);
            return head;
        }


        //private static void DebugPrint(TreeNode<ValidMove> first, TreeNode<ValidMove> second)
        //{
        //    Debug.Write($"[{first.Data.StepNumber}: {first.Data.Priority}], ");
        //    Debug.Write($"[{second.Data.StepNumber}: {second.Data.Priority}]\n");
        //}
        //private static void DebugPrintAllSiblings(TreeNode<ValidMove> firstNode)
        //{
        //    var currentNode = firstNode;
        //    while (currentNode is not null)
        //    {
        //        //Debug.Write($"[{currentNode.Data.StepNumber}: {currentNode.Data.Priority}], ");
        //        Debug.Write($"[{currentNode.Data.Priority}], ");

        //        currentNode = currentNode.NextSibling;
        //    }
        //    Debug.WriteLine("");
        //}
    }
}
