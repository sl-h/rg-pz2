using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp1.Model;
public class EntitySpot
{
    public EntitySpot(float size, Canvas canvas, int x, int y)
    {
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
    float size;
    Canvas canvas;

    public bool IsVisited;
    public bool IsExamined;

    public List<PowerEntity> Entities { get; private set; } = new List<PowerEntity>();
    public Shape Shape { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsOccupied;


    public EntitySpot parent;
    public void AssigntEntity(PowerEntity entity, Color color)
    {
        SolidColorBrush sb = new SolidColorBrush { Color = color };
        Shape.Width = size * 3.5f;
        Shape.Height = size * 3.5f;
        Canvas.SetLeft(Shape, X - size * 3.5 / 2);
        Canvas.SetTop(Shape, Y - size * 3.5 / 2);

        Shape.Fill = sb;
        this.Entities.Add(entity);
        //Canvas.SetZIndex(Shape, 1);
        Shape.MouseEnter += OnHoverOverNode;
    }

    public void AssignCross()
    {
        Shape.Height = 3;
        Shape.Width = 3;
        SolidColorBrush sb = new SolidColorBrush { Color = Color.FromRgb(0, 0, 0) };
        Shape.Fill = sb;
        Canvas.SetLeft(Shape, X - size * 3 / 2);
        Canvas.SetTop(Shape, Y - size * 3 / 2);
    }

    void OnHoverOverNode(object sender, RoutedEventArgs e)
    {
        Shape.ToolTip = ToolTipHelper.Serialize(Entities);
    }
    void OnHoverOverLine(object sender, RoutedEventArgs e)
    {
        //Shape.ToolTip = ToolTipHelper.Serialize(Entities);
    }
}