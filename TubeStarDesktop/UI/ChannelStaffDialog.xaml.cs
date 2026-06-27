using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace TubeStar
{
    public partial class ChannelStaffDialog : ChildWindow
    {
        private Channel _channel;

        public ChannelStaffDialog(Channel channel)
        {
            InitializeComponent();
            _channel = channel;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_channel == null) return;

            txtChannelNameTitle.Text = string.Format("EQUIPE DO CANAL: {0}", _channel.Name.ToUpper());

            // Editor state
            txtEditorLevel.Text = string.Format("Nível {0}", _channel.EditorLevel);
            txtEditorXPText.Text = string.Format("XP: {0} / {1}", _channel.EditorXP % 100, 100);
            pbEditorXP.Value = _channel.EditorXP % 100;

            double edSal = _channel.EditorCurrentSalary;
            txtEditorCost.Text = string.Format("{0} + 5% Comissão", edSal.ToCurrencyString());
            btnToggleEditor.Content = _channel.HiredEditor ? "Demitir" : "Contratar";

            // Manager state
            double manSal = _channel.ManagerCurrentSalary;
            txtManagerCost.Text = string.Format("{0} + 10% Comissão", manSal.ToCurrencyString());
            btnToggleManager.Content = _channel.HiredManager ? "Demitir" : "Contratar";
        }

        private void BtnToggleEditor_Click(object sender, RoutedEventArgs e)
        {
            if (_channel == null) return;

            _channel.HiredEditor = !_channel.HiredEditor;
            CustomMessageBox.ShowDialog(
                _channel.HiredEditor ? "Editor Contratado!" : "Editor Demitido!",
                _channel.HiredEditor 
                    ? "Você contratou um Editor de Vídeo. Ele editará todos os vídeos deste canal automaticamente a cada dia (bônus de qualidade e sem gastar horas de agenda)." 
                    : "Você demitiu o Editor de Vídeo deste canal.",
                MessagePicture.Work);

            UpdateUI();
        }

        private void BtnToggleManager_Click(object sender, RoutedEventArgs e)
        {
            if (_channel == null) return;

            _channel.HiredManager = !_channel.HiredManager;
            CustomMessageBox.ShowDialog(
                _channel.HiredManager ? "Gestor Contratado!" : "Gestor Demitido!",
                _channel.HiredManager 
                    ? "Você contratou um Gestor de Canal. Ele gravará todos os vídeos deste canal automaticamente a cada dia (não requer agendamento de horas)." 
                    : "Você demitiu o Gestor de Canal deste canal.",
                MessagePicture.Work);

            UpdateUI();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
