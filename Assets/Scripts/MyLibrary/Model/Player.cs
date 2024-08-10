namespace MyLibrary.Model 
{
    public class Player
    {
        public float x, y, z;
        public float a, b, c;

        public string Serialize()
        {
            return $"{x},{y},{z},{a},{b},{c}";
        }

        public static Player Deserialize(string data)
        {
            string[] parts = data.Split(',');
            return new Player
            {
                x = float.Parse(parts[0]),
                y = float.Parse(parts[1]),
                z = float.Parse(parts[2]),
                a = float.Parse(parts[3]),
                b = float.Parse(parts[4]),
                c = float.Parse(parts[5])
            };
        }
    }
}
