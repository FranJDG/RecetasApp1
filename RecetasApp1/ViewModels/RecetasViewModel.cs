using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RecetasApp1.Models;
using RecetasApp1.Data;
using Microsoft.Maui.Controls;

namespace RecetasApp1.ViewModels
{
    public class RecetasViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Receta> Recetas { get; set; }

        public RecetasViewModel()
        {
            Recetas = new ObservableCollection<Receta>();            
            CargarRecetas();
        }

        public void CargarRecetas()
        {
            try
            {
                var db = new SQLiteService().GetConnection();
                var recetas = db.Table<Receta>().ToList(); // Obtiene las recetas desde la base de datos
                Recetas = new ObservableCollection<Receta>(recetas); // Carga las recetas en la colección Observable
            }
            catch (Exception)
            {

                throw;
            }            
        }

        public void ActualizarReceta(Receta recetaActualizada)
        {
            Receta recetaExistente = Recetas.FirstOrDefault(r => r.IdReceta == recetaActualizada.IdReceta);
            if (recetaExistente != null)
            {
                recetaExistente.Name = recetaActualizada.Name;                
                recetaExistente.Elavoration = recetaActualizada.Elavoration;

                try
                {
                    var db = new SQLiteService().GetConnection();
                    db.Update(recetaExistente);
                }
                catch (Exception)
                {

                    throw;
                }                

                OnPropertyChanged(nameof(Recetas));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

