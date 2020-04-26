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
        public static int fieldSize;
        public static int pointOffset = 170;
        public MainWindow()
        {
            InitializeComponent();
            fieldSize = (int)(canvas.Width / matrixSize);
            LoadButton_Click(null, new RoutedEventArgs());
        }
        List<List<EntitySpot>> spots = new List<List<EntitySpot>>(400);

        Dictionary<long, EntitySpot> entities = new Dictionary<long, EntitySpot>();



        int ConvertToCanvas(double point, double start, double scale) => Utility.ConvertToCanvas(point, scale, start, fieldSize, canvas.Width);

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

        EntitySpot CreateDot(PowerEntity entity, MyPoint minimalPoint, double scale, System.Windows.Media.Color color)
        {
            Utility.ToLatLon(entity.X, entity.Y, 34, out double x, out double y);

            var i = ConvertToCanvas(x, minimalPoint.x, scale * 50);
            var j = ConvertToCanvas(y, minimalPoint.y, scale * 50);

            spots[i + pointOffset][j + pointOffset].AssigntEntity(entity, color);
            spots[i + pointOffset][j + pointOffset].Shape.MouseLeftButtonDown += LeftClickOnPoint;
            return spots[i + pointOffset][j + pointOffset];
        }

        void PopulateMatrix()
        {
            for (int i = 0; i < matrixSize; i++)
            {
                spots.Add(new List<EntitySpot>(matrixSize));
                for (int j = 0; j < matrixSize; j++)
                {
                    spots[i].Add(new EntitySpot(1f, canvas, i * fieldSize, j * fieldSize));
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
                var spot = CreateDot(entity, minMax.min, scale, System.Windows.Media.Color.FromRgb(0, 255, 0));
                entities.Add(entity.Id, spot);
            }

            foreach (XmlNode node in nodeListNode)
            {
                var entity = new NodeEntity();
                ParsePowerEntity(node, entity);

                var spot = CreateDot(entity, minMax.min, scale, System.Windows.Media.Color.FromRgb(255, 255, 0));
                entities.Add(entity.Id, spot);
            }

            foreach (XmlNode node in nodeListSwitch)
            {
                var entity = new SwitchEntity();
                ParsePowerEntity(node, entity);
                var spot = CreateDot(entity, minMax.min, scale, System.Windows.Media.Color.FromRgb(0, 0, 255));
                entities.Add(entity.Id, spot);
            }

            List<LineEntity> lines = new List<LineEntity>();
            foreach (XmlNode item in nodeListLines)
            {
                var line = new LineEntity();
                line.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
                line.Name = item.SelectSingleNode("Name").InnerText;
                line.IsUnderground = bool.Parse(item.SelectSingleNode("IsUnderground").InnerText);
                line.LineType = item.SelectSingleNode("LineType").InnerText;
                line.R = float.Parse(item.SelectSingleNode("R").InnerText);
                line.FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd").InnerText);
                line.SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd").InnerText);
                line.ConductorMaterial = item.SelectSingleNode("ConductorMaterial").InnerText;
                line.ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat").InnerText);
                lines.Add(line);
            }
            BFSLineIterator iterator = new BFSLineIterator();


            foreach (var item in iterator.FindPaths(entities, spots, lines))
            {
                Polyline s = new Polyline();
                s.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                s.StrokeThickness = 2;
                foreach (var pat in item.spots)
                {
                    s.Points.Add(new System.Windows.Point(pat.X, pat.Y));
                }
                canvas.Children.Add(s);
            }


        }



        public void LeftClickOnPoint(object sender, MouseButtonEventArgs e)
        {
            zoomslider.Value = 10;
            (e.OriginalSource as Shape).BringIntoView(new Rect(-20, 20, 40, 40));
        }

    }
}