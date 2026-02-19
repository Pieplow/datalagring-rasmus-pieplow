using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace Datalagring.WPF
{
    public partial class MainWindow : Window
    {
        // Du måste definiera _client här för att den ska kunna användas i metoderna
        private readonly HttpClient _client =
            new HttpClient { BaseAddress = new Uri("http://localhost:63606/") };

        public MainWindow()
        {
            InitializeComponent();
            LoadInitialData(); // Denna metod fyller dina ComboBoxar vid start
        }

        private async void btnSaveStudent_Click(object sender, RoutedEventArgs e)
        {
            // Vi skapar ett objekt som matchar din Participant-entitet i API:et
            var newStudent = new
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Email = txtEmail.Text
            };

            try
            {
                var response = await _client.PostAsJsonAsync("/participants", newStudent);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Studenten har sparats i databasen!");

                    // Rensa fälten med rätt namn (txt...)
                    txtFirstName.Clear();
                    txtLastName.Clear();
                    txtEmail.Clear();

                    LoadInitialData(); // Uppdatera rullistan så den nya studenten syns
                }
                else
                {
                    MessageBox.Show("Kunde inte spara studenten. Kontrollera API-loggen.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ett fel uppstod: {ex.Message}");
            }
        }

        private async void LoadInitialData()
        {
            try
            {
                // Här hämtar vi deltagare för att fylla cbParticipants
                // Ersätt "/participants" med din faktiska GET-rutt om den heter något annat
                var participants = await _client.GetFromJsonAsync<System.Collections.Generic.List<dynamic>>("/participants");
                cbParticipants.ItemsSource = participants;
            }
            catch { /* API kanske inte är startat än */ }
        }

        // Metod för själva kursregistreringen (M:N-kopplingen)
      
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (cbParticipants.SelectedItem == null)
            {
                MessageBox.Show("Välj en deltagare.");
                return;
            }

            if (cbCourseInstances.SelectedItem == null)
            {
                MessageBox.Show("Välj ett kurstillfälle.");
                return;
            }

            try
            {
                // Eftersom du använder dynamic i LoadInitialData
                var selectedParticipant = cbParticipants.SelectedItem;
                var selectedInstance = cbCourseInstances.SelectedItem;

                Guid participantId = Guid.Parse(selectedParticipant.id.ToString());
                Guid instanceId = Guid.Parse(selectedInstance.id.ToString());

                var response = await _client.PostAsJsonAsync(
                    $"/courseinstances/{instanceId}/registrations",
                    participantId);   // Endast Guid skickas

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Registrering lyckades!");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Registrering misslyckades:\n{error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid registrering: {ex.Message}");
            }
        }

    }
}
}