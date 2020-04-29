﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp1.Model;


public class EntitySpot
{
    private const float nodeSpotSize = 3.5f;
    private const float crossSpotSize = 3f;
    private const float emptySpotSize = 1f;

    float size;
    Canvas canvas;
    Action<long, Color, bool> onLineClick;
    SolidColorBrush defaultSolidbrush;

    public EntitySpot(float size, Canvas canvas, int x, int y, Action<long, Color, bool> clickOnlinee)
    {
        this.onLineClick = clickOnlinee;
        this.canvas = canvas;
        this.size = size;
        this.X = x;
        this.Y = y;
        Shape = new Ellipse() { Height = size, Width = size };
        SolidColorBrush sb = new SolidColorBrush { Color = Color.FromRgb(125, 125, 125) };
        Shape.Fill = sb;
        canvas.Children.Add(Shape);

        Canvas.SetLeft(Shape, X - size / 2);
        Canvas.SetTop(Shape, Y - size / 2);
    }

    public bool IsVisited;
    public bool IsExamined;

    public List<PowerEntity> Entities { get; private set; } = new List<PowerEntity>();
    public long LineId = -1;
    public Shape Shape { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }


    public bool IsOccupied;


    public EntitySpot parent;
    public void AssigntEntity(PowerEntity entity, Color color)
    {
        defaultSolidbrush = new SolidColorBrush { Color = color };
        Shape.Width = size * nodeSpotSize;
        Shape.Height = size * nodeSpotSize;
        Canvas.SetLeft(Shape, X - size * nodeSpotSize / 2);
        Canvas.SetTop(Shape, Y - size * nodeSpotSize / 2);

        Shape.Fill = defaultSolidbrush;
        this.Entities.Add(entity);
        //Canvas.SetZIndex(Shape, 1);
        Shape.MouseEnter += OnHoverOverNode;
        Shape.MouseLeftButtonDown += LeftClickOnThis;
    }

    public void Highlight(bool enable)
    {
        if (enable)
        {
            Shape.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }
        else
        {
            Shape.Fill = defaultSolidbrush;
        }
    }

    public void AssignCross()
    {
        Shape.Height = 3;
        Shape.Width = 3;
        SolidColorBrush sb = new SolidColorBrush { Color = Color.FromRgb(0, 0, 0) };
        Shape.Fill = sb;
        Canvas.SetLeft(Shape, X - size * crossSpotSize / 2);
        Canvas.SetTop(Shape, Y - size * crossSpotSize / 2);
    }
    public void AssignLinePart(long lineID)
    {
        if (Entities.Count == 0)
            LineId = lineID;
    }

    void OnHoverOverNode(object sender, RoutedEventArgs e)
    {
        Shape.ToolTip = ToolTipHelper.Serialize(Entities);
    }
    void OnHoverOverLine(object sender, RoutedEventArgs e)
    {
        //Shape.ToolTip = ToolTipHelper.Serialize(Entities);
    }


    void Scale(float scale)
    {
        Shape.Width = size * nodeSpotSize * scale;
        Shape.Height = size * nodeSpotSize * scale;
        Canvas.SetLeft(Shape, X - size * nodeSpotSize * scale / 2);
        Canvas.SetTop(Shape, Y - size * nodeSpotSize * scale / 2);
    }

    public void RightClickOnThis(object sender, RoutedEventArgs e)
    {
        if (LineId >= 0)
        {
            OnClickLine();
        }
    }
    async void OnClickLine()
    {
        onLineClick(LineId, Color.FromRgb(255, 0, 0), true);
        await DelayThenDoSomeWork();
        onLineClick(LineId, Color.FromRgb(0, 0, 0), false);
    }

    async void LeftClickOnThis(object sender, RoutedEventArgs e)
    {
        if (Entities.Count > 0)
        {
            Scale(10);
            await DelayThenDoSomeWork();
            Scale(1);
        }
    }

    async Task DelayThenDoSomeWork()
    {
        await Task.Delay(5000);
    }
}