using Microsoft.Data.Sqlite;
using RecetasApp1.Data;
using RecetasApp1.Models;
//using static Android.Content.ClipData;

namespace RecetasApp1;

public partial class MisRecetas : ContentPage
{
    public MisRecetas()
	{
		InitializeComponent();       

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
            listaRecetas.ItemsSource = db.Table<Recetas>().ToList();
        }
        catch (Exception ex)
        {

            DisplayAlert("Error", ex.Message, "Ok");
        }

    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        var item = (Recetas)(sender as MenuItem).CommandParameter;

        if (await DisplayAlert("Confirmación", $"¿Seguro que desea eliminar '{item.Name.ToUpper()}'?", "Si", "No"))
        {            
            try
            {
                var db = new SQLiteService().GetConnection();
                db.Delete<Recetas>(item.IdReceta);
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
        if (e.Item is Recetas recetaSeleccionada)
        {
            await Navigation.PushAsync(new VerEditarReceta(recetaSeleccionada));            
        }
    }
}