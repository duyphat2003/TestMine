namespace MyLibrary.Model 
{
    public class Prop
    {
        public string name;
        public float x, y, z;

        public string Serialize()
        {
            return $"{name},{x},{y},{z}";
        }

        public static Prop Deserialize(string data)
        {
            string[] parts = data.Split(',');
            return new Prop
            {
                name = parts[0],
                x = float.Parse(parts[1]),
                y = float.Parse(parts[2]),
                z = float.Parse(parts[3])
            };
        }
    }
}

