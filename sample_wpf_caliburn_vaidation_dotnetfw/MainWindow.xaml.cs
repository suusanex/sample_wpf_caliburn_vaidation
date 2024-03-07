using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sample_wpf_caliburn_vaidation_dotnetfw
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel m_ViewModel;

        public MainWindow()
        {
            m_ViewModel = new MainWindowViewModel();
            DataContext = m_ViewModel;
            InitializeComponent();
        }

        private void Password1_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            m_ViewModel.Password1 = Password1.Password;
        }

        private void Password1Confirm_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            m_ViewModel.Password1Confirm = Password1Confirm.Password;
        }
    }
}
