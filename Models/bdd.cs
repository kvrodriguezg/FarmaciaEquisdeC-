namespace FarmaciaEquisde.Models
{
    
    public class bdd
    {
        private string cadena = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=bddFarmacia;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        string GetConexion
        {
            get { return cadena; }
        }
    }
    
}
