using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TubeStar
{
    public partial class AssetBlock : UserControl
    {
        public event Action PurchaseMade;

        private RealEstateItem _realEstate;
        private VehicleItem _vehicle;

        public RealEstateItem RealEstate
        {
            get { return _realEstate; }
            set
            {
                _realEstate = value;
                _vehicle = null;
                UpdateState();
            }
        }

        public VehicleItem Vehicle
        {
            get { return _vehicle; }
            set
            {
                _vehicle = value;
                _realEstate = null;
                UpdateState();
            }
        }

        private bool IsPurchased
        {
            get
            {
                if (_realEstate != null)
                    return Player.Current.OwnedRealEstate.Contains(_realEstate.Id);
                if (_vehicle != null)
                    return Player.Current.OwnedVehicles.Contains(_vehicle.Id);
                return false;
            }
        }

        private int Cost
        {
            get
            {
                if (_realEstate != null) return _realEstate.Cost;
                if (_vehicle != null) return _vehicle.Cost;
                return 0;
            }
        }

        public AssetBlock()
        {
            InitializeComponent();
        }

        public void UpdateState()
        {
            if (_realEstate == null && _vehicle == null) return;

            string name = "";
            string imageName = "";
            int cost = 0;
            int dailyTax = 0;
            int dailyRent = 0;

            if (_realEstate != null)
            {
                name = _realEstate.Name;
                cost = _realEstate.Cost;
                dailyTax = _realEstate.DailyTax;
                dailyRent = _realEstate.DailyRent;

                switch (_realEstate.Category)
                {
                    case "Apartamento": imageName = "Apartment"; break;
                    case "Casa": imageName = "House"; break;
                    case "Mansão": imageName = "Mansion"; break;
                    case "Ilha": imageName = "Island"; break;
                    case "Sala Comercial": imageName = "Office"; break;
                    default: imageName = "House"; break;
                }
            }
            else if (_vehicle != null)
            {
                name = _vehicle.Name;
                cost = _vehicle.Cost;
                dailyTax = _vehicle.DailyTax;

                switch (_vehicle.Category)
                {
                    case "Carro": imageName = "Car"; break;
                    case "Avião": imageName = "Airplane"; break;
                    case "Lancha": imageName = "Boat"; break;
                    case "Jetski": imageName = "Jetski"; break;
                    default: imageName = "Car"; break;
                }
            }

            txtName.Text = name;
            txtPrice.Text = cost.ToCurrencyString();
            txtTax.Text = String.Format("Taxa Diária: {0}", dailyTax.ToCurrencyString());

            if (dailyRent > 0)
            {
                txtRent.Visibility = Visibility.Visible;
                txtRent.Text = String.Format("Aluguel Ganho: +{0}", dailyRent.ToCurrencyString());
            }
            else
            {
                txtRent.Visibility = Visibility.Collapsed;
            }

            try
            {
                imgAsset.Source = new BitmapImage(new Uri(String.Format("../Resources/Assets/{0}.jpg", imageName), UriKind.Relative));
            }
            catch (Exception)
            {
                // Fallback
            }

            if (IsPurchased)
            {
                borderFrame.Opacity = 0.5;
                btnBuy.Visibility = Visibility.Collapsed;
                txtStatus.Visibility = Visibility.Visible;
                txtStatus.Text = "Adquirido";
                txtStatus.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FF666666") as Color? ?? Colors.Gray);
                txtPrice.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FF666666") as Color? ?? Colors.Gray);
            }
            else
            {
                borderFrame.Opacity = 1.0;
                txtStatus.Visibility = Visibility.Collapsed;
                btnBuy.Visibility = Visibility.Visible;

                bool canAfford = Player.Current.Money >= cost;
                if (Player.Current.TaxDebtAmount > 0)
                {
                    txtPrice.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FFE74C3C") as Color? ?? Colors.Red);
                    btnBuy.IsEnabled = false;
                    btnBuy.Content = "Nome Sujo";
                }
                else if (canAfford)
                {
                    txtPrice.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FF2ECC71") as Color? ?? Colors.Green);
                    btnBuy.IsEnabled = true;
                    btnBuy.Content = "Comprar";
                }
                else
                {
                    txtPrice.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FFE74C3C") as Color? ?? Colors.Red);
                    btnBuy.IsEnabled = false;
                    btnBuy.Content = "Sem Saldo";
                }
            }
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            TryBuyAsset();
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != btnBuy)
            {
                TryBuyAsset();
            }
        }

        private void TryBuyAsset()
        {
            if (IsPurchased) return;

            if (Player.Current.TaxDebtAmount > 0)
            {
                CustomMessageBox.ShowDialog(
                    "Operação Bloqueada!",
                    "Você possui pendências na Dívida Ativa governamental e não pode adquirir novos bens. Regularize seu CPF no Portal do Governo!",
                    MessagePicture.Axe
                );
                return;
            }

            this.PlayAnimation(() =>
            {
                if (Player.Current.Money - Cost < 0)
                {
                    CustomMessageBox.ShowDialog(EnglishStrings.LowCashHeader.Translate(), EnglishStrings.LowCashMessage.Translate(), MessagePicture.Money);
                    return;
                }

                CustomMessageBox.ShowDialog(EnglishStrings.Buy.Translate(), String.Format(EnglishStrings.BuyItem.Translate(), Cost.ToCurrencyString()), MessagePicture.Question, result =>
                {
                    if (result == true)
                    {
                        Player.Current.Money -= Cost;

                        if (_realEstate != null)
                        {
                            Player.Current.OwnedRealEstate.Add(_realEstate.Id);
                        }
                        else if (_vehicle != null)
                        {
                            Player.Current.OwnedVehicles.Add(_vehicle.Id);
                        }

                        if (PurchaseMade != null)
                            PurchaseMade();

                        UpdateState();
                    }
                });
            });
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (borderFrame != null)
            {
                borderFrame.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#252525") as Color? ?? Colors.Gray);
                borderFrame.BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#FF2222") as Color? ?? Colors.Red);
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (borderFrame != null)
            {
                borderFrame.Background = new SolidColorBrush(ColorConverter.ConvertFromString("#1E1E1E") as Color? ?? Colors.Black);
                borderFrame.BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#2E2E2E") as Color? ?? Colors.Gray);
            }
        }
    }
}
