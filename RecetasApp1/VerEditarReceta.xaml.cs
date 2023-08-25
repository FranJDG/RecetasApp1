using RecetasApp1.Models;
using RecetasApp1.Data;
using System.Text.RegularExpressions;

namespace RecetasApp1;

public partial class VerEditarReceta : ContentPage
{    
    private Receta receta;
    private int numeroComensales;
    private int tiempoCoccion;

    public VerEditarReceta(Receta recetaSeleccionada)
	{
		InitializeComponent();

        EditButton();

		receta = recetaSeleccionada;
        BindingContext = recetaSeleccionada;   //Es necesario para poder hacer el binding en el código xaml  
		Title = recetaSeleccionada.Name.ToUpper();		
    }

	private void Editar()
	{
        SaveButton();		

        SoloLectura(false);
        MostrarMenuEdicion(true);
	}

	private void Actualizar()
	{
        if (!string.IsNullOrEmpty(nombre.Text) && !string.IsNullOrEmpty(instrucciones.Text))
        { 
            try
            {
                var db = new SQLiteService().GetConnection();

                receta.Name = nombre.Text;
                receta.Category = categoria.SelectedItem.ToString();
                receta.Diners = numeroComensales;
                receta.Instructions = instrucciones.Text;
                receta.Time = tiempoCoccion;

                db.Update(receta);               

                SoloLectura(true);
                MostrarMenuEdicion(false);

                Title = receta.Name.ToUpper();
                categoriaEntry.Text = receta.Category;
                comensalesEntry.Text = receta.Diners.ToString();
                tiempoEntry.Text = receta.Time.ToString();

                EditButton();

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

    private void MostrarMenuEdicion(bool edit) 
    {
        categoriaLabel.IsVisible = !edit;
        categoriaEntry.IsVisible = !edit;
        categoria.IsVisible = edit;

        comensalesLabel.IsVisible = !edit;
        comensalesEntry.IsVisible = !edit;
        numComensales.IsVisible = edit;
        sliderComensales.IsVisible = edit;

        tiempoLabel.IsVisible = !edit;
        tiempoEntry.IsVisible = !edit;
        tiempoPreparacion.IsVisible = edit;
        sliderMinutos.IsVisible = edit;
    }

    private void slider_ValueChangedUno(object sender, ValueChangedEventArgs e)
    {
        // Ajustar el valor del Slider para que sea múltiplo de 1 (sin decimales)
        int step = 1;
        int newStep = (int)Math.Round(e.NewValue / step);
        int personas = newStep * step;
        ((Slider)sender).Value = personas;

        numeroComensales = personas;
    }

    private void slider_ValueChangedCinco(object sender, ValueChangedEventArgs e)
    {
        // Ajustar el valor del Slider para que sea múltiplo de 5 (sin decimales)
        int step = 5;
        int newStep = (int)Math.Round(e.NewValue / step);
        int minutos = newStep * step;
        ((Slider)sender).Value = minutos;

        tiempoCoccion = minutos;
    }

    //***********************************************************************************************
    //Métodos para cambiar botones del menú

    private void EditButton()
    {
        ToolbarItems.Clear();

        var editButton = new ToolbarItem("Editar", "editar.png", () =>
        {
            Editar();
        });

        ToolbarItems.Add(editButton);
    }

    private void SaveButton()
    {
        ToolbarItems.Clear();

        var saveButton = new ToolbarItem("Actualizar", "guardar.png", () =>
        {
            Actualizar();
        });

        ToolbarItems.Add(saveButton);
    }

    //*************************************************************************************************
}