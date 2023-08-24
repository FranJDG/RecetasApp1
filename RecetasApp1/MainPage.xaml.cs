﻿using RecetasApp1.Data;
using RecetasApp1.Models;
using RecetasApp1.ViewModels;

namespace RecetasApp1
{
    public partial class MainPage : ContentPage
    {
        private readonly RecetasViewModel _recetasViewModel;

        public MainPage()
        {
            InitializeComponent();
            _recetasViewModel = new RecetasViewModel();
            BindingContext = _recetasViewModel;
        }

        private async void NuevaReceta_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NuevaRecetaPage());
        }

        private async void MisRecetas_Clicked(object sender, EventArgs e)
        {
            var db = new SQLiteService().GetConnection();

            // Verificar si la tabla Receta existe en la base de datos. De esta forma no necesitamos Try...catch
            // pero no podemos verficar si hay elementos en al tabla. Para verificar si hay elementos y si existe
            // la tabla, la forma correcta puede ser mediante el uso de excepciones con try...catch.
            //bool tableExists = db.TableMappings.Any(m => m.MappedType.Name == typeof(Receta).Name);

            try
            {
                var item = db.Table<Receta>().Any();
                if (item)
                {
                    await Navigation.PushAsync(new MisRecetas(_recetasViewModel));
                }
                else
                {
                    await DisplayAlert("", "No hay ninguna receta guardada.", "Ok");
                }
            }
            catch (Exception)
            {
                await DisplayAlert("", "Debes crear al menos una receta.", "Ok");
            }

        }
    }
}