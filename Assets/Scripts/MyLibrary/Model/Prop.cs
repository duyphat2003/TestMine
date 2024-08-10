namespace MyLibrary.Model 
{
    public class Prop
    {
        public string name;
        public int index;
        public float x, y, z, a, b, c;

        public string Serialize()
        {
            return $"{name},{index},{x},{y},{z},{a},{b},{c}";
        }

        public static Prop Deserialize(string data)
        {
            string[] parts = data.Split(',');
            return new Prop
            {
                name = parts[0],
                index = int.Parse(parts[1]),
                x = float.Parse(parts[2]),
                y = float.Parse(parts[3]),
                z = float.Parse(parts[4]),
                a = float.Parse(parts[5]),
                b = float.Parse(parts[6]),
                c = float.Parse(parts[7])
            };
        }
    }
}

