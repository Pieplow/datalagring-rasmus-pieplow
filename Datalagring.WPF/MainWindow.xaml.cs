
using Contracts;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace Datalagring.WPF
{
    public partial class MainWindow : Window
    {
        
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
            var newStudent = new CreateParticipantDto(
                 txtFirstName.Text,
                 txtLastName.Text,
                 txtEmail.Text
                );
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
                var participants = await _client
                    .GetFromJsonAsync<List<ParticipantDto>>("/participants");

                cbParticipants.ItemsSource = participants;

                var instances = await _client
                    .GetFromJsonAsync<List<CourseInstanceDto>>("/courseinstances");

                cbCourseInstances.ItemsSource = instances;
            }
            catch
            {
                MessageBox.Show("Kunde inte hämta data från API.");
            }
        }

        // Metod för själva kursregistreringen (M:N-kopplingen)

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (cbParticipants.SelectedItem is not ParticipantDto selectedParticipant)
            {
                MessageBox.Show("Välj en deltagare.");
                return;
            }

            if (cbCourseInstances.SelectedItem is not CourseInstanceDto selectedInstance)
            {
                MessageBox.Show("Välj ett kurstillfälle.");
                return;
            }

            try
            {
                var response = await _client.PostAsJsonAsync(
                    $"/courseinstances/{selectedInstance.Id}/registrations",
                    selectedParticipant.Id);

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

