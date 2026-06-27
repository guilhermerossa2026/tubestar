using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace TubeStar
{
    /// <summary>
    /// Interaction logic for UniversitySelectionDialog.xaml
    /// </summary>
    public partial class UniversitySelectionDialog : ChildWindow
    {
        public UniversitySelectionDialog()
        {
            InitializeComponent();
            UpdateBalance();
        }

        private void UpdateBalance()
        {
            if (Player.Current != null)
            {
                txtCurrentBalance.Text = Player.Current.Money.ToCurrencyString();
            }
        }

        private void EnrollEAD_Click(object sender, RoutedEventArgs e)
        {
            Enroll("ead_pop");
        }

        private void EnrollPUC_Click(object sender, RoutedEventArgs e)
        {
            Enroll("puc_rio");
        }

        private void EnrollMIT_Click(object sender, RoutedEventArgs e)
        {
            Enroll("mit_insper");
        }

        private void EnrollIA_Click(object sender, RoutedEventArgs e)
        {
            Enroll("faculdade_ia");
        }

        private void Enroll(string universityId)
        {
            var uni = UniversityCatalog.GetUniversityById(universityId);
            if (uni == null) return;

            if (Player.Current.Money < uni.EnrollmentFee)
            {
                CustomMessageBox.ShowDialog(
                    "Saldo Insuficiente!",
                    "Você não tem dinheiro suficiente para pagar a taxa de matrícula desta universidade.",
                    MessagePicture.Money
                );
                return;
            }

            Player.Current.Money -= uni.EnrollmentFee;
            Player.Current.EnrolledUniversityId = uni.Id;

            CustomMessageBox.ShowDialog(
                "Matrícula Confirmada!",
                string.Format("Parabéns! Você se matriculou na {0}. Uma mensalidade de {1}/dia será cobrada a partir de amanhã.", uni.Name, uni.DailyTuition.ToCurrencyString()),
                MessagePicture.Study
            );

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
