using RecetasApp1.Models;
using RecetasApp1.Data;

namespace RecetasApp1;

public partial class VerEditarReceta : ContentPage
{    
    private Recetas receta;

	public VerEditarReceta(Recetas recetaSeleccionada)
	{
		InitializeComponent();

		var editButton = new ToolbarItem("Editar", "editar.png", () =>
		{
			Editar();
		});

		ToolbarItems.Add(editButton);

		receta = recetaSeleccionada;
        BindingContext = recetaSeleccionada;        
		Title = recetaSeleccionada.Name.ToUpper();		
    }

	private void Editar()
	{
		ToolbarItems.Clear();

        SoloLectura(false);		

		var saveButton = new ToolbarItem("Actualizar", "guardar.png", () =>
		{			
			Actualizar();
		});
		
		ToolbarItems.Add(saveButton);
	}

	private void Actualizar()
	{
        if (!string.IsNullOrEmpty(nombre.Text) && !string.IsNullOrEmpty(instrucciones.Text))
        { 
            try
            {
                var db = new SQLiteService().GetConnection();

                receta.Name = nombre.Text;
                receta.Instructions = instrucciones.Text;

                db.Update(receta);               

                SoloLectura(true);

                Title = receta.Name.ToUpper();

                ShowMessage("Se ha actualizado correctamente", 3000);
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

    private void SoloLectura(bool readable)
    {
        nombre.IsReadOnly = readable;
        instrucciones.IsReadOnly = readable;
    }
}