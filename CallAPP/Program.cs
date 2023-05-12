using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Data;
using CallAPP.BD;
using System.Linq;

namespace CallAPP
{
    class Program
    {
        static void Main(string[] args)
        {
           
            try
            {
                // Configuración del driver de Appium
                AppiumOptions appiumOptions = new AppiumOptions();
                appiumOptions.PlatformName = "Android";
                appiumOptions.AddAdditionalCapability("deviceName", "emulator-5554");
                appiumOptions.AddAdditionalCapability("appPackage", "com.callapp.contacts");
                appiumOptions.AddAdditionalCapability("appActivity", "com.callapp.contacts.activity.contact.list.ContactsListActivity");

                // URL del servidor de Appium
                Uri serverUri = new Uri("http://localhost:4723/wd/hub");

                // Crear instancia del driver de Appium
                AndroidDriver<AndroidElement> driver = new AndroidDriver<AndroidElement>(serverUri, appiumOptions);

                // Esperar a que se abra la aplicación
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Console.WriteLine("Aplicacion abierta");


                // Iniciar el proceso de scrap
                PersonaModel persona = new PersonaModel();

                //persona.telefono = "50255163087";
                DataTable telefonos = new DataTable();
                string[] Variables = { "@opcion" };
                string[] Valores = { "1" };

                telefonos = conexionBD.Consulta("sp_obtiene_telefonos", Variables, Valores);

                List<PersonaModel> lst = telefonos.AsEnumerable().Select(m => new PersonaModel()
                {
                    telefono = m.Field<string>("telefonoReferencia"),
                    idCliente = m.Field<string>("idCliente"),
                    idReferencia = m.Field<string>("idReferencia"),
                }).ToList();

                // Click en el botón "Allow"
                By Allow = By.Id("com.android.permissioncontroller:id/permission_allow_button");
                driver.FindElement(Allow).Click();
                Console.WriteLine("Clic en el botón de Allow realizado exitosamente.");

                Console.WriteLine("Complete los pasos: ");
                Console.WriteLine("1. Click en el boton Google.");
                Console.WriteLine("2. Click en el boton AGREE & CONTINUE.");
                Console.WriteLine("3. Seleccionar el correo.");
                Console.WriteLine("4. Cerrar los planes.");
                Console.WriteLine("5. Cerrar el tutorial.");
                Console.WriteLine("-----> Presiona Enter cuando estes listo <-----");
                Console.ReadKey();

                foreach (var person in lst)
                {
                    RealizarConsulta(person, driver);
                }

                Console.WriteLine("-----> Proceso finalizado exitosamente presione enter para finalizar <-----");
                Console.ReadKey();

                // Cerrar la aplicación
                driver.Quit();

            } catch (Exception ex)
            {
                Console.WriteLine("Error al inicializar: " + ex);
            }

        }
        public static void RealizarConsulta (PersonaModel persona, IWebDriver driver)
        {
            // Realizar acciones en la aplicación
            try
            {
               // Seleccionar el textbox
                By busqueda = By.Id("com.callapp.contacts:id/title");
                driver.FindElement(busqueda).Click();

                // Llenar el textbox
                By search = By.Id("com.callapp.contacts:id/search_src_text");
                driver.FindElement(search).SendKeys(persona.telefono);

                // Seleccionar buscar
                By buscar = By.XPath("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/androidx.drawerlayout.widget.DrawerLayout/android.widget.RelativeLayout/android.widget.ScrollView/android.widget.FrameLayout[2]/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.RelativeLayout/androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.TextView");
                driver.FindElement(buscar).Click();

                //Obtener el resultado
                By nombre = By.Id("com.callapp.contacts:id/nameText");
                By telefono = By.XPath("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.ScrollView/android.widget.LinearLayout[2]/androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[1]/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[1]/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.TextView[1]");
                By correo = By.XPath("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.ScrollView/android.widget.LinearLayout[2]/androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[1]/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout[2]/android.widget.LinearLayout/androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.TextView");
               
                persona.nombre = driver.FindElement(nombre).Text;
                persona.informacion = driver.FindElement(correo).Text;
                persona.telefono = driver.FindElement(telefono).Text;

                // Regresar al menu principal
                By regresar = By.Id("com.callapp.contacts:id/backButton");
                driver.FindElement(regresar).Click();
                driver.FindElement(search).Clear();

                // guardar en la BD
                string [] Variables = { "@opcion", "@nombre", "@telefono", "@informacion", "@idCliente", "@idReferencia" };
                string[] Valores = { "2", persona.nombre, persona.telefono, persona.informacion, persona.idCliente, persona.idReferencia};

                conexionBD.Consulta("sp_obtiene_telefonos", Variables, Valores);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error al momento de consultar: " + ex.Message);
            }
        }
    }
}
