//using Android.App;
using RecetasApp1.Data;
using RecetasApp1.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
//using static Android.Webkit.ConsoleMessage;

namespace RecetasApp1;

public partial class NuevaRecetaPage : ContentPage
{
    private ObservableCollection<IngredienteClass> ingredientes = new ObservableCollection<IngredienteClass>();
    private int tiempoCoccion = 30;
    private int numeroComensales = 4;

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

        listaIngredientes.ItemsSource = ingredientes;
    }

    private void Guardar()
    {
        if (!string.IsNullOrWhiteSpace(nombre.Text) && !string.IsNullOrWhiteSpace(instrucciones.Text)
            && categoria.SelectedItem != null)
        {
            try
            {
                var db = new SQLiteService().GetConnection();
                var newReceta = new Receta
                {
                    Name = nombre.Text.Trim(),
                    Category = categoria.SelectedItem.ToString(),
                    Diners = numeroComensales,
                    Time = tiempoCoccion,
                    Instructions = instrucciones.Text.Trim()
                };

                if (!string.IsNullOrEmpty(_tempImagePath))
                {
                    newReceta.ImagePath = SaveImage(_tempImagePath);
                }

                db.CreateTable<Receta>();
                db.Insert(newReceta);

                db.CreateTable<Ingrediente>();
                foreach (var item in ingredientes)
                {
                    Ingrediente newIngrediente = new Ingrediente
                    {
                        NameI = item.Nombre,
                        Quantity = item.Cantidad,
                        Unit = item.Medida,
                        RecetaId = newReceta.IdReceta
                    };
                    db.Insert(newIngrediente);
                }

                ingredientes.Clear();
                LimpiarFormulario();
                DisplayAlert("", "Se ha guardado correctamente", "Ok");
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
        categoria.SelectedItem = string.Empty;
        sliderComensales.Value = 4;
        sliderMinutos.Value = 30;
        instrucciones.Text = string.Empty;
        AgregarImagen.Source = "agregar_imagen.png";
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

    //Ajuste de los valores del slider ************************************************************************

    private void slider_ValueChangedCinco(object sender, ValueChangedEventArgs e)
    {
        // Ajustar el valor del Slider para que sea múltiplo de 5 (sin decimales)
        int step = 5;
        int newStep = (int)Math.Round(e.NewValue / step);
        int minutos = newStep * step;
        ((Slider)sender).Value = minutos;

        tiempoCoccion = minutos;
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

    //Foto ******************************************************************************************************

    private string _tempImagePath;

    private async void btnFoto_Clicked(object sender, EventArgs e)
    {
        // Verificar y solicitar permisos de acceso a la galería y la cámara
        var statusGallery = await Permissions.CheckStatusAsync<Permissions.Photos>();
        var statusCamera = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (statusGallery != PermissionStatus.Granted)
        {
            statusGallery = await Permissions.RequestAsync<Permissions.Photos>();
            if (statusGallery != PermissionStatus.Granted)
            {
                await DisplayAlert("Permiso necesario", "Se requiere acceso a la galería para seleccionar fotos.", "OK");
                return;
            }
        }

        if (statusCamera != PermissionStatus.Granted)
        {
            statusCamera = await Permissions.RequestAsync<Permissions.Camera>();
            if (statusCamera != PermissionStatus.Granted)
            {
                await DisplayAlert("Permiso necesario", "Se requiere acceso a la cámara para tomar fotos.", "OK");
                return;
            }
        }

        var action = await DisplayActionSheet("Seleccionar foto", "Cancelar", null, "Hacer foto", "Elegir de la galería");

        if (action == "Hacer foto")
        {
            await TakePhoto();
        }
        else if (action == "Elegir de la galería")
        {
            await ChoosePhoto();
        }
    }

    private async Task TakePhoto()
    {
        var result = await MediaPicker.CapturePhotoAsync();
        if (result != null)
        {
            AgregarImagen.Source = result.FullPath;
            _tempImagePath = result.FullPath;
        }
    }

    private async Task ChoosePhoto()
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result != null)
        {
            AgregarImagen.Source = result.FullPath;
            _tempImagePath = result.FullPath;
        }
    }

    private string SaveImage(string sourcePath)
    {
        if (string.IsNullOrEmpty(sourcePath))
            return null;

        string newImagePath = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid()}.jpg");
        File.Copy(sourcePath, newImagePath);

        return newImagePath;
    }
}