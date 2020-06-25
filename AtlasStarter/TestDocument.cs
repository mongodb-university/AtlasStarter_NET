namespace AtlasStarter
{
    public class TestDocument
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public Coordinates Coordinates { get; set; }

        public TestDocument()
        {
        }

        public TestDocument(string name, int type, int x, int y)
        {
            this.Name = name;
            this.Type = type;
            this.Coordinates = new Coordinates(x, y);
        }
    }

    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}