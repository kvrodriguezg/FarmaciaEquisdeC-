namespace FarmaciaEquisde.Models
{
    public class DosFactores
    {
        int largo = 6;
        private Random randomGenerador = new Random();
        const string chars = "0123456789FEXPB";
        char[] codigo = new char[6];
        public string generador()
        {
            for(int i=0; i < largo; i++)
            {
                codigo[i] = chars[randomGenerador.Next(chars.Length)];
            }
            return new string (codigo);
        }
    }
}
