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

        Canvas.SetLeft(Shape, X);
        Canvas.SetTop(Shape, Y);
    }
    float size;
    Canvas canvas;

    public List<PowerEntity> Entities { get; private set; } = new List<PowerEntity>();
    public Shape Shape { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsOccupied;

    public void AssigntEntity(PowerEntity entity, Color color)
    {
        SolidColorBrush sb = new SolidColorBrush { Color = color };
        Shape.Width = size * 3.5f;
        Shape.Height = size * 3.5f;
        Shape.Fill = sb;
        this.Entities.Add(entity);
        Shape.MouseEnter += OnHover;
    }

    void OnHover(object sender, RoutedEventArgs e)
    {
        Shape.ToolTip = ToolTipHelper.Serialize(Entities);
    }
}