using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TubeStar
{
    public partial class StockMarketControl : UserControl
    {
        private StockCompany _selectedComp;

        public StockMarketControl()
        {
            InitializeComponent();

            tradeSlider.ValueChanged += (s, ev) =>
            {
                UpdateTradeDetails();
            };
        }

        public void Update()
        {
            // Garantir que a Bolsa esteja inicializada
            StockMarketManager.InitializeOrLoad();

            if (_selectedComp == null && StockMarketManager.Companies.Count > 0)
            {
                _selectedComp = StockMarketManager.Companies[0];
            }

            PopulateCompaniesList();
            UpdateUI();
        }

        private void PopulateCompaniesList()
        {
            listContainer.Children.Clear();
            foreach (var comp in StockMarketManager.Companies)
            {
                Border card = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(comp == _selectedComp ? "#FF2222" : "#2E2E2E")),
                    BorderThickness = new Thickness(comp == _selectedComp ? 1.5 : 1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 3, 0, 3),
                    Cursor = Cursors.Hand
                };

                Grid cardGrid = new Grid();
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                card.Child = cardGrid;

                StackPanel cLeft = new StackPanel();
                TextBlock cTicker = new TextBlock { Text = comp.Ticker, FontSize = 12, FontWeight = FontWeights.Bold, Foreground = Brushes.White };
                TextBlock cName = new TextBlock { Text = comp.Name, FontSize = 9, Foreground = Brushes.Gray };
                cLeft.Children.Add(cTicker);
                cLeft.Children.Add(cName);
                cardGrid.Children.Add(cLeft);
                Grid.SetColumn(cLeft, 0);

                StackPanel cRight = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
                TextBlock cPrice = new TextBlock { Text = comp.CurrentPrice.ToCurrencyString(), FontSize = 11, FontWeight = FontWeights.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Right };
                bool compUp = comp.ChangePercentage >= 0;
                string sign = compUp ? "+" : "";
                string colorHex = compUp ? "#00FF00" : "#FF2222";
                TextBlock cChange = new TextBlock
                {
                    Text = string.Format("{0}{1}%", sign, comp.ChangePercentage),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 1, 0, 0)
                };
                cRight.Children.Add(cPrice);
                cRight.Children.Add(cChange);
                cardGrid.Children.Add(cRight);
                Grid.SetColumn(cRight, 1);

                var currentComp = comp;
                card.MouseLeftButtonDown += (s, ev) =>
                {
                    _selectedComp = currentComp;
                    UpdateUI();
                    PopulateCompaniesList();
                };

                listContainer.Children.Add(card);
            }
        }

        private void UpdateUI()
        {
            if (_selectedComp == null) return;

            // Header Detalhes
            txtCompName.Text = _selectedComp.Name.ToUpper();
            bool compIsUp = _selectedComp.ChangePercentage >= 0;
            string changeSign = compIsUp ? "+" : "";
            string changeHex = compIsUp ? "#00FF00" : "#FF2222";
            txtCompTickerPrice.Text = string.Format("{0}  |  PREÇO: {1}  ({2}{3}%)", 
                _selectedComp.Ticker, 
                _selectedComp.CurrentPrice.ToCurrencyString(), 
                changeSign, 
                _selectedComp.ChangePercentage);
            txtCompTickerPrice.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(changeHex));

            // Desenhar gráfico
            DrawStockChart();

            // Notícias
            txtNewsContent.Text = _selectedComp.DailyNews;

            // Dados da Carteira
            UpdateTradeDetails();

            // Atualizar Patrocínio e Tendência
            UpdateSponsorAndTrendDetails();
        }

        private void UpdateSponsorAndTrendDetails()
        {
            if (_selectedComp == null) return;
            
            // Sponsor
            int remainingSponsor = StockMarketManager.GetSponsorRemainingDays(_selectedComp.Ticker);
            if (remainingSponsor > 0)
            {
                txtSponsorStatus.Text = string.Format("Patrocínio: ATIVO ({0} dia{1} restante{2})", remainingSponsor, remainingSponsor > 1 ? "s" : "", remainingSponsor > 1 ? "s" : "");
                txtSponsorStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
                btnSignSponsor.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtSponsorStatus.Text = "Patrocínio: Inativo";
                txtSponsorStatus.Foreground = Brushes.Gray;
                btnSignSponsor.Visibility = Visibility.Visible;
            }
            
            int reqShares, reqSubs;
            double signBonus;
            StockMarketManager.GetSponsorRequirements(_selectedComp.Ticker, out reqShares, out reqSubs, out signBonus);
            txtSponsorRequirements.Text = string.Format("Requisitos: Possuir {0} ações (Você tem: {1}) e {2} inscritos no canal (Você tem: {3}).\nBônus de Assinatura: {4}", 
                reqShares, 
                StockMarketManager.GetPlayerShares(_selectedComp.Ticker), 
                reqSubs.ToString("N0"), 
                StockMarketManager.GetPlayerSubscribers().ToString("N0"),
                signBonus.ToCurrencyString());
            
            // Trend
            int remainingTrend = StockMarketManager.GetTrendRemainingDays(_selectedComp.Ticker);
            string categoryName = GetCategoryNameForTicker(_selectedComp.Ticker);
            if (remainingTrend > 0)
            {
                txtTrendStatus.Text = string.Format("Tendência de Mercado: ATIVA ({0} dia{1} restante{2})", remainingTrend, remainingTrend > 1 ? "s" : "", remainingTrend > 1 ? "s" : "");
                txtTrendStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
                txtTrendEffect.Text = string.Format("Efeito: Vídeos da categoria '{0}' ganham +50% de visualizações diárias.", categoryName);
                txtTrendEffect.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
            }
            else
            {
                txtTrendStatus.Text = "Tendência de Mercado: Inativa";
                txtTrendStatus.Foreground = Brushes.Gray;
                txtTrendEffect.Text = string.Format("Efeito: Vídeos da categoria '{0}' ganham +50% de visualizações diárias quando ativa.", categoryName);
                txtTrendEffect.Foreground = Brushes.Gray;
            }
        }

        private string GetCategoryNameForTicker(string ticker)
        {
            switch (ticker)
            {
                case "STB": return VideoCategory.Comedy.GetString();
                case "PEAR": return VideoCategory.Technology.GetString();
                case "RVG": return VideoCategory.Gaming.GetString();
                case "GDR": return VideoCategory.Vlog.GetString();
                case "WHP": return VideoCategory.HowTo.GetString();
                default: return "";
            }
        }

        private void btnSignSponsor_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedComp == null) return;
            
            int reqShares, reqSubs;
            double signBonus;
            StockMarketManager.GetSponsorRequirements(_selectedComp.Ticker, out reqShares, out reqSubs, out signBonus);
            
            int playerShares = StockMarketManager.GetPlayerShares(_selectedComp.Ticker);
            int playerSubs = StockMarketManager.GetPlayerSubscribers();
            
            if (playerShares < reqShares || playerSubs < reqSubs)
            {
                CustomMessageBox.ShowDialog("Contrato Negado", 
                    string.Format("Você não atende aos requisitos mínimos para ser patrocinado pela {0}.\n\nRequisitos:\n- {1} ações da empresa (Você possui: {2})\n- {3} inscritos totais (Você possui: {4})", 
                        _selectedComp.Name, 
                        reqShares, 
                        playerShares, 
                        reqSubs.ToString("N0"), 
                        playerSubs.ToString("N0")), 
                    MessagePicture.Sad);
                return;
            }
            
            // Sign the contract!
            StockMarketManager.SetSponsorRemainingDays(_selectedComp.Ticker, 10);
            Player.Current.Money += signBonus;
            StockMarketManager.SaveState();
            
            CustomMessageBox.ShowDialog("Contrato Assinado!", 
                string.Format("Parabéns! Você assinou um contrato de patrocínio de 10 dias com a {0}.\nVocê recebeu um bônus de assinatura imediato de {1}.\n\nAgora você pode selecionar a {2} como patrocinadora ao lançar novos vídeos!", 
                    _selectedComp.Name, 
                    signBonus.ToCurrencyString(), 
                    _selectedComp.Ticker), 
                MessagePicture.Money);
                
            UpdateUI();
        }

        private void UpdateTradeDetails()
        {
            if (_selectedComp == null) return;

            int qty = 0;
            double avg = 0;
            if (_selectedComp.Ticker == "STB") { qty = Player.Current.SharesSTB; avg = Player.Current.PricePaidSTB; }
            else if (_selectedComp.Ticker == "PEAR") { qty = Player.Current.SharesPEAR; avg = Player.Current.PricePaidPEAR; }
            else if (_selectedComp.Ticker == "RVG") { qty = Player.Current.SharesRVG; avg = Player.Current.PricePaidRVG; }
            else if (_selectedComp.Ticker == "GDR") { qty = Player.Current.SharesGDR; avg = Player.Current.PricePaidGDR; }
            else if (_selectedComp.Ticker == "WHP") { qty = Player.Current.SharesWHP; avg = Player.Current.PricePaidWHP; }

            // Portfólio
            txtMyShares.Text = "Cotas Possuídas: " + qty;
            txtMyAverage.Text = "Preço Médio Pago: " + (avg > 0 ? avg.ToCurrencyString() : "-");

            double returnVal = 0;
            if (qty > 0)
            {
                returnVal = (_selectedComp.CurrentPrice - avg) * qty;
                txtMyReturn.Text = "Retorno Atual: " + (returnVal >= 0 ? "+" : "") + returnVal.ToCurrencyString();
                txtMyReturn.Foreground = new SolidColorBrush(returnVal >= 0 ? (Color)ColorConverter.ConvertFromString("#00FF00") : (Color)ColorConverter.ConvertFromString("#FF2222"));
            }
            else
            {
                txtMyReturn.Text = "Retorno Atual: -";
                txtMyReturn.Foreground = Brushes.Gray;
            }

            txtMyMoney.Text = "Saldo Disponível: " + Player.Current.Money.ToCurrencyString();

            // Limites do Slider de Negociação
            int maxBuy = (int)(Player.Current.Money / _selectedComp.CurrentPrice);
            int maxSell = qty;

            int sliderMax = Math.Max(maxBuy, maxSell);
            if (sliderMax < 1) sliderMax = 1;

            tradeSlider.Maximum = sliderMax;
            if (tradeSlider.Value > tradeSlider.Maximum) tradeSlider.Value = tradeSlider.Maximum;
            if (tradeSlider.Value < 1) tradeSlider.Value = 1;

            int currentTradeQty = (int)tradeSlider.Value;
            txtTradeQty.Text = string.Format("{0} cota{1}", currentTradeQty, currentTradeQty > 1 ? "s" : "");

            // Habilitar / desabilitar botões
            btnBuy.IsEnabled = (Player.Current.Money >= (_selectedComp.CurrentPrice * currentTradeQty));
            btnSell.IsEnabled = (qty >= currentTradeQty);
            btnBuy.Opacity = btnBuy.IsEnabled ? 1.0 : 0.4;
            btnSell.Opacity = btnSell.IsEnabled ? 1.0 : 0.4;
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedComp == null) return;

            int qtyToBuy = (int)tradeSlider.Value;
            double totalCost = qtyToBuy * _selectedComp.CurrentPrice;
            if (Player.Current.Money >= totalCost)
            {
                Player.Current.Money -= totalCost;

                int qty = 0;
                double avg = 0;
                if (_selectedComp.Ticker == "STB") { qty = Player.Current.SharesSTB; avg = Player.Current.PricePaidSTB; }
                else if (_selectedComp.Ticker == "PEAR") { qty = Player.Current.SharesPEAR; avg = Player.Current.PricePaidPEAR; }
                else if (_selectedComp.Ticker == "RVG") { qty = Player.Current.SharesRVG; avg = Player.Current.PricePaidRVG; }
                else if (_selectedComp.Ticker == "GDR") { qty = Player.Current.SharesGDR; avg = Player.Current.PricePaidGDR; }
                else if (_selectedComp.Ticker == "WHP") { qty = Player.Current.SharesWHP; avg = Player.Current.PricePaidWHP; }

                double totalInvested = (qty * avg) + totalCost;
                int newQty = qty + qtyToBuy;
                double newAvg = Math.Round(totalInvested / newQty, 2);

                SaveTransaction(newQty, newAvg);
                UpdateUI();
                PopulateCompaniesList();
            }
        }

        private void btnSell_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedComp == null) return;

            int qty = 0;
            double avg = 0;
            if (_selectedComp.Ticker == "STB") { qty = Player.Current.SharesSTB; avg = Player.Current.PricePaidSTB; }
            else if (_selectedComp.Ticker == "PEAR") { qty = Player.Current.SharesPEAR; avg = Player.Current.PricePaidPEAR; }
            else if (_selectedComp.Ticker == "RVG") { qty = Player.Current.SharesRVG; avg = Player.Current.PricePaidRVG; }
            else if (_selectedComp.Ticker == "GDR") { qty = Player.Current.SharesGDR; avg = Player.Current.PricePaidGDR; }
            else if (_selectedComp.Ticker == "WHP") { qty = Player.Current.SharesWHP; avg = Player.Current.PricePaidWHP; }

            int qtyToSell = (int)tradeSlider.Value;
            if (qtyToSell > qty) qtyToSell = qty;

            if (qtyToSell > 0)
            {
                double revenue = qtyToSell * _selectedComp.CurrentPrice;
                Player.Current.Money += revenue;

                int newQty = qty - qtyToSell;
                double newAvg = newQty > 0 ? avg : 0;

                SaveTransaction(newQty, newAvg);
                UpdateUI();
                PopulateCompaniesList();
            }
        }

        private void SaveTransaction(int newQty, double newPricePaid)
        {
            if (_selectedComp == null) return;
            if (_selectedComp.Ticker == "STB") { Player.Current.SharesSTB = newQty; Player.Current.PricePaidSTB = newPricePaid; }
            else if (_selectedComp.Ticker == "PEAR") { Player.Current.SharesPEAR = newQty; Player.Current.PricePaidPEAR = newPricePaid; }
            else if (_selectedComp.Ticker == "RVG") { Player.Current.SharesRVG = newQty; Player.Current.PricePaidRVG = newPricePaid; }
            else if (_selectedComp.Ticker == "GDR") { Player.Current.SharesGDR = newQty; Player.Current.PricePaidGDR = newPricePaid; }
            else if (_selectedComp.Ticker == "WHP") { Player.Current.SharesWHP = newQty; Player.Current.PricePaidWHP = newPricePaid; }
            StockMarketManager.SaveState();
        }

        private void DrawStockChart()
        {
            if (_selectedComp == null || chartCanvas == null) return;
            chartCanvas.Children.Clear();
            if (_selectedComp.PriceHistory == null || _selectedComp.PriceHistory.Count < 2) return;

            double width = chartCanvas.Width;
            double height = chartCanvas.Height;

            if (width <= 0) width = 370;
            if (height <= 0) height = 140;

            // Desenhar linhas de grade horizontais
            for (int i = 1; i < 4; i++)
            {
                double gridY = (height / 4) * i;
                Line gridLine = new Line
                {
                    X1 = 0,
                    Y1 = gridY,
                    X2 = width,
                    Y2 = gridY,
                    Stroke = new SolidColorBrush(Color.FromArgb(20, 255, 255, 255)),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 4, 4 }
                };
                chartCanvas.Children.Add(gridLine);
            }

            // Encontrar mínimo e máximo
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (var price in _selectedComp.PriceHistory)
            {
                if (price < min) min = price;
                if (price > max) max = price;
            }

            double delta = max - min;
            if (delta <= 0) delta = 1.0;

            // Criar a Polyline do gráfico
            Polyline polyline = new Polyline
            {
                StrokeThickness = 2.5,
                StrokeLineJoin = PenLineJoin.Round
            };

            bool isUp = _selectedComp.CurrentPrice >= _selectedComp.PriceHistory[0];
            Color chartColor = isUp ? (Color)ColorConverter.ConvertFromString("#00FF00") : (Color)ColorConverter.ConvertFromString("#FF2222");
            polyline.Stroke = new SolidColorBrush(chartColor);

            PointCollection points = new PointCollection();
            int count = _selectedComp.PriceHistory.Count;
            for (int i = 0; i < count; i++)
            {
                double price = _selectedComp.PriceHistory[i];
                double x = (width / (count - 1)) * i;
                double y = height - (((price - min) / delta) * (height - 30) + 15);
                points.Add(new Point(x, y));

                // Desenhar pequenos círculos
                Ellipse dot = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = new SolidColorBrush(chartColor),
                    Margin = new Thickness(x - 3, y - 3, 0, 0)
                };
                chartCanvas.Children.Add(dot);
            }
            polyline.Points = points;
            chartCanvas.Children.Add(polyline);

            // Rótulos de mínimo e máximo
            TextBlock lblMax = new TextBlock
            {
                Text = max.ToCurrencyString(),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromArgb(140, 255, 255, 255)),
                Margin = new Thickness(5, 5, 0, 0)
            };
            chartCanvas.Children.Add(lblMax);

            TextBlock lblMin = new TextBlock
            {
                Text = min.ToCurrencyString(),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromArgb(140, 255, 255, 255)),
                Margin = new Thickness(5, height - 18, 0, 0)
            };
            chartCanvas.Children.Add(lblMin);
        }
    }
}
