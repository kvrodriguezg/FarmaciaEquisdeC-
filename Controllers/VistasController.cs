
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.CodeAnalysis.Scripting;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using NuGet.Protocol;
using Grpc.Core;
using NuGet.Packaging.Signing;
using FarmaciaEquisde.Models;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace vistas.Controllers
{
    public class VistasController : Controller
    {
        public string rutaArchivos = "C:\\Users\\httpd\\Documents\\INSTITUTO\\taller de programacion\\Evalucion_3_Farmacia\\FarmaciaEquisde\\Archivos\\";
        public string ConnectionBdd = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=bdd_EquisdeSPA;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public async Task<IActionResult> Captcha(LoginCaptcha model)
        {
            if (model == null)
            {
                return View("Error");   
            }
            if (validaUsuario(model.Rut, model.Password)[0] > 200 && validaUsuario(model.Rut, model.Password)[0] < 204)
            {
                Datos.rut = model.Rut;
                Datos.clave = model.Password;
                Datos.nivel = validaUsuario(model.Rut, model.Password)[0];
                Datos.area = validaUsuario(model.Rut, model.Password)[1];
                using (var httpClient = new HttpClient())
                {
                    string secretKey = "6LfT6d8oAAAAAOIqEhtGqn8eMgfN9PAFI3bOXmQy";
                    var result = await httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={model.RecaptchaResponse}");

                    if (result.Contains("\"success\": true"))
                    {
                        return RedirectToAction("SendMail");
                    }
                    else
                    {
                        Datos.rut = "";
                        Datos.clave = "";
                        Datos.nivel = 0;
                        Datos.area =0;
                        return View("Views/Vistas/Login.cshtml"); 
                    }
                }
                
            }
            else if (validaUsuario(model.Rut, model.Password)[0] == 204)
            {
                ViewBag.mensaje = "RUT INVALIDO";
                return View("Views/Vistas/login.cshtml");
            }
            else
            {
                ViewBag.mensaje = "PASSWORD INVALIDO";
                return View("Views/Vistas/login.cshtml");
            }
        }
        public IActionResult SendMail()
        {
            string correo = "http.diomar@gmail.com";
            string pw = "ntfykfzlkvaonibg";
            var correoDe = new MailAddress(correo);
            var correoPara = new MailAddress("katlheen.rodriguez@alumnos.ipleones.cl");
            DosFactores dosFactores = new DosFactores();
            string codigoAutenticacion = dosFactores.generador();
            string subjet = "Two Factory Farmacia Equisde";
            string body = "<html><head><style>" +
                          "body { text-align: center; font-family: Arial, sans-serif; }" +
                          "#header { background-color: #3498db; color: #ffffff; padding: 10px; }" +
                          "#footer { background-color: #2c3e50; color: #ffffff; padding: 10px; }" +
                          "</style></head><body>";

            body += "<div id='header'><img src='https://cdn-icons-png.flaticon.com/512/7641/7641727.png' width='50px;'/>" +
                    "<h1>Farmacia Equisde</h1></div>";
            body += "<p>Tu código de verificación es: <strong>" + codigoAutenticacion + "</strong></p><br>" +
                    "<h3>Tu seguridad nos importa</h3><br><p>No compartas este código con nadie</p>";
            body += "<div id='footer'><p>&copy; 2023 Farmacia Equisde</p></div>";
            body += "</body></html>";

            MailMessage mensaje = new MailMessage(correoDe, correoPara);
            mensaje.Subject = subjet;
            mensaje.Body = body;
            mensaje.IsBodyHtml = true;

            SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com");
            clienteSmtp.Credentials = new NetworkCredential(correo, pw);
            clienteSmtp.Port = 587;
            clienteSmtp.EnableSsl = true;
            clienteSmtp.Send(mensaje);

            TempData["codigoEmail"] = codigoAutenticacion;
            return View("Views/Vistas/verificador.cshtml");
        }
        public IActionResult ValidarCodigo(LoginCaptcha model)
        {
            string codigoEnviado = TempData["codigoEmail"] as string;

            if (model.EmailVerificador == codigoEnviado)
            {
                Accion(Datos.rut, "Inicio Sesion");
                return Menu();
            }
            else
            {
                Datos.rut = "";
                Datos.clave = "";
                Datos.nivel = 0;
                Datos.area = 0;
                return View("Views/Vistas/login.cshtml");
            }
        }
        public IActionResult Menu()
        {
          
            if(Datos.nivel == 201)
            {
                return Menu1();
            }else if (Datos.nivel == 202)
            {
                return Menu2();
            }else if (Datos.nivel == 203)
            {
                return Menu3();
            }else
            {
                Datos.rut = "";
                Datos.clave = "";
                Datos.nivel = 0;
                Datos.area = 0;
                return View("Views/Vistas/login.cshtml");
            }
        }
        public IActionResult Menu1()
        {
            if (Datos.nivel == 201)
            {
                Accion(Datos.rut, "Ingreso al Menu1");
                return View("Views/Vistas/Menu1.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Menu2()
        {
            if (Datos.nivel == 202)
            {
                Accion(Datos.rut, "Ingreso al Menu2");
                return View("Views/Vistas/Menu2.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Menu3()
        {
            if (Datos.nivel == 203)
            {
                Accion(Datos.rut, "Ingreso al Menu3");
                return View("Views/Vistas/Menu3.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult NArchivo()
        {
            if(Datos.nivel>200 && Datos.nivel < 204)
            {
                Accion(Datos.rut, "Ingreso a vista NArchivo");
                return View("Views/Vistas/NArchivo.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Revision()
        {
            if (Datos.nivel > 201 && Datos.nivel < 204)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                SqlDataReader dr;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "select * from Documentos";
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                dr = sentencia.ExecuteReader();
                var revision = "";
                if (Datos.nivel == 203)
                {
                    revision += "<tr><th>Nombre</th><th>Area</th><th>Revisar</th></tr>";
                    while (dr.Read())
                    {
                        if (dr["cod_estado"].ToString() == "301" )
                        {
                            revision += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td>Area " + dr["cod_area"].ToString()[2] +"</td><td><form action='../Vistas/Revisar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Revisar'></form> </td></tr>";
                        }
                    }
                }
                else
                {
                    revision += "<tr><th>Nombre</th><th>Revisar</th></tr>";
                    while (dr.Read())
                    {
                        if (dr["cod_estado"].ToString() == "301" && dr["cod_area"].ToString() == Datos.area.ToString())
                        {
                            revision += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td><form action='../Vistas/Revisar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Revisar'></form> </td></tr>";
                        }
                    }
                }
                
                ViewBag.revision = revision;
                ViewBag.area = Datos.area.ToString()[2];
                con.Close();
                Accion(Datos.rut, "Ingreso a Revisión");
                return View("Views/Vistas/Revision.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult NUsuario()
        {
            if (Datos.nivel == 203 )
            {
                Accion(Datos.rut, "Ingreso a vista NUsuario");
                return View("Views/Vistas/NUsuario.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Area()
        {
            if (Datos.nivel > 201 && Datos.nivel < 204)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                SqlDataReader dr;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "select * from Documentos";
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                dr = sentencia.ExecuteReader();
                var revision = "<tr><th>Nombre</th></tr>";
                var aprovados = "<tr><th>Nombre</th></tr>";
                var rechazados = "<tr><th>Nombre</th></tr>";
                while (dr.Read())
                {
                    if (dr["cod_estado"].ToString() == "301" && dr["cod_area"].ToString() == Datos.area.ToString())
                    {
                        revision += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                    else if (dr["cod_estado"].ToString() == "302" && dr["cod_area"].ToString() == Datos.area.ToString())
                    {
                        aprovados += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                    else if (dr["cod_estado"].ToString() == "303" && dr["cod_area"].ToString() == Datos.area.ToString())
                    {
                        rechazados += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                }
                ViewBag.revision = revision;
                ViewBag.aprovados = aprovados;
                ViewBag.rechazados = rechazados;
                ViewBag.area = Datos.area.ToString()[2];
                con.Close();
                Accion(Datos.rut, "Ingreso a Biblioteca de Area");
                return View("Views/Vistas/Area1.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Admin()
        {
            if (Datos.nivel ==203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                SqlDataReader dr;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "select * from Documentos";
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                dr = sentencia.ExecuteReader();
                var revision = "<tr><th>Nombre</th><th>Area</th><th>Descargar</th></tr>";
                var aprovados = "<tr><th>Nombre</th><th>Area</th><th>Descargar</th></tr>";
                var rechazados = "<tr><th>Nombre</th><th>Area</th><th>Descargar</th></tr>";
                while (dr.Read())
                {
                    if (dr["cod_estado"].ToString() == "301" )
                    {
                        revision += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td>Area " + dr["cod_area"].ToString()[2] +"</td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                    else if (dr["cod_estado"].ToString() == "302" )
                    {
                        aprovados += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td>Area " + dr["cod_area"].ToString()[2] +"</td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                    else if (dr["cod_estado"].ToString() == "303" )
                    {
                        rechazados += "<tr><td><a href='#'>" + dr["nombre"].ToString() + "</a></td><td>Area " + dr["cod_area"].ToString()[2] +"</td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString() + "'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString() + "'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                }
                ViewBag.revision = revision;
                ViewBag.aprovados = aprovados;
                ViewBag.rechazados = rechazados;
                con.Close();
                Accion(Datos.rut, "Ingreso a Biblioteca de Admin");
                return View("Views/Vistas/Admin.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Aprobados()
        {
            if (Datos.nivel > 200 && Datos.nivel < 204)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                SqlDataReader dr;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "select * from Documentos";
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                dr = sentencia.ExecuteReader();
                var mensaje = "<tr><th>Nombre</th></tr>";
                while (dr.Read())
                {
                    if (dr["cod_estado"].ToString()=="302" && dr["cod_area"].ToString()==Datos.area.ToString())
                    {
                        mensaje += "<tr><td><a href='#'>" + dr["nombre"].ToString()+ "</a></td><td><form action='../Vistas/Descargar' method='post'><input type='hidden' name='nombre' value='" + dr["nombre"].ToString()+"'/><input type='hidden' name='area' value='" + dr["cod_area"].ToString()+"'/><input type='submit' value='Descargar'></form> </td></tr>";
                    }
                }
                ViewBag.mensaje = mensaje;
                con.Close();
                Accion(Datos.rut, "Ingreso a Biblioteca de Aprovados");
                return View("Views/Vistas/Aprobados1.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Acciones()
        {
            if (Datos.nivel == 203 )
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                SqlDataReader dr;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "select * from Acciones";
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                dr = sentencia.ExecuteReader();
                var mensaje = "<tr><th>Rut</th><th>fecha</th><th>accion</th></tr>";
                while (dr.Read())
                {
                    mensaje += "<tr><td><a href='#'>" + dr["rut"].ToString() + "</a></td><td>" + dr["fecha"].ToString() + "</td><td>" + dr["accion"].ToString() +"</td></tr>";
                }
                ViewBag.mensaje = mensaje;
                con.Close();
                Accion(Datos.rut, "Ingreso a Aciones");
                return View("Views/Vistas/Acciones.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Usuarios()
        {
            if (Datos.nivel == 203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                SqlDataReader dr;
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = @"SELECT Usuarios.nombre AS nombre, Usuarios.rut AS rut,Usuarios.correo AS correo, 
                ISNULL(Areas.nombre, 'Sin área') AS area, ISNULL(Cargos.nombre, 'Sin cargo') AS cargo, Usuarios.clave AS clave
                FROM Usuarios LEFT JOIN Areas ON Usuarios.cod_area = Areas.cod_area LEFT JOIN  Cargos ON Usuarios.cod_cargo = Cargos.cod_cargo;";
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                dr = sentencia.ExecuteReader();
                var mensaje = "<tr><th>Nombre</th><th>Rut</th><th>Correo</th><th>Area</th><th>Cargo</th><th>Clave</th></tr>";
                while (dr.Read())
                {
                    mensaje += "<tr><td>" + dr["nombre"].ToString() + "</td><td>" + dr["rut"].ToString() + "</td><td>" + dr["correo"].ToString() + "</td><td>" + dr["area"].ToString() + "</td><td>" + dr["cargo"].ToString() + "</td><td>" + dr["clave"].ToString() + "</td></tr>";
                }
                ViewBag.mensaje = mensaje;
                con.Close();
                Accion(Datos.rut, "Ingreso a Usuarios");
                return View("Views/Vistas/Usuarios.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult revisar(string nombre, string area)
        {
            if (Datos.nivel > 201 && Datos.nivel < 204)
            {
                Datos.nombre_doc = nombre;
                Datos.area_doc = area;
                ViewBag.nombre = nombre;
                Accion(Datos.rut, "Ingreso a Revisar archivo "+nombre);
                return View("Views/Vistas/Revisar.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult soporte()
        {
            return View("Views/Vistas/Soporte.cshtml");
        }
        public int[] validaUsuario(string rut, string clave)
        {
            SqlConnection con = new SqlConnection(ConnectionBdd);
            con.Open();
            var sentencia = new SqlCommand();
            SqlDataReader dr;
            sentencia.CommandType = System.Data.CommandType.Text;
            sentencia.CommandText = "select * from Usuarios";
            sentencia.Connection = con;
            dr = sentencia.ExecuteReader();
            int nivel = 204,area=0;
            while (dr.Read())
            {
                if (dr["rut"].ToString() == rut)
                {
                    if (dr["clave"].ToString() == clave)
                    {
                        nivel = int.Parse(dr["cod_cargo"].ToString());
                        area = int.Parse(dr["cod_area"].ToString());
                    }
                    else
                    {
                        nivel = 205;
                    }
                }
            }
            int[] cred = { nivel, area };
            con.Close();
            return cred;
        }
        public int validarut(string ruta)
        {
            string texto = ruta, valores = "1234567890k-", prueba = "";
            int[] nums = new int[8];
            int[] mult = { 3, 2, 7, 6, 5, 4, 3, 2 };
            double suma = 0, var = 0, k = 0;
            int[] rut = new int[10];
            for (int i = 0; i < texto.Length; i++)
            {
                for (int j = 0; j < valores.Length; j++)
                {
                    if (texto[i] == valores[j])
                    {
                        prueba += texto[i];
                        if (texto[i] == 'k')
                        {
                            k++;
                        }
                    }
                }
            }
            if (prueba.Length < 3)
            {
                k = 2;
            }
            if (texto.Length == prueba.Length && texto != "" && prueba[prueba.Length - 2] == '-' && k < 2)
            {
                if (prueba.Length < 10)
                {
                    int largo = 10 - prueba.Length;
                    for (int i = 0; i < (largo); i++)
                    {
                        prueba = "0" + prueba;
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        rut[i] = int.Parse(prueba[i].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        rut[i] = int.Parse(prueba[i].ToString());
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    suma += rut[i] * mult[i];
                }
                suma = suma / 11;
                suma = 11 - (11 * (suma - Math.Truncate(suma)));
                var = Math.Round(suma);
                if ((var.ToString() == prueba[9].ToString()) || (var == 11 && prueba[9].ToString() == "0") || (var == 10 && prueba[9].ToString() == "k"))
                {
                    return 1;
                }
                else
                {
                    if (var == 10)
                    {
                        return 0;
                    }
                    else if (var == 11)
                    {
                        return 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return 0;
            }
        }
        public IActionResult Ingreso(string nombre,string rut,string correo,int area,int cargo,string clave)
        {
            if (validarut(rut) == 1)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "insert into Usuarios(nombre,rut,correo,cod_area,cod_cargo,clave) values(@nombre,@rut,@correo,@cod_area,@cod_cargo,@clave)";
                sentencia.Parameters.Add(new SqlParameter("nombre", nombre));
                sentencia.Parameters.Add(new SqlParameter("rut", rut));
                sentencia.Parameters.Add(new SqlParameter("correo", correo));
                sentencia.Parameters.Add(new SqlParameter("cod_area", area));
                sentencia.Parameters.Add(new SqlParameter("cod_cargo", cargo));
                sentencia.Parameters.Add(new SqlParameter("clave", clave));
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                if (resultado == 1)
                {
                    ViewBag.men = "GUARDADO CORRECTAMENTE";
                    Accion(Datos.rut, "Registro el nuevo usuario "+rut);
                    return Usuarios();
                }
                else
                {
                    ViewBag.men = "ERROR AL GUARDAR";
                    return View("Views/Vistas/NUsuario.cshtml");
                }
            }
            else
            {
                ViewBag.men = "RUT INVALIDO";
                return View("Views/Vistas/NUsuario.cshtml");
            }
        }
        public IActionResult Modifico()
        {
            if (Datos.nivel == 203)
            {
                Accion(Datos.rut, "Ingreso a Modificar");
                return View("Views/Vistas/Modificar.cshtml");
            }
            else
            {
                return Menu();
            }
        }
        public IActionResult Modificar(string nombre, string rut, string correo, int area, int cargo, string clave)
        {
            SqlConnection cone = new SqlConnection(ConnectionBdd);
            cone.Open();
            var busca = new SqlCommand();
            SqlDataReader dr;
            busca.CommandType = System.Data.CommandType.Text;
            busca.CommandText = "select * from Usuarios";
            busca.Connection = cone;
            dr = busca.ExecuteReader();
            int x = 0;
            while (dr.Read())
            {
                if (dr["rut"].ToString() == rut)
                {
                    x = 1;
                }
            }
            if (validarut(rut) == 1 && x==1)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "Update Usuarios set nombre=@nombre,rut=@rut,correo=@correo,cod_area=@cod_area,cod_cargo=@cod_cargo,clave=@clave where rut=@rut";
                sentencia.Parameters.Add(new SqlParameter("nombre", nombre));
                sentencia.Parameters.Add(new SqlParameter("rut", rut));
                sentencia.Parameters.Add(new SqlParameter("correo", correo));
                sentencia.Parameters.Add(new SqlParameter("cod_area", area));
                sentencia.Parameters.Add(new SqlParameter("cod_cargo", cargo));
                sentencia.Parameters.Add(new SqlParameter("clave", clave));
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                if (resultado == 1)
                {
                    ViewBag.men = "GUARDADO CORRECTAMENTE";
                    Accion(Datos.rut, "Modifico el Usuario "+rut);
                    return Usuarios();
                }
                else
                {
                    ViewBag.men = "ERROR AL GUARDAR";
                    return View("Views/Vistas/NUsuario.cshtml");
                }
            }
            else
            {
                ViewBag.mene = "RUT INVALIDO";
                return View("Views/Vistas/NUsuario.cshtml");
            }
        }
        public IActionResult Eliminar(string rut)
        {
            SqlConnection con = new SqlConnection(ConnectionBdd);
            con.Open();
            var sentencia = new SqlCommand();
            sentencia.CommandType = System.Data.CommandType.Text;
            sentencia.CommandText = "Delete from Usuarios where rut=@rut";
            sentencia.Parameters.Add(new SqlParameter("rut", rut));
            sentencia.Connection = con;
            var resultado = sentencia.ExecuteNonQuery();
            con.Close();
            ViewBag.men = "SE ELIMINO CORRECTAMENTE";
            Accion(Datos.rut, "Elimino al Usuario "+rut);
            return Usuarios();
        }
        public void Accion(string rut, string accion)
        {
            SqlConnection con = new SqlConnection(ConnectionBdd);
            con.Open();
            var sentencia = new SqlCommand();
            sentencia.CommandType = System.Data.CommandType.Text;
            sentencia.CommandText = "insert into Acciones(rut,fecha,accion) values (@rut,@fecha,@accion)";
            sentencia.Parameters.Add(new SqlParameter("rut", rut));
            sentencia.Parameters.Add(new SqlParameter("fecha", DateTime.Now));
            sentencia.Parameters.Add(new SqlParameter("accion", accion));
            sentencia.Connection = con;
            var resultado = sentencia.ExecuteNonQuery();
            con.Close();
        }
        public IActionResult Subir(IFormFile archivo)
        {
            string path = Path.Combine(rutaArchivos+Datos.area, archivo.FileName);
            using var stream = new FileStream(path, FileMode.Create);
            archivo.CopyTo(stream);
            var md5 = MD5.Create();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] hashBytes = md5.ComputeHash(stream);
            string a = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            string encript = Encriptado(a);
            stream.Close();
            SqlConnection con = new SqlConnection(ConnectionBdd);
            con.Open();
            var sentencia = new SqlCommand();
            sentencia.CommandType = System.Data.CommandType.Text;
            sentencia.CommandText = "insert into Documentos(cod,nombre,f_subida,cod_estado,creador,cod_area) values (@cod,@nombre,@f_subida,@cod_estado,@creador,@cod_area)";
            sentencia.Parameters.Add(new SqlParameter("cod", encript));
            sentencia.Parameters.Add(new SqlParameter("nombre", archivo.FileName));
            sentencia.Parameters.Add(new SqlParameter("f_subida", DateTime.Now));
            sentencia.Parameters.Add(new SqlParameter("cod_estado", 301));
            sentencia.Parameters.Add(new SqlParameter("creador", Datos.rut));
            sentencia.Parameters.Add(new SqlParameter("cod_area", Datos.area));
            sentencia.Connection = con;
            var resultado = sentencia.ExecuteNonQuery();
            Accion(Datos.rut, "Subio el Archivo "+archivo.FileName);
            return Menu();
        }
        public string Encriptado(string archivo)
        {
            string hash = "Hola profe";
            byte[] data = UTF8Encoding.UTF8.GetBytes(archivo);

            MD5 md5 = MD5.Create();
            TripleDES tripleDES = TripleDES.Create();

            tripleDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripleDES.Mode = CipherMode.ECB;

            ICryptoTransform transform = tripleDES.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

            return Convert.ToBase64String(result);
        }
        public FileResult Descargar(string nombre, string area)
        {
            var path = rutaArchivos + "\\" + area + "\\" + nombre;
            //var path = "C:\\Users\\franc\\source\\repos\\FarmaciaEquisde\\Archivos\\"+area+"\\"+nombre;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string encodedFileName = System.Web.HttpUtility.UrlEncode(nombre, System.Text.Encoding.UTF8);

            Response.Headers.Add("Content-Disposition", $"attachment; filename={encodedFileName}");
            Accion(Datos.rut, "Descargo el Archivo "+nombre);
            return File(bytes, "application/pdf", encodedFileName);
        }
        public FileResult Descarga()
        {
            string nombre = Datos.nombre_doc;
            string area = Datos.area_doc;
            var path = rutaArchivos + "\\" + area + "\\" + nombre;
            //var path = "C:\\Users\\franc\\source\\repos\\FarmaciaEquisde\\Archivos\\" + area + "\\" + nombre;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string encodedFileName = System.Web.HttpUtility.UrlEncode(nombre, System.Text.Encoding.UTF8);

            Response.Headers.Add("Content-Disposition", $"attachment; filename={encodedFileName}");
            Accion(Datos.rut, "Descargo el Archivo "+nombre);
            return File(bytes, "application/pdf", encodedFileName);
        }
        public IActionResult Aprovar()
        {
            string nombre = Datos.nombre_doc;
            string area = Datos.area_doc;
            SqlConnection con = new SqlConnection(ConnectionBdd);
            con.Open();
            var sentencia1 = new SqlCommand();
            sentencia1.CommandType = System.Data.CommandType.Text;
            sentencia1.CommandText = "select * from Documentos";
            sentencia1.Connection = con;
            SqlDataReader dr;
            dr = sentencia1.ExecuteReader();
            var path = Path.Combine(rutaArchivos + "\\" + area, nombre);
            //var path = Path.Combine("C:\\Users\\franc\\source\\repos\\FarmaciaEquisde\\Archivos\\"+area,nombre);
            using var stream = new FileStream(path, FileMode.Open);
            var md5b = MD5.Create();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] hashBytesb = md5b.ComputeHash(stream);
            string b = BitConverter.ToString(hashBytesb).Replace("-", "").ToLower();
            string cod = Encriptado(b);
            stream.Close();
            int x = 0;
            while (dr.Read())
            {
                if (dr["cod"].ToString() == cod && dr["nombre"].ToString()==nombre)
                {
                    x = 1;
                }
            }
            con.Close();
            SqlConnection cone = new SqlConnection(ConnectionBdd);
            cone.Open();
            if (x == 1)
            {
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "Update Documentos set cod_estado = @estado, f_revision = @revision, aprovador = @aprovador where cod = @cod";
                sentencia.Parameters.Add(new SqlParameter("estado", 302));
                sentencia.Parameters.Add(new SqlParameter("revision", DateTime.Now));
                sentencia.Parameters.Add(new SqlParameter("aprovador", Datos.rut));
                sentencia.Parameters.Add(new SqlParameter("cod", cod));
                sentencia.Connection = cone;
                var resultado = sentencia.ExecuteNonQuery();
                cone.Close();
                Accion(Datos.rut, "Aprovo el Archivo "+nombre);
                return Menu();
            }
            else
            {
                ViewBag.mensaje = "DOCUMENTO ALTERADO";
                cone.Close();
                stream.Close();
                ViewBag.eliminar = "< form method = 'post' action = '../Vistas/Eliminar_Ar' >< input type = 'submit' value = 'Eliminar' class='botones' /></form>";
                return View("Views/Vistas/Revisar.cshtml");
            }
        }
        public IActionResult Rechazar()
        {
            string nombre = Datos.nombre_doc;
            string area = Datos.area_doc;
            SqlConnection con = new SqlConnection(ConnectionBdd);
            con.Open();
            var sentencia1 = new SqlCommand();
            sentencia1.CommandType = System.Data.CommandType.Text;
            sentencia1.CommandText = "select * from Documentos";
            sentencia1.Connection = con;
            SqlDataReader dr;
            dr = sentencia1.ExecuteReader();
            var path = Path.Combine(rutaArchivos + "\\" + area, nombre);
            //var path = Path.Combine("C:\\Users\\franc\\source\\repos\\FarmaciaEquisde\\Archivos\\" + area, nombre);
            using var stream = new FileStream(path, FileMode.Open);
            var md5b = MD5.Create();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] hashBytesb = md5b.ComputeHash(stream);
            string b = BitConverter.ToString(hashBytesb).Replace("-", "").ToLower();
            string cod = Encriptado(b);
            bool bandera = false;
            stream.Close();
           
            int x = 0;
            while (dr.Read() && !bandera)
            {
                if (dr["cod"].ToString() == cod && dr["nombre"].ToString() == nombre)
                {
                    x = 1;
                    bandera = true;
                }
            }
            dr.Close();
            if (x == 1)
            {
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "Update Documentos set cod_estado = @estado, f_revision = @revision, aprovador = @aprovador where cod = @cod";
                sentencia.Parameters.Add(new SqlParameter("estado", 303));
                sentencia.Parameters.Add(new SqlParameter("revision", DateTime.Now));
                sentencia.Parameters.Add(new SqlParameter("aprovador", Datos.rut));
                sentencia.Parameters.Add(new SqlParameter("cod", cod));
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                Accion(Datos.rut, "Rechazo el Archivo " + nombre);
                return Menu();
            }
            else
            {
                ViewBag.mensaje = "DOCUMENTO ALTERADO";
                con.Close();
                stream.Close();
                ViewBag.eliminar = "<form method=\"post\" action=\"../Vistas/Eliminar_Ar\">\r\n<input type=\"submit\" value=\"Eliminar\" class=\"botones\"/>\r\n </form>";
                return View("Views/Vistas/Revisar.cshtml");
            }
        }
        public IActionResult Eliminar_Ar()
        {
            if (Datos.nivel == 203)
            {
                string nombre = Datos.nombre_doc;
                string area = Datos.area_doc;
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = "Delete from Documentos where nombre=@nombre and cod_area=@area";
                sentencia.Parameters.Add(new SqlParameter("nombre", nombre));
                sentencia.Parameters.Add(new SqlParameter("area", area));
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                var path = Path.Combine(rutaArchivos + "\\" + area, nombre);
                System.IO.File.Delete(path);
                Accion(Datos.rut, "Elimino archivo " + nombre);
                return Menu();
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
        }
        public IActionResult modifyArea(string codigo)
        {
            string consulta = string.Format("select * from Areas where cod_area = '{0}'", codigo);
            if (Datos.nivel == 203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia1 = new SqlCommand();
                sentencia1.CommandType = System.Data.CommandType.Text;
                sentencia1.CommandText = consulta;
                sentencia1.Connection = con;
                SqlDataReader dr;
                dr = sentencia1.ExecuteReader();
                string dtConsulta = "";
                string dtConsulta2 = "";
                while (dr.Read())
                {
                    dtConsulta += dr["nombre"].ToString();
                    dtConsulta2 += dr["cod_area"].ToString();
                }
                dr.Close();
                ViewBag.dt = dtConsulta;
                ViewBag.dt2 = dtConsulta2;
                return View("Views/Vistas/CRUD-areas.cshtml");
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
        }
        public IActionResult modifyCargo(string codigo)
        {
            if (Datos.nivel == 203)
            {
                string consulta = string.Format("select * from Cargos where cod_cargo = '{0}'", codigo);
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia1 = new SqlCommand();
                sentencia1.CommandType = System.Data.CommandType.Text;
                sentencia1.CommandText = consulta;
                sentencia1.Connection = con;
                SqlDataReader dr;
                dr = sentencia1.ExecuteReader();
                string dtConsulta = "";
                string dtConsulta2 = "";
                while (dr.Read())
                {
                    dtConsulta += dr["nombre"].ToString();
                    dtConsulta2 += dr["cod_area"].ToString();
                }
                dr.Close();
                ViewBag.dt = dtConsulta;
                ViewBag.dt2 = dtConsulta2;
                return View("Views/Vistas/CRUD-Cargos.cshtml");
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
        }

        public IActionResult modificarArea(string codigo, string nombre)
        {
            string consulta = string.Format("update Areas set nombre = '{0}' where cod_area = '{1}'",nombre, codigo);
            if (Datos.nivel == 203)
            { 
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = consulta;
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                return View("Views/Vistas/CRUD-areas.cshtml");

            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }

        }
        public IActionResult eliminarArea(string codigo) 
        {

            string consulta = string.Format("Delete from Areas where cod_area='{0}'",codigo);
            if (Datos.nivel == 203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = consulta;
            
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                if (resultado > 0)
                {
                    ViewBag.alerta = "Registro Eliminado exitosamente";
                    return View("Views/Vistas/CRUD-areas.cshtml");  
                }
            
                con.Close();
                return View("Views/Vistas/CRUD-areas.cshtml");

            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }


        }
        public IActionResult modificarCargo(string codigo, string nombre)
        {
            string consulta = string.Format("update Cargos set nombre = '{0}' where cod_cargo = '{1}'", nombre, codigo);
            if (Datos.nivel == 203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = consulta;
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                return View("Views/Vistas/CRUD-areas.cshtml");
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }

        }
        public IActionResult eliminarCargo(string codigo)
        {
           if(Datos.nivel == 203)
            {
                if (ForeignKey(codigo))
                {
                    ViewBag.alerta = "No se Puede eliminar Registro, porque hay datos asociados en otras tablas";
                    return View("Views/Vistas/CRUD-areas.cshtml");
                }
                string consulta = string.Format("Delete from Cargos where cod_cargo='{0}'", codigo);
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = consulta;
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                if (resultado > 0)
                {
                    ViewBag.alerta = "Registro Eliminado exitosamente";
                    return View("Views/Vistas/CRUD-areas.cshtml");
                }

                con.Close();
                return View("Views/Vistas/CRUD-areas.cshtml");
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }

        }
        public IActionResult insertarArea(string codigo, string nombre)
        {
            string consulta = string.Format("insert into Areas (cod_area, nombre) Values('{0}', '{1}')",codigo, nombre);
            if (Datos.nivel == 203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = consulta;
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                return View("Views/Vistas/CRUD-areas.cshtml");

            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
               
        }
        public IActionResult insertarCargo(string codigo, string nombre)
        {
            string consulta = string.Format("insert into Cargos (cod_cargo, nombre) Values('{0}', '{1}')", codigo, nombre);
            if (Datos.nivel == 203)
            {
                SqlConnection con = new SqlConnection(ConnectionBdd);
                con.Open();
                var sentencia = new SqlCommand();
                sentencia.CommandType = System.Data.CommandType.Text;
                sentencia.CommandText = consulta;
                sentencia.Connection = con;
                var resultado = sentencia.ExecuteNonQuery();
                con.Close();
                return View("Views/Vistas/CRUD-cargos.cshtml");

            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
           
        }
        private bool ForeignKey(string codigo)
        {
            
            string consulta = string.Format("SELECT COUNT(*) FROM Usuarios WHERE cod_cargo='{0}'", codigo);

            using (SqlConnection con = new SqlConnection(ConnectionBdd))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(consulta, con))
                {
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public IActionResult crudAreas()
        {
            if (Datos.nivel == 203)
            {
                return View("Views/Vistas/CRUD-areas.cshtml");
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
           
        }
        public IActionResult crudCargos()
        {
            if (Datos.nivel == 203)
            {
                return View("Views/Vistas/CRUD-cargos.cshtml");
            }
            else
            {
                return View("Views/Vistas/Login.cshtml");
            }
            
        }
    }
}
