namespace MyLibrary.Model 
{
    public class Inventory
    {
        public string name;
        public int index;
        public int amount;

        public string Serialize()
        {
            return $"{name},{index},{amount}";
        }

        public static Inventory Deserialize(string data)
        {
            string[] parts = data.Split(',');
            return new Inventory
            {
                name = parts[0],
                index = int.Parse(parts[1]),
                amount = int.Parse(parts[2])
            };
        }
    }
}


