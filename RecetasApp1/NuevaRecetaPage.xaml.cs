//using Android.App;
using RecetasApp1.Data;
using RecetasApp1.Models;
//using static Android.Webkit.ConsoleMessage;

namespace RecetasApp1;

public partial class NuevaRecetaPage : ContentPage
{
    public NuevaRecetaPage()
	{
		InitializeComponent();		

        // Crear los botones que deseas agregar a la barra de navegaci�n
        var saveButton = new ToolbarItem("Guardar", "guardar.png", () =>
        {
            Guardar();
        });

        // Agregar los botones a la colecci�n ToolbarItems
        ToolbarItems.Add(saveButton);
        
    }

    private void Guardar()
    {        
        if (!string.IsNullOrEmpty(nombre.Text) && !string.IsNullOrEmpty(instrucciones.Text))
        {
            try
            {
                var db = new SQLiteService().GetConnection();
                var newReceta = new Recetas
                {
                    Name = nombre.Text,
                    Instructions = instrucciones.Text
                };

                db.CreateTable<Recetas>();
                db.Insert(newReceta);

                LimpiarFormulario();
                ShowMessage("Se ha guardado correctamente", 3000);
            }
            catch (Exception ex)
            {

                DisplayAlert("Error", ex.Message, "Ok");
            }
                               
        }
        else
        {
            ShowMessage("Introduce todos los datos", 3000);
        }
    }

    private void LimpiarFormulario()
    {
        nombre.Text = string.Empty;
        instrucciones.Text = string.Empty;
    }

    //M�todo para mostrar mensajes en pantalla de manera temporal
    private async void ShowMessage(string message, int durationMilliseconds)
    {
        mensaje.Text = message;
        mensaje.IsVisible = true;

        await Task.Delay(durationMilliseconds);

        mensaje.IsVisible = false;
        mensaje.Text = "";

        /*
          Para llamar al m�todo
          ShowMessage("�Mensaje temporal!", 2000); // Mostrar el mensaje durante 2 segundos (2000 ms)
         */
    }
    
}