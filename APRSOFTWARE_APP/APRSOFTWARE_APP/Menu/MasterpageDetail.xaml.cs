using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterpageDetail : ContentPage
    {
        public MasterpageDetail()
        {
            InitializeComponent();
            lbl_usuario.Text =Modulo.nombre_operador.ToUpper();
        }
    }
}