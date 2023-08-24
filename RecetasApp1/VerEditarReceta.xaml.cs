using RecetasApp1.Models;
using RecetasApp1.Data;
using RecetasApp1.ViewModels;

namespace RecetasApp1;

public partial class VerEditarReceta : ContentPage
{
    private readonly RecetasViewModel _recetasViewModel;
    private Receta receta;

	public VerEditarReceta(Receta recetaSeleccionada, RecetasViewModel recetasViewModel)
	{
		InitializeComponent();

		var editButton = new ToolbarItem("Editar", "editar.png", () =>
		{
			Editar();
		});

		ToolbarItems.Add(editButton);

		receta = recetaSeleccionada;
        BindingContext = recetaSeleccionada;
        _recetasViewModel = recetasViewModel;
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
        if (!string.IsNullOrEmpty(nombre.Text) && !string.IsNullOrEmpty(elavoracion.Text))
        { 
            try
            {
                var db = new SQLiteService().GetConnection();

                receta.Name = nombre.Text;
                receta.Elavoration = elavoracion.Text;

                _recetasViewModel.ActualizarReceta(receta);                

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
        elavoracion.IsReadOnly = readable;
    }
}