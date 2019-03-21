using FrontendWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Data;
using System.Windows;

namespace FrontendWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static IEnumerable<Employee> employees;
        private DataView table;
        private DataRowView currentRow;
        private string name;
        private string empColor;
        private static HttpClient client = new HttpClient();
        private Employee currentEmployee;
        private IEnumerable<Vacation> vacations;
        private DateTime start;
        private string duration;
        private Vacation currentVacation;

        public RelayCommand AddEmployee { get; set; }
        public RelayCommand DeleteEmployee { get; set; }
        public RelayCommand CommandAddVacation { get; set; }
        public RelayCommand CommandDeleteVacation { get; set; }    
        
        public Vacation CurrentVacation
        {
            get => currentVacation;
            set
            {
                currentVacation = value;
                OnPropertyChanged(nameof(CurrentVacation));
                if (currentVacation != null)
                {
                    Start = currentVacation.Start;
                    Duration = currentVacation.Duration.ToString();
                }                
            }
        }

        public string Duration
        {
            get => duration;
            set
            {
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        public IEnumerable<Vacation> Vacations
        {
            get => vacations;
            set
            {
                vacations = value;
                OnPropertyChanged(nameof(Vacations));
            }
        }

        public string[] Colors { get; set; } = 
            {
                "Red",
                "Orange",
                "Yellow",
                "Green",
                "Azure",
                "Blue",
                "Purple"
            };        

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string EmpColor
        {
            get => empColor;
            set
            {
                empColor = value;
                OnPropertyChanged(nameof(empColor));
            }
        }

        public DataView Table
        {
            get => table;
            set
            {
                table = value;
                OnPropertyChanged(nameof(Table));
            }
        }

        public DataRowView CurrentRow
        {
            get => currentRow;
            set
            {
                currentRow = value;
                OnPropertyChanged(nameof(CurrentRow));
                if (currentRow != null)
                {
                    currentEmployee = employees.FirstOrDefault(e => e.Name == currentRow.Row.ItemArray.ElementAt(0) as string);
                    Name = currentEmployee.Name;
                    EmpColor = currentEmployee.Color;
                    Vacations = currentEmployee.Vacations; 
                }                    
            }
        }

        public MainViewModel()
        {
            //bool success = false;
            //employees = CallWebAPi<IEnumerable<Employee>>(new Uri("http://localhost:52909"), "api/values", out success);

            client.BaseAddress = new Uri("http://localhost:52909");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GetAllEmployees();

            AddEmployee = new RelayCommand(o => NewEmployee(), v => EmpColor != null);
            DeleteEmployee = new RelayCommand(o => 
            {
                DelEmployee();
                Name = "";
                EmpColor = null;
            }, 
            o => currentEmployee != null && currentEmployee.Name == this.Name);

            CommandAddVacation = new RelayCommand(o => AddVacation());
            CommandDeleteVacation = new RelayCommand(o => DeleteVacation());
            
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:52909");

            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));

            //HttpResponseMessage response = client.GetAsync("api/values").Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    Employees = response.Content.ReadAsAsync &
            //    lt; IEnumerable & lt; Employee & gt; &gt; ().Result
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private async void AddVacation()
        {
            try
            {
                Vacation newVacation = new Vacation() { Start = this.Start, Duration = int.Parse(this.Duration) };
                var response = await client.PostAsJsonAsync("/api/values/vacation/", 
                    new VacationViewModel() { EmployeeId = currentEmployee.EmployeeId, Vacation = newVacation });
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                //MessageBox.Show("Student Added Successfully", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                //studentsListView.ItemsSource = await GetAllStudents();
                //studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        public async void DeleteVacation()
        {
            try
            {
                var response = await client.DeleteAsync($"/api/values/{currentEmployee.EmployeeId}/{currentVacation.VacationId}");
                response.EnsureSuccessStatusCode();
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        private DataView CreateDataView(IEnumerable<Employee> employees)
        {
            DataTable dataTable = new DataTable();

            // Вычисляю текущее маскимальное количество отпусков.
            int maxVacationNumber = 0;
            foreach (Employee emp in employees)
            {
                if (maxVacationNumber < emp.Vacations.Count)
                    maxVacationNumber = emp.Vacations.Count;
            }

            // Создаю пустые столбцы с заголовками.

            dataTable.Columns.Add("ФИО");

            for (int i = 1; i <= maxVacationNumber * 3; i++)
            {
                switch (i % 3)
                {
                    case 1:
                        dataTable.Columns.Add($"Дата начала {i / 3 + 1}");
                        break;
                    case 2:
                        dataTable.Columns.Add($"Продолжительность {i / 3 + 1}, дней");
                        break;
                    case 0:
                        dataTable.Columns.Add($"Дата окончания {i / 3}");
                        break;
                }
            }

            // Создаю строки, "разбитые" по ячейкам
            foreach (Employee emp in employees)
            {
                string[] myRow = new string[maxVacationNumber * 3 + 1];
                myRow[0] = emp.Name.ToString();
                for (int i = 1; i <= emp.Vacations.Count * 3; i++)
                {
                    switch (i % 3)
                    {
                        case 1:
                            myRow[i] = emp.Vacations[i / 3].Start.ToShortDateString();
                            break;
                        case 2:
                            myRow[i] = emp.Vacations[i / 3].Duration.ToString();
                            break;
                        case 0:
                            myRow[i] = DateTime.Parse(myRow[i - 2]).AddDays(int.Parse(myRow[i - 1])).ToShortDateString();
                            break;
                    }
                }
                dataTable.Rows.Add(myRow);
            }

            return dataTable.DefaultView;
        }

        public async void GetAllEmployees()
        {            
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/values/");
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();
                Table = CreateDataView(employees);
            }
            catch (Exception)
            {
                //MessageBox.Show("Error!");
            }
            
        }

        public async void NewEmployee()
        {
            try
            {
                Employee newEmployee = new Employee() { Name = this.Name, Color = EmpColor };
                var response = await client.PostAsJsonAsync("/api/values/", newEmployee);
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                //MessageBox.Show("Student Added Successfully", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                //studentsListView.ItemsSource = await GetAllStudents();
                //studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        public async void DelEmployee()
        {
            try
            {
                var response = await client.DeleteAsync($"/api/values/{currentEmployee.EmployeeId}");
                response.EnsureSuccessStatusCode();
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        //public T CallWebAPi<T>(Uri url, string method, out bool isSuccessStatusCode)
        //{
        //    T result = default(T);

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = url;
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
        //        //    System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password))));

        //        HttpResponseMessage response = client.GetAsync(method).Result;
        //        isSuccessStatusCode = response.IsSuccessStatusCode;
        //        var javaScriptSerializer = new JavaScriptSerializer();
        //        if (isSuccessStatusCode)
        //        {
        //            var dataobj = response.Content.ReadAsStringAsync();
        //            result = javaScriptSerializer.Deserialize<T>(dataobj.Result);
        //        }
        //        else if (Convert.ToString(response.StatusCode) != "InternalServerError")
        //        {
        //            result = javaScriptSerializer.Deserialize<T>("{ \"APIMessage\":\"" + response.ReasonPhrase + "\" }");
        //        }
        //        else
        //        {
        //            result = javaScriptSerializer.Deserialize<T>("{ \"APIMessage\":\"InternalServerError\" }");
        //        }
        //    }
        //    return result;
        //}
    }    
}
