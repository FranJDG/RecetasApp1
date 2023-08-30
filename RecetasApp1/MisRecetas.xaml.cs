using RecetasApp1.Data;
using RecetasApp1.Models;
using System.Collections.ObjectModel;

namespace RecetasApp1;

public partial class MisRecetas : ContentPage
{
    private ObservableCollection<Receta> recetas;

    public MisRecetas()
    {
        InitializeComponent();
                
        recetas = new ObservableCollection<Receta>();
        listaRecetas.ItemsSource = recetas;

        MostrarRecetas();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MostrarRecetas();
    }


    private void MostrarRecetas()
    {
        try
        {
            var db = new SQLiteService().GetConnection();
            recetas.Clear(); // Limpia la colección antes de añadir las recetas
            List<Receta> lista = new List<Receta>();  
            lista = db.Table<Receta>().OrderBy(r => r.Name).ToList();  
            
            foreach (Receta rec in lista)
            {
                recetas.Add(rec);
            }
          
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        var item = (Receta)(sender as MenuItem).CommandParameter;

        if (await DisplayAlert("Confirmación", $"¿Seguro que desea eliminar '{item.Name.ToUpper()}'?", "Si", "No"))
        {
            try
            {
                var db = new SQLiteService().GetConnection();
                db.Delete<Receta>(item.IdReceta);
                DeleteImage(item.ImagePath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }

            MostrarRecetas();
        }
    }

    private async void listaRecetas_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Receta recetaSeleccionada)
        {
            await Navigation.PushAsync(new VerEditarReceta(recetaSeleccionada));
        }
    }

    private void DeleteImage(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
        }
    }    

    //Búsqueda por nombre o categoría
    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();

        if (string.IsNullOrWhiteSpace(searchText)) // Si no hay texto en el Entry
        {
            listaRecetas.ItemsSource = recetas; // Mostrar todas las recetas
        }
        else
        {
            listaRecetas.ItemsSource = recetas
                .Where(receta => receta.Name.ToLower().Contains(searchText) || receta.Category.ToLower().Contains(searchText))
                .ToList();
        }
    }

}