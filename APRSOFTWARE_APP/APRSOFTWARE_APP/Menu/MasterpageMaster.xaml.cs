using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterpageMaster : ContentPage
    {
        public ListView ListView;

        public MasterpageMaster()
        {
            InitializeComponent();
            BindingContext = new MasterpageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MasterpageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterpageMasterMenuItem> MenuItems { get; set; }

            public MasterpageMasterViewModel()
            {
              
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}