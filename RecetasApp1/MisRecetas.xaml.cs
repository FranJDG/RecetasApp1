using Microsoft.Data.Sqlite;
using RecetasApp1.Data;
using RecetasApp1.Models;
using RecetasApp1.ViewModels;
//using static Android.Content.ClipData;

namespace RecetasApp1;

public partial class MisRecetas : ContentPage
{
    private readonly RecetasViewModel _recetasViewModel;    

    public MisRecetas(RecetasViewModel recetasViewModel)
	{
		InitializeComponent();

        _recetasViewModel = recetasViewModel;
        BindingContext = _recetasViewModel;

        //MostrarRecetas();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _recetasViewModel.CargarRecetas(); // Recargar la colección de recetas
    }


    //private void MostrarRecetas()
    //{
    //    try
    //    {
    //        var db = new SQLiteService().GetConnection();
    //        listaRecetas.ItemsSource = db.Table<Receta>().ToList();
    //    }
    //    catch (Exception ex)
    //    {

    //        DisplayAlert("Error", ex.Message, "Ok");
    //    }

    //}

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        var item = (Receta)(sender as MenuItem).CommandParameter;

        if (await DisplayAlert("Confirmación", $"¿Seguro que desea eliminar '{item.Name.ToUpper()}'?", "Si", "No"))
        {            
            try
            {
                var db = new SQLiteService().GetConnection();
                db.Delete<Receta>(item.IdReceta);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }                              
                
            //MostrarRecetas();  
        }
    }

    private async void listaRecetas_ItemTapped(object sender, ItemTappedEventArgs e)
    { 
        if (e.Item is Receta recetaSeleccionada)
        {
            await Navigation.PushAsync(new VerEditarReceta(recetaSeleccionada, _recetasViewModel));            
        }
    }
}