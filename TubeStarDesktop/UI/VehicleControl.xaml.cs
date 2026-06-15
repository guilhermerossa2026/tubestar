using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TubeStar
{
    public partial class VehicleControl : UserControl
    {
        public VehicleControl()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (txtMoneyBalance != null && Player.Current != null)
            {
                txtMoneyBalance.Text = Player.Current.Money.ToCurrencyString();
            }

            if (itemPanel == null) return;
            itemPanel.Children.Clear();

            bool showAll = tabAll.IsChecked == true;
            bool showCar = tabCar.IsChecked == true;
            bool showPlane = tabPlane.IsChecked == true;
            bool showBoat = tabBoat.IsChecked == true;
            bool showJetski = tabJetski.IsChecked == true;
            bool showOwned = tabOwned.IsChecked == true;

            foreach (var item in AssetCatalog.Vehicles)
            {
                bool shouldDisplay = false;
                bool isOwned = Player.Current.OwnedVehicles.Contains(item.Id);

                if (showOwned)
                {
                    shouldDisplay = isOwned;
                }
                else
                {
                    if (showAll)
                        shouldDisplay = true;
                    else if (showCar && item.Category == "Carro")
                        shouldDisplay = true;
                    else if (showPlane && item.Category == "Avião")
                        shouldDisplay = true;
                    else if (showBoat && item.Category == "Lancha")
                        shouldDisplay = true;
                    else if (showJetski && item.Category == "Jetski")
                        shouldDisplay = true;
                }

                if (shouldDisplay)
                {
                    var assetBlock = new AssetBlock();
                    assetBlock.Vehicle = item;
                    assetBlock.PurchaseMade += AssetBlock_PurchaseMade;
                    itemPanel.Children.Add(assetBlock);
                }
            }
        }

        private void AssetBlock_PurchaseMade()
        {
            Update();
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as ToggleButton;
            if (clickedButton == null) return;

            foreach (var child in tabGrid.Children)
            {
                var btn = child as ToggleButton;
                if (btn != null && btn != clickedButton)
                {
                    btn.IsChecked = false;
                }
            }

            clickedButton.IsChecked = true;
            Update();
        }
    }
}
