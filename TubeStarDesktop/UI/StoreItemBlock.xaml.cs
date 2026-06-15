using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TubeStar
{
    /// <summary>
    /// Interaction logic for StoreItemBlock.xaml
    /// </summary>
    public partial class StoreItemBlock : UserControl
    {
        public event Action PurchaseMade;

        private StoreItem _storeItem;
        public StoreItem StoreItem
        {
            get { return _storeItem; }
            private set
            {
                _storeItem = value;
                UpdateState();
            }
        }

        public StoreItemBlock(StoreItem item)
        {
            InitializeComponent();
            StoreItem = item;
        }

        public void UpdateState()
        {
            if (_storeItem == null) return;

            txtName.Text = _storeItem.Name;
            txtDescription.Text = _storeItem.Description;
            txtPrice.Text = _storeItem.Cost.ToCurrencyString();
            
            try
            {
                imgItem.Source = new BitmapImage(new Uri(String.Format("../Resources/StoreItems/{0}.jpg", _storeItem.ImageName), UriKind.Relative));
            }
            catch (Exception)
            {
                // Fallback in case of image load error
            }

            if (_storeItem.Purchased)
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

                bool canAfford = Player.Current.Money >= _storeItem.Cost;
                if (canAfford)
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
            TryBuyItem();
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Only trigger if clicking the card body and not already purchased
            if (e.OriginalSource != btnBuy)
            {
                TryBuyItem();
            }
        }

        private void TryBuyItem()
        {
            if (_storeItem == null || _storeItem.Purchased)
                return;

            this.PlayAnimation(() =>
            {
                if (Player.Current.Money - _storeItem.Cost < 0)
                {
                    CustomMessageBox.ShowDialog(EnglishStrings.LowCashHeader.Translate(), EnglishStrings.LowCashMessage.Translate(), MessagePicture.Money);
                    return;
                }

                CustomMessageBox.ShowDialog(EnglishStrings.Buy.Translate(), String.Format(EnglishStrings.BuyItem.Translate(), _storeItem.Cost.ToCurrencyString()), MessagePicture.Question, result =>
                {
                    if (result == true)
                    {
                        Player.Current.Money -= _storeItem.Cost;
                        _storeItem.Purchased = true;

                        if (_storeItem == StoreItems.Current.Loan)
                        {
                            Player.Current.LoanPayOff = (StoreItems.Current.Loan.Payout * (100 + StoreItems.Current.Loan.Interest) / 100);
                            Player.Current.Money += StoreItems.Current.Loan.Payout;
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