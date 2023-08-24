//using Android.App;
using RecetasApp1.Data;
using RecetasApp1.Models;
using RecetasApp1.ViewModels;
//using static Android.Webkit.ConsoleMessage;

namespace RecetasApp1;

public partial class NuevaRecetaPage : ContentPage
{
    private readonly RecetasViewModel _recetasViewModel;

    public NuevaRecetaPage()
	{
		InitializeComponent();		

        // Crear los botones que deseas agregar a la barra de navegación
        var saveButton = new ToolbarItem("Guardar", "guardar.png", () =>
        {
            Guardar();
        });

        // Agregar los botones a la colección ToolbarItems
        ToolbarItems.Add(saveButton);
        
    }

    private void Guardar()
    {        
        if (!string.IsNullOrEmpty(nombre.Text) && !string.IsNullOrEmpty(elavoracion.Text))
        {
            try
            {
                var db = new SQLiteService().GetConnection();
                var newReceta = new Receta
                {
                    Name = nombre.Text,
                    Elavoration = elavoracion.Text
                };

                db.CreateTable<Receta>();
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
        elavoracion.Text = string.Empty;
    }

    //Método para mostrar mensajes en pantalla de manera temporal
    private async void ShowMessage(string message, int durationMilliseconds)
    {
        mensaje.Text = message;
        mensaje.IsVisible = true;

        await Task.Delay(durationMilliseconds);

        mensaje.IsVisible = false;
        mensaje.Text = "";

        /*
          Para llamar al método
          ShowMessage("¡Mensaje temporal!", 2000); // Mostrar el mensaje durante 2 segundos (2000 ms)
         */
    }
    
}