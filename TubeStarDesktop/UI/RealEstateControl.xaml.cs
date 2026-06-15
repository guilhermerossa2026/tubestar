using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TubeStar
{
    public partial class RealEstateControl : UserControl
    {
        public RealEstateControl()
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
            bool showApartment = tabApartment.IsChecked == true;
            bool showHouse = tabHouse.IsChecked == true;
            bool showMansion = tabMansion.IsChecked == true;
            bool showIsland = tabIsland.IsChecked == true;
            bool showOffice = tabOffice.IsChecked == true;
            bool showOwned = tabOwned.IsChecked == true;

            foreach (var item in AssetCatalog.RealEstate)
            {
                bool shouldDisplay = false;
                bool isOwned = Player.Current.OwnedRealEstate.Contains(item.Id);

                if (showOwned)
                {
                    shouldDisplay = isOwned;
                }
                else
                {
                    if (showAll)
                        shouldDisplay = true;
                    else if (showApartment && item.Category == "Apartamento")
                        shouldDisplay = true;
                    else if (showHouse && item.Category == "Casa")
                        shouldDisplay = true;
                    else if (showMansion && item.Category == "Mansão")
                        shouldDisplay = true;
                    else if (showIsland && item.Category == "Ilha")
                        shouldDisplay = true;
                    else if (showOffice && item.Category == "Sala Comercial")
                        shouldDisplay = true;
                }

                if (shouldDisplay)
                {
                    var assetBlock = new AssetBlock();
                    assetBlock.RealEstate = item;
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
