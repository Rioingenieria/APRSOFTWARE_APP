using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using APRSOFTWARE_APP.Data;
using Xamarin.Forms;
using System.Linq;

namespace APRSOFTWARE_APP.ViewModel
{
    public class ExportarViewModel:BaseViewModel
    {
        #region Attributes
        Page page;
        private string _itemExportarImportar;
        private string _mes;
        private int _año;
        private string _mensaje;
        private string _tipo;
        #endregion
        #region Properties
        public string PickerImportarExportar
        {
            get { return _itemExportarImportar; }
            set { SetValue(ref this._itemExportarImportar,value); }
        }
        public string PickerMes
        {
            get { return _mes; }
            set { SetValue(ref this._mes, value); }
        }
        public int TxtAño
        {
            get { return _año; }
            set { SetValue(ref this._año, value); }
        }
        public string TxtMensaje
        {
            get { return _mensaje; }
            set { SetValue(ref this._mensaje, value); }
        }
        public string PickerTipo
        {
            get { return _tipo; }
            set { SetValue(ref this._tipo, value); }
        }
        #endregion
        #region Commands
        public ICommand RegisterCommand
        {
            get { return new RelayCommand(RegisterMethod); }
        }
        #endregion
        #region Methods
        private async void RegisterMethod()
        {
            try
            {       

            if (PickerImportarExportar == "Exportar")
            {
                int TotalLecturas = 0;
                int Contador = 0;
                //Consultamos lecturas locales SQlite
                LecturasDatabase LecturasLocales = new LecturasDatabase();
                List<Lecturas> ListadoLecturasLocales = LecturasLocales.GetLecturasLocales(_mes,_año);
                if (ListadoLecturasLocales.Count == 0)
                {
                    await page.DisplayAlert("Información", "No existen lecturas para el mes de " + _mes.ToUpper() + " de " + _año.ToString() + ".", "Aceptar");
                }
                else
                {
                    TotalLecturas = ListadoLecturasLocales.Count;
                    //Traemos lecturas existentes en el servidor para el mes y año seleccionado
                    LecturasServidorDatabase LecturasServidor = new LecturasServidorDatabase();
                    List<Lecturas> ListadoLecturasServidor = new List<Lecturas>();
                    ListadoLecturasServidor = await LecturasServidor.GetLecturas(_mes,_año);
                    foreach (var LecturaLocal in ListadoLecturasLocales)
                    {
                        Contador++;
                        TxtMensaje = "Exportando lectura "+Contador+" de "+TotalLecturas+" ...";
                        var result =ListadoLecturasServidor.Any(x=>x.id_cliente==LecturaLocal.id_cliente);
                    if (result)
                        {
                            //update
                           await LecturasServidor.ActualizarLectura(LecturaLocal);
                        }
                        else
                        {
                            //insert
                          await  LecturasServidor.InsertarLectura(LecturaLocal);

                        }

                    }       

                }
            }
            }
            catch (Exception)
            {

                await page.DisplayAlert("Información", "No se ha completado la exportacion.", "Aceptar");
            }
        }
        #endregion
        #region Constructor
        public ExportarViewModel(Page _page)
        {
            TxtAño = DateTime.Now.Year;
            PickerMes = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-ES")).ToLower();
            PickerImportarExportar = "Exportar";
            PickerTipo = "Lecturas";
            TxtMensaje = "Seleccione una opcion..";
            page = _page;
          
        }
        #endregion
    }
}
