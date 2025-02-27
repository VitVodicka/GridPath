namespace GridPath.Models
{
    public class CadastralArea
    {
        public CadastralArea(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }

}
