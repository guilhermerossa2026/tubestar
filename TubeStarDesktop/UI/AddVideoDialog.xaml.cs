using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace TubeStar
{
    /// <summary>
    /// Interaction logic for AddVideoDialog.xaml
    /// </summary>
    public partial class AddVideoDialog : ChildWindow
    {
        public Video Video { get; private set; }

        public AddVideoDialog()
        {
            InitializeComponent();
            Translate();
            FocusedElement = txtName;

            var data = GetData();
            cmbCategory.ItemsSource = data;
            cmbCategory.DisplayMemberPath = "Value";
            cmbCategory.SelectedValuePath = "Key";

            if (Settings.LastCategory.HasValue)
            {
                cmbCategory.SelectedValue = Settings.LastCategory.Value;
            }

            if (Player.Current.Channels != null)
            {
                var availableChannels = Player.Current.Channels.Where(c => c != Channel.UnreleasedVideos).ToList();
                cmbChannel.ItemsSource = availableChannels;
                cmbChannel.DisplayMemberPath = "Name";
                cmbChannel.SelectedValuePath = "Id";
                if (availableChannels.Count > 0)
                {
                    cmbChannel.SelectedIndex = 0;
                }
            }

            if (Player.Current != null && Player.Current.HasAIEnhancedTitles)
            {
                btnGenerateAITitle.Visibility = Visibility.Visible;
            }
        }

        private void Translate()
        {
            Caption = EnglishStrings.AddVideo.Translate();
            lblName.Text = EnglishStrings.Name.Translate() + ":";
            lblCategory.Text = EnglishStrings.Category.Translate() + ":";
            lblHourSelect.Text = EnglishStrings.Hours.Translate() + ":";
            lblCost.Text = EnglishStrings.Cost.Translate() + ":";
            btnOk.Content = EnglishStrings.Next.Translate();
            btnCancel.Content = EnglishStrings.Cancel.Translate();
            lblHours.Text = "2 " + EnglishStrings.Hours.Translate().ToLower();
        }

        private Dictionary<VideoCategory, string> GetData()
        {
            Dictionary<VideoCategory, string> data = new Dictionary<VideoCategory, string>();
            foreach (VideoCategory category in Enum.GetValues(typeof(VideoCategory)))
            {
                data[category] = category.GetString();
            }

            List<KeyValuePair<VideoCategory, string>> sortTemp = data.ToList();
            sortTemp.Sort((l, r) => l.Value.CompareTo(r.Value));

            return sortTemp.ToDictionary((s) => s.Key, (s) => s.Value);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text))
            {
                CustomMessageBox.ShowDialog(EnglishStrings.MissingValueHeader.Translate(), EnglishStrings.MissingName.Translate(), MessagePicture.Puzzle);
                return;
            }

            if (cmbCategory.SelectedValue == null)
            {
                CustomMessageBox.ShowDialog(EnglishStrings.MissingValueHeader.Translate(), EnglishStrings.MissingCategory.Translate(), MessagePicture.Puzzle);
                return;
            }

            if (cmbChannel.SelectedValue == null)
            {
                CustomMessageBox.ShowDialog("Canal Requerido", "Por favor, selecione um canal de destino para o vídeo.", MessagePicture.Puzzle);
                return;
            }

            if (Player.Current.Money - (sldrMoney.Value * 100) < 0)
            {
                CustomMessageBox.ShowDialog(EnglishStrings.LowCashHeader.Translate(), EnglishStrings.LowCashMessage.Translate(), MessagePicture.Money);
                return;
            }

            Video = new Video();
            Video.Name = txtName.Text;
            Video.Category = (VideoCategory)cmbCategory.SelectedValue;
            Video.ChannelId = cmbChannel.SelectedValue.ToString();
            Video.ExtraShootingHours = (int)sldrHours.Value - ShootVideo.MinimumShootTime;
            Video.Cost = (int)sldrMoney.Value * 100;

            Settings.LastCategory = Video.Category;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void sldrHours_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lblHours != null)
                lblHours.Text = String.Format("{0} {1}", (int)sldrHours.Value, EnglishStrings.Hours.Translate().ToLower());
        }

        private void sldrMoney_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lblMoney != null)
                lblMoney.Text = String.Format("${0}", (int)sldrMoney.Value * 100);
        }

        private static readonly Random _rnd = new Random();
        private void btnGenerateAITitle_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategory.SelectedValue == null)
            {
                CustomMessageBox.ShowDialog("Selecione a Categoria", "Escolha a categoria do vídeo primeiro para gerarmos um título adequado!", MessagePicture.Puzzle);
                return;
            }

            var cat = (VideoCategory)cmbCategory.SelectedValue;
            string title = "";
            switch (cat)
            {
                case VideoCategory.Gaming:
                    string[] gamingTitles = {
                        "JOGUEI ISSO E QUASE CHOREI! 😭",
                        "COMO SUBIR DE ELO RÁPIDO NA NOVA TEMPORADA!",
                        "Eles disseram que era impossível ganhar desse boss...",
                        "O JOGO QUE DESTRUIU MINHA SANIDADE MENTAL! 🤬",
                        "TENTEI SER PRO PLAYER POR UM DIA E DEU RUIM!"
                    };
                    title = gamingTitles[_rnd.Next(gamingTitles.Length)];
                    break;
                case VideoCategory.Technology:
                    string[] techTitles = {
                        "ESTE NOVO SMARTPHONE VAI FALIR A CONCORRÊNCIA! 📱",
                        "Montei o PC Gamer dos meus sonhos e deu tudo errado...",
                        "5 Tecnologias revolucionárias que você precisa conhecer!",
                        "A nova IA que programa melhor que a maioria dos juniores...",
                        "Não compre este hardware antes de assistir este vídeo!"
                    };
                    title = techTitles[_rnd.Next(techTitles.Length)];
                    break;
                case VideoCategory.Comedy:
                    string[] comedyTitles = {
                        "TENTE NÃO RIR! 😂 (Impossível)",
                        "MINHA MÃE REAGINDO AOS MEUS VÍDEOS!",
                        "O dia em que fui expulso do shopping por um motivo besta...",
                        "RECRUTANDO INSCRITOS PARA PASSAR TROTE! 📞",
                        "EXPECTATIVA vs REALIDADE: Vida de Youtuber!"
                    };
                    title = comedyTitles[_rnd.Next(comedyTitles.Length)];
                    break;
                case VideoCategory.Vlog:
                    string[] vlogTitles = {
                        "PASSEI 24 HORAS NO LUGAR MAIS ESTRANHO DO MUNDO!",
                        "A VERDADE sobre a minha carreira de Youtuber...",
                        "Rotina realista de um criador de conteúdo digital!",
                        "Fui assaltado gravando vlogs? Mostrei tudo! 😱",
                        "MINHA NOVA AQUISIÇÃO MILIONÁRIA!"
                    };
                    title = vlogTitles[_rnd.Next(vlogTitles.Length)];
                    break;
                case VideoCategory.HowTo:
                    string[] howToTitles = {
                        "COMO CRESCER NO YOUTUBE RÁPIDO E SEM SEGREDO!",
                        "Aprenda a cozinhar isso em 5 minutos! (Fácil)",
                        "O tutorial definitivo para iniciantes de WPF/XAML!",
                        "Como programar uma IA de alta qualidade passo a passo!",
                        "Como sonegar impostos sem ser pego pela malha fina!"
                    };
                    title = howToTitles[_rnd.Next(howToTitles.Length)];
                    break;
            }
            txtName.Text = title;
        }
    }
}