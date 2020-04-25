using System;
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
        const int matrixSize = 400;
        public double noviX, noviY;
        public MainWindow()
        {
            InitializeComponent();
            LoadButton_Click(null, new RoutedEventArgs());
        }
        List<List<EntitySpot>> spots = new List<List<EntitySpot>>(400);

        List<PowerEntity> entities = new List<PowerEntity>();

        int ConvertToCanvas(double point, double start, double scale) => Utility.ConvertToCanvas(point, scale, start, canvas.Width / matrixSize, canvas.Width);

        (MyPoint min, MyPoint max) FindMinMax(List<XmlNodeList> xmlPointDataHolders)
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

            return Utility.MinMax(sortPoints);
        }

        void ParsePowerEntity(XmlNode node, PowerEntity entity)
        {
            entity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
            entity.Name = node.SelectSingleNode("Name").InnerText;
            entity.X = double.Parse(node.SelectSingleNode("X").InnerText);
            entity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
            if (entity is SwitchEntity) (entity as SwitchEntity).Status = node.SelectSingleNode("Status").InnerText;
        }

        void CreateDot(PowerEntity entity, MyPoint minimalPoint, double scale, System.Windows.Media.Color color)
        {
            Utility.ToLatLon(entity.X, entity.Y, 34, out double x, out double y);

            var i = ConvertToCanvas(x, minimalPoint.x, scale * 50);
            var j = ConvertToCanvas(y, minimalPoint.y, scale * 50);

            spots[i + 170][j + 170].AssigntEntity(entity, color);
            spots[i + 170][j + 170].Shape.MouseLeftButtonDown += LeftClickOnPoint;
        }

        void PopulateMatrix()
        {
            int offset = (int)(canvas.Width / matrixSize);
            for (int i = 0; i < matrixSize; i++)
            {
                spots.Add(new List<EntitySpot>(matrixSize));
                for (int j = 0; j < matrixSize; j++)
                {
                    spots[i].Add(new EntitySpot(1f, canvas, i * offset, j * offset));
                }
            }
        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            PopulateMatrix();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load("Geographic.xml");
            XmlNodeList nodeListSubstation = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList nodeListSwitch = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeListLines = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            XmlNodeList nodeListNode = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            var minMax = FindMinMax(new List<XmlNodeList>() { nodeListNode, nodeListSubstation, nodeListSwitch });
            var distance = Math.Sqrt(Math.Pow(minMax.max.x - minMax.min.x, 2) + Math.Pow(minMax.max.y - minMax.min.y, 2));
            var scale = canvas.Width / distance;

            foreach (XmlNode node in nodeListSubstation)
            {
                var entity = new SubstationEntity();
                ParsePowerEntity(node, entity);
                entities.Add(entity);
                CreateDot(entity, minMax.min, scale, System.Windows.Media.Color.FromRgb(0, 255, 0));
            }

            foreach (XmlNode node in nodeListNode)
            {
                var entity = new NodeEntity();
                ParsePowerEntity(node, entity);
                entities.Add(entity);
                CreateDot(entity, minMax.min, scale, System.Windows.Media.Color.FromRgb(255, 255, 0));
            }

            foreach (XmlNode node in nodeListSwitch)
            {
                var entity = new SwitchEntity();
                ParsePowerEntity(node, entity);
                entities.Add(entity);
                CreateDot(entity, minMax.min, scale, System.Windows.Media.Color.FromRgb(0, 0, 255));
            }

            //LineEntity line = new LineEntity();
            //foreach (XmlNode item in nodeListLines)
            //{
            //    line.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
            //    line.Name = item.SelectSingleNode("Name").InnerText;
            //    line.IsUnderground = bool.Parse(item.SelectSingleNode("IsUnderground").InnerText);
            //    line.LineType = item.SelectSingleNode("LineType").InnerText;
            //    line.R = float.Parse(item.SelectSingleNode("R").InnerText);
            //    line.FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd").InnerText);
            //    line.SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd").InnerText);
            //    line.ConductorMaterial = item.SelectSingleNode("ConductorMaterial").InnerText;
            //    line.ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat").InnerText);
            //    line.Vertices = new List<Point>();
            //    foreach (XmlNode pointNode in item.ChildNodes[9].ChildNodes)
            //    {
            //        double x = double.Parse(pointNode.SelectSingleNode("X").InnerText);
            //        double y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);
            //        line.Vertices.Add(new Point() { X = x, Y = y });
            //    }

            //    Shape dot = new Ellipse() { Height = 4, Width = 4 };
            //    SolidColorBrush sb = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(0, 0, 0) };
            //    dot.Fill = sb;

            //    // canvas.Children.Add(dot);

            //    //  ToLatLon(swtc.X, swtc.Y, 34, out noviX, out noviY);
            //    //Canvas.SetLeft(dot, ConvertToCanvas(noviX, 1000000, lowest.x, 1000, canvas.Width) * 6);
            //    //Canvas.SetTop(dot, ConvertToCanvas(noviY, 1000000, lowest.y, 1000, canvas.Height) * 3);
            //}

        }



        public void LeftClickOnPoint(object sender, MouseButtonEventArgs e)
        {
            zoomslider.Value = 10;
            (e.OriginalSource as Shape).BringIntoView(new Rect(-20, 20, 40, 40));
        }
    }
}