using SQLite;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatoAPR : ContentPage
    {
        private SQLiteAsyncConnection _cnsqlite;
        public DatoAPR()
        {
            InitializeComponent();
            _cnsqlite = DependencyService.Get<ISqlite>().GetConnection();
            var consulta = Modulo.cnSqlite.Query<DatosAPR>("select*from datos_apr");
            if (consulta.Count > 0)
            {
                
                txt_rut.Text =consulta[0].rut.ToString();
                txt_razon_social.Text = consulta[0].razon_social.ToString();
                txt_id.Text = consulta[0].id_apr_global.ToString();
            }           
        }
        private async void btn_guardar_clicked(object sender, EventArgs e)
        {
                DataTable dtDAtosAPR = new DataTable();
                dtDAtosAPR = Modulo.GetDatosAPR(txt_rut.Text);
                if (dtDAtosAPR.Rows.Count == 0)
                {
                   await DisplayAlert("Información", "No existe el rut ingresado en el servidor.", "ACEPTAR");
                }
                else
                {
                    txt_razon_social.Text = dtDAtosAPR.Rows[0]["razon_social"].ToString();
                    txt_id.Text = dtDAtosAPR.Rows[0]["id_apr_global"].ToString();
                    Modulo.cnSqlite.DeleteAll<DatosAPR>();
                    Modulo.cnSqlite.Query<DatosAPR>("insert into datos_apr(id_apr_global,razon_social,rut,giro,direccion,comuna,ciudad,telefono,correo,ultima_fecha_sincronizacion)" +
                        "values(?,?,?,?,?,?,?,?,?,?)", Convert.ToInt32(dtDAtosAPR.Rows[0]["id_apr_global"]), dtDAtosAPR.Rows[0]["razon_social"].ToString(),
                        dtDAtosAPR.Rows[0]["rut_empresa"].ToString(), dtDAtosAPR.Rows[0]["giro"].ToString(), dtDAtosAPR.Rows[0]["direccion"].ToString(), dtDAtosAPR.Rows[0]["comuna"].ToString(),
                        dtDAtosAPR.Rows[0]["ciudad"].ToString(), dtDAtosAPR.Rows[0]["telefono"].ToString(), dtDAtosAPR.Rows[0]["correo"].ToString(), dtDAtosAPR.Rows[0]["ultima_fecha_sincronizacion"].ToString());
                     await DisplayAlert("Información", "Se ha sincronizado correctamente información.", "ACEPTAR");
                }                      
        }
        private void btn_volver_clicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new Login());
        }
    }

}