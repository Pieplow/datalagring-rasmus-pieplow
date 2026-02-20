using Contracts;
using System;
using System.Collections.Generic;
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
            LoadInitialData();
        }

        // ================= LOAD DATA =================
        private async void LoadInitialData()
        {
            try
            {
                cbParticipants.ItemsSource =
                    await _client.GetFromJsonAsync<List<ParticipantDto>>("/participants");

                cbCourses.ItemsSource =
                    await _client.GetFromJsonAsync<List<CourseDto>>("/courses");

                cbInstructors.ItemsSource =
                    await _client.GetFromJsonAsync<List<InstructorDto>>("/instructors");

                cbCourseInstances.ItemsSource =
                    await _client.GetFromJsonAsync<List<CourseInstanceDto>>("/courseinstances");
            }
            catch
            {
                MessageBox.Show("Kunde inte hämta data från API.");
            }
        }

        // ================= CREATE STUDENT =================
        private async void btnSaveStudent_Click(object sender, RoutedEventArgs e)
        {
            var dto = new CreateParticipantDto(
                txtFirstName.Text,
                txtLastName.Text,
                txtEmail.Text
            );

            var response = await _client.PostAsJsonAsync("/participants", dto);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Student skapad!");
                txtFirstName.Clear();
                txtLastName.Clear();
                txtEmail.Clear();
                LoadInitialData();
            }
        }

        // ================= CREATE COURSE =================
        private async void btnCreateCourse_Click(object sender, RoutedEventArgs e)
        {
            var dto = new CreateCourseDto(txtCourseName.Text);

            var response = await _client.PostAsJsonAsync("/courses", dto);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Kurs skapad!");
                txtCourseName.Clear();
                LoadInitialData();
            }
        }

        // ================= CREATE INSTRUCTOR =================
        private async void btnCreateInstructor_Click(object sender, RoutedEventArgs e)
        {
            var dto = new CreateInstructorDto(
                txtInstructorFirstName.Text,
                txtInstructorLastName.Text
            );

            var response = await _client.PostAsJsonAsync("/instructors", dto);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Lärare skapad!");
                txtInstructorFirstName.Clear();
                txtInstructorLastName.Clear();
                LoadInitialData();
            }
        }

        // ================= CREATE COURSE INSTANCE =================
        private async void btnCreateInstance_Click(object sender, RoutedEventArgs e)
        {
            if (cbCourses.SelectedItem is not CourseDto selectedCourse)
            {
                MessageBox.Show("Välj kurs");
                return;
            }

            if (cbInstructors.SelectedItem is not InstructorDto selectedInstructor)
            {
                MessageBox.Show("Välj lärare");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity))
            {
                MessageBox.Show("Ange giltig kapacitet");
                return;
            }

            var dto = new CreateCourseInstanceDto(
                selectedCourse.Id,
                selectedInstructor.Id,
                capacity
            );

            var response = await _client.PostAsJsonAsync("/courseinstances", dto);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Kurstillfälle skapat!");
                txtCapacity.Clear();
                LoadInitialData();
            }
        }

        // ================= REGISTER STUDENT TO COURSE =================
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (cbParticipants.SelectedItem is not ParticipantDto selectedParticipant)
            {
                MessageBox.Show("Välj deltagare");
                return;
            }

            if (cbCourseInstances.SelectedItem is not CourseInstanceDto selectedInstance)
            {
                MessageBox.Show("Välj kurstillfälle");
                return;
            }

            var response = await _client.PostAsJsonAsync(
                $"/courseinstances/{selectedInstance.Id}/registrations",
                selectedParticipant.Id);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Registrering lyckades!");
            }
        }
    }
}