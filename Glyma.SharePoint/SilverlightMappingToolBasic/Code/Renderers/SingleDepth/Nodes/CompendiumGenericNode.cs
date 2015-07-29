using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IoC;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumGenericNode : BaseCompendiumNode
    {
        private const double UnfocusedTextMarginX = 55;
        private const double UnfocusedTextMarginY = 4;
        private const double UnfocusedTextWidth = 115;
        private const double UnfocusedTextHeight = 40;
        private const double UnfocusedNodeUIElementWidth = 180;
        private const double UnfocusedNodeUIElementHeight = 50;

        private const double FocusedTextMarginX = 10;
        private const double FocusedTextMarginY = 128;
        private const double FocusedTextWidth = 159;
        private const double FocusedTextHeight = 94;
        private const double FocusedNodeUIElementWidth = 180;
        private const double FocusedNodeUIElementHeight = 250;

        public CompendiumGenericNode()
        {
        }

        protected override Rect TextLocation
        {
            get
            {
                Rect textLocation = new Rect();

                if (NodeSettings.IsFocused)
                {
                    textLocation.X = FocusedTextMarginX + NodeUIElementLocation.X;
                    textLocation.Y = FocusedTextMarginY + NodeUIElementLocation.Y;
                    textLocation.Width = FocusedTextWidth;
                    textLocation.Height = FocusedTextHeight;
                }
                else
                {
                    textLocation.X = UnfocusedTextMarginX + NodeUIElementLocation.X;
                    textLocation.Y = UnfocusedTextMarginY + NodeUIElementLocation.Y;
                    textLocation.Width = UnfocusedTextWidth;
                    textLocation.Height = UnfocusedTextHeight;
                }

                return textLocation;
            }
        }

        protected override Rect NodeUIElementLocation
        {
            get
            {
                Rect nodeUIElementLocation = new Rect();

                if (NodeSettings.IsFocused)
                {
                    nodeUIElementLocation.X = (NodeSettings.CentreX - (FocusedNodeUIElementWidth / 2));
                    nodeUIElementLocation.Y = (NodeSettings.CentreY - (FocusedNodeUIElementHeight / 2));
                    nodeUIElementLocation.Width = FocusedNodeUIElementWidth;
                    nodeUIElementLocation.Height = FocusedNodeUIElementHeight;
                }
                else
                {
                    Point location = CalculateLocation();

                    nodeUIElementLocation.X = (location.X - (UnfocusedNodeUIElementWidth / 2));
                    nodeUIElementLocation.Y = (location.Y - (UnfocusedNodeUIElementHeight / 2));
                    nodeUIElementLocation.Width = UnfocusedNodeUIElementWidth;
                    nodeUIElementLocation.Height = UnfocusedNodeUIElementHeight;
                }

                return nodeUIElementLocation;
            }
        }

        protected override Brush NodeImage
        {
            get
            {
                if (NodeSettings.IsFocused)
                {
                    return IoCContainer.GetInjectionInstance().GetInstance<FullGenericResource>().SpriteResouce;
                }
                else
                {
                    return IoCContainer.GetInjectionInstance().GetInstance<CompactGenericResource>().SpriteResouce;
                }
            }
        }

        #region ICompendiumNode Members

        public override void LoadSettings(CompendiumNodeSettings settings)
        {
            base.LoadSettings(settings);
        }

        public override UIElement[] RenderUIElements()
        {
            UIElement[] uiElements;

            if (!NodeSettings.IsFocused)
            {
                ILineSegment arrowLineSegment = new LineSegment(new Point(NodeUIElementLocation.X + 25, NodeUIElementLocation.Y + 23), new Point(NodeUIElementLocation.X + 130, NodeUIElementLocation.Y + 27));

                Point A = new Point(NodeUIElementLocation.X + 5, NodeUIElementLocation.Y + 5);
                Point B = new Point(NodeUIElementLocation.X + 5, NodeUIElementLocation.Y + 22);
                Point C = new Point(NodeUIElementLocation.X + 5, NodeUIElementLocation.Y + 45);
                Point D = new Point(NodeUIElementLocation.X + 89, NodeUIElementLocation.Y + 45);
                Point E = new Point(NodeUIElementLocation.X + 174, NodeUIElementLocation.Y + 45);
                Point F = new Point(NodeUIElementLocation.X + 174, NodeUIElementLocation.Y + 22);
                Point G = new Point(NodeUIElementLocation.X + 174, NodeUIElementLocation.Y + 5);
                Point H = new Point(NodeUIElementLocation.X + 89, NodeUIElementLocation.Y + 5);

                Point arrowEnd = new Point(NodeSettings.CentreX, NodeSettings.CentreY);

                List<Point> elements = new List<Point>();
                elements.Add(A);
                elements.Add(B);
                elements.Add(C);
                elements.Add(D);
                elements.Add(E);
                elements.Add(F);
                elements.Add(G);
                elements.Add(H);
                Point arrowStart = elements.FindClosestPoint(arrowEnd);

                ILineSegment arrowCoordinates = new LineSegment(arrowStart, arrowEnd);

                Point cornerA = new Point(NodeUIElementLocation.X, NodeUIElementLocation.Y);
                Point cornerB = new Point(NodeUIElementLocation.X + 5, NodeUIElementLocation.Y + 55);
                Point cornerC = new Point(NodeUIElementLocation.X + 179, NodeUIElementLocation.Y + 50);
                Point cornerD = new Point(NodeUIElementLocation.X + 184, NodeUIElementLocation.Y + 5);
                Point triangulationPoint = new Point(NodeUIElementLocation.X + 90, NodeUIElementLocation.Y + 25);

                double mainTopLeftX = NodeSettings.CentreX - 90;
                double mainTopLeftY = NodeSettings.CentreY - 125;

                Point mainCornerA = new Point(mainTopLeftX - 5, mainTopLeftY - 6);
                Point mainCornerB = new Point(mainTopLeftX - 6, mainTopLeftY + 255);
                Point mainCornerC = new Point(mainTopLeftX + 185, mainTopLeftY + 256);
                Point mainCornerD = new Point(mainTopLeftX + 186, mainTopLeftY - 5);
                Point mainTriangulationPoint = new Point(NodeSettings.CentreX, NodeSettings.CentreY);

                IntersectionDetector detector = new IntersectionDetector();
                detector.Add(new LineSegment(cornerA, cornerB, mainTriangulationPoint));
                detector.Add(new LineSegment(cornerB, cornerC, mainTriangulationPoint));
                detector.Add(new LineSegment(cornerC, cornerD, mainTriangulationPoint));
                detector.Add(new LineSegment(cornerD, cornerA, mainTriangulationPoint));

                IntersectionDetector detector2 = new IntersectionDetector();
                detector2.Add(new LineSegment(mainCornerA, mainCornerB, triangulationPoint));
                detector2.Add(new LineSegment(mainCornerB, mainCornerC, triangulationPoint));
                detector2.Add(new LineSegment(mainCornerC, mainCornerD, triangulationPoint));
                detector2.Add(new LineSegment(mainCornerD, mainCornerA, triangulationPoint));

                arrowCoordinates = arrowCoordinates.Resize(detector);
                arrowCoordinates = arrowCoordinates.Resize(detector2);

                Arrow arrow = new Arrow(arrowCoordinates);

                List<UIElement> newElements = new List<UIElement>(base.RenderUIElements());

                newElements.Add(arrow.Sprite);

                uiElements = newElements.ToArray();
            }
            else
            {
                uiElements = base.RenderUIElements();
            }

            return uiElements;
        }

        #endregion
    }
}
