using RecetasApp1.Models;
using RecetasApp1.Data;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace RecetasApp1;

public partial class VerEditarReceta : ContentPage
{
    private Receta receta;
    private int numeroComensales;
    private int tiempoCoccion;
    private ObservableCollection<IngredienteClass> ingredientes = new ObservableCollection<IngredienteClass>();

    public VerEditarReceta(Receta recetaSeleccionada)
    {
        InitializeComponent();

        EditButton();

        receta = recetaSeleccionada;
        BindingContext = recetaSeleccionada;   //Es necesario para poder hacer el binding en el código xaml  
        Title = recetaSeleccionada.Name.ToUpper();
        CargarIngredientes();
        listaIngredientes.ItemsSource = ingredientes;
    }

    private void Editar()
    {
        SaveButton();

        SoloLectura(false);
        MostrarMenuEdicion(true);
    }

    private void Actualizar()
    {
        if (!string.IsNullOrWhiteSpace(nombre.Text) && !string.IsNullOrWhiteSpace(instrucciones.Text))
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

                //Primero elimino los ingredientes anteriores y despues inserto los nuevos
                var oldIngredientes = db.Table<Ingrediente>().Where(i => i.RecetaId == receta.IdReceta).ToList();

                foreach (var item in oldIngredientes)
                {
                    db.Delete(item);
                }

                foreach (var item in ingredientes)
                {
                    Ingrediente newIngrediente = new Ingrediente
                    {
                        NameI = item.Nombre,
                        Quantity = item.Cantidad,
                        Unit = item.Medida,
                        RecetaId = receta.IdReceta
                    };
                    db.Insert(newIngrediente);
                }

                SoloLectura(true);
                MostrarMenuEdicion(false);

                Title = receta.Name.ToUpper();
                categoriaEntry.Text = receta.Category;
                comensalesEntry.Text = receta.Diners.ToString();
                tiempoEntry.Text = receta.Time.ToString();

                EditButton();

                DisplayAlert("", "Se ha actualizado correctamente", "Ok");
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
        modoEdicion.IsVisible = edit;

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

        ingrediente.IsVisible = edit;
        cantidadMedidaGrid.IsVisible = edit;
        ingredientesBtn.IsVisible = edit;
    }

    //Ajuste de los valores del slider ************************************************************************

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


    //Ingredientes ************************************************************************************

    private void CargarIngredientes()
    {
        var db = new SQLiteService().GetConnection();

        var listaIngredientes = db.Table<Ingrediente>().Where(i => i.RecetaId == receta.IdReceta).ToList();

        foreach (var i in listaIngredientes)
        {
            ingredientes.Add(new IngredienteClass
            {
                Nombre = i.NameI,
                Cantidad = i.Quantity,
                Medida = i.Unit
            });
        }
    }


    private async void ShowMessageIngrediente(string message, int durationMilliseconds)
    {
        mensaje2.Text = message;
        mensaje2.IsVisible = true;

        await Task.Delay(durationMilliseconds);

        mensaje2.IsVisible = false;
        mensaje2.Text = "";
    }


    private void btnAgregarIngrediente_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(ingrediente.Text) && !string.IsNullOrEmpty(cantidad.Text)
            && medida.SelectedItem != null)
        {
            ingredientes.Add(new IngredienteClass
            {
                Nombre = ingrediente.Text.ToUpper().Trim(),
                Cantidad = Convert.ToDouble(cantidad.Text),
                Medida = medida.SelectedItem.ToString()
            });

            ingrediente.Text = string.Empty;
            cantidad.Text = string.Empty;
            medida.SelectedItem = string.Empty;

        }
        else
        {
            ShowMessageIngrediente("Rellena todos los campos del ingrediente", 3000);
        }
    }

    private void btnEliminarIngrediente_Clicked(object sender, EventArgs e)
    {
        if (listaIngredientes.SelectedItem != null)
        {
            IngredienteClass ingredienteSeleccionado = listaIngredientes.SelectedItem as IngredienteClass;

            if (ingredienteSeleccionado != null)
            {
                ingredientes.Remove(ingredienteSeleccionado);
            }

        }
        else
        {
            ShowMessageIngrediente("Selecciona el ingrediente que desea eliminar", 3000);
        }
    }

    public class IngredienteClass
    {
        public string Nombre { get; set; }
        public double Cantidad { get; set; }
        public string Medida { get; set; }
    }

    //Comprobar que sea de tipo double *************************************************************************

    private void cantidad_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue) && !IsValidDoubleFormat(e.NewTextValue))
        {
            ((Entry)sender).Text = e.OldTextValue;
        }
    }

    private bool IsValidDoubleFormat(string input)
    {
        // Utilizar una expresión regular para validar el formato de número double con coma o punto
        string pattern = @"^[0-9]+([,.][0-9]*)?$";
        bool validFormat = Regex.IsMatch(input, pattern);

        // Si se encontró una coincidencia, reemplazar punto por coma
        if (validFormat)
        {
            input = input.Replace(".", ",");
            cantidad.Text = input;
        }

        return validFormat;
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