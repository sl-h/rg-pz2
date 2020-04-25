using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using WpfApp1.Model;
using Point = WpfApp1.Model.Point;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public double noviX, noviY;
        public MainWindow()
        {
            InitializeComponent();
            LoadButton_Click(null, new RoutedEventArgs());
        }




        double ConvertToCanvas(double point, double start) => Utility.ConvertToCanvas(point, 2000000, start, 1, canvas.Width);

        MyPoint GetBottomLeftPoint(List<XmlNodeList> xmlPointDataHolders)
        {
            List<MyPoint> sortPoints = new List<MyPoint>();
            foreach (var pointDataColliection in xmlPointDataHolders)
            {
                foreach (XmlNode node in pointDataColliection)
                {
                    sortPoints.Add(new MyPoint()
                    {
                        x = double.Parse(node.SelectSingleNode("X").InnerText),
                        y = double.Parse(node.SelectSingleNode("Y").InnerText)
                    });
                }
            }

            return Utility.GetBottomLeftPoint(sortPoints);
        }

        PowerEntity ParsePowerEntity(XmlNode node)
        {
            return new PowerEntity()
            {
                Id = long.Parse(node.SelectSingleNode("Id").InnerText),
                Name = node.SelectSingleNode("Name").InnerText,
                X = double.Parse(node.SelectSingleNode("X").InnerText),
                Y = double.Parse(node.SelectSingleNode("Y").InnerText)
            };
        }
     
        Shape CreateDot(float size, MyPoint position, MyPoint minimalPoint, System.Windows.Media.Color color)
        {
            Shape dot = new Ellipse() { Height = size, Width = size };
            SolidColorBrush sb = new SolidColorBrush { Color = color };

            dot.Fill = sb;
            dot.MouseLeftButtonDown += LeftClickOnPoint;
            canvas.Children.Add(dot);

            Utility.ToLatLon(position.x, position.y, 34, out double x, out double y);
            Canvas.SetLeft(dot, ConvertToCanvas(x, minimalPoint.x) * 6);
            Canvas.SetTop(dot, ConvertToCanvas(y, minimalPoint.y) * 3);
            return dot;
        }



        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load("Geographic.xml");
            XmlNodeList nodeListSubstation = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList nodeListSwitch = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeListLines = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            XmlNodeList nodeListNode = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            var minimalPoint = GetBottomLeftPoint(new List<XmlNodeList>() { nodeListNode, nodeListSubstation, nodeListSwitch });

            foreach (XmlNode node in nodeListSubstation)
            {
                var entity = ParsePowerEntity(node);
                var shape = CreateDot(2, new MyPoint(entity.X, entity.Y), minimalPoint, System.Windows.Media.Color.FromRgb(0, 255, 0));
            }

            foreach (XmlNode node in nodeListNode)
            {
                var entity = ParsePowerEntity(node);
                var shape = CreateDot(2, new MyPoint(entity.X, entity.Y), minimalPoint, System.Windows.Media.Color.FromRgb(255, 0, 0));
            }

            foreach (XmlNode node in nodeListSwitch)
            {
                var entity = ParsePowerEntity(node);
                var shape = CreateDot(2, new MyPoint(entity.X, entity.Y), minimalPoint, System.Windows.Media.Color.FromRgb(0, 0, 255));
            }

            LineEntity line = new LineEntity();
            foreach (XmlNode item in nodeListLines)
            {
                line.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
                line.Name = item.SelectSingleNode("Name").InnerText;
                line.IsUnderground = bool.Parse(item.SelectSingleNode("IsUnderground").InnerText);
                line.LineType = item.SelectSingleNode("LineType").InnerText;
                line.R = float.Parse(item.SelectSingleNode("R").InnerText);
                line.FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd").InnerText);
                line.SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd").InnerText);
                line.ConductorMaterial = item.SelectSingleNode("ConductorMaterial").InnerText;
                line.ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat").InnerText);
                line.Vertices = new List<Point>();
                foreach (XmlNode pointNode in item.ChildNodes[9].ChildNodes)
                {
                    double x = double.Parse(pointNode.SelectSingleNode("X").InnerText);
                    double y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);
                    line.Vertices.Add(new Point() { X = x, Y = y });
                }

                Shape dot = new Ellipse() { Height = 4, Width = 4 };
                SolidColorBrush sb = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(0, 0, 0) };
                dot.Fill = sb;

                // canvas.Children.Add(dot);

                //  ToLatLon(swtc.X, swtc.Y, 34, out noviX, out noviY);
                //Canvas.SetLeft(dot, ConvertToCanvas(noviX, 1000000, lowest.x, 1000, canvas.Width) * 6);
                //Canvas.SetTop(dot, ConvertToCanvas(noviY, 1000000, lowest.y, 1000, canvas.Height) * 3);
            }

        }



        public void LeftClickOnPoint(object sender, MouseButtonEventArgs e)
        {
            zoomslider.Value = 10;
            (e.OriginalSource as Shape).BringIntoView(new Rect(-20, 20, 40, 40));
        }
    }
}
