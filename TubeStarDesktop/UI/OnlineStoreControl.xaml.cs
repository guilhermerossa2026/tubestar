using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TubeStar
{
    /// <summary>
    /// Interaction logic for OnlineStoreControl.xaml
    /// </summary>
    public partial class OnlineStoreControl : UserControl
    {
        public OnlineStoreControl()
        {
            InitializeComponent();
            Translate();
        }

        private void Translate()
        {
            if (lblStoreTitle != null)
                lblStoreTitle.Text = EnglishStrings.OnlineStore.Translate().ToUpper();
            if (lblStoreSubtitle != null)
                lblStoreSubtitle.Text = EnglishStrings.StoreSubtitle.Translate();
            if (tabAll != null)
                tabAll.Content = EnglishStrings.StoreTabAll.Translate();
            if (tabEquipment != null)
                tabEquipment.Content = EnglishStrings.StoreTabEquipment.Translate();
            if (tabServices != null)
                tabServices.Content = EnglishStrings.StoreTabServices.Translate();
            if (tabFinance != null)
                tabFinance.Content = EnglishStrings.StoreTabFinance.Translate();
            if (tabOwned != null)
                tabOwned.Content = EnglishStrings.StoreTabOwned.Translate();
        }

        private bool IsEquipment(StoreItem item)
        {
            return item is VideoCameraI || 
                   item is VideoCameraII || 
                   item is EditingSoftwareI || 
                   item is EditingSoftwareII || 
                   item is Microphone || 
                   item is StudioLighting || 
                   item is CaptureCard;
        }

        private bool IsService(StoreItem item)
        {
            return item is Lawyer || 
                   item is Consultant;
        }

        private bool IsFinance(StoreItem item)
        {
            return item is Loan;
        }

        public void Update()
        {
            if (txtMoneyBalance != null && Player.Current != null)
            {
                txtMoneyBalance.Text = Player.Current.Money.ToCurrencyString();
            }

            if (itemPanel == null) return;
            itemPanel.Children.Clear();

            // Determine active tab
            bool showAll = tabAll.IsChecked == true;
            bool showEquipment = tabEquipment.IsChecked == true;
            bool showServices = tabServices.IsChecked == true;
            bool showFinance = tabFinance.IsChecked == true;
            bool showOwned = tabOwned.IsChecked == true;

            foreach (var item in StoreItems.Current.All)
            {
                bool shouldDisplay = false;

                if (showOwned)
                {
                    shouldDisplay = item.Purchased;
                }
                else
                {
                    if (!item.Purchased)
                    {
                        if (showAll)
                            shouldDisplay = true;
                        else if (showEquipment && IsEquipment(item))
                            shouldDisplay = true;
                        else if (showServices && IsService(item))
                            shouldDisplay = true;
                        else if (showFinance && IsFinance(item))
                            shouldDisplay = true;
                    }
                }

                if (shouldDisplay)
                {
                    var storeBlock = new StoreItemBlock(item);
                    storeBlock.PurchaseMade += StoreBlock_PurchaseMade;
                    itemPanel.Children.Insert(0, storeBlock);
                }
            }
        }

        private void StoreBlock_PurchaseMade()
        {
            Update();
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as ToggleButton;
            if (clickedButton == null) return;

            // Uncheck other buttons
            foreach (var child in tabGrid.Children)
            {
                var btn = child as ToggleButton;
                if (btn != null && btn != clickedButton)
                {
                    btn.IsChecked = false;
                }
            }

            // Ensure clicked button remains checked
            clickedButton.IsChecked = true;

            // Refresh items list
            Update();
        }
    }
}