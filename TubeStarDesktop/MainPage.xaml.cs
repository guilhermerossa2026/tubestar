using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace TubeStar
{
    public partial class MainPage : UserControl
    {
        public event Action GameDeath;
        public event Action GameLose;
        public event Action GameExit;

        private readonly string[] _avatarEmojis = new string[] { "🚀", "👾", "🎧", "🔥", "💻", "👑", "🎨", "🦄" };

        private readonly string[] _fanNames = new string[]
        {
            "CyberGamer", "NeonLegend", "PixelMaster", "SpeedyPro", "TechGuru",
            "ShadowPlay", "RetroStar", "VlogQueen", "SubHunter", "ByteSize",
            "GamerMom", "UnboxKing", "GlitchFix", "LevelUp", "AlphaStream"
        };

        private readonly string[] _fanComments = new string[]
        {
            "Nossa, a qualidade de edição desse canal tá subindo demais! 😱",
            "Melhor canal do momento! Me inscrevi correndo! 😍",
            "Amei o último vídeo, assisti 3 vezes seguidas! 🚀",
            "Aquele momento gamer no final foi épico demais kkkk 😂",
            "Esse canal merece 1 milhão de inscritos fácil! 🏆",
            "O áudio e iluminação estão perfeitos, que setup incrível! 🎧",
            "Faz collab com outros Tube Stars! Seria insano! 🤝",
            "Alguém assistindo isso em 2026? Com certeza o melhor! 🔥",
            "Que roteiro sensacional, muito original e bem pensado! 💡",
            "Comentando aqui antes do canal bombar e ficar gigante! 😎"
        };

        public MainPage()
        {
            // Garante que os singletons sejam inicializados na Thread de UI de forma síncrona
            var dummyRivals = Rivals.Current;
            var dummyPlayer = Player.Current;

            InitializeComponent();
            Translate();

            // Inicializar ou carregar a Bolsa de Valores
            StockMarketManager.InitializeOrLoad();
            
            // Wire player state update
            UpdateYoutuberProfile();
            UpdateSummaryStats();

            Player.Current.MoneyChanged += () =>
            {
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    txtMoney.Text = Player.Current.Money.ToCurrencyString();
                    UpdateSummaryStats();
                }));
            };

            dailyPlanner.NewDayCompleted += () =>
            {
                this.IsEnabled = true;
                progress.Visibility = System.Windows.Visibility.Collapsed;

                // Avançar o dia na Bolsa de Valores
                StockMarketManager.NextDay();

                if (videoManager.Visibility == System.Windows.Visibility.Visible)
                    videoManager.Update();

                if (rivalViewer.Visibility == System.Windows.Visibility.Visible)
                    rivalViewer.Update();

                if (stockMarket != null && stockMarket.Visibility == System.Windows.Visibility.Visible)
                    stockMarket.Update();

                if (careerControl != null && careerControl.Visibility == System.Windows.Visibility.Visible)
                    careerControl.Update();

                if (governmentControl != null && governmentControl.Visibility == System.Windows.Visibility.Visible)
                    governmentControl.Update();

                txtNewDay.Text = String.Format("{0} {1}!", EnglishStrings.StartDay.Translate(), Player.Current.Iterations + 1);

                // Update customized profile days and stats
                UpdateYoutuberProfile();
                UpdateSummaryStats();
                GenerateDaySocialComments();

                // InstaFans Organic Growth & Day Count
                int totalInstaSubs = Player.Current.Channels.Sum(c => c.Subscribers);
                double targetInstaFollowers = totalInstaSubs * 0.40;
                if (Player.Current.InstaFollowers < targetInstaFollowers)
                {
                    int organicGrowth = (int)((targetInstaFollowers - Player.Current.InstaFollowers) * 0.05);
                    organicGrowth = Math.Max(1, organicGrowth);
                    Player.Current.InstaFollowers += organicGrowth;
                }
                Player.Current.InstaDaysSinceLastAd++;
                UpdateInstaFansUI();

                if (Player.Current.Iterations > 1 && Player.Current.Iterations % 3 == 0)
                {
                    RandomEvents.RunEvent();
                    videoManager.Update();
                    dailyPlanner.Update();
                    rivalViewer.Update();
                }

                CheckTrophies();
            };

            dailyPlanner.GameExit += () =>
            {
                if (GameLose != null)
                    GameLose();
            };
            
            dailyPlanner.Death += () =>
            {
                if (GameDeath != null)
                    GameDeath();
            };

            // Set initial day text
            txtNewDay.Text = String.Format("{0} {1}!", EnglishStrings.StartDay.Translate(), Player.Current.Iterations + 1);
        }

        private void Translate()
        {
            gridHelp.ToolTip = EnglishStrings.Help.Translate();
            gridExit.ToolTip = EnglishStrings.Exit.Translate();
            imgSave.ToolTip = EnglishStrings.SaveGame.Translate();
            txtDailyPlanner.Text = EnglishStrings.DailyPlanner.Translate();
            txtVideoPlanner.Text = EnglishStrings.VideoManager.Translate();
            txtOnlineStore.Text = EnglishStrings.OnlineStore.Translate();
            txtRivals.Text = EnglishStrings.TopTubeStars.Translate();
            if (txtStockMarket != null)
                txtStockMarket.Text = "Bolsa de Valores";
        }

        private void UpdateYoutuberProfile()
        {
            try
            {
                txtProfileName.Text = string.IsNullOrEmpty(Player.Current.YoutuberName) ? "Gamer Pro" : Player.Current.YoutuberName;

                // 1. Atualizar Mini Avatar de Alta Definição
                if (imgProfileAvatarMini != null)
                {
                    int avatarIdx = Player.Current.YoutuberAvatarId;
                    if (avatarIdx < 0 || avatarIdx >= 15) avatarIdx = 0;

                    string[] avatarUris = new string[] {
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_3.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_4.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_gangstar_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_gangstar_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelolongo_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelolongo_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelocurto_1.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_cabelocurto_2.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_trap_3.jpg",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_3.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_4.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_5.png",
                        "pack://application:,,,/TubeStar;component/Resources/avatar_casual_6.png"
                    };

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(avatarUris[avatarIdx], UriKind.Absolute);
                    bitmap.DecodePixelWidth = 128;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgProfileAvatarMini.Source = bitmap;
                }

                // 2. Atualizar Aura de Brilho Dinâmica
                string hairColorHex = string.IsNullOrEmpty(Player.Current.YoutuberHairColor) ? "#FF2222" : Player.Current.YoutuberHairColor;
                var neonColor = ColorConverter.ConvertFromString(hairColorHex) as Color? ?? Colors.Red;
                var hairBrush = new SolidColorBrush(neonColor);

                if (profileAvatarGlow != null)
                {
                    profileAvatarGlow.BorderBrush = hairBrush;
                }
                
                if (profileAvatarGlowEffect != null)
                {
                    profileAvatarGlowEffect.Color = neonColor;
                }

                txtCareerDays.Text = "Dia " + (Player.Current.Iterations + 1);
                txtMoney.Text = Player.Current.Money.ToCurrencyString();
                UpdateInstaFansUI();
            }
            catch
            {
                // Fallback in case of conversion issues
            }
        }

        private void UpdateSummaryStats()
        {
            try
            {
                UpdateChannelFilterList();
                UpdateSummaryStatsFiltered();
            }
            catch
            {
            }
        }

        private void UpdateSummaryStatsFiltered()
        {
            try
            {
                if (cbActiveChannelFilter == null) return;

                string selectedChannelName = null;
                ComboBoxItem item = cbActiveChannelFilter.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    selectedChannelName = item.Content.ToString();
                }
                else if (cbActiveChannelFilter.SelectedItem != null)
                {
                    selectedChannelName = cbActiveChannelFilter.SelectedItem.ToString();
                }

                int totalSubs = 0;
                int totalVideos = 0;
                int totalViews = 0;

                if (string.IsNullOrEmpty(selectedChannelName) || selectedChannelName == "Todos os Canais")
                {
                    // Agregado
                    if (Player.Current.Channels != null)
                    {
                        foreach (var c in Player.Current.Channels)
                        {
                            totalSubs += c.Subscribers;
                            if (c.Videos != null)
                            {
                                foreach (var v in c.Videos)
                                {
                                    totalViews += v.Views;
                                }
                            }
                        }
                    }
                    totalVideos = Player.Current.Videos != null ? Player.Current.Videos.Count : 0;
                }
                else
                {
                    // Canal específico
                    Channel channel = null;
                    if (Player.Current.Channels != null)
                    {
                        foreach (var c in Player.Current.Channels)
                        {
                            if (c.Name == selectedChannelName)
                            {
                                channel = c;
                                break;
                            }
                        }
                    }

                    if (channel != null)
                    {
                        totalSubs = channel.Subscribers;
                        totalVideos = channel.Videos != null ? channel.Videos.Count : 0;
                        if (channel.Videos != null)
                        {
                            foreach (var v in channel.Videos)
                            {
                                totalViews += v.Views;
                            }
                        }
                    }
                }

                txtTotalSubscribers.Text = totalSubs.ToString("N0");
                txtTotalVideos.Text = totalVideos.ToString();
                txtTotalViews.Text = totalViews.ToString("N0");

                double costOfLiving = 50 + Player.Current.CostOfLivingExtra;
                txtCostOfLiving.Text = costOfLiving.ToCurrencyString();
            }
            catch
            {
            }
        }

        private void UpdateChannelFilterList()
        {
            if (cbActiveChannelFilter == null) return;

            // Salvar a seleção atual
            string selectedChannelName = null;
            ComboBoxItem currentItem = cbActiveChannelFilter.SelectedItem as ComboBoxItem;
            if (currentItem != null)
            {
                selectedChannelName = currentItem.Content.ToString();
            }
            else if (cbActiveChannelFilter.SelectedItem != null)
            {
                selectedChannelName = cbActiveChannelFilter.SelectedItem.ToString();
            }

            cbActiveChannelFilter.SelectionChanged -= cbActiveChannelFilter_SelectionChanged;
            cbActiveChannelFilter.Items.Clear();

            // Adicionar opção global
            cbActiveChannelFilter.Items.Add("Todos os Canais");

            // Adicionar cada canal de fato (ignorando o canal técnico de rascunhos)
            if (Player.Current.Channels != null)
            {
                foreach (var channel in Player.Current.Channels)
                {
                    if (channel != Channel.UnreleasedVideos)
                    {
                        cbActiveChannelFilter.Items.Add(channel.Name);
                    }
                }
            }

            // Restaurar seleção
            int indexToSelect = 0;
            if (!string.IsNullOrEmpty(selectedChannelName))
            {
                for (int i = 0; i < cbActiveChannelFilter.Items.Count; i++)
                {
                    if (cbActiveChannelFilter.Items[i].ToString() == selectedChannelName)
                    {
                        indexToSelect = i;
                        break;
                    }
                }
            }

            cbActiveChannelFilter.SelectedIndex = indexToSelect;
            cbActiveChannelFilter.SelectionChanged += cbActiveChannelFilter_SelectionChanged;
        }

        private void cbActiveChannelFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSummaryStatsFiltered();
        }

        private void GenerateDaySocialComments()
        {
            try
            {
                socialFeedContainer.Children.Clear();
                Random rand = new Random();

                // Generate 4 randomized recent feedback items
                for (int i = 0; i < 4; i++)
                {
                    string name = _fanNames[rand.Next(_fanNames.Length)];
                    string comment = _fanComments[rand.Next(_fanComments.Length)];
                    int minutesAgo = rand.Next(1, 60);

                    Border bubble = new Border
                    {
                        Background = new SolidColorBrush(ColorConverter.ConvertFromString("#181818") as Color? ?? Colors.DarkGray),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 0, 10),
                        BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#262626") as Color? ?? Colors.Gray),
                        BorderThickness = new Thickness(1)
                    };

                    StackPanel inner = new StackPanel();
                    
                    Grid header = new Grid();
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    TextBlock txtUser = new TextBlock
                    {
                        Text = "👤 " + name + ":",
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#FF2222") as Color? ?? Colors.Red),
                        Margin = new Thickness(0, 0, 5, 0)
                    };
                    Grid.SetColumn(txtUser, 0);
                    header.Children.Add(txtUser);

                    TextBlock txtTime = new TextBlock
                    {
                        Text = minutesAgo + " min atrás",
                        FontSize = 9,
                        Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#555555") as Color? ?? Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    Grid.SetColumn(txtTime, 1);
                    header.Children.Add(txtTime);

                    inner.Children.Add(header);

                    TextBlock txtMsg = new TextBlock
                    {
                        Text = comment,
                        FontSize = 11,
                        Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#DDDDDD") as Color? ?? Colors.White),
                        Margin = new Thickness(0, 4, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    inner.Children.Add(txtMsg);

                    bubble.Child = inner;
                    socialFeedContainer.Children.Add(bubble);
                }
            }
            catch
            {
            }
        }

        private void ProfileCard_Click(object sender, MouseButtonEventArgs e)
        {
            // Open character edit dialog to customize aesthetic mid-game!
            Xceed.Wpf.Toolkit.ChildWindow dialog = new Xceed.Wpf.Toolkit.ChildWindow
            {
                Caption = "Personalizar Aparência Gamer",
                Width = 420,
                Height = 480,
                WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center,
                Background = new SolidColorBrush(ColorConverter.ConvertFromString("#121212") as Color? ?? Colors.Black),
                BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#2E2E2E") as Color? ?? Colors.Gray),
                BorderThickness = new Thickness(1.5)
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(15) };

            TextBlock lblTitle = new TextBlock { Text = "PERSONALIZAR MEU ESTILO", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Red), Margin = new Thickness(0, 0, 0, 10) };
            panel.Children.Add(lblTitle);

            // Cabelo
            // Seleção de Avatar
            TextBlock lblAvatar = new TextBlock { Text = "Selecione seu Avatar Gamer AAA", FontSize = 11, Foreground = new SolidColorBrush(Colors.Gray), Margin = new Thickness(0, 5, 0, 4) };
            panel.Children.Add(lblAvatar);

            ComboBox cbAvatar = new ComboBox { Height = 28, Margin = new Thickness(0, 0, 0, 15) };
            string[] avatarNames = new string[] {
                "Mauricinho Executivo",
                "Mauricinho Casual",
                "Empresário Jovem",
                "Empresário Maduro",
                "Trap Messy Pink",
                "Trap Hip-Hop",
                "Casual E-boy",
                "Casual Rocker",
                "Casual Cyberpunk",
                "Casual Contemporâneo",
                "Trap Platinado",
                "Casual Cachos",
                "Casual Bigode",
                "Casual Óculos",
                "Casual Jaqueta Couro"
            };

            for (int i = 0; i < avatarNames.Length; i++)
            {
                cbAvatar.Items.Add(avatarNames[i]);
            }

            cbAvatar.SelectedIndex = Math.Min(14, Math.Max(0, Player.Current.YoutuberAvatarId));
            panel.Children.Add(cbAvatar);

            Button btnSave = new Button { Content = "SALVAR ESTILO GAMER", Height = 35, Background = new SolidColorBrush(Colors.Red), Foreground = new SolidColorBrush(Colors.White), FontWeight = FontWeights.Bold };
            btnSave.Click += (s, ev) =>
            {
                int index = cbAvatar.SelectedIndex;
                if (index < 0 || index >= 15) index = 0;

                string[] avatarGlowColors = new string[] {
                    "#FFDD44", "#00FFFF", "#FF2222", "#FF5500", "#FF00FF", "#FF2222", "#00FF00", "#FFFFFF", "#00FF00", "#8A2BE2",
                    "#FFFFFF", "#FF00FF", "#00FFFF", "#FF5500", "#FFFFFF"
                };
                string[] avatarStyles = new string[] {
                    "Terno Premium Cinza", "Suéter de Lã Azul Premium", "Terno Azul de Fino Trato", "Camisa Preta de Sucesso",
                    "Jaqueta Puffer Preta de Grife", "Jaqueta Bomber Vermelha", "Jaqueta Jeans com Ovelha", "Jaqueta de Couro Rocker",
                    "Windbreaker Neon Militar", "Moletom Oversized Branco",
                    "Jaqueta Corta Vento & Correntes", "Camiseta Branca Básica de Grife", "Camiseta Algodão Egípcio Branca", "Camisa Flanela Xadrez Clássica", "Jaqueta de Couro Rocker Preta"
                };
                string[] avatarAccessories = new string[] {
                    "Óculos Escuros Italianos", "Colarinho de Lã Fina", "Gravata de Seda Fina", "Gola Aberta Confiante",
                    "Corrente de Ouro com Pingente $", "Correntes de Prata de Grife", "Cabelo Longo Despojado", "Cabelo Longo Platinado",
                    "Windbreaker Neon de Rua", "Corte Degradê Contemporâneo",
                    "Correntes de Prata Premium", "Fundo Rosa Alegre Contemporâneo", "Visual Despojado Confiante", "Óculos de Grau Intelectuais", "Camiseta Preta Básica"
                };
                string[] avatarTattoos = new string[] {
                    "Nenhuma (Barba Alinhada)", "Nenhuma (Cabelo Social)", "Nenhuma (Fade Degradê)", "Nenhuma (Barba Cerrada)",
                    "Trevo no Pescoço & Face", "Tranças com Dreads Dourados", "Colar de Couro (Estilo Indie)", "Cavanhaque Moderno",
                    "Riscos no Buzzcut Verde", "Barba de Linha Fina",
                    "Loiro Platinado & Tatuado", "Cachos Naturais & Aparelho", "Cabelo Ondulado & Bigode Retrô", "Cabelo Social & Barba Curta", "Expressão Confusa & Atitude"
                };

                Player.Current.YoutuberAvatarId = index;
                Player.Current.YoutuberHairColor = avatarGlowColors[index];
                Player.Current.YoutuberOutfit = avatarStyles[index];
                Player.Current.YoutuberAccessories = avatarAccessories[index];
                Player.Current.YoutuberTattoos = avatarTattoos[index];

                UpdateYoutuberProfile();
                dialog.Close();
            };
            panel.Children.Add(btnSave);

            dialog.Content = panel;
            dialog.ShowDialog();
        }

        private void CheckTrophies()
        {
            foreach (var channel in Player.Current.Channels)
            {
                int heelCount = 0;
                int rantCount = 0;
                int nerdCount = 0;
                foreach (var video in channel.Videos)
                {
                    if (!TrophyManager.HasTrophy(Trophy.InternetFamous) && video.Views >= 100000)
                        TrophyManager.UnlockTrophy(Trophy.InternetFamous);

                    if (!TrophyManager.HasTrophy(Trophy.PropDepartment) && video.Cost == 400)
                        TrophyManager.UnlockTrophy(Trophy.PropDepartment);

                    if (!TrophyManager.HasTrophy(Trophy.WellHeeld) && video.Category == VideoCategory.Hauls && video.Name.ToLower().Contains("heels"))
                        heelCount++;

                    if (!TrophyManager.HasTrophy(Trophy.RantMaster) && video.Category == VideoCategory.Vlog && video.Name.ToLower().Contains("rant"))
                        rantCount++;

                    if (!TrophyManager.HasTrophy(Trophy.Procrastinator) && video.Category == VideoCategory.Gaming && video.Name.ToLower().Contains("nerd"))
                        nerdCount++;
                }

                if (heelCount >= 5)
                    TrophyManager.UnlockTrophy(Trophy.WellHeeld);

                if (rantCount >= 5)
                    TrophyManager.UnlockTrophy(Trophy.RantMaster);

                if (nerdCount >= 3)
                    TrophyManager.UnlockTrophy(Trophy.Procrastinator);
            }

            if (!TrophyManager.HasTrophy(Trophy.AptPupil))
            {
                bool completed = true;
                foreach (var study in Studies.Current.All)
                {
                    if (!study.IsCompleted)
                    {
                        completed = false;
                        break;
                    }
                }

                if (completed)
                    TrophyManager.UnlockTrophy(Trophy.AptPupil);
            }
        }

        private void btnDailyPlanner_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            btnDailyPlanner.IsChecked = true;
            HideAllViews();
            dailyPlanner.Visibility = Visibility.Visible;
            SetRightPanelVisibility(true);
        }

        private void btnVideoPlanner_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            btnVideoPlanner.IsChecked = true;
            HideAllViews();
            videoManager.Visibility = Visibility.Visible;
            videoManager.Update();
        }

        private void btnOnlineStore_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            btnOnlineStore.IsChecked = true;
            HideAllViews();
            onlineStore.Visibility = Visibility.Visible;
            onlineStore.Update();
        }

        private void btnRivals_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            btnRivals.IsChecked = true;
            HideAllViews();
            rivalViewer.Visibility = Visibility.Visible;
            rivalViewer.Update();
        }



        private void UncheckAllButtons()
        {
            btnDailyPlanner.IsChecked = false;
            btnVideoPlanner.IsChecked = false;
            btnOnlineStore.IsChecked = false;
            btnRivals.IsChecked = false;
            if (btnStockMarket != null) btnStockMarket.IsChecked = false;
            if (btnCareer != null) btnCareer.IsChecked = false;
            if (btnGovernment != null) btnGovernment.IsChecked = false;
        }

        private void HideAllViews()
        {
            dailyPlanner.Visibility = Visibility.Collapsed;
            videoManager.Visibility = Visibility.Collapsed;
            onlineStore.Visibility = Visibility.Collapsed;
            rivalViewer.Visibility = Visibility.Collapsed;
            if (stockMarket != null) stockMarket.Visibility = Visibility.Collapsed;
            if (careerControl != null) careerControl.Visibility = Visibility.Collapsed;
            if (governmentControl != null) governmentControl.Visibility = Visibility.Collapsed;
            SetRightPanelVisibility(false);
        }

        private void btnNewDay_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            progress.Visibility = System.Windows.Visibility.Visible;

            dailyPlanner.NewDay();
            rivalViewer.NewDay();
        }

        private void Tutorial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!TrophyManager.HasTrophy(Trophy.Pupil))
                TrophyManager.UnlockTrophy(Trophy.Pupil);

            System.Diagnostics.Process.Start("http://www.youtube.com/watch?v=oKfNSm1SLSQ");
        }

        private void Save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Directory.Exists(SaveLoadHelper.SaveDirectory))
            {
                Directory.CreateDirectory(SaveLoadHelper.SaveDirectory);
            }

            if (File.Exists(SaveLoadHelper.SaveFile))
            {
                CustomMessageBox.ShowDialog(EnglishStrings.OverwriteSave.Translate(), EnglishStrings.SaveExists.Translate(), MessagePicture.Question, (result) =>
                    {
                        if (result == true)
                            DoSave();
                    });
            }
            else
            {
                DoSave();
            }
        }

        private void DoSave()
        {
            dailyPlanner.Appointments.ForEach(a => a.HoursPutIn--);
            SaveLoadHelper.Save(SaveLoadHelper.SaveFile);
            dailyPlanner.Appointments.ForEach(a => a.HoursPutIn++);
        }

        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CustomMessageBox.ShowDialog(EnglishStrings.Confirm.Translate(), EnglishStrings.LeaveGame.Translate(), MessagePicture.Question, (result) =>
            {
                if (result == true)
                {
                    if (GameExit != null)
                        GameExit();
                }
            });
        }

        private void imgSave_MouseEnter(object sender, MouseEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri("Resources/Disk_hover.png", UriKind.Relative));
        }

        private void imgSave_MouseLeave(object sender, MouseEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri("Resources/Disk.png", UriKind.Relative));
        }

        private void gridHelp_MouseEnter(object sender, MouseEventArgs e)
        {
            gridHelp.Source = new BitmapImage(new Uri("Resources/Help_hover.png", UriKind.Relative));
        }

        private void gridHelp_MouseLeave(object sender, MouseEventArgs e)
        {
            gridHelp.Source = new BitmapImage(new Uri("Resources/Help.png", UriKind.Relative));
        }

        private void gridExit_MouseEnter(object sender, MouseEventArgs e)
        {
            gridExit.Source = new BitmapImage(new Uri("Resources/Exit_hover.png", UriKind.Relative));
        }

        private void gridExit_MouseLeave(object sender, MouseEventArgs e)
        {
            gridExit.Source = new BitmapImage(new Uri("Resources/Exit.png", UriKind.Relative));
        }

        private void btnStockMarket_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            if (btnStockMarket != null) btnStockMarket.IsChecked = true;
            HideAllViews();
            if (stockMarket != null)
            {
                stockMarket.Visibility = Visibility.Visible;
                stockMarket.Update();
            }
        }



        private void btnCareer_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            if (btnCareer != null) btnCareer.IsChecked = true;
            HideAllViews();
            if (careerControl != null)
            {
                careerControl.Visibility = Visibility.Visible;
                careerControl.Update();
            }
        }

        private void btnGovernment_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllButtons();
            if (btnGovernment != null) btnGovernment.IsChecked = true;
            HideAllViews();
            if (governmentControl != null)
            {
                governmentControl.Visibility = Visibility.Visible;
                governmentControl.Update();
            }
        }

        private void btnNavAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            if (btnNavAnalyzer == null || btnNavInstaFans == null || appAnalyzer == null || appInstaFans == null) return;
            btnNavAnalyzer.IsChecked = true;
            btnNavInstaFans.IsChecked = false;
            btnNavAnalyzer.Foreground = new SolidColorBrush(Colors.White);
            btnNavInstaFans.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#888888") as Color? ?? Colors.Gray);
            appAnalyzer.Visibility = Visibility.Visible;
            appInstaFans.Visibility = Visibility.Collapsed;
        }

        private void btnNavInstaFans_Click(object sender, RoutedEventArgs e)
        {
            if (btnNavAnalyzer == null || btnNavInstaFans == null || appAnalyzer == null || appInstaFans == null) return;
            btnNavAnalyzer.IsChecked = false;
            btnNavInstaFans.IsChecked = true;
            btnNavAnalyzer.Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#888888") as Color? ?? Colors.Gray);
            btnNavInstaFans.Foreground = new SolidColorBrush(Colors.White);
            appAnalyzer.Visibility = Visibility.Collapsed;
            appInstaFans.Visibility = Visibility.Visible;
            UpdateInstaFansUI();
        }

        private void UpdatePhoneStatus()
        {
            try
            {
                if (txtPhoneTime == null || txtPhoneBattery == null) return;

                // Relógio simulado integrado com as rodadas
                int baseHour = 9;
                int currentDay = Player.Current.Iterations;
                if (currentDay < 0) currentDay = 0;
                int hour = (baseHour + (currentDay * 2)) % 24;
                txtPhoneTime.Text = string.Format("{0:D2}:41", hour);

                // Bateria simulada integrada com a energia
                int batteryPct = 100;
                if (Player.Current.Overtime)
                {
                    batteryPct = 15;
                }
                else
                {
                    int taskCount = Player.Current.TasksInProgress != null ? Player.Current.TasksInProgress.Count : 0;
                    batteryPct = Math.Max(20, 100 - (taskCount * 15));
                }

                txtPhoneBattery.Text = string.Format("🔋 {0}%", batteryPct);

                // Cor da bateria
                if (batteryPct <= 20)
                {
                    txtPhoneBattery.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3333"));
                }
                else if (batteryPct <= 50)
                {
                    txtPhoneBattery.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));
                }
                else
                {
                    txtPhoneBattery.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
                }
            }
            catch { }
        }

        private void UpdateInstaFansUI()
        {
            try
            {
                UpdatePhoneStatus();

                if (txtInstaFollowers == null || txtInstaPosts == null) return;

                // Sincronizar contadores
                txtInstaFollowers.Text = Player.Current.InstaFollowers.ToString("N0");
                txtInstaPosts.Text = Player.Current.InstaPosts.ToString();

                // Sincronizar Avatar
                if (imgInstaAvatar != null && imgProfileAvatarMini != null)
                {
                    imgInstaAvatar.Source = imgProfileAvatarMini.Source;
                }

                // Renderizar posts no Feed do Smartphone
                if (instaPostsContainer != null)
                {
                    instaPostsContainer.Children.Clear();
                    if (Player.Current.InstaPostsList != null)
                    {
                        foreach (var post in Player.Current.InstaPostsList)
                        {
                            RenderInstaPostInUI(post);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void RenderInstaPostInUI(InstaPost post)
        {
            Border card = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 12),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E32")),
                BorderThickness = new Thickness(1)
            };

            card.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                BlurRadius = 8,
                ShadowDepth = 1,
                Opacity = 0.2,
                Color = Colors.Black
            };

            StackPanel stack = new StackPanel();

            // Header: Usuário, Avatar circular e Tag
            Grid header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Avatar
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Nome do Usuário
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Badge / Ganho

            // Avatar circular pequeno com borda
            Border avatarBorder = new Border
            {
                Width = 26,
                Height = 26,
                CornerRadius = new CornerRadius(13),
                BorderThickness = new Thickness(1.5),
                BorderBrush = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop((Color)ColorConverter.ConvertFromString("#833AB4"), 0.0),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#FD1D1D"), 0.5),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#FCAF45"), 1.0)
                    },
                    new Point(0, 0),
                    new Point(1, 1)
                ),
                Padding = new Thickness(0.5),
                Margin = new Thickness(0, 0, 6, 0)
            };

            Border avatarInner = new Border
            {
                CornerRadius = new CornerRadius(13),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#282828")),
                ClipToBounds = true
            };
            Image avatarImg = new Image
            {
                Source = imgInstaAvatar != null ? imgInstaAvatar.Source : null,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(-1)
            };
            avatarInner.Child = avatarImg;
            avatarBorder.Child = avatarInner;
            Grid.SetColumn(avatarBorder, 0);
            header.Children.Add(avatarBorder);

            TextBlock txtUser = new TextBlock
            {
                Text = "@" + (string.IsNullOrEmpty(Player.Current.YoutuberName) ? "GamerPro" : Player.Current.YoutuberName),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(txtUser, 1);
            header.Children.Add(txtUser);

            if (post.IsAd)
            {
                TextBlock txtAdBadge = new TextBlock
                {
                    Text = "Parceria Paga",
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700")),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(txtAdBadge, 2);
                header.Children.Add(txtAdBadge);
            }
            else
            {
                string sign = post.FollowersChange >= 0 ? "+" : "";
                TextBlock txtGain = new TextBlock
                {
                    Text = sign + post.FollowersChange.ToString("N0") + " segs",
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(post.FollowersChange >= 0 ? Colors.LightGreen : Colors.Red),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(txtGain, 2);
                header.Children.Add(txtGain);
            }

            stack.Children.Add(header);

            // Imagem do Post
            try
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(post.ImagePath, UriKind.Absolute)),
                    Height = 120,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0, 6, 0, 6)
                };

                Border imgBorder = new Border
                {
                    CornerRadius = new CornerRadius(8),
                    ClipToBounds = true,
                    Child = img
                };
                stack.Children.Add(imgBorder);
            }
            catch
            {
            }

            // Barra de Ação de Feedback (Like Button + Counter)
            Grid interactionGrid = new Grid();
            interactionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Heart Icon
            interactionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Likes Text

            Button btnLike = new Button
            {
                Content = "🤍",
                FontSize = 14,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.White,
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };

            TextBlock txtLikes = new TextBlock
            {
                Text = post.Likes.ToString("N0") + " curtidas",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            };

            bool isLiked = false;
            btnLike.Click += (sender, ev) =>
            {
                ScaleTransform scale = new ScaleTransform(1.0, 1.0);
                btnLike.RenderTransform = scale;
                btnLike.RenderTransformOrigin = new Point(0.5, 0.5);

                var animX = new System.Windows.Media.Animation.DoubleAnimation(1.0, 1.4, TimeSpan.FromMilliseconds(100)) { AutoReverse = true };
                var animY = new System.Windows.Media.Animation.DoubleAnimation(1.0, 1.4, TimeSpan.FromMilliseconds(100)) { AutoReverse = true };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, animX);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, animY);

                if (!isLiked)
                {
                    btnLike.Content = "❤️";
                    btnLike.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2255"));
                    post.Likes++;
                    txtLikes.Text = post.Likes.ToString("N0") + " curtidas";
                    isLiked = true;
                }
                else
                {
                    btnLike.Content = "🤍";
                    btnLike.Foreground = Brushes.White;
                    post.Likes--;
                    txtLikes.Text = post.Likes.ToString("N0") + " curtidas";
                    isLiked = false;
                }
            };

            Grid.SetColumn(btnLike, 0);
            Grid.SetColumn(txtLikes, 1);
            interactionGrid.Children.Add(btnLike);
            interactionGrid.Children.Add(txtLikes);
            stack.Children.Add(interactionGrid);

            // Legenda
            TextBlock txtCaption = new TextBlock
            {
                Text = post.Caption,
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0)
            };
            stack.Children.Add(txtCaption);

            if (post.IsAd)
            {
                TextBlock txtAdEarnings = new TextBlock
                {
                    Text = "+ " + post.AdEarnings.ToCurrencyString() + " de receita!",
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.LightGreen),
                    Margin = new Thickness(0, 4, 0, 0)
                };
                stack.Children.Add(txtAdEarnings);
            }

            card.Child = stack;
            instaPostsContainer.Children.Insert(0, card); // Adiciona no topo
        }

        private void btnPostInsta_Click(object sender, RoutedEventArgs e)
        {
            Xceed.Wpf.Toolkit.ChildWindow dialog = new Xceed.Wpf.Toolkit.ChildWindow
            {
                Caption = "Publicar no InstaFans 📸",
                Width = 360,
                Height = 360,
                WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center,
                Background = new SolidColorBrush(ColorConverter.ConvertFromString("#121212") as Color? ?? Colors.Black),
                BorderBrush = new SolidColorBrush(ColorConverter.ConvertFromString("#E1306C") as Color? ?? Colors.DeepPink),
                BorderThickness = new Thickness(1.5)
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(15) };

            TextBlock lblTitle = new TextBlock 
            { 
                Text = "ESCOLHA O TIPO DE POSTAGEM", 
                FontSize = 13, 
                FontWeight = FontWeights.Bold, 
                Foreground = new SolidColorBrush(ColorConverter.ConvertFromString("#E1306C") as Color? ?? Colors.DeepPink), 
                Margin = new Thickness(0, 0, 0, 15) 
            };
            panel.Children.Add(lblTitle);

            // Botão Setup
            Button btnSetup = new Button
            {
                Content = "🖥️ SETUP GAMER\n(Ganha seguidores baseado no canal)",
                Height = 45,
                Margin = new Thickness(0, 0, 0, 10),
                Background = new SolidColorBrush(ColorConverter.ConvertFromString("#1C1C1C") as Color? ?? Colors.DarkGray),
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            btnSetup.Click += (s, ev) =>
            {
                PostToInstaFans(0);
                dialog.Close();
            };
            panel.Children.Add(btnSetup);

            // Botão Selfie
            Button btnSelfie = new Button
            {
                Content = "🤳 SELFIE / VLOG DO DIA\n(Ganha seguidores baseado no canal)",
                Height = 45,
                Margin = new Thickness(0, 0, 0, 10),
                Background = new SolidColorBrush(ColorConverter.ConvertFromString("#1C1C1C") as Color? ?? Colors.DarkGray),
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            btnSelfie.Click += (s, ev) =>
            {
                PostToInstaFans(1);
                dialog.Close();
            };
            panel.Children.Add(btnSelfie);

            // Botão Publi
            bool canAd = Player.Current.InstaFollowers >= 5000 && Player.Current.InstaDaysSinceLastAd >= 3;
            double adPayout = Math.Round(Player.Current.InstaFollowers * 0.03, 2);
            string adButtonText = string.Format("💰 PARCERIA PATROCINADA (PUBLI)\n(Ganha {0} | Perde alguns seguidores)", adPayout.ToCurrencyString());
            if (Player.Current.InstaFollowers < 5000)
            {
                adButtonText = "💰 PARCERIA PATROCINADA (PUBLI)\n(Bloqueado: Requer 5.000 seguidores)";
            }
            else if (Player.Current.InstaDaysSinceLastAd < 3)
            {
                adButtonText = string.Format("💰 PARCERIA PATROCINADA (PUBLI)\n(Aguarde {0} dias para postar nova publi)", 3 - Player.Current.InstaDaysSinceLastAd);
            }

            Button btnAd = new Button
            {
                Content = adButtonText,
                Height = 45,
                Margin = new Thickness(0, 0, 0, 15),
                Background = new SolidColorBrush(canAd ? ColorConverter.ConvertFromString("#FFD700") as Color? ?? Colors.Gold : ColorConverter.ConvertFromString("#2E2E2E") as Color? ?? Colors.Gray),
                Foreground = new SolidColorBrush(canAd ? Colors.Black : Colors.Gray),
                FontWeight = FontWeights.Bold,
                IsEnabled = canAd,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            btnAd.Click += (s, ev) =>
            {
                PostToInstaFans(2);
                dialog.Close();
            };
            panel.Children.Add(btnAd);

            Button btnCancel = new Button
            {
                Content = "CANCELAR",
                Height = 30,
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.Gray),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1)
            };
            btnCancel.Click += (s, ev) => dialog.Close();
            panel.Children.Add(btnCancel);

            dialog.Content = panel;
            dialog.ShowDialog();
        }

        private void PostToInstaFans(int postType)
        {
            try
            {
                Random rand = new Random();
                int totalSubs = Player.Current.Channels.Sum(c => c.Subscribers);
                double baseGain = rand.Next(100, 500);
                double influenceMultiplier = 1.0 + (totalSubs / 10000.0);
                int followersGained = (int)(baseGain * influenceMultiplier);

                string imagePath = "";
                string caption = "";
                bool isAd = false;
                double adEarnings = 0;

                if (postType == 0)
                {
                    imagePath = "pack://application:,,,/TubeStar;component/Resources/Shoot.jpg";
                    string[] setupCaptions = new string[] {
                        "Setup gamer atualizado! Rumo ao topo! 🖥️🔥",
                        "Luzes RGB ligadas, pronto para a próxima gravação! 🎮⚡",
                        "Minha estação de batalha atual. O que acharam? 👾"
                    };
                    caption = setupCaptions[rand.Next(setupCaptions.Length)];
                }
                else if (postType == 1)
                {
                    imagePath = "pack://application:,,,/TubeStar;component/Resources/Study.jpg";
                    string[] selfieCaptions = new string[] {
                        "Mais um dia de muito foco e gravação! 🤳✨",
                        "Preparando novidades pro canal... fiquem ligados! 😉",
                        "Cafezinho e roteiro, melhor combinação. ☕📖"
                    };
                    caption = selfieCaptions[rand.Next(selfieCaptions.Length)];
                }
                else if (postType == 2)
                {
                    isAd = true;
                    imagePath = "pack://application:,,,/TubeStar;component/Resources/Ultra.jpg";
                    adEarnings = Math.Round(Player.Current.InstaFollowers * 0.03, 2);
                    
                    followersGained = (int)(-50 - (Player.Current.InstaFollowers * 0.005));

                    string[] adCaptions = new string[] {
                        "Parceria de respeito com a DevCorp! Qualidade garantida! 💰⌨️",
                        "Equipamento novo patrocinado pela PEAR. Incrível! 🍎🎧",
                        "Joguem o novo RPG da RVG, tá sensacional! Cupom: STAR 🚀"
                    };
                    caption = adCaptions[rand.Next(adCaptions.Length)];

                    Player.Current.Money += adEarnings;
                    Player.Current.InstaDaysSinceLastAd = 0;
                }

                Player.Current.InstaPosts++;
                Player.Current.InstaFollowers += followersGained;
                if (Player.Current.InstaFollowers < 0) Player.Current.InstaFollowers = 0;

                InstaPost newPost = new InstaPost
                {
                    ImagePath = imagePath,
                    Caption = caption,
                    Likes = (int)(Player.Current.InstaFollowers * (rand.Next(5, 15) / 100.0)) + rand.Next(10, 50),
                    FollowersChange = followersGained,
                    IsAd = isAd,
                    AdEarnings = adEarnings
                };

                if (Player.Current.InstaPostsList == null)
                {
                    Player.Current.InstaPostsList = new List<InstaPost>();
                }
                Player.Current.InstaPostsList.Add(newPost);

                UpdateInstaFansUI();
            }
            catch
            {
            }
        }

        private void SetRightPanelVisibility(bool isVisible)
        {
            if (colRight != null && borderRight != null)
            {
                if (isVisible)
                {
                    colRight.Width = new GridLength(300);
                    borderRight.Visibility = Visibility.Visible;
                }
                else
                {
                    colRight.Width = new GridLength(0);
                    borderRight.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}